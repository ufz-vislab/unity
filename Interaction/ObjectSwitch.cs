using System.Collections;
using UnityEngine;
using System.Linq;

namespace UFZ.Interaction
{
	public class ObjectSwitch : ObjectSwitchBase
	{
		public float Fps;

		void Reset()
		{
			ActiveChild = 0;
			Fps = 5;
		}

		protected override void Update()
		{
			base.Update();
			if (!IsPlaying) return;
			if (ElapsedTime > (1f / Fps))
				SetActiveChild(ActiveChild + 1);
		}

		public virtual void OnEnable() { }
		public virtual void OnDisable()
		{
			IsPlaying = false;
		}

		public override void Play()
		{
			IsPlaying = true;
		}

		public override void Stop()
		{
			IsPlaying = false;
		}

		public override void TogglePlay()
		{
			IsPlaying = !IsPlaying;
		}

		public override void Forward()
		{
			IsPlaying = false;
			//if (ElapsedTime > (1f / Fps))
			SetActiveChild(ActiveChild + 1);
		}

		public override void Back()
		{
			IsPlaying = false;
			//if (ElapsedTime > (1f / Fps))
			SetActiveChild(ActiveChild - 1);
		}

		public override void Begin()
		{
			IsPlaying = false;
			base.Begin();
		}

		public override void End()
		{
			IsPlaying = false;
			base.End();
		}
	}
}
