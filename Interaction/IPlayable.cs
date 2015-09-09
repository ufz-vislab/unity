using UnityEngine;

namespace UFZ.Interaction
{
	/// <summary>
	/// An interface for something which has playing controls.
	/// </summary>
	public abstract class IPlayable : MonoBehaviour
	{
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
		// Commands
		public vrCommand BeginCommand;
		public vrCommand EndCommand;
		public vrCommand ForwardCommand;
		public vrCommand BackCommand;
		public vrCommand PlayCommand;
		public vrCommand StopCommand;
		public vrCommand TogglePlayCommand;
#endif

		public bool IsPlaying = false;

		private void Start()
		{
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
			BeginCommand = new vrCommand("Begin Command - " + gameObject.name, Begin);
			EndCommand = new vrCommand("End Command - " + gameObject.name, End);
			ForwardCommand = new vrCommand("Forward Command - " + gameObject.name, Forward);
			BackCommand = new vrCommand("Back Command - " + gameObject.name, Back);
			PlayCommand = new vrCommand("Play Command - " + gameObject.name, Play);
			StopCommand = new vrCommand("Stop Command - " + gameObject.name, Stop);
			TogglePlayCommand = new vrCommand("Toggle Play Command - " + gameObject.name, TogglePlay);
#endif
		}

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
		public abstract vrValue Begin(vrValue iValue = null);
		public abstract vrValue End(vrValue iValue = null);
		public abstract vrValue Forward(vrValue iValue = null);
		public abstract vrValue Back(vrValue iValue = null);
		public abstract vrValue Play(vrValue iValue = null);
		public abstract vrValue Stop(vrValue iValue = null);
		public abstract vrValue TogglePlay(vrValue iValue = null);
#else
		public abstract void Begin();
		public abstract void End();
		public abstract void Forward();
		public abstract void Back();
		public abstract void Play();
		public abstract void Stop();
		public abstract void TogglePlay();
#endif
	}
}
