using UnityEngine;
using UnityEditor;

//  namespace UFZ.Editor
//  {
class ShowHideWireFrame : MonoBehaviour
{
	[MenuItem("UFZ/Wireframe/Show WireFrame %s")]
	static void Show()
	{
		foreach (var s in Selection.gameObjects)
		{
			var rends = s.GetComponentsInChildren<Renderer>();
			foreach (var r in rends)
				EditorUtility.SetSelectedWireframeHidden(r, false);
		}
	}


	[MenuItem("UFZ/Wireframe/Show WireFrame %s", true)]
	static bool CheckShow()
	{
		return Selection.activeGameObject != null;
	}


	[MenuItem("UFZ/Wireframe/Hide WireFrame %h")]
	static void Hide()
	{
		foreach (var h in Selection.gameObjects)
		{
			var rends = h.GetComponentsInChildren<Renderer>();

			foreach (var r in rends)
				EditorUtility.SetSelectedWireframeHidden(r, true);
		}
	}


	[MenuItem("UFZ/Wireframe/Hide WireFrame %h", true)]
	static bool CheckHide()
	{
		return Selection.activeGameObject != null;
	}
}
//  }