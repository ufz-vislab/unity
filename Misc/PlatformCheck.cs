using UnityEngine;
using System.Collections;

public class PlatformCheck : MonoBehaviour
{
	public MonoBehaviour[] disabledScripts;
	public GameObject[] disabledGameObjects;
	void Awake()
	{
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		foreach(var script in disabledScripts)
			script.enabled = false;

		foreach(var go in disabledGameObjects)
			go.SetActive(false);
#endif

		var cam = GameObject.Find("VRWand").AddComponent<Camera>();
		cam.enabled = false;
		cam.cullingMask = LayerMask.GetMask("UI");
		if(!GameObject.Find("EventSystem").GetComponent<WandInputModule>())
		{
			var inputModule = GameObject.Find("EventSystem").AddComponent<WandInputModule>();
			inputModule.cursor = GameObject.Find("WandCursor").GetComponent<RectTransform>();
		}

		var canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
		if(canvas != null && canvas.renderMode == RenderMode.WorldSpace)
		{
			canvas.worldCamera = cam;
		}
	}
}
