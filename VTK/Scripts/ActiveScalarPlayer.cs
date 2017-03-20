#if UNITY_STANDALONE_WIN
using UnityEngine;
using System.Collections;
using UFZ.Interaction;
using UFZ.VTK;

public class ActiveScalarPlayer : IPlayable
{
	public VtkRenderer Renderer;
	public float Fps;
	protected float ElapsedTime = 0f;

	public override void Begin()
	{
		if (Renderer == null)
			return;
		Renderer.ActiveColorArrayIndex = 0;
		IsPlaying = false;
	}

	public override void End()
	{
		if (Renderer == null)
			return;
		Renderer.ActiveColorArrayIndex = 999999;
		IsPlaying = false;
	}

	public override void Forward()
	{
		if (Renderer == null)
			return;
		Renderer.ActiveColorArrayIndex += 1;
		IsPlaying = false;
	}

	public override void Back()
	{
		if (Renderer == null)
			return;
		Renderer.ActiveColorArrayIndex -= 1;
		IsPlaying = false;
	}

	public override void Play()
	{
		if (Renderer == null)
			return;
		ElapsedTime = 0;
		IsPlaying = true;
	}

	public override void Stop()
	{
		IsPlaying = false;
	}

	public override void TogglePlay()
	{
		if (Renderer == null)
			return;
		IsPlaying = !IsPlaying;
		ElapsedTime = 0;
	}

	void Reset()
	{
		if(Renderer != null)
			Renderer.ActiveColorArrayIndex = 0;
		Fps = 5;
	}

	void Update()
	{
		if (!IsPlaying) return;
		ElapsedTime += UFZ.IOC.Core.Instance.Time.DeltaTime();
		if (ElapsedTime > (1f / Fps))
			Renderer.ActiveColorArrayIndex += 1;
	}
}
#endif
