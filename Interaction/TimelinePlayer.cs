using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace UFZ.Interaction
{
	public class TimelinePlayer : IPlayable
	{
		public List<PlayableDirector> PlayableDirectors;

		public override void TogglePlay()
		{
			if (IsPlaying)
			{
				foreach (var director in PlayableDirectors)
					director.Pause();
			}
			else
			{
				foreach (var director in PlayableDirectors)
				{
					director.Play();
				}
			}
			base.TogglePlay();
		}

		public override void Stop()
		{
			base.Stop();
			foreach (var director in PlayableDirectors)
			{
				director.Stop();
			}
		}

		public override void Forward()
		{
			base.Forward();
			foreach (var director in PlayableDirectors)
			{
				director.time += 0.2;
			}
		}

		public override void Back()
		{
			base.Back();
			foreach (var director in PlayableDirectors)
				director.time -= 0.2;
		}
	}
}