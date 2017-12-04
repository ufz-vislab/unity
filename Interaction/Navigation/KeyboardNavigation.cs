using UnityEngine;

namespace UFZ.Interaction
{
	public class KeyboardNavigation : NavigationBase
	{
		protected override void GetInputs()
		{
			if (Core.IsKeyPressed(KeyCode.W))
				Forward = 1f;
			if (Core.IsKeyPressed(KeyCode.S))
				Forward = -1f;
			if (Core.IsKeyPressed(KeyCode.A))
				Sideward = -1f;
			if (Core.IsKeyPressed(KeyCode.D))
				Sideward = 1f;
			if (Core.IsKeyPressed(KeyCode.PageUp))
				Upward = 1f;
			if (Core.IsKeyPressed(KeyCode.PageDown))
				Upward = -1f;
			if (Core.IsKeyPressed(KeyCode.Q))
				HorizontalRotation = -1f;
			if (Core.IsKeyPressed(KeyCode.E))
				HorizontalRotation = 1f;
			if (Core.IsKeyPressed(KeyCode.F))
				VerticalRotation = -1f;
			if (Core.IsKeyPressed(KeyCode.R))
				VerticalRotation = 1f;

			if (Core.IsKeyPressed(KeyCode.LeftShift))
				Running = 1f;
		}
	}
}
