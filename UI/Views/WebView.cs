﻿using MarkLight;
using MarkLight.Views.UI;
using UnityEngine;

namespace UFZ.UI.Views
{
	[HideInPresenter]
	public class WebView : UIView
	{
		#region Fields

		[ChangeHandler("UpdateURL")]
		// ReSharper disable once InconsistentNaming
		public string URL;
		private bool _initialized;
		private VRWebView _vrWebView;

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

			if(!Application.isPlaying)
				return;

			// TODO: Hardcoded values
			Height.DirectValue = ElementSize.FromPixels(480f);
			Width.DirectValue = ElementSize.FromPixels(800f);
			LayoutChanged();

			var height = (int) ActualHeight;
			var width = (int) ActualWidth;

			var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
			//plane.transform.position = new Vector3(0f, -0.005f, -0.01f);
			plane.transform.rotation = Quaternion.Euler(90, 180, 0);
			plane.transform.localScale = new Vector3(width * 0.0001f, 1f, height * 0.0001f);
			plane.transform.parent = transform;
			plane.transform.localPosition = new Vector3(0f, 0f, -0.01f);
			//plane.GetComponent<MeshCollider>().convex = true;
			var planeRigidbody = plane.AddComponent<Rigidbody>();
			planeRigidbody.useGravity = false;
			planeRigidbody.isKinematic = true;
			// VRActor needed for mouse / flystick interactions
			plane.AddComponent<VRActor>().Grabable = false;

			_vrWebView = plane.AddComponent<VRWebView>();
			_vrWebView.Width = (int)(width * 1.5);
			_vrWebView.Height = (int)(height * 1.5);
			_vrWebView.MouseInteractionsIn3D = true;
		}

		public void Update()
		{
			if(!_initialized)
				UpdateURL();
		}

		// ReSharper disable once InconsistentNaming
		public virtual void UpdateURL()
		{
			if (_vrWebView == null || _vrWebView.webView == null)
				return;
			//Debug.Log("Setting URL: " + URL);
			_initialized = true;
			_vrWebView.webView.SetURL(URL);
		}

		public void Back()
		{
			if (_vrWebView == null || _vrWebView.webView == null)
				return;
			_vrWebView.webView.GoBack();
		}

		public void Forward()
		{
			if (_vrWebView == null || _vrWebView.webView == null)
				return;
			_vrWebView.webView.GoForward();
		}

		#endregion
	}

}
