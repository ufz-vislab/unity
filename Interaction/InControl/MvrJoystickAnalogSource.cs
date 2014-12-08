using System;
using UnityEngine;


namespace InControl
{
	public class MvrJoystickAnalogSource : InputControlSource
	{
		uint analogId;
		vrJoystick joystick;


		public MvrJoystickAnalogSource( uint analogId, vrJoystick joystick )
		{
			this.analogId = analogId;
			this.joystick = joystick;
		}


		public float GetValue( InputDevice inputDevice )
		{
			if(joystick != null)
				return joystick.GetAxisValue(analogId);
			else
				return 0f;
		}


		public bool GetState( InputDevice inputDevice )
		{
			return !Mathf.Approximately( GetValue( inputDevice ), 0.0f );
		}

	}
}

