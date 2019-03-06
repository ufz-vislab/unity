using System;
using UnityEngine;
using MarkLight.Views.UI;
using UFZ.UI;
using ZenFulcrum.EmbeddedBrowser;

namespace UFZ.Interaction
{
	public class ObjectInfoWeb : ClickableObject
    {
        public Transform BrowserPrefab;
        
		public string URL = "http://www.ufz.de";
		public Vector3 Position;
		public string Caption = "";
		public bool ShowControls = true;
        public bool HideOnStart = true;

        private GameObject _go;
		

		public void Start()
		{
            _go = Instantiate(BrowserPrefab).GetComponent<Canvas>().gameObject;
            _go.transform.SetParent(FindObjectOfType<UserInterface>().transform);
            _go.transform.localPosition = Position * 1000; // Factor from UserInterface scale 0.001
            var browserInterface = _go.GetComponent<BrowserInterface>();
            browserInterface.URL = URL;
            browserInterface.Caption = Caption;
            browserInterface.ShowControls = ShowControls;

            if(HideOnStart)
                _go.SetActive(false);
        }

		protected override void Activate()
		{
            _go.SetActive(true);
		}
	}
}
