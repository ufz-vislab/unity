using MarkLight.Views.UI;

namespace UFZ.Views
{
	public class WebBrowserView : UIView
	{
		public WebView WebViewWidget;
		public DragableUIView DragableUIView;
		public Group Controls;

		public void BackButtonClick()
		{
			WebViewWidget.Back();
		}

		public void ForwardButtonClick()
		{
			WebViewWidget.Forward();
		}

		public void Close()
		{
			Deactivate();
		}
	}
}
