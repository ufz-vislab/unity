using UnityEngine;

public class Player : MonoBehaviour
{
	public vrCommand ResetRotationCommand;
	
	void Awake ()
	{
		ResetRotationCommand = new vrCommand("Reset Rotation Command", ResetRotation);
	}

	private vrValue ResetRotation(vrValue iValue)
	{
		var node = MiddleVR.VRDisplayMgr.GetNode("VRSystemCenterNode");
		node.SetOrientationLocal(new vrQuat(node.GetYawLocal(), 0, 0));
		return 0;
	}
}
