using System;
using UnityEngine;

namespace UFZ.Rendering
{
	/// <summary>
	/// Helper class to set the eye distance on the stereo cameras.
	/// </summary>
	public class EyeDistance : MonoBehaviour
	{
		/// <summary>
		/// Gets or sets the distance in m. Defaults to 0.063 m.
		/// </summary>
		public float Distance
		{
			get { return _distance; }
			set
			{
				_distance = value;
				SetEyeDistance(value);
			}
		}
		private float _distance = 0.063f;

		public float DefaultDistance = 0.063f;

		private void Start()
		{
			SetEyeDistance(_distance);
		}

		private void Update()
		{
			var keyb = MiddleVR.VRDeviceMgr.GetKeyboard();

			if (keyb == null) return;
			if (keyb.IsKeyPressed(MiddleVR.VRK_LSHIFT) || keyb.IsKeyPressed(MiddleVR.VRK_RSHIFT))
			{

				if (keyb.IsKeyToggled(MiddleVR.VRK_S))
					ToggleDistance();
			}
		}

		private static void SetEyeDistance(float distance)
		{
			var numCams = MiddleVR.VRDisplayMgr.GetCamerasNb();
			for (uint i = 0; i < numCams; i++)
			{
				var cam = MiddleVR.VRDisplayMgr.GetCameraStereo(i);
				if (cam != null)
					cam.SetInterEyeDistance(distance);
			}
		}

		private void ToggleDistance()
		{
			SetEyeDistance(Mathf.Approximately(_distance, DefaultDistance) ? 0f : DefaultDistance);
		}
	}
}
