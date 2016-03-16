using UnityEngine;
using System.Collections;

namespace UFZ.Interaction
{
	public class CutscenePlayer : IPlayable
	{
		public Slate.Cutscene Cutscene;

		public override void Begin()
		{
			Cutscene.Rewind();
			IsPlaying = false;
		}

		public override void End()
		{
			//throw new System.NotImplementedException();
		}

		public override void Forward()
		{
			//throw new System.NotImplementedException();
		}

		public override void Back()
		{
			//throw new System.NotImplementedException();
		}

		public override void Play()
		{
			Debug.Log("Play");
			Cutscene.Play();
			IsPlaying = true;
		}

		public override void Stop()
		{
			Cutscene.Pause();
			IsPlaying = false;
		}

		public override void TogglePlay()
		{
			if (Cutscene.isPaused || !Cutscene.isActive)
				Play();
			else
				Stop();
		}
	}
}