using UnityEngine;

namespace UFZ.Interaction
{
	/// <summary>
	/// An interface for something which has playing controls.
	/// </summary>
	public abstract class IPlayable : MonoBehaviour
	{
		public bool IsPlaying;
		public string Name = "";
		public virtual float Percentage { get; set; }
		[HideInInspector]
		public string TimeInfo;

		public abstract void Begin();
		public abstract void End();
		public abstract void Forward();
		public abstract void Back();
		public abstract void Play();
		public abstract void Stop();
		public abstract void TogglePlay();
	}
}
