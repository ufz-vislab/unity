using System.Collections.Generic;
using UnityEngine;

namespace UFZ.Interaction
{
	public class ViewpointsKeyboard : MonoBehaviour
	{
		private List<Viewpoint> _viewpoints;

        private void Start()
        {
            var viewpointGroup = GameObject.Find("Viewpoints");
            //if (viewpointGroup == null || viewpointGroup.transform.childCount == 0 ||
            //    viewpointGroup.GetComponentsInChildren<Viewpoint>() == null)
            //    return;

            _viewpoints = new List<Viewpoint>();
            // Iterate over childs to preserve order from editor
            for (var i = 0; i < viewpointGroup.transform.childCount; ++i)
            {
                var child = viewpointGroup.transform.GetChild(i);
                if (!child.gameObject.activeSelf)
                    continue;
                var viewpoint = child.gameObject.GetComponent<Viewpoint>();
                if (viewpoint == null) continue;
                _viewpoints.Add(viewpoint);
            }
        }

		#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
		private void Update()
		{
			var keyb = MiddleVR.VRDeviceMgr.GetKeyboard();

			if (keyb == null) return;
			if (keyb.IsKeyPressed(MiddleVR.VRK_LSHIFT) || keyb.IsKeyPressed(MiddleVR.VRK_RSHIFT))
			{
				var index = -1;
				if (keyb.IsKeyToggled(MiddleVR.VRK_1))
					index = 0;
				if (keyb.IsKeyToggled(MiddleVR.VRK_2))
					index = 1;
				if (keyb.IsKeyToggled(MiddleVR.VRK_3))
					index = 2;
				if (keyb.IsKeyToggled(MiddleVR.VRK_4))
					index = 3;
				if (keyb.IsKeyToggled(MiddleVR.VRK_5))
					index = 4;
				if (keyb.IsKeyToggled(MiddleVR.VRK_6))
					index = 5;
				if (keyb.IsKeyToggled(MiddleVR.VRK_7))
					index = 6;
				if (keyb.IsKeyToggled(MiddleVR.VRK_8))
					index = 7;
				if (keyb.IsKeyToggled(MiddleVR.VRK_9))
					index = 8;
				if (keyb.IsKeyToggled(MiddleVR.VRK_0))
					index = 9;

				if (index >= 0 && index < _viewpoints.Count)
					_viewpoints[index].Move();
			}
		}
		#endif
	}
}
