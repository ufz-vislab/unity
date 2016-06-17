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

	public void Start()
	{
		if (Playables.Count > 0)
			SetValue(() => Playables.SelectedIndex, 0);
	}

	public void Update()
	{
		if (!IsActive || Playables.SelectedItem == null)
			return;

		var buttonText = Playables.SelectedItem.IsPlaying ? "Pause" : "Play";
		SetValue(() => ToggleButtonText, buttonText);

		if (Playables.SelectedItem.IsPlaying)
			SetValue(() => SelectedPosition, Playables.SelectedItem.Percentage);
	}

	public void OnActivate()
	{
		if (Playables.SelectedItem != null)
			SetValue(() => SelectedPosition, Playables.SelectedItem.Percentage);
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

	public void SliderChanged(Slider slider)
	{
		if (Playables.SelectedItem == null || Playables.SelectedItem.IsPlaying)
			return;

		var objSwitch = Playables.SelectedItem as ObjectSwitchBase;
		if (objSwitch)
			objSwitch.SetActiveChild(slider.Value);
	}
}

