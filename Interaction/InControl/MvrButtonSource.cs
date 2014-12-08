using System;
using UnityEngine;

namespace InControl
{
	public class MvrButtonSource : InputControlSource
	{
		uint buttonId;
		vrButtons buttons;


		public MvrButtonSource( uint buttonId, vrButtons buttons )
		{
			this.buttonId = buttonId;
			this.buttons = buttons;
		}


		public float GetValue( InputDevice inputDevice )
		{
			return GetState( inputDevice ) ? 1.0f : 0.0f;
		}


		public bool GetState( InputDevice inputDevice )
		{
			if(buttons != null && buttonId < buttons.GetButtonsNb())
				return buttons.IsPressed(buttonId);
			return false;
		}
	}
}
