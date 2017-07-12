using MarkLight;
using MarkLight.Views.UI;
using UFZ.Interaction;
using UFZ.Misc;
using UnityEngine;

public class PlayablesView : UIView
{
	public ObservableList<IPlayable> Playables;
	public string MenuHeader = "Animations";
	public string[] Names;
	public string ToggleButtonText = "Play";
	public float SelectedPosition = 0f;
	public float Speed = 1f;
	public string TimeInfo = "0";

	public override void Initialize()
	{
		base.Initialize();

		Playables = new ObservableList<IPlayable>();

		var vis = GameObject.Find("Animations");
		if (vis != null && vis.GetComponentInChildren<GameObjectList>().Objects != null)
		{
			foreach (var go in vis.GetComponentInChildren<GameObjectList>().Objects)
			{
				if (go.GetComponent<IPlayable>())
					Playables.Add(go.GetComponent<IPlayable>());
			}
		}
		else
		{
			foreach (var t in Resources.FindObjectsOfTypeAll<IPlayable>())
				//if ((TimeObjectSwitch)t == null)
				Playables.Add(t);
		}
		
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
			UpdateTime();
	}

	public void OnActivate()
	{
		if (Playables.SelectedItem != null)
			UpdateTime();
	}

	public void TogglePlay()
	{
		Playables.SelectedItem.TogglePlay();
	}

	public void ToStart()
	{
		Playables.SelectedItem.Begin();
		UpdateTime();
	}

	public void ToEnd()
	{
		Playables.SelectedItem.End();
		UpdateTime();
	}

	public void Forward()
	{
		Playables.SelectedItem.Forward();
		UpdateTime();
	}

	public void Back()
	{
		Playables.SelectedItem.Back();
		UpdateTime();
	}

	public void SliderChanged(Slider slider)
	{
		if (Playables.SelectedItem == null || Playables.SelectedItem.IsPlaying)
			return;

		var objSwitch = Playables.SelectedItem as ObjectSwitchBase;
		if (objSwitch)
		{
			objSwitch.SetActiveChild(slider.Value);
			SetValue(() => TimeInfo, Playables.SelectedItem.TimeInfo);
		}
		else if (Playables.SelectedItem as TimeObjectSwitchController)
		{
			var tmp = (TimeObjectSwitchController) Playables.SelectedItem;
			tmp.SetPercentage(slider.Value);
			SetValue(() => TimeInfo, Playables.SelectedItem.TimeInfo);
		}
	}

	protected void UpdateTime()
	{
		SetValue(() => SelectedPosition, Playables.SelectedItem.Percentage);
		SetValue(() => TimeInfo, Playables.SelectedItem.TimeInfo);
	}
}

