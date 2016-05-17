using System.Runtime.Remoting;
using FullInspector;
using UFZ.Initialization;
using UnityEngine;
using MarkLight.Views;
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
			_webView.WebViewWidget.URL = URL;
			_webView.Deactivate();
		}

		protected override void Activate()
		{
			_webView.Activate();
		}
	}
}
