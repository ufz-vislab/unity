using UFZ.Annotations;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UFZ.Annotations
{
// This should be subclassed
//
// Add this script to your object in the scene
//
// Properties can be accessed in this way:
//   MeshInfo info = (MeshInfo)ScriptableObject;
//   if (info)
//   {
//     Debug.Log (info.Properties[0].Bools["UseVertexColors"].ToString ());
//   }
	public class MeshInfoLoaderBase : MonoBehaviour
	{
		public MeshInfo ScriptableObject;

#if UNITY_EDITOR
		private void Reset()
		{
			ScriptableObject = AssetDatabase.LoadAssetAtPath(
				AssetDatabase.GetAssetPath(PrefabUtility.GetPrefabParent(gameObject))
				+ ".asset", typeof (MeshInfo)) as MeshInfo;
		}
#endif

	}
}
