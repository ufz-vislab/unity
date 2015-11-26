using UFZ.Annotations;
using UFZ.Rendering;
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace UFZ.Initialization
{
	/// <summary>
	/// An AssetPostprocessor for importing VTK exported FBX files.
	/// </summary>
	public class MeshImportSettings : AssetPostprocessor
	{
		private static readonly string[] Paths = {
			"Assets/_project",
			"Assets/UFZ/Tests/Objects"
		};

		private bool CheckPath()
		{
			return Paths.Any(assetPath.Contains);
		}

		private bool IsCityEngine()
		{
			return assetPath.Contains("CityEngine");
		}

		private bool CheckParaViewExport()
		{
			// Check for filename-0 [1] and Properties [last] (filename-0/filename-0-1 [2])
			var filename = Path.GetFileNameWithoutExtension(assetPath);
			var modelImporter = (ModelImporter)assetImporter;
			var transformPaths = modelImporter.transformPaths;
			if (transformPaths.Count() < 2)
				return false;
			if (transformPaths[0].Count() == 0 && (transformPaths[1] == filename + "-0" || transformPaths[1] == "Properties")) // && transformPaths.Last() == "Properties"
				return true;
			else
				return false;
		}

		private void OnPreprocessModel()
		{
			if (!CheckPath())
				return;
			if (!CheckParaViewExport())
				return;
			
			Debug.Log("ParaView import: " + Path.GetFileNameWithoutExtension(assetPath));
			var modelImporter = (ModelImporter)assetImporter;
			modelImporter.animationType = ModelImporterAnimationType.None;
			modelImporter.optimizeMesh = IsCityEngine();
			modelImporter.globalScale = 1.0f;
			modelImporter.importMaterials = IsCityEngine();
			modelImporter.userData = "ParaView";
		}

		private void OnPostprocessModel(GameObject go)
		{
			if (!CheckPath() || IsCityEngine())
				return;
			var modelImporter = (ModelImporter)assetImporter;
			if (!modelImporter.userData.Contains("ParaView"))
				return;

			//Debug.Log ("OnPostprocessModel for " + go.name);
			var meshInfo = AssetDatabase.LoadAssetAtPath(assetPath + ".asset",
				               typeof(MeshInfo)) as MeshInfo;
			foreach (var component in go.GetComponentsInChildren(typeof(Renderer), true))
			{
				var renderer = (Renderer)component;
				var subMeshIndex = 0;
				var gameObject = renderer.gameObject;
				var match = Regex.Match(gameObject.name, @"(?<name>[\w]*)-(?<index>[0-9])*");
				if (!match.Success)
					Debug.Log("Could not match " + gameObject.name + " to [name]-[index]! Assuming single object.");
				else
					Int32.TryParse(match.Groups["index"].Value, out subMeshIndex);

				if (meshInfo == null)
					continue;

				var useVertexColors = meshInfo.GetBool("UseVertexColors", subMeshIndex);
				var pointRendering = meshInfo.GetBool("PointRendering", subMeshIndex);
				var lineRendering = meshInfo.GetBool("LineRendering", subMeshIndex);
				var useTexture = meshInfo.GetBool("UseTexture", subMeshIndex);
				var opacity = 1.0f;
				if (meshInfo.HasFloat("Opacity", subMeshIndex))
					opacity = meshInfo.GetFloat("Opacity", subMeshIndex);
				var solidColor = Color.white;
				if (meshInfo.HasColor("DiffuseColor", subMeshIndex))
					solidColor = meshInfo.GetColor("DiffuseColor", subMeshIndex);
				var matProps = renderer.gameObject.AddComponent<MaterialProperties>();
				matProps.Opacity = opacity;
				matProps.SolidColor = solidColor;
				if (useTexture)
				{
					matProps.ColorBy = MaterialProperties.ColorMode.Texture;

					var modelName = Path.GetFileName(assetPath.Substring(0, assetPath.LastIndexOf(".fbx", StringComparison.Ordinal)));
					var fbmPath = assetPath.Replace(".fbx", ".fbm");
					var tex = (Texture2D)(AssetDatabase.LoadAssetAtPath(fbmPath + "/" + modelName + "-0_vtk_texture.png", typeof(Texture2D)));
					matProps.Texture = tex;
				} else
				{
					matProps.ColorBy = useVertexColors
						? MaterialProperties.ColorMode.VertexColor
						: MaterialProperties.ColorMode.SolidColor;
				}
				if (pointRendering || lineRendering)
					matProps.Lighting = MaterialProperties.LightingMode.Unlit;
				matProps.UpdateRenderers();
				matProps.UpdateShader();
				matProps.SaveState();

				// Convert to points or lines
				if (pointRendering)
					ConvertMeshToPoints(renderer);
				else if (lineRendering)
					ConvertMeshToLines(renderer);

				renderer.gameObject.AddComponent<MeshInfoVtkProperties>().ScriptableObject = meshInfo;
			}

			// Remove Properties-child
			var tmp = go.transform.Find("Properties");
			if (tmp)
				UnityEngine.Object.DestroyImmediate(tmp.gameObject);
		}

		private void OnPostprocessGameObjectWithUserProperties(GameObject go, string[] propNames, System.Object[] values)
		{
			if (!CheckPath())
				return;

			//Debug.Log("OnPostprocessGameObjectWithUserProperties for " + go.name);

			var meshInfo = ScriptableObject.CreateInstance<MeshInfo>();

			for (var i = 0; i < propNames.Length; i++)
			{
				// Parse the properties in the format: [Index]-[Property Name]
				var match = Regex.Match(propNames[i], @"(?<index>[0-9])*-(?<name>[\w]*)");
				if (!match.Success)
					continue;

				int subMeshIndex;
				if (Int32.TryParse(match.Groups["index"].Value, out subMeshIndex))
				{
					if (subMeshIndex >= meshInfo.Properties.Count)
					{
						var newProps = new MeshInfo.PropertyDictionaries {
							SubMeshIndex = subMeshIndex,
							Bools = new Dictionary<string, bool>(),
							Floats = new Dictionary<string, float>(),
							Colors = new Dictionary<string, Color>()
						};
						meshInfo.Properties.Add(newProps);
					}

					var props = meshInfo.Properties[subMeshIndex];
					if (values[i] is bool)
						props.Bools.Add(match.Groups["name"].Value, (bool)values[i]);
					else if (values[i] is float)
						props.Floats.Add(match.Groups["name"].Value, (float)values[i]);
					else if (values[i] is Color)
						props.Colors.Add(match.Groups["name"].Value, (Color)values[i]);
				} else
					Debug.Log("AssetPostprocessor: Property does not match naming convention: index-propname");
			}

			AssetDatabase.CreateAsset(meshInfo, assetPath + ".asset");
			meshInfo.SaveState();

			AssetDatabase.Refresh();
		}

		private Material OnAssignMaterialModel(Material material, Renderer renderer)
		{
			if (!IsCityEngine()) return null;
			
			if (!material.mainTexture || !material.mainTexture.name.Contains("Billboard") ||
			    (int) material.GetFloat("_Mode") == 2) return null;
			
			// Create new materials folder if it not exists
			if (!AssetDatabase.IsValidFolder((Path.GetDirectoryName(assetPath) + "/Materials")))
				AssetDatabase.CreateFolder(Path.GetDirectoryName(assetPath), "Materials");

			// Workaround: sometimes materials have weird names such as CreateFan.. use mainTexture as name
			var materialPath = Path.GetDirectoryName(assetPath) + "/Materials/" + material.mainTexture.name + ".mat";
			if (AssetDatabase.LoadAssetAtPath(materialPath, typeof (Material)))
				return AssetDatabase.LoadAssetAtPath(materialPath, typeof (Material)) as Material;

			SetupStandardMaterialWithBlendMode(material, BlendMode.Fade);

			AssetDatabase.CreateAsset(material, materialPath);
			return material;
		}

		private void OnPreprocessTexture()
		{
			if (!CheckPath())
				return;

			//Debug.Log ("OnPreprocessTexture");
			if (!assetPath.Contains("vtk"))
				return;

			var textureImporter = (TextureImporter)assetImporter;
			textureImporter.filterMode = FilterMode.Point;
			textureImporter.wrapMode = TextureWrapMode.Repeat;
		}

		private void OnPostprocessTexture(Texture2D texture)
		{
			//Debug.Log ("OnPostprocessTexture: " + assetPath);
		}

		private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
		                                           string[] movedFromPath)
		{
			//Debug.Log ("OnPostprocessAllAssets");
		}

		private static void ConvertMeshToPoints(Component renderer)
		{
			var meshFilter = renderer.gameObject.GetComponent<MeshFilter>();
			if (!meshFilter)
			{
				Debug.LogWarning("Could not convert Mesh to points: no MeshFilter found!");
				return;
			}

			var mesh = meshFilter.sharedMesh;
			// Is it already converted to points?
			if (mesh.GetTopology(0) == MeshTopology.Points)
				return;

			mesh.SetIndices(Enumerable.Range(0, mesh.vertices.Length).ToArray(),
				MeshTopology.Points, 0);
			mesh.RecalculateBounds();
		}

		private static void ConvertMeshToLines(Renderer renderer)
		{
			var meshFilter = renderer.gameObject.GetComponent<MeshFilter>();
			if (!meshFilter)
			{
				Debug.LogWarning("Could not convert Mesh to points: no MeshFilter found!");
				return;
			}

			var mesh = meshFilter.sharedMesh;
			// Is it already converted to lines?
			if (mesh.GetTopology(0) == MeshTopology.Lines)
				return;

			// Re-index
			var triangles = mesh.triangles;
			var indicesForLines = new List<int>();
			for (var line = 0; line < (triangles.Length - 3) / 3; line++)
			{
				indicesForLines.Add(triangles[(line * 3 + 0)]);
				indicesForLines.Add(triangles[(line * 3 + 2)]);
			}
			// After 11 hours of work I am too stupid to merge this into the for-loop
			indicesForLines.Add(triangles[triangles.Length - 3]);
			indicesForLines.Add(triangles[triangles.Length - 1]);

			mesh.SetIndices(indicesForLines.ToArray(), MeshTopology.Lines, 0);
			mesh.RecalculateBounds();
		}

		public enum BlendMode
		{
			Opaque,
			Cutout,
			Fade,		// Old school alpha-blending mode, fresnel does not affect amount of transparency
			Transparent // Physically plausible transparency mode, implemented as alpha pre-multiply
		}

		// From StandardShaderGUI.cs in Unity builtin Shader package, https://unity3d.com/get-unity/download/archive
		public static void SetupStandardMaterialWithBlendMode(Material material, BlendMode blendMode)
		{
			switch (blendMode)
			{
				case BlendMode.Opaque:
					material.SetOverrideTag("RenderType", "");
					material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
					material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
					material.SetInt("_ZWrite", 1);
					material.DisableKeyword("_ALPHATEST_ON");
					material.DisableKeyword("_ALPHABLEND_ON");
					material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
					material.renderQueue = -1;
					break;
				case BlendMode.Cutout:
					material.SetOverrideTag("RenderType", "TransparentCutout");
					material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
					material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
					material.SetInt("_ZWrite", 1);
					material.EnableKeyword("_ALPHATEST_ON");
					material.DisableKeyword("_ALPHABLEND_ON");
					material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
					material.renderQueue = 2450;
					break;
				case BlendMode.Fade:
					material.SetOverrideTag("RenderType", "Transparent");
					material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
					material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
					material.SetInt("_ZWrite", 0);
					material.DisableKeyword("_ALPHATEST_ON");
					material.EnableKeyword("_ALPHABLEND_ON");
					material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
					material.renderQueue = 3000;
					break;
				case BlendMode.Transparent:
					material.SetOverrideTag("RenderType", "Transparent");
					material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
					material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
					material.SetInt("_ZWrite", 0);
					material.DisableKeyword("_ALPHATEST_ON");
					material.DisableKeyword("_ALPHABLEND_ON");
					material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
					material.renderQueue = 3000;
					break;
			}
		}
	}
}
