using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

public class MeshImportSettings : AssetPostprocessor {

	void OnPreprocessModel ()
	{
		if(assetPath.Contains("Assets/Libs"))
			return;
		//Debug.Log("OnPreprocessModel");
		ModelImporter modelImporter = (ModelImporter)assetImporter;
		modelImporter.animationType = ModelImporterAnimationType.None;
		modelImporter.optimizeMesh = false;
		modelImporter.globalScale = 1.0f;
	}

	void OnPostprocessModel (GameObject go)
	{
		if(assetPath.Contains("Assets/Libs"))
			return;

		//Debug.Log ("OnPostprocessModel for " + go.name);
		MeshInfo meshInfo = AssetDatabase.LoadAssetAtPath(assetPath + ".asset",
			typeof(MeshInfo)) as MeshInfo;
		foreach(Renderer renderer in go.GetComponentsInChildren(typeof(Renderer), true))
		{
			int subMeshIndex = 0;
			GameObject gameObject = renderer.gameObject;
			var match = Regex.Match(gameObject.name, @"(?<name>[\w]*)-(?<index>[0-9])*");
			if(!match.Success)
				Debug.Log("Could not match " + gameObject.name + " to [name]-[index]! Assuming single object.");
			else
				Int32.TryParse(match.Groups["index"].Value, out subMeshIndex);

			if(meshInfo == null)
				continue;

			float alpha = 1f;
			Material material = renderer.sharedMaterial;
			if (material.HasProperty("_Color") && material.color.a < 1.0f)
				alpha = material.color.a;
			bool useVertexColors = meshInfo.GetBool("UseVertexColors", subMeshIndex);
			bool pointRendering = meshInfo.GetBool("PointRendering", subMeshIndex);
			bool lineRendering = meshInfo.GetBool("LineRendering", subMeshIndex);
			MaterialProperties matProps = renderer.gameObject.AddComponent<MaterialProperties>();
			matProps.Opacity = alpha;
			matProps.ColorBy = useVertexColors ? MaterialProperties.ColorMode.VertexColor
				: MaterialProperties.ColorMode.SolidColor;
			if(pointRendering || lineRendering)
				matProps.Lighting = MaterialProperties.LightingMode.Unlit;
			matProps.UpdateShader();
			FullInspector.FullInspectorSaveManager.SaveAll();
			matProps.SaveState();

			// Convert to points or lines
			if(pointRendering) convertMeshToPoints(renderer);
			else if (lineRendering) convertMeshToLines(renderer);

			renderer.gameObject.AddComponent<MeshInfoVtkProperties>().ScriptableObject = meshInfo;
		}

		// Remove Properties-child
		Transform tmp = go.transform.Find("Properties");
		if(tmp)
			UnityEngine.Object.DestroyImmediate(tmp.gameObject);
	}

	void OnPostprocessGameObjectWithUserProperties (GameObject go, string[] propNames, System.Object[] values)
	{
		if(assetPath.Contains("Assets/Libs"))
			return;

		//Debug.Log("OnPostprocessGameObjectWithUserProperties for " + go.name);

		MeshInfo meshInfo = ScriptableObject.CreateInstance<MeshInfo> ();

		for (int i = 0; i < propNames.Length; i++)
		{
			// Parse the properties in the format: [Index]-[Property Name]
			var match = Regex.Match(propNames[i], @"(?<index>[0-9])*-(?<name>[\w]*)");
			if(!match.Success)
				continue;

			int subMeshIndex = -1;
			if(Int32.TryParse(match.Groups["index"].Value, out subMeshIndex))
			{
				if(subMeshIndex >= meshInfo.Properties.Count)
				{
					MeshInfo.PropertyDictionaries newProps = new MeshInfo.PropertyDictionaries();
					newProps.SubMeshIndex = subMeshIndex;
					newProps.Bools = new Dictionary<string, bool> ();
					newProps.Floats = new Dictionary<string, float> ();
					newProps.Colors = new Dictionary<string, Color> ();
					meshInfo.Properties.Add(newProps);
				}

				MeshInfo.PropertyDictionaries props = meshInfo.Properties[subMeshIndex];
				if (values [i].GetType () == typeof(bool))
					props.Bools.Add(match.Groups["name"].Value, (bool)values [i]);
				else if (values [i].GetType () == typeof(float))
					props.Floats.Add(match.Groups["name"].Value, (float)values [i]);
				else if (values [i].GetType () == typeof(Color))
					props.Colors.Add(match.Groups["name"].Value, (Color)values [i]);
			}
			else
				Debug.Log("AssetPostprocessor: Property does not match naming convention: index-propname");
		}

		AssetDatabase.CreateAsset(meshInfo, assetPath + ".asset");
		meshInfo.SaveState();

		AssetDatabase.Refresh ();
	}

