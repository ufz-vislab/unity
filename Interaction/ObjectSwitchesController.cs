using UnityEngine;
using System.Collections;
using UFZ.Interaction;

public class ObjectSwitchesController : IPlayable
{
	public ObjectSwitch[] Switches;

	public override void Begin()
	{
		IsPlaying = false;
		foreach (var objectSwitch in Switches)
			objectSwitch.Begin();
	}

	public override void End()
	{
		IsPlaying = false;
		foreach (var objectSwitch in Switches)
			objectSwitch.End();
	}

	public override void Forward()
	{
		IsPlaying = false;
		foreach (var objectSwitch in Switches)
			objectSwitch.Forward();
	}

	public override void Back()
	{
		IsPlaying = false;
		foreach (var objectSwitch in Switches)
			objectSwitch.Back();
	}

	public override void Play()
	{
		IsPlaying = true;
		foreach (var objectSwitch in Switches)
			objectSwitch.Play();
	}

	public override void Stop()
	{
		IsPlaying = false;
		foreach (var objectSwitch in Switches)
			objectSwitch.Stop();
	}

	public override void TogglePlay()
	{
		if (Switches.Length == 0 || Switches[0] == null)
			return;
		foreach (var objectSwitch in Switches)
			objectSwitch.TogglePlay();
		IsPlaying = Switches[0].IsPlaying;
	}

	public void Update()
	{
		if (Switches.Length == 0 || Switches[0] == null)
			return;
		TimeInfo = Switches[0].TimeInfo;
		Percentage = Switches[0].Percentage;
	}
}
