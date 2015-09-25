using MarkUX;
using MarkUX.Views;
using UnityEngine;

public class PlayablesView : View
{
	public FlowList PlayablesFlowList;
	public string MenuHeader = "Animations";
	public ObjectSwitchBase[] Playables;
	public string[] Names;
	public string ToggleButtonText = "Play";
	public float SelectedPosition = 0f;
	public float Speed = 1f;

	protected ObjectSwitchBase Selected;

	public override void Initialize()
	{
		base.Initialize();

		// Resources.FindObjectsOfTypeAll instead of FindObjectsOfType
		// finds also disabled objects.
		Playables = Resources.FindObjectsOfTypeAll<ObjectSwitchBase>();

		if (Playables != null && Playables.Length > 0)
			Selected = Playables[0];

		Names = new string[Playables.Length];
		for (var i = 0; i < Playables.Length; i++)
			Names[i] = Playables[i].name;
	}

	public void PlayableChanged(FlowListSelectionActionData eventData)
	{
		Selected = Playables[eventData.FlowListItem.ZeroBasedIndex];
		SetChanged(() => Selected);
	}

	public void Update()
	{
		if (!Enabled || Selected == null)
			return;

		SetValue(() => ToggleButtonText, Selected.IsPlaying ? "Pause" : "Play");

		SetValue(() => SelectedPosition,
			Selected.ActiveChild / ((float)Selected.transform.childCount - 1));
	}

	public void TogglePlay()
	{
		Selected.TogglePlay();
	}

	public void ToStart()
	{
		Selected.Begin();
	}

	public void ToEnd()
	{
		Selected.End();
	}

	public void Forward()
	{
		Selected.Forward();
	}

	public void Back()
	{
		Selected.Back();
	}
}
