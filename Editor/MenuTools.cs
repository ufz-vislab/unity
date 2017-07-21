using UnityEditor;
using UnityEngine;

namespace UFZ.Editor
{
	public class MenuTools
	{
		[MenuItem("UFZ/Tools/Unhide All")]
		static void ShowAll()
		{
			var allObjects = GameObject.FindObjectsOfType<GameObject>();
			foreach (var g in allObjects)
			{
				g.hideFlags = g.hideFlags & ~HideFlags.HideInHierarchy;
			}
		}
	}
}