	void OnPreprocessTexture ()
	{
		if(assetPath.Contains("Assets/Libs"))
			return;

		//Debug.Log ("OnPreprocessTexture");
		if (assetPath.Contains ("vtk"))
		{
			TextureImporter textureImporter = (TextureImporter)assetImporter;
			textureImporter.filterMode = FilterMode.Point;
			textureImporter.wrapMode = TextureWrapMode.Clamp;
		}
	}

	void OnPostprocessTexture (Texture2D texture)
	{
		if(assetPath.Contains("Assets/Libs"))
			return;

		//Debug.Log ("OnPostprocessTexture");

		if (assetPath.Contains (".fbm"))
		{
			string modelName = Path.GetFileName (assetPath.Substring (0, assetPath.LastIndexOf (".fbm")));
			string basePath = Path.GetDirectoryName (assetPath.Substring (0, assetPath.LastIndexOf (".fbm")));
			string materialPath = basePath + "/Materials/" + modelName + "_material.mat";
			Material material = (Material)(AssetDatabase.LoadAssetAtPath (materialPath, typeof(Material)));;
			if (material) {
				Texture2D tex = (Texture2D)(AssetDatabase.LoadAssetAtPath (assetPath, typeof(Texture2D)));
				if (tex)
				{
					material.mainTexture = tex;
					material.SetColor("_Color", new Color(1f, 1f, 1f, 1f));
				}
				else
				{
					AssetDatabase.Refresh ();
					tex = (Texture2D)(AssetDatabase.LoadAssetAtPath (assetPath, typeof(Texture2D)));
					if (tex)
					{
						material.mainTexture = tex;
						material.SetColor("_Color", new Color(1f, 1f, 1f, 1f));
					}
				}
			}
			else
				Debug.Log ("Error: material not found for " + assetPath);
		}
	}

	private static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
	{
		//Debug.Log ("OnPostprocessAllAssets");
	}

	void convertMeshToPoints(Renderer renderer)
	{
		MeshFilter meshFilter = renderer.gameObject.GetComponent<MeshFilter>();
		if(!meshFilter)
		{
			Debug.LogWarning("Could not convert mesh to points: no MeshFilter found!");
			return;
		}

		Mesh mesh = meshFilter.sharedMesh;
		// Is it already converted to points?
		if(mesh.GetTopology(0) == UnityEngine.MeshTopology.Points)
			return;

		mesh.SetIndices(Enumerable.Range(0, mesh.vertices.Length).ToArray(),
			UnityEngine.MeshTopology.Points, 0);
		mesh.RecalculateBounds();
	}

	void convertMeshToLines(Renderer renderer)
	{
		MeshFilter meshFilter = renderer.gameObject.GetComponent<MeshFilter>();
		if(!meshFilter)
		{
			Debug.LogWarning("Could not convert mesh to points: no MeshFilter found!");
			return;
		}

		Mesh mesh = meshFilter.sharedMesh;
		// Is it already converted to lines?
		if(mesh.GetTopology(0) == UnityEngine.MeshTopology.Lines)
			return;

		// Re-index
		int[] triangles = mesh.triangles;
		List<int> indicesForLines = new List<int>();
		for(int line = 0; line < (triangles.Length-3) / 3; line++)
		{
			indicesForLines.Add(triangles[(line*3 + 0)]);
			indicesForLines.Add(triangles[(line*3 + 2)]);
		}
		// After 11 hours of work I am too stupid to merge this into the for-loop
		indicesForLines.Add(triangles[triangles.Length-3]);
		indicesForLines.Add(triangles[triangles.Length-1]);

		mesh.SetIndices(indicesForLines.ToArray(), UnityEngine.MeshTopology.Lines, 0);
		mesh.RecalculateBounds();
	}
}
