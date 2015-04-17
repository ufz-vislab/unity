using FullInspector;
using UnityEditor;
using UnityEngine;

namespace UFZ.VTK
{
	[CustomBehaviorEditor(typeof (ClipDataSet))]
	public class ClipDataSetEditor : tkCustomBehaviorEditor<ClipDataSet>
	{
		protected override void OnSceneGUI(ClipDataSet behavior)
		{
			var direction = Vector3.one;
			switch (behavior.PlaneAxis)
			{
				case Axis.X:
					direction = Vector3.right;
					break;
				case Axis.Y:
					direction = Vector3.down;
					break;
				case Axis.Z:
					direction = Vector3.back;
					break;
			}
			if (behavior.Flip)
				direction *= -1.0f;
			behavior.PlanePosition =
				Handles.Slider(behavior.transform.position + behavior.PlanePosition, direction)
					- behavior.transform.position;
			if (GUI.changed)
				EditorUtility.SetDirty(behavior);
		}

		protected override tkControlEditor GetEditor(ClipDataSet behavior)
		{
			return behavior.GetEditor();
		}
	}
}
