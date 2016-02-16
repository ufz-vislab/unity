using System;
using System.Collections.Generic;
using UnityEngine;

namespace UFZ.Interaction
{
	/// <summary>
	/// Similar to an ObjectSwitch but with additional info on time step sizes.
	/// </summary>
	public class TimeObjectSwitch : ObjectSwitchBase
	{
		public float StartTime = 0f;
		public float StepSize = 1f;
		public bool ShowOutOfRange = true;

		[HideInInspector] public Vector2 Range;

		[HideInInspector] public List<float> Times;

		private void OnValidate()
		{
			Times.Clear();
			Range = new Vector2(StartTime, StartTime + (transform.childCount)*StepSize);
			for (var i = 0; i < transform.childCount; i++)
				Times.Add(StartTime + i*StepSize);
		}

		public void SetTime(float time)
		{
			if ((time < Range.x || time > Range.y) && !ShowOutOfRange)
			{
				NoActiveChild();
				return;
			}

			if (time < StartTime)
			{
				Begin();
				return;
			}

			if (time > (StartTime + transform.childCount*StepSize))
			{
				End();
				return;
			}

			SetActiveChild((int) Math.Floor((time - StartTime)/StepSize));
		}

		public override void Forward()
		{
			throw new NotImplementedException();
		}

		public override void Back()
		{
			throw new NotImplementedException();
		}

		public override void Play()
		{
			throw new NotImplementedException();
		}

		public override void Stop()
		{
			throw new NotImplementedException();
		}

		public override void TogglePlay()
		{
			throw new NotImplementedException();
		}
	}
}
