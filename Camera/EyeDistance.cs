using FullInspector;

namespace UFZ.Rendering
{
	/// <summary>
	/// Helper class to set the eye distance on the stereo cameras.
	/// </summary>
	public class EyeDistance : BaseBehavior
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

		void Start()
		{
			SetEyeDistance(_distance);
		}

		private static void SetEyeDistance(float distance)
		{
			string camName;
			if (MiddleVR.VRClusterMgr.GetMyClusterNode() != null)
				camName = MiddleVR.VRClusterMgr.GetMyClusterNode().GetViewport().GetCamera().GetName();
			else
				camName = MiddleVR.VRDisplayMgr.GetCameraByIndex().GetName();
			var stereoCam = MiddleVR.VRDisplayMgr.GetCameraStereo(camName);

			if (stereoCam != null)
				stereoCam.SetInterEyeDistance(distance);
		}
	}
}
