using UFZ.Interaction;
using UnityEngine;
using System.Collections;

public class VrMenuPlayable : MonoBehaviour
{
	private vrWidgetMenu _menu;

	private vrWidgetList _list;
	private vrWidgetGroup _group;
	private vrWidgetButton _playButton;
	private vrWidgetButton _stopButton;

	// Start waits on VRMenu creation with a coroutine
	IEnumerator Start()
	{
		VRMenu MiddleVRMenu = null;
		while (MiddleVRMenu == null || MiddleVRMenu.menu == null)
		{
			// Wait for VRMenu to be created
			yield return null;
			MiddleVRMenu = FindObjectOfType(typeof(VRMenu)) as VRMenu;
		}
		
		_menu = new vrWidgetMenu("Play Menu", MiddleVRMenu.menu, "Play Menu");
		MiddleVRMenu.menu.SetChildIndex(_menu, 0);
		AddMenu(_menu);

		// End coroutine
		yield break;
	}

	vrValue MyItemCommandHandler(vrValue iValue)
	{
		print("My menu item has been clicked");
		return null;
	}

	private void AddMenu(vrWidgetMenu vrmenu)
	{
		_list = new vrWidgetList("Playable objects", vrmenu);
		//_group = new vrWidgetGroup("Play controls", vrmenu);
		_playButton = new vrWidgetButton("Play Button", vrmenu, "Play");
		_stopButton = new vrWidgetButton("Stop Button", vrmenu, "Stop");

		var player = FindObjectOfType(typeof(ObjectSwitch)) as ObjectSwitch;
		if (player != null) _playButton.AddCommand(new vrCommand("Play Command", MyItemCommandHandler));
	}

	private void SetupList(vrWidgetList list)
	{
		//var scripts = FindObjectOfType<IPlayable>();
		//vrValue listValue = new vrValue(scripts.);
		//listValue.AddListItem();
		//list.SetList();
	}
}
