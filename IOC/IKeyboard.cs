using UnityEngine;
using System.Collections.Generic;

namespace UFZ.IOC
{
	public interface IKeyboard
	{
		bool IsKeyPressed(KeyCode key);
		bool WasKeyPressed(KeyCode key);
	}

	public class UnityKeyboard : IKeyboard
	{
		public bool IsKeyPressed(KeyCode key)
		{
			return Input.GetKey(key);
		}

		public bool WasKeyPressed(KeyCode key)
		{
			return Input.GetKeyUp(key);
		}
	}

	public class MiddleVrKeyboard : IKeyboard
	{
		public bool IsKeyPressed(KeyCode key)
		{
			return MiddleVR.VRDeviceMgr.GetKeyboard().IsKeyPressed(Map(key));
		}

		public bool WasKeyPressed(KeyCode key)
		{
			return MiddleVR.VRDeviceMgr.GetKeyboard().IsKeyToggled(Map(key));
		}

		static Dictionary<KeyCode, uint> _mapping;

		static public uint Map(KeyCode key)
		{
			if (_mapping == null)
				createMapping();
			return _mapping[key];
		}

		// ReSharper disable once InconsistentNaming
		private static void createMapping()
		{
			_mapping = new Dictionary<KeyCode, uint>();
			_mapping[KeyCode.A] = MiddleVR.VRK_A;
			_mapping[KeyCode.B] = MiddleVR.VRK_B;
			_mapping[KeyCode.C] = MiddleVR.VRK_C;
			_mapping[KeyCode.D] = MiddleVR.VRK_D;
			_mapping[KeyCode.E] = MiddleVR.VRK_E;
			_mapping[KeyCode.F] = MiddleVR.VRK_F;
			_mapping[KeyCode.G] = MiddleVR.VRK_G;
			_mapping[KeyCode.H] = MiddleVR.VRK_H;
			_mapping[KeyCode.I] = MiddleVR.VRK_I;
			_mapping[KeyCode.J] = MiddleVR.VRK_J;
			_mapping[KeyCode.K] = MiddleVR.VRK_K;
			_mapping[KeyCode.L] = MiddleVR.VRK_L;
			_mapping[KeyCode.M] = MiddleVR.VRK_M;
			_mapping[KeyCode.N] = MiddleVR.VRK_N;
			_mapping[KeyCode.O] = MiddleVR.VRK_O;
			_mapping[KeyCode.P] = MiddleVR.VRK_P;
			_mapping[KeyCode.Q] = MiddleVR.VRK_Q;
			_mapping[KeyCode.R] = MiddleVR.VRK_R;
			_mapping[KeyCode.S] = MiddleVR.VRK_S;
			_mapping[KeyCode.T] = MiddleVR.VRK_T;
			_mapping[KeyCode.U] = MiddleVR.VRK_U;
			_mapping[KeyCode.V] = MiddleVR.VRK_V;
			_mapping[KeyCode.W] = MiddleVR.VRK_W;
			_mapping[KeyCode.X] = MiddleVR.VRK_X;
			_mapping[KeyCode.Y] = MiddleVR.VRK_Y;
			_mapping[KeyCode.Z] = MiddleVR.VRK_Z;

			_mapping[KeyCode.Alpha0] = MiddleVR.VRK_0;
			_mapping[KeyCode.Alpha1] = MiddleVR.VRK_1;
			_mapping[KeyCode.Alpha2] = MiddleVR.VRK_2;
			_mapping[KeyCode.Alpha3] = MiddleVR.VRK_3;
			_mapping[KeyCode.Alpha4] = MiddleVR.VRK_4;
			_mapping[KeyCode.Alpha5] = MiddleVR.VRK_5;
			_mapping[KeyCode.Alpha6] = MiddleVR.VRK_6;
			_mapping[KeyCode.Alpha7] = MiddleVR.VRK_7;
			_mapping[KeyCode.Alpha8] = MiddleVR.VRK_8;
			_mapping[KeyCode.Alpha9] = MiddleVR.VRK_9;

			_mapping[KeyCode.Escape] = MiddleVR.VRK_EQUALS;
			_mapping[KeyCode.Minus] = MiddleVR.VRK_MINUS;
			_mapping[KeyCode.Equals] = MiddleVR.VRK_EQUALS;
			_mapping[KeyCode.Backspace] = MiddleVR.VRK_BACK;
			_mapping[KeyCode.Tab] = MiddleVR.VRK_TAB;
			_mapping[KeyCode.LeftBracket] = MiddleVR.VRK_LBRACKET;
			_mapping[KeyCode.RightBracket] = MiddleVR.VRK_RBRACKET;
			_mapping[KeyCode.Return] = MiddleVR.VRK_RETURN;
			_mapping[KeyCode.LeftControl] = MiddleVR.VRK_LCONTROL;
			_mapping[KeyCode.Semicolon] = MiddleVR.VRK_SEMICOLON;
			//_mapping[KeyCode.] = MiddleVR.VRK_APOSTROPHE;
			//_mapping[KeyCode.] = MiddleVR.VRK_GRAVE;
			_mapping[KeyCode.LeftShift] = MiddleVR.VRK_LSHIFT;
			_mapping[KeyCode.Backslash] = MiddleVR.VRK_BACKSLASH;
			_mapping[KeyCode.Comma] = MiddleVR.VRK_COMMA;
			_mapping[KeyCode.Period] = MiddleVR.VRK_PERIOD;
			_mapping[KeyCode.Slash] = MiddleVR.VRK_SLASH;
			_mapping[KeyCode.RightShift] = MiddleVR.VRK_RSHIFT;
			//_mapping[KeyCode.Left] = MiddleVR.VRK_LMENU;
			_mapping[KeyCode.LeftAlt] = MiddleVR.VRK_ALTLEFT;
			_mapping[KeyCode.Space] = MiddleVR.VRK_SPACE;
			_mapping[KeyCode.CapsLock] = MiddleVR.VRK_CAPITAL;
			_mapping[KeyCode.Numlock] = MiddleVR.VRK_NUMLOCK;
			_mapping[KeyCode.ScrollLock] = MiddleVR.VRK_SCROLL;

			// Numpad
			_mapping[KeyCode.Keypad0] = MiddleVR.VRK_NUMPAD0;
			_mapping[KeyCode.Keypad1] = MiddleVR.VRK_NUMPAD1;
			_mapping[KeyCode.Keypad2] = MiddleVR.VRK_NUMPAD2;
			_mapping[KeyCode.Keypad3] = MiddleVR.VRK_NUMPAD3;
			_mapping[KeyCode.Keypad4] = MiddleVR.VRK_NUMPAD4;
			_mapping[KeyCode.Keypad5] = MiddleVR.VRK_NUMPAD5;
			_mapping[KeyCode.Keypad6] = MiddleVR.VRK_NUMPAD6;
			_mapping[KeyCode.Keypad7] = MiddleVR.VRK_NUMPAD7;
			_mapping[KeyCode.Keypad8] = MiddleVR.VRK_NUMPAD8;
			_mapping[KeyCode.Keypad9] = MiddleVR.VRK_NUMPAD9;

			_mapping[KeyCode.KeypadMinus] = MiddleVR.VRK_SUBTRACT;
			_mapping[KeyCode.KeypadPlus] = MiddleVR.VRK_ADD;
			//_mapping[KeyCode.KeypadPeriod] = MiddleVR.VRK_DECIMAL;
			_mapping[KeyCode.KeypadEnter] = MiddleVR.VRK_NUMPADENTER;
			_mapping[KeyCode.KeypadDivide] = MiddleVR.VRK_DIVIDE;
			_mapping[KeyCode.KeypadEquals] = MiddleVR.VRK_NUMPADEQUALS;
			_mapping[KeyCode.KeypadPeriod] = MiddleVR.VRK_NUMPADCOMMA;
			_mapping[KeyCode.KeypadMultiply] = MiddleVR.VRK_MULTIPLY;

			// Arrow keys
			_mapping[KeyCode.UpArrow] = MiddleVR.VRK_UP;
			_mapping[KeyCode.DownArrow] = MiddleVR.VRK_DOWN;
			_mapping[KeyCode.LeftArrow] = MiddleVR.VRK_LEFT;
			_mapping[KeyCode.RightArrow] = MiddleVR.VRK_RIGHT;
			_mapping[KeyCode.Home] = MiddleVR.VRK_HOME;
			_mapping[KeyCode.PageUp] = MiddleVR.VRK_PRIOR;
			_mapping[KeyCode.PageDown] = MiddleVR.VRK_NEXT;
			_mapping[KeyCode.End] = MiddleVR.VRK_END;
			_mapping[KeyCode.Insert] = MiddleVR.VRK_INSERT;
			_mapping[KeyCode.Delete] = MiddleVR.VRK_DELETE;

			_mapping[KeyCode.LeftWindows] = MiddleVR.VRK_LWIN;
			_mapping[KeyCode.RightWindows] = MiddleVR.VRK_RWIN;
		}
	}
}
