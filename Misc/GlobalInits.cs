using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using FullInspector;
using UFZ.Interaction;
using UnityEngine;

namespace UFZ.Initialization
{
	/// <summary>
	/// Do global initialization stuff here.
	/// </summary>
	/// Is part of the VRBase scene.
	public class GlobalInits : MonoBehaviour
	{
		public enum InputType
		{
			Mouse,
			Wand
		}

		public InputType GuiInputType = InputType.Mouse;

		public bool IsGuiDisabledOnStart = true;

		public MonoBehaviour[] disabledScripts;
		public GameObject[] disabledGameObjects;

		public Vector3 CanvasPosition = new Vector3(0.5f, 0f, 1f);

		private Canvas _mainMenuCanvas;

		protected void Awake()
		{
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		foreach(var script in disabledScripts)
			script.enabled = false;

		foreach(var go in disabledGameObjects)
			go.SetActive(false);
#endif
			if (IOC.Core.Instance.Environment.IsCluster())
				GuiInputType = InputType.Wand;

			Camera guiCamera;
			if (GuiInputType == InputType.Wand)
			{
				guiCamera = GameObject.Find("VRWand").AddComponent<Camera>();
				guiCamera.enabled = false;
				guiCamera.cullingMask = LayerMask.GetMask("UI");
			}
			else
				guiCamera = GameObject.Find("HeadNode").GetComponentInChildren<Camera>();

			var canvases = FindObjectsOfType<Canvas>().Where(
				canvas => canvas.renderMode == RenderMode.WorldSpace);
			var enumerable = canvases as Canvas[] ?? canvases.ToArray();
			if (!enumerable.Any())
			{
				Debug.LogError("No Gui Canvas found!");
				return;
			}
			foreach (var canvas in enumerable)
			{
				canvas.transform.SetParent(
					GameObject.Find("Player").transform, false);
				canvas.transform.localPosition = CanvasPosition;
				canvas.worldCamera = guiCamera;
				if (canvas.name == "ApplicationMenuCanvas")
				{
					canvas.gameObject.SetActive(!IsGuiDisabledOnStart);
					_mainMenuCanvas = canvas;
					continue;
				}
				if (canvas.name == "InfoCanvas")
				{
					foreach (var objectInfo in FindObjectsOfType<ObjectInfo>())
						objectInfo.Menu = canvas.GetComponentInChildren<InfoView>();

				}

				canvas.gameObject.SetActive(false);
			}

			if (GuiInputType == InputType.Wand)
				return;
			var inputModule = GameObject.Find("EventSystem").GetComponent<WandInputModule>();
			if (inputModule == null)
				return;

			inputModule.enabled = false;
			inputModule.cursor.gameObject.SetActive(false);
		}

		public void Start()
		{
			DOTween.Init();
		}

		public void Update()
		{
			var input = IOC.Core.Instance.Input;
			if (!input.WasCancelButtonPressed()) return;

			if (_mainMenuCanvas == null)
				return;

			_mainMenuCanvas.gameObject.SetActive(!_mainMenuCanvas.gameObject.activeSelf);
		}
	}
}
