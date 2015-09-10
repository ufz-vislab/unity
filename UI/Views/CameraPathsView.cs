using UnityEngine;
using System.Collections.Generic;
using MarkUX;
using MarkUX.Views;
using ModestTree;

[InternalView]
public class CameraPathsView : View
{
	public FlowList PathsFlowList;

	public string MenuHeader = "Cam Paths";
	public List<CameraPathAnimator> CameraPaths;
	public List<StringWrapper> CameraPathNames;
	public CameraPathAnimator SelectedPath;
	public float SelectedPathPosition;

	public override void Initialize()
	{
		base.Initialize();

		CameraPaths = GetPaths();
		SetChanged(() => CameraPaths);
		if (CameraPaths == null || CameraPaths.IsEmpty())
		{
			MenuHeader = "No paths";
			return;
		}

		CameraPathNames = new List<StringWrapper>();
		foreach (var cameraPath in CameraPaths)
			CameraPathNames.Add(new StringWrapper(cameraPath.name));
		SetPathByIndex(0);
	}

	/*
	public void PathChanged(FlowListSelectionEventData eventData)
	{
		SetPathByIndex(eventData.FlowListItem.ZeroBasedIndex);
		SetChanged(() => SelectedPath);
	}
	*/

	private void SetPathByIndex(int index)
	{
		SelectedPath = CameraPaths[index];
		SelectedPathPosition = SelectedPath.percentage;
	}

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

public class StringWrapper
{
	public string Value;

	public StringWrapper(string value)
	{
		Value = value;
	}
}
