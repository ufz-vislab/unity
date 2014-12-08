using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;


namespace InControl
{
	public class UfzInputDeviceProfile : UnityInputDeviceProfile
	{
		protected InputControlSource[] Buttons;
		protected InputControlSource[] Analogs;

		public UfzInputDeviceProfile()
		{
			InitButtonsAndAnalogs("");
		}

		public UfzInputDeviceProfile(string deviceName)
		{
			InitButtonsAndAnalogs(deviceName);
		}

		private void InitButtonsAndAnalogs(string deviceName)
		{
			Buttons = new InputControlSource[20];
			Analogs = new InputControlSource[20];

			for (uint i = 0; i < 20; ++i)
			{
				Buttons[i] = Button(i, deviceName);
				Analogs[i] = Analog(i, deviceName);
			}

			Button0 = Buttons[0];
			Button1 = Buttons[1];
			Button2 = Buttons[2];
			Button3 = Buttons[3];
			Button4 = Buttons[4];
			Button5 = Buttons[5];
			Button6 = Buttons[6];
			Button7 = Buttons[7];
			Button8 = Buttons[8];
			Button9 = Buttons[9];
			Button10 = Buttons[10];
			Button11 = Buttons[11];
			Button12 = Buttons[12];
			Button13 = Buttons[13];
			Button14 = Buttons[14];
			Button15 = Buttons[15];
			Button16 = Buttons[16];
			Button17 = Buttons[17];
			Button18 = Buttons[18];
			Button19 = Buttons[19];

			Analog0 = Analogs[0];
			Analog1 = Analogs[1];
			Analog2 = Analogs[2];
			Analog3 = Analogs[3];
			Analog4 = Analogs[4];
			Analog5 = Analogs[5];
			Analog6 = Analogs[6];
			Analog7 = Analogs[7];
			Analog8 = Analogs[8];
			Analog9 = Analogs[9];
			Analog10 = Analogs[10];
			Analog11 = Analogs[11];
			Analog12 = Analogs[12];
			Analog13 = Analogs[13];
			Analog14 = Analogs[14];
			Analog15 = Analogs[15];
			Analog16 = Analogs[16];
			Analog17 = Analogs[17];
			Analog18 = Analogs[18];
			Analog19 = Analogs[19];

			MouseButton0     = new UfzMouseButtonSource( 0 );
			MouseButton1     = new UfzMouseButtonSource( 1 );
			MouseButton2     = new UfzMouseButtonSource( 2 );

			MouseXAxis       = new UfzMouseAxisSource( "x" );
			MouseYAxis       = new UfzMouseAxisSource( "y" );
			MouseScrollWheel = new UfzMouseAxisSource( "z" );
		}

		protected static InputControlSource Button( uint index, string deviceName )
		{
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
			vrButtons buttons = MiddleVR.VRDeviceMgr.GetButtons(deviceName + ".Buttons");
			if(buttons != null)
				return new MvrButtonSource( index, buttons );
			else
#endif
				return null;
		}

		protected static InputControlSource Analog( uint index, string deviceName )
		{
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
			vrAxis analogs = MiddleVR.VRDeviceMgr.GetAxis(deviceName + ".Axis");
			if(analogs != null)
				return new MvrAnalogSource( index, analogs );
			else
#endif
				return null;
		}

		protected new static InputControlSource KeyCodeButton( params KeyCode[] keyCodeList )
		{
			return new UfzKeyCodeSource( keyCodeList );
		}

		protected new static InputControlSource KeyCodeAxis( KeyCode negativeKeyCode, KeyCode positiveKeyCode )
		{
			return new UfzKeyCodeAxisSource( negativeKeyCode, positiveKeyCode );
		}
	}
}

