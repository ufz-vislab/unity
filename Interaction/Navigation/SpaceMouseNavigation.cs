using MiddleVR_Unity3D;
using UnityEngine;

namespace UFZ.Interaction
{
	/// <summary>
	/// Space Mouse navigation by using vrpn device with 6 axes and 2 buttons.
	/// </summary>
	public class SpaceMouseNavigation : NavigationBase
	{
		/// <summary>
		/// The VRPN device name.
		/// </summary>
		public string DeviceName = "SpaceMouse";

		void Start()
		{
			DirectionReferenceNode = "HeadNode";
			DeadZone = 0.05f;

			CreateInteraction("SpaceMouseNavigation");
			GetInteraction().AddTag("ContinuousNavigation");
			Activate();
			MVRTools.Log("[ ] Navigation SpaceMouse activated.");
		}

		protected override void GetInputs()
		{
			var axis = MiddleVR.VRDeviceMgr.GetAxis(DeviceName + ".Axis");
			var buttons = MiddleVR.VRDeviceMgr.GetButtons(DeviceName + ".Buttons");

			if(axis == null)
				return;
			Forward = -axis.GetValue(1);
			Sideward = axis.GetValue(0);
			Upward = -axis.GetValue(2);
			//HorizontalRotation = axis.GetValue(3);

			if(buttons == null)
				return;

			Running = buttons.IsPressed(0) ? 1.0f : 0.0f;
		}
	}
}
