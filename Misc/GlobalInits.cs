using System.Linq;
using DG.Tweening;
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
			Wand,
			Head
		}

		public InputType GuiInputType = InputType.Mouse;

		public bool IsGuiDisabledOnStart = true;

		public MonoBehaviour[] disabledScripts;
		public GameObject[] disabledGameObjects;

		public Vector3 CanvasPosition = new Vector3(0.5f, 0f, 1f);

		private Canvas _mainMenuCanvas;

		protected void Awake()
		{
			// Setup player
			var playerGo = GameObject.Find("Player");
			if (playerGo.transform.FindChild("HeadNode") == null)
			{
				var headGo = new GameObject("HeadNode");
				headGo.transform.SetParent(playerGo.transform, false);
				var camGo = GameObject.FindWithTag("MainCamera");
				camGo.transform.SetParent(headGo.transform, false);

				var handGo = new GameObject("HandNode");
				handGo.transform.SetParent(playerGo.transform, false);
			}

			var wandGo = GameObject.Find("VRWand");

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
			foreach(var script in disabledScripts)
				script.enabled = false;

			foreach(var go in disabledGameObjects)
				go.SetActive(false);
#endif
#if !(UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
			foreach(var actor in FindObjectsOfType<VRActor>())
				actor.enabled = false;
#endif
			if (IOC.Core.Instance.Environment.IsCluster())
				GuiInputType = InputType.Wand;
			else
			{
				if (IOC.Core.Instance.Environment.HasDevice("Rift"))
				{
					GuiInputType = InputType.Head;
					CanvasPosition = new Vector3(0f, 0f, 0.2f);
					var navigations = FindObjectsOfType<NavigationBase>();
					foreach (var navigation in navigations)
						navigation.DirectionReferenceNode = "HeadNode";
				}
			}

			Camera guiCamera;
			if (GuiInputType == InputType.Wand)
			{
				guiCamera = wandGo.AddComponent<Camera>();
				guiCamera.enabled = false;
				guiCamera.cullingMask = LayerMask.GetMask("UI");
			}
			else
				guiCamera = GameObject.Find("HeadNode").GetComponentInChildren<Camera>();

			if (GuiInputType == InputType.Head)
			{
				FindObjectOfType<VRManagerScript>().ShowWand = false;
				wandGo.GetComponent<VRAttachToNode>().VRParentNode = "HeadNode";
			}

			var canvases = GameObject.Find("Menus").GetComponentsInChildren<Canvas>().Where(
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

			if (GuiInputType == InputType.Wand || GuiInputType == InputType.Head)
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
