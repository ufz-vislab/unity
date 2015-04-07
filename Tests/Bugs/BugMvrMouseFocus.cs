using UnityEngine;

namespace UFZ.Tests.Bugs
{
	public class MvrMouseFocus : MonoBehaviour
	{
		private void Update()
		{
			Debug.Log("MVR Mouse 0: " + MiddleVR.VRDeviceMgr.GetMouse().IsButtonPressed(0));
			Debug.Log("MVR Mouse 0 up: " + MiddleVR.VRDeviceMgr.GetMouse().IsButtonToggled(0, false));
		}

		private void OnApplicationFocus(bool focusStatus)
		{
			if (!focusStatus)
			{
				var dmgr = MiddleVR.VRDeviceMgr;
				var mouse = dmgr.GetDevice("Mouse");
				Debug.Log(mouse);
				MiddleVR.VRDeviceMgr.Dispose();
				//dmgr.RemoveDevice(mouse);
				//dmgr.DestroyDevice(mouse);
				//dmgr.AddDevice(mouse);
				//dmgr.AddDevice(dmgr.CreateMouse("Mouse"));
			}
		}
	}
}
