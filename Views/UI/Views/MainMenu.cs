using MarkLight.Views.UI;

namespace UFZ.Views
{
	public class MainMenu : UIView
	{
		public ViewSwitcher ContentViewSwitcher;
		// ReSharper disable once InconsistentNaming
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
}
