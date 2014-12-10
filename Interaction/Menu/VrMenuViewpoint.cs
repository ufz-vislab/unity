using System;
using System.Collections.Generic;
using System.Linq;
using UFZ.Interaction;
using UnityEngine;
using System.Collections;

public class VrMenuViewpoint : MonoBehaviour
{
	private vrWidgetMenu _menu;

	// Start waits on VRMenu creation with a coroutine
	IEnumerator Start()
	{
		VRMenu middleVrMenu = null;
		while (middleVrMenu == null || middleVrMenu.menu == null)
		{
			// Wait for VRMenu to be created
			yield return null;
			middleVrMenu = FindObjectOfType(typeof(VRMenu)) as VRMenu;
		}

		var viewpointController = FindObjectOfType<ViewpointController>();

		if(viewpointController == null)
			yield break;

		_menu = new vrWidgetMenu("Viewpoints", middleVrMenu.menu);
		middleVrMenu.menu.SetChildIndex(_menu, 0);

		foreach (var go in viewpointController.Viewpoints)
		{
			var btn = new vrWidgetButton(go.name, _menu);
			btn.AddCommand(viewpointController.MoveToViewpointCommand);
			// TODO pass the button index as argument to the command
		}

		// End coroutine
		yield break;
	}
}
