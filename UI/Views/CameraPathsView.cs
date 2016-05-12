using UnityEngine;
using System.Collections.Generic;
using MarkLight;
using MarkLight.Views.UI;
using ModestTree;

//[InternalView]
public class CameraPathsView : UIView
{
	public List PathsFlowList;

	public string MenuHeader = "Cam Paths";
	public ObservableList<CameraPathAnimator> CameraPaths;

	public override void Initialize()
	{
		base.Initialize();
		
		//SetChanged(() => CameraPaths);
		if (CameraPaths == null || CameraPaths.IsEmpty())
		{
			MenuHeader = "No paths";
			return;
		}
		CameraPaths = new ObservableList<CameraPathAnimator>();
		foreach (var path in GetPaths())
			CameraPaths.Add(path);
	}

	/*
	public void PathChanged(FlowListSelectionEventData eventData)
	{
		SetPathByIndex(eventData.FlowListItem.ZeroBasedIndex);
		SetChanged(() => SelectedPath);
	}
	*/

	private static List<CameraPathAnimator> GetPaths()
	{
		var pathGroup = GameObject.Find("CameraPaths");
		if (pathGroup == null)
			return null;
		if (pathGroup.GetComponentsInChildren<CameraPathAnimator>() == null)
			return null;

		var paths = new List<CameraPathAnimator>();

		// Iterate over childs to preserve order from editor
		for (var i = 0; i < pathGroup.transform.childCount; ++i)
		{
			var child = pathGroup.transform.GetChild(i);
			var path = child.gameObject.GetComponent<CameraPathAnimator>();
			if (path == null) continue;
			paths.Add(path);
		}

		return paths;
	}
}
