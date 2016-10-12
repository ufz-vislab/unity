using FullInspector;
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace UFZ.VTK
{
	[CustomBehaviorEditor(typeof(VtkSphereSource))]
	class VtkSphereSourceEditor : tkCustomBehaviorEditor<VtkSphereSource>
	{
		protected override void OnSceneGUI(VtkSphereSource behavior)
		{
			behavior.Radius = Handles.RadiusHandle(
				Quaternion.identity,
				behavior.transform.position,
				behavior.Radius);
		}

		protected override tkControlEditor GetEditor(VtkSphereSource behavior)
		{
			return behavior.GetEditor();
		}
	}
}
