using UnityEngine;

namespace UFZ.Interaction
{
	public class KeyboardNavigation : NavigationBase
	{
		protected override void GetInputs()
		{
			UFZ.IOC.IKeyboard keyboard = UFZ.IOC.Core.Instance.Keyboard;
			if (keyboard.IsKeyPressed(KeyCode.W))
				Forward = 1f;
			if (keyboard.IsKeyPressed(KeyCode.S))
				Forward = -1f;
			if (keyboard.IsKeyPressed(KeyCode.A))
				Sideward = -1f;
			if (keyboard.IsKeyPressed(KeyCode.D))
				Sideward = 1f;

			if (keyboard.IsKeyPressed(KeyCode.LeftShift))
				Running = 1f;
		}
	}
}
