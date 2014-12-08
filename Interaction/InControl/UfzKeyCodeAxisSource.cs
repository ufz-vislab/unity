using System;
using UnityEngine;


namespace InControl
{
	public class UfzKeyCodeAxisSource : InputControlSource
	{
		KeyCode negativeKeyCode;
		KeyCode positiveKeyCode;


		public UfzKeyCodeAxisSource( KeyCode negativeKeyCode, KeyCode positiveKeyCode )
		{
			this.negativeKeyCode = negativeKeyCode;
			this.positiveKeyCode = positiveKeyCode;
		}


		public float GetValue( InputDevice inputDevice )
		{
			int axisValue = 0;

			if (UFZ.IOC.Core.Instance.Keyboard.IsKeyPressed( negativeKeyCode ))
			{
				axisValue--;
			}

			if (UFZ.IOC.Core.Instance.Keyboard.IsKeyPressed( positiveKeyCode ))
			{
				axisValue++;
			}

			return axisValue;
		}


		public bool GetState( InputDevice inputDevice )
		{
			return !Mathf.Approximately( GetValue( inputDevice ), 0.0f );
		}
	}
}

