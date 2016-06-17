using MarkLight.Views;
using MarkLight.Views.UI;

public class WebBrowserView : UIView
{
	public WebView WebViewWidget;
	public DragableUIView DragableUIView;

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
