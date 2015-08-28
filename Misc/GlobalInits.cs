using DG.Tweening;
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

		public InputType GuiInput = InputType.Mouse;
		public bool IsGuiDisabledOnStart = true;

		public MonoBehaviour[] disabledScripts;
		public GameObject[] disabledGameObjects;

		private Canvas _canvas;

		void Awake()
		{
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		foreach(var script in disabledScripts)
			script.enabled = false;

		foreach(var go in disabledGameObjects)
			go.SetActive(false);
#endif

			_canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
			if (_canvas == null)
			{
				Debug.LogError("Gui Canvas not found!");
				return;
			}
			if (GuiInput == InputType.Wand)
			{
				var cam = GameObject.Find("VRWand").AddComponent<Camera>();
				cam.enabled = false;
				cam.cullingMask = LayerMask.GetMask("UI");
				if (!GameObject.Find("EventSystem").GetComponent<WandInputModule>())
				{
					var inputModule = GameObject.Find("EventSystem").AddComponent<WandInputModule>();
					inputModule.cursor = GameObject.Find("WandCursor").GetComponent<RectTransform>();
				}


				if (_canvas != null && _canvas.renderMode == RenderMode.WorldSpace)
				{
					_canvas.worldCamera = cam;
				}
			}
			else
			{
				var cam = GameObject.Find("HeadNode").GetComponentInChildren<Camera>();
				_canvas.worldCamera = cam;
			}
			_canvas.transform.SetParent(GameObject.Find("HeadNode").transform, false);
			_canvas.gameObject.SetActive(!IsGuiDisabledOnStart);
		}

		public void Start()
		{
			DOTween.Init();
		}

		public void Update()
		{
			var input = IOC.Core.Instance.Input;
			if (!input.WasCancelButtonPressed()) return;

			if (_canvas == null)
				return;

			_canvas.gameObject.SetActive(!_canvas.gameObject.activeSelf);
		}
	}
}
