using System.Collections.Generic;
using System.Linq;
using UFZ.Interaction;
using UnityEngine;
using System.Collections;

namespace UFZ.Menu
{
	public class VrMenuPlayable : MonoBehaviour
	{
#if MVR
		private vrWidgetMenu _menu;

#pragma warning disable 414
		private vrWidgetList _list;
#pragma warning restore 414
		private vrWidgetGroup _group;
		private vrWidgetButton _beginButton;
		private vrWidgetButton _backButton;
		private vrWidgetButton _playButton;
		private vrWidgetButton _stopButton;
		private vrWidgetButton _forwardButton;
		private vrWidgetButton _endButton;
		//private vrWidgetToggleButton _isPlayingCheckbox;

		private vrCommand _playableObjectChangedCommand;

		private List<IPlayable> _playables;

		// Start waits on VRMenu creation with a coroutine
		private IEnumerator Start()
		{
			VRMenu middleVrMenu = null;
			while (middleVrMenu == null || middleVrMenu.menu == null)
			{
				// Wait for VRMenu to be created
				yield return null;
				middleVrMenu = FindObjectOfType(typeof (VRMenu)) as VRMenu;
			}

			var tmp = FindObjectsOfType(typeof (IPlayable)) as IPlayable[];
			if (tmp == null || tmp.Length == 0)
				yield break;

			_menu = new vrWidgetMenu("Play Menu", middleVrMenu.menu, "Play Menu");
			middleVrMenu.menu.SetChildIndex(_menu, 0);

			_playables = tmp.ToList();
			_playables.RemoveAll(s => s.GetType() == typeof (TimeObjectSwitch));
			var valueList = vrValue.CreateList();
			foreach (var player in _playables)
				valueList.AddListItem(player.name);

			_playableObjectChangedCommand = new vrCommand("Playable object command", PlayableObjectChanged);
			_list = new vrWidgetList("Playable objects:", _menu, "Playable objects:",
				_playableObjectChangedCommand, valueList);

			_group = new vrWidgetGroup("Play controls", _menu);
			_beginButton = new vrWidgetButton("Begin", _group);
			_backButton = new vrWidgetButton("Back", _group);
			_playButton = new vrWidgetButton("Play", _group);
			//_isPlayingCheckbox = new vrWidgetToggleButton("Is Playing", vrmenu, "Is Playing:");
			_stopButton = new vrWidgetButton("Stop", _group);
			_forwardButton = new vrWidgetButton("Forward", _group);
			_endButton = new vrWidgetButton("End", _group);

			PlayableObjectChanged(0);
		}

		private vrValue PlayableObjectChanged(vrValue iValue)
		{
			var index = iValue.GetInt();
			if (index < 0 && index > _playables.Count - 1)
				return null;

			var player = _playables[index];

			if (_beginButton.GetCommandsNb() == 1)
			{
				_beginButton.RemoveCommand(_beginButton.GetCommand(0));
				_backButton.RemoveCommand(_backButton.GetCommand(0));
				_playButton.RemoveCommand(_playButton.GetCommand(0));
				_stopButton.RemoveCommand(_stopButton.GetCommand(0));
				_forwardButton.RemoveCommand(_forwardButton.GetCommand(0));
				_endButton.RemoveCommand(_endButton.GetCommand(0));
			}

			_beginButton.AddCommand(player.BeginCommand);
			_backButton.AddCommand(player.BackCommand);
			_playButton.AddCommand(player.PlayCommand);
			_stopButton.AddCommand(player.StopCommand);
			_forwardButton.AddCommand(player.ForwardCommand);
			_endButton.AddCommand(player.EndCommand);

			return null;
		}
#endif
	}
}
