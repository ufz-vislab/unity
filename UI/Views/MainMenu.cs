using MarkLight.Views.UI;

public class MainMenu : UIView
{
	public ViewSwitcher ContentViewSwitcher;

	public void SectionSelected(ItemSelectionActionData eventData)
	{
		ContentViewSwitcher.SwitchTo(eventData.ItemView.Text + "View");
	}

	public void Close()
	{
		// TODO
		// this.GetLayoutRoot().transform.parent.gameObject.SetActive(false);
		Deactivate();
	}
}
