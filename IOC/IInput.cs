using UnityEngine;

namespace UFZ.IOC
{
	public interface IInput
	{
		bool IsOkButtonPressed();
		bool IsCancelButtonPressed();
		bool WasOkButtonPressed();
		bool WasCancelButtonPressed();

		float GetHorizontalAxis();
		float GetVerticalAxis();
	}

	public class UnityInput : IInput
	{
		private const string _submitButton = "Submit";
		private const string _cancelButton = "Cancel";

		public bool IsOkButtonPressed()
		{
			return Input.GetButtonDown(_submitButton);
		}

		public bool IsCancelButtonPressed()
		{
			return Input.GetButtonDown(_cancelButton);
		}

		public bool WasOkButtonPressed()
		{
			return Input.GetButtonUp(_submitButton);
		}

		public bool WasCancelButtonPressed()
		{
			return Input.GetButtonUp(_cancelButton);
		}

		public float GetHorizontalAxis()
		{
			return Input.GetAxis("Horizontal");
		}

		public float GetVerticalAxis()
		{
			return Input.GetAxis("Vertical");
		}
	}

	public class MiddleVrInput : IInput
	{
		private const int _submitButton = 0;
		private const int _cancelButton = 1;

		public bool IsOkButtonPressed()
		{
			var key = MiddleVR.VRDeviceMgr.GetKeyboard().IsKeyToggled(MiddleVR.VRK_RETURN);
			var button = MiddleVR.VRDeviceMgr.IsWandButtonToggled(_submitButton);
			return key || button;
		}

		public bool IsCancelButtonPressed()
		{
			var key = MiddleVR.VRDeviceMgr.GetKeyboard().IsKeyToggled(MiddleVR.VRK_BACK);
			var button = MiddleVR.VRDeviceMgr.IsWandButtonToggled(_cancelButton);
			return key || button;
		}

		public bool WasOkButtonPressed()
		{
			var key = MiddleVR.VRDeviceMgr.GetKeyboard().IsKeyToggled(MiddleVR.VRK_RETURN, false);
			var button = MiddleVR.VRDeviceMgr.IsWandButtonToggled(_submitButton, false);
			return key || button;
		}

		public bool WasCancelButtonPressed()
		{
			var key = MiddleVR.VRDeviceMgr.GetKeyboard().IsKeyToggled(MiddleVR.VRK_BACK, false);
			var button = MiddleVR.VRDeviceMgr.IsWandButtonToggled(_cancelButton, false);
			return key || button;
		}

		public float GetHorizontalAxis()
		{
			return MiddleVR.VRDeviceMgr.GetWandHorizontalAxisValue();
		}

		public float GetVerticalAxis()
		{
			return MiddleVR.VRDeviceMgr.GetWandVerticalAxisValue();
		}

	}
}
