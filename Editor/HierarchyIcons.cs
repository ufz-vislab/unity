using UnityEngine;
using UnityEditor;
using System.Collections;

namespace UFZ
{
	[InitializeOnLoad]
	public static class HierarchyIcons
	{
		static HierarchyIcons ()
		{
			EditorApplication.hierarchyWindowItemOnGUI += ShowIcons;
			IconStyles.zoneIcon = (Texture2D)Resources.Load ("RecordIcon");
		}

		static void ShowIcons (int ID, Rect r)
		{
			var go = EditorUtility.InstanceIDToObject (ID) as GameObject;
			if (go == null)
				return;
		}
	}
}
