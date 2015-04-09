using UnityEngine;

namespace UFZ.Interaction
{
	/// <summary>
	/// Shows / hides the VR menu on backspace press.
	/// </summary>
	public class VrMenuKeyboard : MonoBehaviour
	{
		private VRMenuManager _menuManager;

		private void Start()
		{
			_menuManager = GameObject.Find("VRMenu").GetComponent<VRMenuManager>();
		}

		private void Update()
		{
			if(!_menuManager)
				return;

			var keyboard = UFZ.IOC.Core.Instance.Keyboard;
			if (keyboard.WasKeyPressed(KeyCode.Backspace))
				_menuManager.ToggleVisiblity();
		}
	}
}
