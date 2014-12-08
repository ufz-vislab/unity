using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: DPad is not reported by MiddleVR

namespace InControl
{
	public class Xbox360MvrJoystickProfile : UfzInputDeviceProfile
	{
		public Xbox360MvrJoystickProfile()
		{
			Name = "XBox 360 Controller (MVR Joystick)";
			Meta = "XBox 360 Controller on Windows (MVR Joytsick)";

			SupportedPlatforms = new[]
			{
				"Windows"
			};

			JoystickNames = new[]
			{
				"Controller (XBOX 360 For Windows)",
				"Controller (XBOX 360 Wireless Receiver for Windows)",
				"XBOX 360 For Windows (Controller)",
				"Controller (Gamepad F310)",
				"Controller (MLG GamePad for Xbox 360)",
				"Controller (Gamepad for Xbox 360)",
				"Controller (Rock Candy Gamepad for Xbox 360)"
			};

			vrJoystick joy = MiddleVR.VRDeviceMgr.GetJoystick(0);
			if(joy != null)
			{
				Button0 = new MvrJoystickButtonSource(0, joy);
				Button1 = new MvrJoystickButtonSource(1, joy);
				Button2 = new MvrJoystickButtonSource(2, joy);
				Button3 = new MvrJoystickButtonSource(3, joy);
				Button4 = new MvrJoystickButtonSource(4, joy);
				Button5 = new MvrJoystickButtonSource(5, joy);
				Button6 = new MvrJoystickButtonSource(6, joy);
				Button7 = new MvrJoystickButtonSource(7, joy);
				Button8 = new MvrJoystickButtonSource(8, joy);
				Button9 = new MvrJoystickButtonSource(9, joy);
				Button10 = new MvrJoystickButtonSource(10, joy);
				Button11 = new MvrJoystickButtonSource(11, joy);
				Button12 = new MvrJoystickButtonSource(12, joy);
				Button13 = new MvrJoystickButtonSource(13, joy);
				Button14 = new MvrJoystickButtonSource(14, joy);
				Button15 = new MvrJoystickButtonSource(15, joy);
				Button16 = new MvrJoystickButtonSource(16, joy);
				Button17 = new MvrJoystickButtonSource(17, joy);
				Button18 = new MvrJoystickButtonSource(18, joy);
				Button19 = new MvrJoystickButtonSource(19, joy);

				Analog0 = new MvrJoystickAnalogSource(0, joy);
				Analog1 = new MvrJoystickAnalogSource(1, joy);
				Analog2 = new MvrJoystickAnalogSource(2, joy);
				Analog3 = new MvrJoystickAnalogSource(3, joy);
				Analog4 = new MvrJoystickAnalogSource(4, joy);
				Analog5 = new MvrJoystickAnalogSource(5, joy);
				Analog6 = new MvrJoystickAnalogSource(6, joy);
				Analog7 = new MvrJoystickAnalogSource(7, joy);
				Analog8 = new MvrJoystickAnalogSource(8, joy);
				Analog9 = new MvrJoystickAnalogSource(9, joy);
				Analog10 = new MvrJoystickAnalogSource(10, joy);
				Analog11 = new MvrJoystickAnalogSource(11, joy);
				Analog12 = new MvrJoystickAnalogSource(12, joy);
				Analog13 = new MvrJoystickAnalogSource(13, joy);
				Analog14 = new MvrJoystickAnalogSource(14, joy);
				Analog15 = new MvrJoystickAnalogSource(15, joy);
				Analog16 = new MvrJoystickAnalogSource(16, joy);
				Analog17 = new MvrJoystickAnalogSource(17, joy);
				Analog18 = new MvrJoystickAnalogSource(18, joy);
				Analog19 = new MvrJoystickAnalogSource(19, joy);
			}
			else
				Debug.LogWarning("MiddleVR joystick not found!");

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
				},
				new InputControlMapping
				{
					Handle = "Left Stick Button",
					Target = InputControlType.LeftStickButton,
					Source = Button8
				},
				new InputControlMapping
				{
					Handle = "Right Stick Button",
					Target = InputControlType.RightStickButton,
					Source = Button9
				},
				new InputControlMapping
				{
					Handle = "Back",
					Target = InputControlType.Select,
					Source = Button6
				},
				new InputControlMapping
				{
					Handle = "Start",
					Target = InputControlType.Start,
					Source = Button7
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
					Invert = true
				},
				new InputControlMapping
				{
					Handle = "Right Stick X",
					Target = InputControlType.RightStickX,
					Source = Analog3
				},
				new InputControlMapping
				{
					Handle = "Right Stick Y",
					Target = InputControlType.RightStickY,
					Source = Analog4,
					Invert = true
				},
				new InputControlMapping
				{
					Handle = "DPad Left",
					Target = InputControlType.DPadLeft,
					Source = Button13
					//Source = Analog5,
					//SourceRange = InputControlMapping.Range.Negative,
					//TargetRange = InputControlMapping.Range.Negative,
					//Invert = true
				},
				new InputControlMapping
				{
					Handle = "DPad Right",
					Target = InputControlType.DPadRight,
					Source = Button11
					//Source = Analog5,
					//SourceRange = InputControlMapping.Range.Positive,
					//TargetRange = InputControlMapping.Range.Positive
				},
				new InputControlMapping
				{
					Handle = "DPad Up",
					Target = InputControlType.DPadUp,
					Source = Button10
					//Source = Analog6,
					//SourceRange = InputControlMapping.Range.Positive,
					//TargetRange = InputControlMapping.Range.Positive
				},
				new InputControlMapping
				{
					Handle = "DPad Down",
					Target = InputControlType.DPadDown,
					Source = Button12
					//Source = Analog6,
					//SourceRange = InputControlMapping.Range.Negative,
					//TargetRange = InputControlMapping.Range.Negative,
					//Invert = true
				},
				new InputControlMapping
				{
					Handle = "Left Trigger",
					Target = InputControlType.LeftTrigger,
					Source = Analog2,
					SourceRange = InputControlMapping.Range.Positive,
					TargetRange = InputControlMapping.Range.Positive
				},
				new InputControlMapping
				{
					Handle = "Right Trigger",
					Target = InputControlType.RightTrigger,
					Source = Analog2,
					SourceRange = InputControlMapping.Range.Negative,
					TargetRange = InputControlMapping.Range.Negative,
					Invert = true
				}
			};
		}
	}
}

