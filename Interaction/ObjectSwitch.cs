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

#if MVR
		public override vrValue Play(vrValue iValue = null)
		{
			IsPlaying = true;
			return iValue;
		}

		public override vrValue Stop(vrValue iValue = null)
		{
			IsPlaying = false;
			return iValue;
		}

		public override vrValue TogglePlay(vrValue iValue = null)
		{
			IsPlaying = !IsPlaying;
			return iValue;
		}

		public override vrValue Forward(vrValue iValue = null)
		{
			IsPlaying = false;
			//if (ElapsedTime > (1f / Fps))
			SetActiveChild(ActiveChild + 1);
			return iValue;
		}

		public override vrValue Back(vrValue iValue = null)
		{
			IsPlaying = false;
			//if (ElapsedTime > (1f / Fps))
			SetActiveChild(ActiveChild - 1);
			return iValue;
		}

		public override vrValue Begin(vrValue iValue = null)
		{
			IsPlaying = false;
			return base.Begin(iValue);
		}

		public override vrValue End(vrValue iValue = null)
		{
			IsPlaying = false;
			return base.End(iValue);
		}
#else
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
		}

		public override void End()
		{
			IsPlaying = false;
		}
#endif
	}
}
