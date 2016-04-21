using System.Runtime.Remoting;
using FullInspector;
using MarkUX;
using MarkUX.Views;
using UFZ.Initialization;
using UnityEngine;

namespace UFZ.Interaction
{
	public class ObjectInfoWeb : ClickableObject
	{
		public string URL = "http://www.ufz.de";
		public Vector3 Position;

		private GameObject _menu;

		public void Start()
		{
			_menu = SRResources.WebCanvas.Instantiate();
			_menu.SetActive(false);
			_menu.GetComponentInChildren<WebView>().URL = URL;
			FindObjectOfType<GlobalInits>().RegisterUi(_menu.GetComponent<Canvas>(), Position);
		}

		protected override void Activate()
		{
			_menu.SetActive(true);
		}
	}
}
