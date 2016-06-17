using MarkLight.Views.UI;

public class MainMenu : UIView
{
	public ViewSwitcher ContentViewSwitcher;
	public DragableUIView DragableUIView;

	public override void Initialize()
	{
		base.Initialize();

		DragableUIView.SetValue("Caption", "Main Menu");
	}

	public void SectionSelected(ItemSelectionActionData eventData)
	{
		ContentViewSwitcher.SwitchTo(eventData.ItemView.Text + "View");
	}

	public void Close()
	{
		Deactivate();
	}

}
