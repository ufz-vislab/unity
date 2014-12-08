using System;
using System.Collections;
using UnityEngine;
using InControl;


namespace InControl
{
	// This custom profile is enabled by adding it to the Custom Profiles list
	// on the InControlManager component, or you can attach it yourself like so:
	// InputManager.AttachDevice( new UnityInputDevice( "KeyboardAndMouseProfile" ) );
	// TODO: Mouse
	public class UfzKeyboardAndMouseProfile : UnityInputDeviceProfile
	{
		public UfzKeyboardAndMouseProfile()
		{
			Name = "Keyboard/Mouse";
			Meta = "A keyboard and mouse combination profile appropriate for FPS.";

			// This profile only works on desktops.
			SupportedPlatforms = new[]
			{
				"Windows",
				"Mac",
				"Linux"
			};

			Sensitivity = 1.0f;
			LowerDeadZone = 0.0f;
			UpperDeadZone = 1.0f;

			ButtonMappings = new[]
			{
				new InputControlMapping
				{
					Handle = "OK",
					Target = InputControlType.Action1,
					Source = new UfzKeyCodeSource( KeyCode.Return )
				},
				new InputControlMapping
				{
					Handle = "Back",
					Target = InputControlType.Action2,
					Source = new UfzKeyCodeSource( KeyCode.Backspace )
				}
			};

			AnalogMappings = new[]
			{
				new InputControlMapping
				{
					Handle = "Move X",
					Target = InputControlType.LeftStickX,
					// KeyCodeAxis splits the two KeyCodes over an axis. The first is negative, the second positive.
					Source = new UfzKeyCodeAxisSource( KeyCode.A, KeyCode.D )
				},
				new InputControlMapping
				{
					Handle = "Move Y",
					Target = InputControlType.LeftStickY,
					// Notes that up is positive in Unity, therefore the order of KeyCodes is down, up.
					Source = new UfzKeyCodeAxisSource( KeyCode.S, KeyCode.W )
				},
				new InputControlMapping
				{
					Handle = "Move Z",
					Target = InputControlType.RightStickY,
					Source = new UfzKeyCodeAxisSource( KeyCode.DownArrow, KeyCode.UpArrow )
				},
				new InputControlMapping
				{
					Handle = "Look X",
					Target = InputControlType.RightStickX,
					Source = new UfzKeyCodeAxisSource( KeyCode.LeftArrow, KeyCode.RightArrow )
				}
			};
		}
	}
}

