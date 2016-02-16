using UnityEngine;
using System.Collections.Generic;
using MarkUX;
using MarkUX.Views;
using ModestTree;
using UFZ.Interaction;

[InternalView]
public class ViewpointsView : View
{
	public string MenuHeader = "Viewpoints";
	public List<Viewpoint> Viewpoints;

	public override void Initialize()
	{
		base.Initialize();

		Viewpoints = GetViewpoints();
		SetChanged(() => Viewpoints);
		if (Viewpoints == null || Viewpoints.IsEmpty())
			MenuHeader = "No viewpoints";
	}


	public void ViewpointChanged(FlowListSelectionActionData eventData)
	{
		Viewpoints[eventData.FlowListItem.ZeroBasedIndex].Move();
	}


	private static List<Viewpoint> GetViewpoints()
	{
		var viewpointGroup = GameObject.Find("Viewpoints");
		if (viewpointGroup == null)
			return null;
		if (viewpointGroup.GetComponentsInChildren<Viewpoint>() == null)
			return null;

		var viewpoints = new List<Viewpoint>();

		// Iterate over childs to preserve order from editor
		for (var i = 0; i < viewpointGroup.transform.childCount; ++i)
		{
			var child = viewpointGroup.transform.GetChild(i);
			var viewpoint = child.gameObject.GetComponent<Viewpoint>();
			if (viewpoint == null) continue;
			viewpoints.Add(viewpoint);
		}

		return viewpoints;
	}
}
