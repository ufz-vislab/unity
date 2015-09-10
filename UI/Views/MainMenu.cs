using UnityEngine;
using MarkUX;
using MarkUX.Views;

public class MainMenu : View
{
	public ViewSwitcher ContentViewSwitcher;

	public void SectionSelected(FlowListSelectionActionData eventData)
	{
		ContentViewSwitcher.SwitchTo(eventData.FlowListItem.Text + "View");
	}

	public void Close()
	{
		this.GetLayoutRoot().transform.parent.gameObject.SetActive(false);
	}
}
