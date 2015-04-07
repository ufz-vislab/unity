using UnityEngine;
using UnityEditor;

namespace UFZ.Rendering
{
	public class WizardTwoSidedMaterials : ScriptableWizard
	{
		[MenuItem("UFZ/Two Sided Material")]
		private static void CreateWizard()
		{
			var gos = Selection.gameObjects;
			foreach (var go in gos)
			{
				foreach (var renderer in go.GetComponentsInChildren<MeshRenderer>())
				{
					if (renderer.sharedMaterials.Length > 1)
						continue;

					var frontMaterial = renderer.sharedMaterial;
					var frontMaterialPath = AssetDatabase.GetAssetPath(frontMaterial);
					var backMaterialPath = frontMaterialPath.Insert(frontMaterialPath.Length - 4, "_back");
					var frontMaterialShaderName = frontMaterial.shader.name;
					var backMaterialShaderName = frontMaterialShaderName.Replace("Front", "Back");
					var backMaterialShader = Shader.Find(backMaterialShaderName);
					if (!backMaterialShader)
					{
						Debug.LogWarning("No shader with name " + backMaterialShaderName + " found!");
						continue;
					}

					var mats = new Material[2];
					mats[0] = frontMaterial;
					var backMaterial = new Material(backMaterialShader);
					var backMatAsset = AssetDatabase.LoadAssetAtPath(backMaterialPath, typeof (Material)) as Material;
					if (!backMatAsset)
					{
						AssetDatabase.CreateAsset(backMaterial, backMaterialPath);
						mats[1] = backMaterial;
					}
					else
						mats[1] = backMatAsset;

					renderer.sharedMaterials = mats;

					var matProps = renderer.gameObject.GetComponent<MaterialProperties>();
					if (matProps)
						matProps.UpdateProperties();
				}
			}
		}
	}
}
