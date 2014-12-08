using System;
using UnityEngine;


namespace InControl
{
	public class UfzMouseAxisSource : InputControlSource
	{
		string mouseAxisQuery;


		public UfzMouseAxisSource( string axis )
		{
			this.mouseAxisQuery = "mouse " + axis;
		}


		public float GetValue( InputDevice inputDevice )
		{
			return UFZ.IOC.Core.Instance.Mouse.AxisRaw( mouseAxisQuery );
		}


		public bool GetState( InputDevice inputDevice )
		{
			return !Mathf.Approximately( GetValue( inputDevice ), 0.0f );
		}
	}
}

