namespace UFZ.Interaction
{
	public class InControlNavigation : NavigationBase
	{
		void Start()
		{
			DeadZone = 0.05f;
		}

		protected override void GetInputs()
		{
			var device = InControl.InputManager.ActiveDevice;

			Forward  = device.LeftStickY;
			Sideward = device.LeftStickX;
		    Upward = device.RightStickY;
			HorizontalRotation = device.RightStickX;
			// VerticalRotation = device.RightStickY;
			Running  = device.LeftTrigger + device.RightTrigger;
		}
	}
}
