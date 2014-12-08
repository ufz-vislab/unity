using System;
using UnityEngine;


namespace InControl
{
	public class UfzKeyCodeComboSource : InputControlSource
	{
		KeyCode[] keyCodeList;


		public UfzKeyCodeComboSource( params KeyCode[] keyCodeList )
		{
			this.keyCodeList = keyCodeList;
		}


		public float GetValue( InputDevice inputDevice )
		{
			return GetState( inputDevice ) ? 1.0f : 0.0f;
		}


		public bool GetState( InputDevice inputDevice )
		{
			for (int i = 0; i < keyCodeList.Length; i++)
			{
				if (!UFZ.IOC.Core.Instance.Keyboard.IsKeyPressed( keyCodeList[i] ))
				{
					return false;
				}
			}
			return true;
		}
	}
}

