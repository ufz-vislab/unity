using UnityEngine;
using UnityEditor;
using System.Collections;

public class WizardTwoSidedMaterials : ScriptableWizard
{
	[MenuItem ("UFZ/Two Sided Material")]
	static void CreateWizard()
	{
		var gos = Selection.gameObjects;
		foreach(GameObject go in gos)
		{
			foreach(MeshRenderer renderer in go.GetComponentsInChildren<MeshRenderer>())
			{
				if(renderer.sharedMaterials.Length > 1)
					continue;

				Material frontMaterial = renderer.sharedMaterial;
				var frontMaterialPath = AssetDatabase.GetAssetPath(frontMaterial);
				var backMaterialPath = frontMaterialPath.Insert(frontMaterialPath.Length - 4, "_back");
				var frontMaterialShaderName = frontMaterial.shader.name;
				var backMaterialShaderName = frontMaterialShaderName.Replace("Front", "Back");
				var backMaterialShader = Shader.Find(backMaterialShaderName);
				if(!backMaterialShader)
				{
					Debug.LogWarning("No shader with name " + backMaterialShaderName + " found!");
					continue;
				}

				Material[] mats = new Material[2];
				mats[0] = frontMaterial;
				var backMaterial = new Material(backMaterialShader);
				var backMatAsset = AssetDatabase.LoadAssetAtPath(backMaterialPath, typeof(Material)) as Material;
				if(!backMatAsset)
				{
					AssetDatabase.CreateAsset(backMaterial, backMaterialPath);
					mats[1] = backMaterial;
				}
				else
					mats[1] = backMatAsset;

				renderer.sharedMaterials = mats;

				MaterialProperties matProps = renderer.gameObject.GetComponent<MaterialProperties>();
				if(matProps)
					matProps.UpdateProperties();
			}
		}
	}
}
