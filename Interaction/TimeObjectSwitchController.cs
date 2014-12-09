using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using FullInspector;
using System.Collections;

public class TimeObjectSwitchController : BaseBehavior, IPlayable
{
	public string Date;
	private float _time = 0f;
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
			_timeString = hours.ToString("00") + ":" + minutes.ToString("00");
		}
	}

	public string TimeString
	{
		get { return _timeString; }
	}
	private string _timeString;

	private Vector2 _range;
	[HideInInspector]
	public List<float> _timeSteps;
	public bool IsPlaying = false;
	public bool IsRepeating = false;

	public float UnitsPerSecond = 1f;

	void OnValidate ()
	{
		_range = new Vector2(float.MaxValue, float.MinValue);
		_timeSteps = new List<float>();
		foreach (TimeObjectSwitch sw in transform.GetComponentsInChildren<TimeObjectSwitch>())
		{
			Vector2 tmp = _range;
			if (sw.Range.x < _range.x)
				tmp.x = sw.Range.x;
			if (sw.Range.y > _range.y)
				tmp.y = sw.Range.y;
			_range = tmp;

			_timeSteps.AddRange(sw.Times);
		}

		_timeSteps = _timeSteps.Distinct().ToList();
		_timeSteps.Sort();
	}

	void Update ()
	{
		if (IsPlaying)
			Time += UFZ.IOC.Core.Instance.Time.DeltaTime() * UnitsPerSecond;
	}

	public void Forward()
	{
		Stop();
		int nearestStep = Mathf.Abs(_timeSteps.BinarySearch(Time + 0.00001f)) - 1;
		if(nearestStep < _timeSteps.Count)
			Time = _timeSteps[nearestStep];
	}

	public void Back()
	{
		Stop();
		int nearestStep = Mathf.Abs(_timeSteps.BinarySearch(Time - 0.00001f)) - 2;
		if(nearestStep >= 0)
			Time = _timeSteps[nearestStep];
	}

	public void Begin()
	{
		Stop();
		Time = _range.x;
	}

	public void End()
	{
		Stop();
		Time = _range.y;
	}

	public vrValue Play(vrValue iValue)
	{
		IsPlaying = true;
		return null;
	}

	public void Stop()
	{
		IsPlaying = false;
	}

	public void TogglePlay()
	{
		IsPlaying = !IsPlaying;
	}
}
