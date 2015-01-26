using UnityEngine;

/// <summary>
/// Creates a menu navigatable camera viewpoint. It is important that the GameObject where
/// this script is attached to is a child of a GameObject named "Viewpoints"!
/// </summary>
public class Viewpoint : MonoBehaviour
{
	public string NodeToMove = "VRSystemCenterNode";
	public vrCommand MoveToViewpointCommand;
	public vrCommand JumpToViewpointCommand;

	private GameObject _nodeToMove;
	private bool _searchedNodeToMove = false;

	void Start ()
	{
		MoveToViewpointCommand = new vrCommand("Move To Viewpoint Command " + GetInstanceID(), MoveToViewpoint);
		JumpToViewpointCommand = new vrCommand("Jump To Viewpoint Command " + GetInstanceID(), JumpToViewpoint);
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
		// TODO calculate speed
		iTween.MoveTo(_nodeToMove, transform.position, 5);
		iTween.RotateTo(_nodeToMove, transform.rotation.eulerAngles, 5);
		return true;
	}

	public vrValue JumpToViewpoint(vrValue ivalue)
	{
		_nodeToMove.transform.position = transform.position;
		_nodeToMove.transform.rotation = transform.rotation;
		return true;
	}
}
