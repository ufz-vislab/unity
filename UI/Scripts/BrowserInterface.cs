using System.Collections;
using System.Collections.Generic;
using MarkLight;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using ZenFulcrum.EmbeddedBrowser;

namespace UFZ.UI
{
    public class BrowserInterface : MonoBehaviour
    {
        [ShowInInspector]
        public string URL
        {
            private get { return _url; }
            set
            {
                _url = value;
                var browser = GetComponentInChildren<Browser>();
                browser.Url = _url;
            }
        }
        private string _url = "http://www.ufz.de";

        [ShowInInspector]
        public string Caption
        {
            get { return _caption; }
            set
            {
                _caption = value;
                var title = transform.GetChild(0).Find("TitleBackgroundImage");
                //if (_caption == "")
                //    title.gameObject.SetActive(false);
                //else
                title.GetChild(0).GetComponent<Text>().text = _caption;
            }
        }
        private string _caption = "";

        [ShowInInspector]
        public bool ShowControls
        {
            get { return _showControls; }
            set
            {
                _showControls = value;
                transform.GetChild(0).Find("ToolbarPanel").gameObject.SetActive(_showControls);
            }
        }
        private bool _showControls = true;

        void Start()
        {
            URL = _url;
            Caption = _caption;
            ShowControls = _showControls;
        }
    }
}
