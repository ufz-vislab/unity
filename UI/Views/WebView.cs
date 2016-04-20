using UnityEngine;
using MarkUX;
using MarkUX.Views;

namespace MarkUX.Views
{
	[InternalView]
	public class WebView : View
	{
		#region Fields

		[ChangeHandler("UpdateURL")]
		public string URL;
		private bool _initialized;
		private VRWebView _vrWebView;

		#endregion

		#region Constructor

		public WebView()
		{
			URL = "http://www.ufz.de/index.php?en=37716";

			Width = new ElementSize(1f, ElementSizeUnit.Percents);
			
			// TODO: Hardcoded height
			Height = new ElementSize(520f, ElementSizeUnit.Pixels);
		}

		#endregion

		#region Methods

		public override void Initialize()
		{
			base.Initialize();
			this.ForEachChild<View>(x => x.Deactivate(), false);

			if(!Application.isPlaying)
				return;

			var height = (int) ActualHeight;
			var width = (int) ActualWidth;

			var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
			plane.transform.position = new Vector3(0.5f, -0.005f, 0.998f);
			plane.transform.rotation = Quaternion.Euler(90, 180, 0);
			plane.transform.localScale = new Vector3(width * 0.0001f, 1f, height * 0.0001f);
			plane.transform.parent = transform;
			plane.GetComponent<MeshCollider>().convex = true;
			plane.AddComponent<VRActor>();

			_vrWebView = plane.AddComponent<VRWebView>();
			_vrWebView.m_Width = width;
			_vrWebView.m_Height = height;

			UpdateURL();
		}

		public virtual void UpdateURL()
		{
			if(_vrWebView != null && _vrWebView.webView != null)
				_vrWebView.webView.SetURL(URL);
		}

		#endregion
}

}