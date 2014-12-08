using System;
using UnityEngine;


namespace InControl
{
	public class UfzMouseButtonSource : InputControlSource
	{
		int buttonId;


		public UfzMouseButtonSource( int buttonId )
		{
			this.buttonId = buttonId;
		}


		public float GetValue( InputDevice inputDevice )
		{
			return GetState( inputDevice ) ? 1.0f : 0.0f;
		}


		public bool GetState( InputDevice inputDevice )
		{
			return UFZ.IOC.Core.Instance.Mouse.Button( buttonId );
		}
	}
}

