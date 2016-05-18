using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FullInspector;
using MiddleVR_Unity3D;

namespace UFZ.Rendering
{
	public class CameraSettings : BaseBehavior
	{
		public List<vrCamera> _cameras;

		protected override void Awake()
		{
			base.Awake();

			var clusterNode = MiddleVR.VRClusterMgr.GetMyClusterNode();
			if (clusterNode == null)
			{
				for (uint i = 0; i < MiddleVR.VRDisplayMgr.GetCamerasNb(); i++)
				{
					var cam = MiddleVR.VRDisplayMgr.GetCamera(i);
					if (cam != null)
						_cameras.Add(cam);
				}
				//var camName = MiddleVR.VRDisplayMgr.GetCameraByIndex().GetName();
				//var cam = MiddleVR.VRDisplayMgr.GetCameraStereo(camName) as vrCamera;
				//if (cam != null)
				//	_cameras.Add(cam);
			}
			else
			{
				for (uint i = 0; i < clusterNode.GetViewportsNb(); i++)
				{
					var vp = clusterNode.GetViewport(i);
					var camName = vp.GetCamera().GetName();
					var cam = MiddleVR.VRDisplayMgr.GetCameraStereo(camName) as vrCamera;
					if (cam != null)
						_cameras.Add(cam);
				}
			}
		}

		[InspectorHeader("Clipping planes")]
		[SerializeField]
		public float Near
		{
			get { return _near; }
			set
			{
				_near = value;
				foreach (var cam in _cameras)
					cam.SetFrustumNear(value);
			}
		}
		private float _near = 0.01f;

		[SerializeField]
		public float Far
		{
			get { return _far; }
			set
			{
				_far = value;
				foreach (var cam in _cameras)
				{
					cam.SetFrustumFar(value);
					cam.SetDirty();
				}
			}
		}
		private float _far = 1000f;
	}
}