using UnityEngine;
using MarkUX;
using MarkUX.Views;

namespace MarkUX.Views
{
	[InternalView]
	[RemoveComponent(typeof(UnityEngine.UI.Image))]
	[RemoveComponent(typeof(RectTransform))]
	//[AddComponent(typeof(MeshFilter))]
	//[AddComponent(typeof(MeshRenderer))]
	//[AddComponent(typeof(VRWebView))]
	//[AddComponent(typeof(VRActor))]
	public class WebView : View
	{
		#region Fields

		[ChangeHandler("UpdateURL")]
		public string URL;
		private bool _initialized;
		private VRWebView vrWebView;

		#endregion

		#region Constructor

		public WebView()
		{
			URL = "http://www.ufz.de/index.php?en=37716";
		}

		#endregion

		#region Methods

		public override void Initialize()
		{
			base.Initialize();
			this.ForEachChild<View>(x => x.Deactivate(), false);
			//_parent = Parent != null ? Parent.GetComponent<View>() : null;

			// TODO: Create once!
			var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
			vrWebView = plane.AddComponent<VRWebView>();
			plane.AddComponent<VRActor>();
			plane.transform.parent = transform;
			//var meshFilterComponent = GetComponent<MeshFilter>();
			//meshFilterComponent.sharedMesh = plane.GetComponent<MeshFilter>().mesh;
			//var meshRenderer = GetComponent<MeshRenderer>();
			//meshRenderer.material = new Material(Shader.Find("Diffuse"));
			//Destroy(plane);
			UpdateURL();
		}

		public virtual void UpdateURL()
		{
			//var webViewComponent = GetComponent<VRWebView>();
			vrWebView.webView.SetURL(URL);
		}

		#endregion
}

}