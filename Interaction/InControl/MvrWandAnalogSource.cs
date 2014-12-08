using System;
using UnityEngine;


namespace InControl
{
	public class MvrWandAnalogSource : InputControlSource
	{
		readonly uint analogId;
		readonly vrWand wand;

		public MvrWandAnalogSource(uint analogId, vrWand wand)
		{
			this.analogId = analogId;
			this.wand = wand;
		}

		public float GetValue(InputDevice inputDevice)
		{
			if (wand == null)
				return 0f;

			vrAxis axis = wand.GetAxis();
			return axis.GetValue(analogId);
		}


		public bool GetState(InputDevice inputDevice)
		{
			return !Mathf.Approximately(GetValue(inputDevice), 0.0f);
		}

	}
}

