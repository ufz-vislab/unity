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
				TimeInfo = hours.ToString("00") + ":" + minutes.ToString("00");
				Percentage = (_time - _range.x)/(_range.y - _range.x);
			}
		}
		private float _time = 0f;

		public void SetPercentage(float percentage)
		{
			Time = percentage * (_range.y - _range.x) + _range.x;
		}

		private Vector2 _range;
		[HideInInspector]
		public List<float> TimeSteps;
		public bool IsRepeating = false;

		public float UnitsPerSecond = 1f;

		private void OnValidate()
		{
			Name = name;
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
			Time = 0.0f;
		}

		public void Start()
		{
			Time = 0.0f;
		}

		private void Update()
		{
			if (IsPlaying)
				Time += UFZ.IOC.Core.Instance.Time.DeltaTime() * UnitsPerSecond;
		}

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
	}
}
