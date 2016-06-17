using System;
using UnityEngine;
using MarkLight.Views.UI;

namespace UFZ.Interaction
{
	public class ObjectInfoWeb : ClickableObject
	{
		public string URL = "http://www.ufz.de";
		public Vector3 Position;

		private WebBrowserView _webView;

		public void Start()
		{
			_webView = FindObjectOfType<UserInterface>().CreateView<WebBrowserView>();
			_webView.InitializeViews();
			_webView.WebViewWidget.SetValue("URL", URL);
			var uri = new Uri(URL);
			_webView.DragableUIView.SetValue("Caption", uri.Host);
			_webView.Deactivate();
		}

		protected override void Activate()
		{
			_webView.Activate();
		}
	}
}
