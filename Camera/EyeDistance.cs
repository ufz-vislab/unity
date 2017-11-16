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

		private void Start()
		{
			SetEyeDistance(_distance);
		}

		private static void SetEyeDistance(float distance)
		{
			var numCams = MiddleVR.VRDisplayMgr.GetCamerasNb();
			for (uint i = 0; i < numCams; i++)
				MiddleVR.VRDisplayMgr.GetCameraStereo(i).SetInterEyeDistance(distance);
		}
	}
}
