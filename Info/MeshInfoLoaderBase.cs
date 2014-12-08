using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using FullInspector;

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
public class MeshInfoLoaderBase : BaseBehavior
{
	public MeshInfo ScriptableObject;

#if UNITY_EDITOR
	void Reset()
	{
		ScriptableObject = AssetDatabase.LoadAssetAtPath(
			AssetDatabase.GetAssetPath(PrefabUtility.GetPrefabParent(gameObject))
			+ ".asset", typeof(MeshInfo)) as MeshInfo;
	}
#endif

}
