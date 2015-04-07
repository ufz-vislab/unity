using UnityEngine;

namespace UFZ.Rendering
{
/**
 * Create additional user layers (http://unity3d.com/support/documentation/Components/Layers):
 *  User Layer 8 - LeftCamera
 *  User Layer 9 - RightCamera
 * Then the culling mask selects the right layer. You can additionally add GUI objects to these layers.
 */

	/// <summary>
	/// Debugs stereo left / right images.
	/// </summary>
	public class Debug_Stereo : MonoBehaviour
	{
		public bool CullingMask = false; // If true overdraws everything
		private bool _init;

		private void Update()
		{
			if (_init)
				return;
			
			foreach (var cam in FindObjectsOfType(typeof (Camera)) as Camera[])
			{
				if (cam.name.Contains(".Left"))
				{
					// Left is red
					cam.backgroundColor = new Color(1, 0, 0);
					if (CullingMask)
						cam.cullingMask = 1 << 8;
				}
				if (cam.name.Contains(".Right"))
				{
					// Right is blue
					cam.backgroundColor = new Color(0, 0, 1);
					if (CullingMask)
						cam.cullingMask = 1 << 9;
				}
			}

			_init = true;
		}
	}
}
