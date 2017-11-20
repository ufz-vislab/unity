using MarkLight;
using UFZ.UI.Views;
using UnityEngine;

namespace UFZ.Interaction
{
	public class ViewpointsKeyboard : MonoBehaviour
	{
		private ObservableList<Viewpoint> _viewpoints;

		private void Update()
		{
			if (_viewpoints == null)
			{
				var viewpointsView = FindObjectOfType<ViewpointsView>();
				if (viewpointsView != null)
					_viewpoints = viewpointsView.Viewpoints;
			}
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

				if (index >= 0 && index < _viewpoints.Count)
					_viewpoints[index].Move();
			}
		}
	}
}
