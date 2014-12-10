using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Collections;

public class ViewpointController : MonoBehaviour
{
	public string NodeToMove = "VRSystemCenterNode";
	public vrCommand MoveToViewpointCommand;
	public List<GameObject> Viewpoints;
	
	private GameObject _nodeToMove;
	private bool _searchedNodeToMove = false;
	

	void Start()
	{
		MoveToViewpointCommand = new vrCommand("Move To Viewpoint Command", MoveToViewpoint);
	}

	void OnValidate ()
	{
		Viewpoints = new List<GameObject>();
		foreach (var viewpoint in transform.GetComponentsInChildren<Viewpoint>())
		{
			Viewpoints.Add(viewpoint.gameObject);
		}
	}
	
	void Update () 
	{
		var logger = UFZ.IOC.Core.Instance.Log;
		if (_nodeToMove == null)
			_nodeToMove = GameObject.Find(NodeToMove);
		if (_searchedNodeToMove == false && _nodeToMove == null)
		{
			logger.Error("BaseNavigation: Couldn't find '" + NodeToMove + "'");
			_searchedNodeToMove = true;
		}
	}

	public vrValue MoveToViewpoint(vrValue ivalue)
	{
		if (!ivalue.IsNumber())
		{
			print(MethodBase.GetCurrentMethod().Name + ": Requires number!");
			return false;
		}
		var index = ivalue.GetInt();
		if (index > Viewpoints.Count - 1)
			return false;
		var viewpointTransform = Viewpoints[index].transform;

		// TODO calculate speed
		iTween.MoveTo(_nodeToMove, viewpointTransform.position, 5);
		iTween.RotateTo(_nodeToMove, viewpointTransform.rotation.eulerAngles, 5);

		return true;
	}

	public vrValue JumpToViewpoint(vrValue ivalue)
	{
		if (!ivalue.IsNumber())
		{
			print(MethodBase.GetCurrentMethod().Name + ": Requires number!");
			return false;
		}
		var index = ivalue.GetInt();
		if (index > Viewpoints.Count - 1)
			return false;
		var viewpointTransform = Viewpoints[index].transform;
		_nodeToMove.transform.position = viewpointTransform.position;
		_nodeToMove.transform.rotation = viewpointTransform.rotation;
		return true;
	}
}
