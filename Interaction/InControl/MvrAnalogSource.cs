using System;
using UnityEngine;


namespace InControl
{
	public class MvrAnalogSource : InputControlSource
	{
		uint analogId;
		vrAxis analogs;


		public MvrAnalogSource( uint analogId, vrAxis analogs )
		{
			this.analogId = analogId;
			this.analogs = analogs;
		}


		public float GetValue( InputDevice inputDevice )
		{
			if(analogs != null)
				return analogs.GetValue(analogId);
			else
				return 0f;
		}


		public bool GetState( InputDevice inputDevice )
		{
			return !Mathf.Approximately( GetValue( inputDevice ), 0.0f );
		}

	}
}

