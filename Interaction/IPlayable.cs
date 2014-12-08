using UnityEngine;
using System.Collections;

public interface IPlayable
{
	void Forward();
	void Back();
	void Begin();
	void End();
	void Play();
	void Stop();
	void TogglePlay();
}
