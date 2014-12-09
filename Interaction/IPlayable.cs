using UnityEngine;
using System.Collections;

public interface IPlayable
{
	void Forward();
	void Back();
	void Begin();
	void End();
	vrValue Play(vrValue iValue);
	void Stop();
	void TogglePlay();
}
