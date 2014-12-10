using System.Linq;
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
	private vrWidgetToggleButton _isPlayingCheckbox;

	private vrCommand _playCommand;
	private vrCommand _playableObjectChangedCommand;

	private ObjectSwitch[] _objectSwitches;

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
		
		vrValue valueList = vrValue.CreateList();
		_objectSwitches = FindObjectsOfType(typeof(ObjectSwitch)) as ObjectSwitch[];
		if (_objectSwitches != null)
			foreach (var player in _objectSwitches)
			{
				valueList.AddListItem(player.name);
			}

		_playableObjectChangedCommand = new vrCommand("Playable object command", PlayableObjectChanged);
		_list = new vrWidgetList("Playable objects:", vrmenu, "Playable objects:", _playableObjectChangedCommand);
		_list.SetList(valueList);
		_list.SetSelectedIndex(0);
		
		//_group = new vrWidgetGroup("Play controls", vrmenu);
		_playButton = new vrWidgetButton("Play Button", vrmenu, "Play");
		//_isPlayingCheckbox = new vrWidgetToggleButton("Is Playing", vrmenu, "Is Playing:");
		_stopButton = new vrWidgetButton("Stop Button", vrmenu, "Stop");
	}

	vrValue PlayableObjectChanged(vrValue iValue)
	{
		var index = iValue.GetInt();
		if (index < _objectSwitches.Length - 1)
			return null;

		var objectSwitch = _objectSwitches[iValue.GetInt()];
		// _playButton.RemoveCommand();
		_playButton.AddCommand(objectSwitch.PlayCommand);
		// _stopButton.RemoveCommand();
		_stopButton.AddCommand(objectSwitch.StopCommand);
		return null;
	}

	private void SetupList(vrWidgetList list)
	{
		//var scripts = FindObjectOfType<IPlayable>();
		//vrValue listValue = new vrValue(scripts.);
		//listValue.AddListItem();
		//list.SetList();
	}
}
