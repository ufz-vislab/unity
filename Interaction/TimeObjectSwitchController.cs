using UFZ.Interaction;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace UFZ.Interaction
{
	/// <summary>
	/// Controls TimeObjectSwitch-components in child GameObjects.
	/// </summary>
	public class TimeObjectSwitchController : IPlayable
	{
		public string Date;

		public float Time
		{
			get { return _time; }
			set
			{
				if (value < _range.x)
					_time = _range.x;
				else if (value > _range.y)
				{
					if (IsRepeating)
						_time = 0;
					else
					{
						Stop();
						_time = _range.y;
					}
				}
				else
					_time = value;

				foreach (var sw in transform.GetComponentsInChildren<TimeObjectSwitch>())
					sw.SetTime(value);
				var hours = Mathf.FloorToInt(_time);
				var minutes = (int)((_time - hours) * 60);
				TimeString = hours.ToString("00") + ":" + minutes.ToString("00");
			}
		}
		private float _time = 0f;

		public string TimeString { get; private set; }

		private Vector2 _range;
		[HideInInspector]
		public List<float> TimeSteps;
		public bool IsRepeating = false;

		public float UnitsPerSecond = 1f;

		private void OnValidate()
		{
			_range = new Vector2(float.MaxValue, float.MinValue);
			TimeSteps = new List<float>();
			foreach (TimeObjectSwitch sw in transform.GetComponentsInChildren<TimeObjectSwitch>())
			{
				Vector2 tmp = _range;
				if (sw.Range.x < _range.x)
					tmp.x = sw.Range.x;
				if (sw.Range.y > _range.y)
					tmp.y = sw.Range.y;
				_range = tmp;

				TimeSteps.AddRange(sw.Times);
			}

			TimeSteps = TimeSteps.Distinct().ToList();
			TimeSteps.Sort();
		}

		private void Update()
		{
			if (IsPlaying)
				Time += UFZ.IOC.Core.Instance.Time.DeltaTime() * UnitsPerSecond;
		}

#if MVR
		public override vrValue Forward(vrValue iValue = null)
		{
			Stop();
			var nearestStep = Mathf.Abs(TimeSteps.BinarySearch(Time + 0.00001f)) - 1;
			if (nearestStep < TimeSteps.Count)
				Time = TimeSteps[nearestStep];
			return iValue;
		}

		public override vrValue Back(vrValue iValue = null)
		{
			Stop();
			var nearestStep = Mathf.Abs(TimeSteps.BinarySearch(Time - 0.00001f)) - 2;
			if (nearestStep >= 0)
				Time = TimeSteps[nearestStep];
			return iValue;
		}

		public override vrValue Begin(vrValue iValue = null)
		{
			Stop();
			Time = _range.x;
			return iValue;
		}

		public override vrValue End(vrValue iValue = null)
		{
			Stop();
			Time = _range.y;
			return iValue;
		}

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
#else
		public override void Forward()
		{
			Stop();
			var nearestStep = Mathf.Abs(TimeSteps.BinarySearch(Time + 0.00001f)) - 1;
			if (nearestStep < TimeSteps.Count)
				Time = TimeSteps[nearestStep];
		}

		public override void Back()
		{
			Stop();
			var nearestStep = Mathf.Abs(TimeSteps.BinarySearch(Time - 0.00001f)) - 2;
			if (nearestStep >= 0)
				Time = TimeSteps[nearestStep];
		}

		public override void Begin()
		{
			Stop();
			Time = _range.x;
		}

		public override void End()
		{
			Stop();
			Time = _range.y;
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
#endif
	}
}
