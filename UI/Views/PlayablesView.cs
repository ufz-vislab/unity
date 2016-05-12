using MarkLight;
using MarkLight.Views.UI;
using UFZ.Interaction;
using UnityEngine;

public class PlayablesView : UIView
{
	public ObservableList<IPlayable> Playables;
	public string MenuHeader = "Animations";
	public string[] Names;
	public string ToggleButtonText = "Play";
	public float SelectedPosition = 0f;
	public float Speed = 1f;

	public override void Initialize()
	{
		base.Initialize();

		Playables = new ObservableList<IPlayable>();
		foreach (var t in Resources.FindObjectsOfTypeAll<IPlayable>())
			Playables.Add(t);
	}

	public void Update()
	{
		if (!IsActive || Playables.SelectedItem == null)
			return;
		ToggleButtonText = Playables.SelectedItem.IsPlaying ? "Pause" : "Play";
		//SetValue(() => ToggleButtonText, Playables.SelectedItem.IsPlaying ? "Pause" : "Play");

		//SetValue(() => SelectedPosition,
		//	Selected.ActiveChild / ((float)Selected.transform.childCount - 1));
	}

	public void TogglePlay()
	{
		Playables.SelectedItem.TogglePlay();
	}

	public void ToStart()
	{
		Playables.SelectedItem.Begin();
	}

	public void ToEnd()
	{
		Playables.SelectedItem.End();
	}

	public void Forward()
	{
		Playables.SelectedItem.Forward();
	}

	public void Back()
	{
		Playables.SelectedItem.Back();
	}
}
