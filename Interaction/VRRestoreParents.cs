using UnityEngine;
using System.Collections;

public class VRRestoreParents : MonoBehaviour
{
	private Transform _parent;
	void Start ()
	{
		_parent = transform.parent;
	}

	protected void OnMVRWandButtonReleased(VRSelection iSelection)
	{
		transform.parent = _parent;
	}
}
