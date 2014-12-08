using UnityEngine;
using System.Collections;
using MiddleVR_Unity3D;

public class SetEyeDistance : MonoBehaviour
{
	public float eyeDistance = 0.063f; // in m
	private vrCameraStereo stereoCam = null;

	void Start ()
	{
		string camName;
		if (MiddleVR.VRClusterMgr.GetMyClusterNode() != null)
			camName = MiddleVR.VRClusterMgr.GetMyClusterNode().GetViewport().GetCamera().GetName();
		else
			camName = MiddleVR.VRDisplayMgr.GetCameraByIndex().GetName();
		stereoCam = MiddleVR.VRDisplayMgr.GetCameraStereo(camName);

		if (stereoCam != null)
			stereoCam.SetInterEyeDistance(eyeDistance);
	}

	void EyeDistance(float distance)
	{
		// Debug.Log("DistanceSet: " + distance);
		if (stereoCam != null)
		{
			if(distance > 0.0f)
				stereoCam.SetInterEyeDistance(eyeDistance * distance);
			else
				stereoCam.SetInterEyeDistance(0.0f);
		}
	}
}
