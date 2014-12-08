using UnityEngine;

namespace UFZ.Interaction
{
	public class DebugVrDevices : MonoBehaviour
	{
		vrKeyboard _keyboard;
		vrAxis _spaceNavAxis;
		vrButtons _spaceNavButtons;
		vrAxis _flystickAxis;
		vrButtons _flystickButtons;

		public DebugVrDevices()
		{
			_spaceNavAxis = null;
		}

		void Start ()
		{
			print ("Start");
			if (MiddleVR.VRDeviceMgr == null) return;
			_keyboard        = MiddleVR.VRDeviceMgr.GetKeyboard();
			_spaceNavAxis    = MiddleVR.VRDeviceMgr.GetAxis("SpaceNavAxes.Axis");
			_spaceNavButtons = MiddleVR.VRDeviceMgr.GetButtons("SpaceNavButtons.Buttons");
			_flystickAxis    = MiddleVR.VRDeviceMgr.GetAxis("FlystickAxes.Axis");
			_flystickButtons = MiddleVR.VRDeviceMgr.GetButtons("FlystickButtons.Buttons");
			print("SpaceNavAxes: "+ _spaceNavAxis);
		}

		void Update ()
		{
			if(_keyboard != null && _keyboard.IsKeyToggled(MiddleVR.VRK_SPACE))
				print("Space!");
			for(uint i = 0; i < 6; ++i)
				if(_spaceNavAxis != null && Mathf.Approximately(_spaceNavAxis.GetValue(i),0))
					print("SpaceNavAxis " + i + " : " + _spaceNavAxis.GetValue(i));

			if(_spaceNavButtons != null)
			{
				for(uint i = 0; i < 2; ++i)
				{
					if(_spaceNavButtons.IsToggled(i))
						print("SpaceNavButton " + i + " pressed.");
					if(_spaceNavButtons.IsToggled(i, false))
						print("SpaceNavButton " + i + " released.");
				}
			}

			if (_flystickAxis != null && Mathf.Approximately(_flystickAxis.GetValue(0),0))
				print("FlystickAxis Horizontal: " + _flystickAxis.GetValue(0));
			if(_flystickAxis != null &&  Mathf.Approximately(_flystickAxis.GetValue(1), 0))
				print("FlystickAxis Vertical: " + _flystickAxis.GetValue(1));

			if (_flystickButtons == null) return;
			for(uint i = 0; i < 6; ++i)
			{
				if(_flystickButtons.IsToggled(i))
					print("FlystickButton " + i + " pressed.");
				if(_flystickButtons.IsToggled(i, false))
					print("FlystickButton " + i + " released.");
			}
		}
	}
}

#if MVR
using MiddleVR_Unity3D;
#endif
