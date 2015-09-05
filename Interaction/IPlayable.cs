using UnityEngine;

namespace UFZ.Interaction
{
	/// <summary>
	/// An interface for something which has playing controls.
	/// </summary>
	public abstract class IPlayable : MonoBehaviour
	{
		// Commands
		public vrCommand BeginCommand;
		public vrCommand EndCommand;
		public vrCommand ForwardCommand;
		public vrCommand BackCommand;
		public vrCommand PlayCommand;
		public vrCommand StopCommand;
		public vrCommand TogglePlayCommand;

		public bool IsPlaying = false;

		private void Start()
		{
			BeginCommand = new vrCommand("Begin Command - " + gameObject.name, Begin);
			EndCommand = new vrCommand("End Command - " + gameObject.name, End);
			ForwardCommand = new vrCommand("Forward Command - " + gameObject.name, Forward);
			BackCommand = new vrCommand("Back Command - " + gameObject.name, Back);
			PlayCommand = new vrCommand("Play Command - " + gameObject.name, Play);
			StopCommand = new vrCommand("Stop Command - " + gameObject.name, Stop);
			TogglePlayCommand = new vrCommand("Toggle Play Command - " + gameObject.name, TogglePlay);
		}

		public abstract vrValue Begin(vrValue iValue = null);
		public abstract vrValue End(vrValue iValue = null);
		public abstract vrValue Forward(vrValue iValue = null);
		public abstract vrValue Back(vrValue iValue = null);
		public abstract vrValue Play(vrValue iValue = null);
		public abstract vrValue Stop(vrValue iValue = null);
		public abstract vrValue TogglePlay(vrValue iValue = null);
	}
}
