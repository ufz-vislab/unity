using UnityEngine;
using System.Collections;
using MarkUX;
using MarkUX.Views;

public class WebBrowserView : View
{
	public WebView WebViewWidget;

	public void BackButtonClick()
	{
		WebViewWidget.Back();
	}

	public void ForwardButtonClick()
	{
		WebViewWidget.Forward();
	}
}
