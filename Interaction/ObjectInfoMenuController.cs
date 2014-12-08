using UnityEngine;
using System.Collections;
using MiddleVR_Unity3D;

public class ObjectInfoMenuController : MonoBehaviour
{
	/*
	GameObject go = null;
	public GameObject webViewGo = null;
	public GameObject imageViewGo = null;
	public GameObject label = null;
	CoherentUIView view = null;
	ObjectInfo info = null;
	int index = 0;
	UITexture imageSurface = null;
	Vector3 imageScale;

	void OnEnable ()
	{
		go = PlayMakerGlobals.Instance.Variables.GetFsmGameObject("Selected Object").Value;
		if(go)
		{
			info = go.GetComponent<ObjectInfo>();
			view = webViewGo.GetComponent<CoherentUIView>();
			imageSurface = imageViewGo.GetComponent<UITexture>();
			if(info && view && info.urls.Length > 0)
				view.Page = info.urls[0];
			imageScale = imageViewGo.transform.localScale;
			label.GetComponent<UILabel>().text = go.name;
		}
	}

	void OnDisable()
	{
		imageViewGo.transform.localScale = imageScale;
	}

	void Update ()
	{
		vrKeyboard keyb = MiddleVR.VRDeviceMgr.GetKeyboard();
		if (keyb == null)
			return;

		float x = MiddleVR.VRDeviceMgr.GetWandHorizontalAxisValue();
		if (keyb.IsKeyToggled(MiddleVR.VRK_LEFT) || x < -0.8f)
			Previous();

		if (keyb.IsKeyToggled(MiddleVR.VRK_RIGHT) || x > 0.8f)
			Next();
	}

	void Previous()
	{
		if(index > info.urls.Length)
		{
			index--;
			SetImage();
		}
		else if(index > 0)
		{
			if(index == info.urls.Length)
			{
				webViewGo.SetActive(true);
				imageViewGo.SetActive(false);
			}
			index--;
			view.Page = info.urls[index];
		}
	}

	void Next()
	{
		if(index < info.urls.Length - 1)
		{
			index++;
			view.Page = info.urls[index];
		}
		else if(index < info.urls.Length + info.images.Length - 1)
		{
			index++;
			if(index == info.urls.Length)
			{
				webViewGo.SetActive(false);
				imageViewGo.SetActive(true);
			}
			SetImage();
		}
	}

	void SetImage()
	{
		imageSurface.mainTexture = info.images[index - info.urls.Length];
		float width = imageSurface.mainTexture.width;
		float height = imageSurface.mainTexture.height;
		if(width > height)
			imageViewGo.transform.localScale = new Vector3(
				imageScale.x, imageScale.y / (width / height), imageScale.z);
		else if( width == height)
			imageViewGo.transform.localScale = imageScale;
		else
			imageViewGo.transform.localScale = new Vector3(
				imageScale.x / (height / width), imageScale.y, imageScale.z);
	}
	*/
}
