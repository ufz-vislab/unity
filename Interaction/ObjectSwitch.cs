using UnityEngine;
using System.Linq;

namespace UFZ.Interaction
{
	public class ObjectSwitch : ObjectSwitchBase, IPlayable
	{
		public float Fps;
		public bool IsPlaying = false;

		void Reset()
		{
			ActiveChild = 0;
			Fps = 5;
		}

		protected override void Update()
		{
			base.Update();
			if (!IsPlaying) return;
			if (_elapsedTime > (1f / Fps))
				SetActiveChild(ActiveChild + 1);
		}

		public virtual void OnEnable() { }
		public virtual void OnDisable()
		{
			IsPlaying = false;
		}

		public void Stop()
		{
			IsPlaying = false;
		}

		public void TogglePlay()
		{
			IsPlaying = !IsPlaying;
		}

		public virtual void Forward()
		{
			IsPlaying = false;
			if(_elapsedTime > (1f / Fps))
				SetActiveChild(ActiveChild + 1);
		}

		public virtual void Back()
		{
			IsPlaying = false;
			if(_elapsedTime > (1f / Fps))
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

		public vrValue Play(vrValue iValue)
		{
			IsPlaying = true;
			return null;
		}
	}
}
