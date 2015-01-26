using System;
using System.Collections.Generic;
using System.Linq;
using UFZ.Interaction;
using UnityEngine;
using System.Collections;

public class VrMenuViewpoint : MonoBehaviour
{
	private vrWidgetMenu _menu;

	IEnumerator Start()
	{
		VRMenu middleVrMenu = null;
		while (middleVrMenu == null || middleVrMenu.menu == null)
		{
			yield return null;
			middleVrMenu = FindObjectOfType(typeof(VRMenu)) as VRMenu;
		}

		var viewpointGroup = GameObject.Find("Viewpoints");
		if (viewpointGroup == null)
			yield break;
		if (viewpointGroup.GetComponentsInChildren<Viewpoint>() == null)
			yield break;

		_menu = new vrWidgetMenu("Viewpoints", middleVrMenu.menu);
		middleVrMenu.menu.SetChildIndex(_menu, 0);

		// Iterate over childs to preserve order from editor
		for (var i = 0; i < viewpointGroup.transform.childCount; ++i)
		{
			var child = viewpointGroup.transform.GetChild(i);
			var viewpoint = child.gameObject.GetComponent<Viewpoint>();
			if (viewpoint == null) continue;
			var btn = new vrWidgetButton(viewpoint.name, _menu);
			btn.AddCommand(viewpoint.MoveToViewpointCommand);
		}
	}
}
