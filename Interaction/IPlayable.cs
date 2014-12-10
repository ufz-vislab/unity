using UnityEngine;
using System.Collections;

public interface IPlayable
{
	void Forward();
	void Back();
	void Begin();
	void End();
	vrValue Play(vrValue iValue);
	vrValue Stop(vrValue iValue = null);
	void TogglePlay();
}
