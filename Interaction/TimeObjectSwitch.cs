using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeObjectSwitch : ObjectSwitchBase
{
	public float StartTime = 0f;
	public float StepSize = 1f;
	public bool ShowOutOfRange = true;

	[HideInInspector]
	public Vector2 Range;

	[HideInInspector]
	public List<float> Times;

	void OnValidate()
	{
		Times.Clear();
		Range = new Vector2(StartTime, StartTime + (transform.childCount) * StepSize);
		for (int i = 0; i < transform.childCount; i++)
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

		if (time > (StartTime + transform.childCount * StepSize))
		{
			End();
			return;
		}

		SetActiveChild((int)Math.Floor((time - StartTime) / StepSize));
	}
}
