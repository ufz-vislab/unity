using System;
using UnityEngine;


namespace InControl
{
	public class MvrJoystickButtonSource : InputControlSource
	{
		readonly uint buttonId;
		readonly vrJoystick joystick;

		public MvrJoystickButtonSource( uint buttonId, vrJoystick joystick )
		{
			this.buttonId = buttonId;
			this.joystick = joystick;
		}

		public float GetValue( InputDevice inputDevice )
		{
			return GetState( inputDevice ) ? 1.0f : 0.0f;
		}

		public bool GetState( InputDevice inputDevice )
		{
			if(joystick != null)
				return joystick.IsButtonPressed(buttonId);
			return false;
		}
	}
}
