using UnityEngine;
using System.Collections;

namespace UFZ.Menu
{
	public class VrMenuVTK : MonoBehaviour
	{
		private vrWidgetMenu _menu;

		private IEnumerator Start()
		{
			VRMenu middleVrMenu = null;
			while (middleVrMenu == null || middleVrMenu.menu == null)
			{
				// Wait for VRMenu to be created
				yield return null;
				middleVrMenu = FindObjectOfType(typeof (VRMenu)) as VRMenu;
			}

			var sphereSources = FindObjectsOfType(typeof(VTK.SphereSource)) as VTK.SphereSource[];
			if (sphereSources == null || sphereSources.Length == 0)
				yield break;

			_menu = new vrWidgetMenu("VTK Menu", middleVrMenu.menu, "VTK Menu");
			middleVrMenu.menu.SetChildIndex(_menu, 0);

			foreach (var sphereSource in sphereSources)
			{
				var radius = (float) sphereSource.Radius;
				new vrWidgetSlider("SphereSource Radius Slider - " +
					sphereSource.GetInstanceID(), _menu, "Radius",
					sphereSource.SetRadiusCommand, radius, radius, radius*2, radius / 20f);
			}
		}
	}
}
