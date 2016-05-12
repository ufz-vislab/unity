using UnityEngine;
using System.Collections.Generic;
using MarkLight;
using MarkLight.Views.UI;
using ModestTree;
using UFZ.Interaction;

// [InternalView]
public class ViewpointsView : UIView
{
	public string MenuHeader = "Viewpoints";
	public ObservableList<Viewpoint> Viewpoints;

	public override void Initialize()
	{
		base.Initialize();

		Viewpoints = new ObservableList<Viewpoint>();
		var viewpointGroup = GameObject.Find("Viewpoints");
		if (viewpointGroup == null ||
		    (viewpointGroup.GetComponentsInChildren<Viewpoint>() == null))
			MenuHeader = "No viewpoints";
		else
		{
			// Iterate over childs to preserve order from editor
			for (var i = 0; i < viewpointGroup.transform.childCount; ++i)
			{
				var child = viewpointGroup.transform.GetChild(i);
				var viewpoint = child.gameObject.GetComponent<Viewpoint>();
				if (viewpoint == null) continue;
				Viewpoints.Add(viewpoint);
			}
			// SetChanged(() => Viewpoints);
		}
	}

	public void ViewpointChanged(ItemSelectionActionData eventData)
	{
		Viewpoints.SelectedItem.Move();
	}
}
