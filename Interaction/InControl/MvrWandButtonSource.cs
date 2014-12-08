using System;
using UnityEngine;

namespace InControl
{
	public class MvrWandButtonSource : InputControlSource
	{
		readonly uint buttonId;
		readonly vrWand wand;

		public MvrWandButtonSource(uint buttonId, vrWand wand)
		{
			this.buttonId = buttonId;
			this.wand = wand;
		}

		public float GetValue(InputDevice inputDevice)
		{
			return GetState(inputDevice) ? 1.0f : 0.0f;
		}

		public bool GetState(InputDevice inputDevice)
		{
			if (wand != null)
				return wand.IsButtonPressed(buttonId);
			return false;
		}
	}
}
