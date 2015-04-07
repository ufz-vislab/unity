using UFZ.Annotations;
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
		private static readonly string[] Paths =
		{
			"Assets/_project",
			"Assets/UFZ/Tests/Objects"
		};

		private static bool CheckPath(string assetPath)
		{
			return Paths.Any(assetPath.Contains);
		}

		private void OnPreprocessModel()
		{
			if (!CheckPath(assetPath))
				return;
			//Debug.Log("OnPreprocessModel");
			var modelImporter = (ModelImporter) assetImporter;
			modelImporter.animationType = ModelImporterAnimationType.None;
			modelImporter.optimizeMesh = false;
			modelImporter.globalScale = 1.0f;
		}

		private void OnPostprocessModel(GameObject go)
		{
			if (!CheckPath(assetPath))
				return;

			//Debug.Log ("OnPostprocessModel for " + go.name);
			var meshInfo = AssetDatabase.LoadAssetAtPath(assetPath + ".asset",
				typeof (MeshInfo)) as MeshInfo;
			foreach (var component in go.GetComponentsInChildren(typeof (Renderer), true))
			{
				var renderer = (Renderer) component;
				var subMeshIndex = 0;
				var gameObject = renderer.gameObject;
				var match = Regex.Match(gameObject.name, @"(?<name>[\w]*)-(?<index>[0-9])*");
				if (!match.Success)
					Debug.Log("Could not match " + gameObject.name + " to [name]-[index]! Assuming single object.");
				else
					Int32.TryParse(match.Groups["index"].Value, out subMeshIndex);

				if (meshInfo == null)
					continue;

				var alpha = 1f;
				var material = renderer.sharedMaterial;
				if (material.HasProperty("_Color") && material.color.a < 1.0f)
					alpha = material.color.a;
				var useVertexColors = meshInfo.GetBool("UseVertexColors", subMeshIndex);
				var pointRendering = meshInfo.GetBool("PointRendering", subMeshIndex);
				var lineRendering = meshInfo.GetBool("LineRendering", subMeshIndex);
				var matProps = renderer.gameObject.AddComponent<MaterialProperties>();
				matProps.Opacity = alpha;
				matProps.ColorBy = useVertexColors
					? MaterialProperties.ColorMode.VertexColor
					: MaterialProperties.ColorMode.SolidColor;
				if (pointRendering || lineRendering)
					matProps.Lighting = MaterialProperties.LightingMode.Unlit;
				matProps.UpdateShader();
				//FullInspector.FullInspectorSaveManager.SaveAll();
				matProps.SaveState();

				// Convert to points or lines
				if (pointRendering) ConvertMeshToPoints(renderer);
				else if (lineRendering) ConvertMeshToLines(renderer);

				renderer.gameObject.AddComponent<MeshInfoVtkProperties>().ScriptableObject = meshInfo;
			}

			// Remove Properties-child
			var tmp = go.transform.Find("Properties");
			if (tmp)
				UnityEngine.Object.DestroyImmediate(tmp.gameObject);
		}

		private void OnPostprocessGameObjectWithUserProperties(GameObject go, string[] propNames, System.Object[] values)
		{
			if (!CheckPath(assetPath))
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
						var newProps = new MeshInfo.PropertyDictionaries
						{
							SubMeshIndex = subMeshIndex,
							Bools = new Dictionary<string, bool>(),
							Floats = new Dictionary<string, float>(),
							Colors = new Dictionary<string, Color>()
						};
						meshInfo.Properties.Add(newProps);
					}

					var props = meshInfo.Properties[subMeshIndex];
					if (values[i] is bool)
						props.Bools.Add(match.Groups["name"].Value, (bool) values[i]);
					else if (values[i] is float)
						props.Floats.Add(match.Groups["name"].Value, (float) values[i]);
					else if (values[i] is Color)
						props.Colors.Add(match.Groups["name"].Value, (Color) values[i]);
				}
				else
					Debug.Log("AssetPostprocessor: Property does not match naming convention: index-propname");
			}

			AssetDatabase.CreateAsset(meshInfo, assetPath + ".asset");
			meshInfo.SaveState();

			AssetDatabase.Refresh();
		}

		private void OnPreprocessTexture()
		{
			if (!CheckPath(assetPath))
				return;

			//Debug.Log ("OnPreprocessTexture");
			if (!assetPath.Contains("vtk"))
				return;

			var textureImporter = (TextureImporter) assetImporter;
			textureImporter.filterMode = FilterMode.Point;
			textureImporter.wrapMode = TextureWrapMode.Clamp;
		}

		private void OnPostprocessTexture(Texture2D texture)
		{
			if (!CheckPath(assetPath))
				return;

			//Debug.Log ("OnPostprocessTexture");

			if (!assetPath.Contains(".fbm"))
				return;
			var modelName = Path.GetFileName(assetPath.Substring(0, assetPath.LastIndexOf(".fbm", StringComparison.Ordinal)));
			var basePath = Path.GetDirectoryName(assetPath.Substring(0, assetPath.LastIndexOf(".fbm", StringComparison.Ordinal)));
			var materialPath = basePath + "/Materials/" + modelName + "_material.mat";
			var material = (Material) (AssetDatabase.LoadAssetAtPath(materialPath, typeof (Material)));
			if (material)
			{
				var tex = (Texture2D) (AssetDatabase.LoadAssetAtPath(assetPath, typeof (Texture2D)));
				if (tex)
				{
					material.mainTexture = tex;
					material.SetColor("_Color", new Color(1f, 1f, 1f, 1f));
				}
				else
				{
					AssetDatabase.Refresh();
					tex = (Texture2D) (AssetDatabase.LoadAssetAtPath(assetPath, typeof (Texture2D)));
					if (!tex)
						return;

					material.mainTexture = tex;
					material.SetColor("_Color", new Color(1f, 1f, 1f, 1f));
				}
			}
			else
				Debug.Log("Error: material not found for " + assetPath);
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
			for (var line = 0; line < (triangles.Length - 3)/3; line++)
			{
				indicesForLines.Add(triangles[(line*3 + 0)]);
				indicesForLines.Add(triangles[(line*3 + 2)]);
			}
			// After 11 hours of work I am too stupid to merge this into the for-loop
			indicesForLines.Add(triangles[triangles.Length - 3]);
			indicesForLines.Add(triangles[triangles.Length - 1]);

			mesh.SetIndices(indicesForLines.ToArray(), MeshTopology.Lines, 0);
			mesh.RecalculateBounds();
		}
	}
}
