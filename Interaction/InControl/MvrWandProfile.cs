using UnityEngine;

namespace InControl
{
	public class MvrWandProfile : UnityInputDeviceProfile
	{
		public MvrWandProfile()
		{
			Initialize(0);
		}
		public MvrWandProfile(uint wandIndex)
		{
			Initialize(wandIndex);
		}

		private void Initialize(uint wandIndex)
		{
			vrWand wand = MiddleVR.VRDeviceMgr.GetWand(wandIndex);
			if (wand == null)
			{
				Debug.LogError("MiddleVR Wand " + wandIndex + " not found!");
				return;
			}
			Name = "MiddleVR Wand";
			Meta = "MiddleVR Wand on Windows";

			SupportedPlatforms = new[]
			{
				"Windows"
			};

			JoystickNames = new[]
			{
				"MiddleVR Wand"
			};

			Button0 = new MvrWandButtonSource(0, wand);
			Button1 = new MvrWandButtonSource(1, wand);
			Button2 = new MvrWandButtonSource(2, wand);
			Button3 = new MvrWandButtonSource(3, wand);
			Button4 = new MvrWandButtonSource(4, wand);
			Button5 = new MvrWandButtonSource(5, wand);

			Analog0 = new MvrWandAnalogSource(0, wand);
			Analog1 = new MvrWandAnalogSource(1, wand);

			Sensitivity = 1.0f;
			LowerDeadZone = 0.2f;

			ButtonMappings = new[]
			{
				new InputControlMapping
				{
					Handle = "A",
					Target = InputControlType.Action1,
					Source = Button0
				},
				new InputControlMapping
				{
					Handle = "B",
					Target = InputControlType.Action2,
					Source = Button1
				},
				new InputControlMapping
				{
					Handle = "X",
					Target = InputControlType.Action3,
					Source = Button2
				},
				new InputControlMapping
				{
					Handle = "Y",
					Target = InputControlType.Action4,
					Source = Button3
				},
				new InputControlMapping
				{
					Handle = "Left Bumper",
					Target = InputControlType.LeftBumper,
					Source = Button4
				},
				new InputControlMapping
				{
					Handle = "Right Bumper",
					Target = InputControlType.RightBumper,
					Source = Button5
				}
			};

			AnalogMappings = new[]
			{
				new InputControlMapping
				{
					Handle = "Left Stick X",
					Target = InputControlType.LeftStickX,
					Source = Analog0
				},
				new InputControlMapping
				{
					Handle = "Left Stick Y",
					Target = InputControlType.LeftStickY,
					Source = Analog1,
				}
			};
		}
	}
}

