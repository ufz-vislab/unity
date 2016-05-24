using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UFZ.IOC
{
	public interface IMouse
	{
		Vector3 Position();
		bool Button(int index);
		bool ButtonDown(int index);
		bool ButtonUp(int index);
		float AxisRaw(string axisName);
	}

	public class UnityMouse : IMouse
	{
		public Vector3 Position()
		{
			return Input.mousePosition;
		}

		public bool Button(int index)
		{
			return Input.GetMouseButton(index);
		}

		public bool ButtonDown(int index)
		{
			return Input.GetMouseButtonDown(index);
		}

		public bool ButtonUp(int index)
		{
			return Input.GetMouseButtonUp(index);
		}

		public float AxisRaw(string axisName)
		{
			return Input.GetAxisRaw(axisName);
		}
	}

	public class MiddleVrMouse : IMouse
	{
		public Vector3 Position()
		{
			vrMouse mouse = MiddleVR.VRDeviceMgr.GetMouse();
			if(mouse == null)
			{
				Core.Instance.Log.Error("MiddleVR Mouse not found!");
				return new Vector3();
			}
			var pos = mouse.GetCursorPosition();
#if UNITY_EDITOR
			// Convert absolute mouse coords from MVR to Unitys game window coordinates ( 	abs |_ vs. rel |- )
			var gameView = GetMainGameView();
			var gameViewRect = gameView.position;
			var gameViewMousePos = new Vector2(pos.x() - gameViewRect.x, (pos.y() - gameViewRect.yMax + 1)*-1);
			//Debug.Log(gameViewMousePos + " | Unity: " + Input.mousePosition);
			return gameViewMousePos;
#else
			//Debug.Log("Mouse : " + pos.x() + ", " + pos.y() + " | Unity: " + Input.mousePosition);
			return new Vector2(pos.x(), pos.y()*-1);
#endif
		}

		public bool Button(int index)
		{
			return MiddleVR.VRDeviceMgr.GetMouse().IsButtonPressed((uint)index);
		}

		public bool ButtonDown(int index)
		{
			return MiddleVR.VRDeviceMgr.GetMouse().IsButtonToggled((uint)index, true);
		}

		public bool ButtonUp(int index)
		{
			return MiddleVR.VRDeviceMgr.GetMouse().IsButtonToggled((uint)index, false);
		}

		public float AxisRaw(string axisName)
		{
			uint index = 0;
			if(axisName == "mouse y")
				index = 1;
			else if(axisName == "mouse z")
				index = 2;

			return MiddleVR.VRDeviceMgr.GetMouse().GetAxisValue(index);
		}

#if UNITY_EDITOR
		public static EditorWindow GetMainGameView()
		{
			System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
			System.Reflection.MethodInfo GetMainGameView = T.GetMethod("GetMainGameView", 	System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.	Static);
			System.Object res = GetMainGameView.Invoke(null, null);
			return (EditorWindow)res;
		}
#endif
	}
}
