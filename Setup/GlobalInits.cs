using System.Linq;
using DG.Tweening;
using FullInspector;
using MarkLight;
using UFZ.Interaction;
using UnityEngine;

namespace UFZ.Initialization
{
	/// <summary>
	/// Do global initialization stuff here.
	/// </summary>
	/// Is part of the VRBase scene.
	public class GlobalInits : BaseBehavior
	{
		public enum InputType
		{
			Mouse,
			Wand,
			Head
		}

		[InspectorHeader("GUI")]
		[HideInInspector]
		public InputType GuiInputType = InputType.Mouse;
		public bool IsGuiDisabledOnStart = true;
		public Vector3 CanvasPositionEditor = new Vector3(0.5f, 0f, 1f);
		public Vector3 CanvasPositionVislab = new Vector3(0.75f, 2f, 1f);
		public Vector3 CanvasPositionRift = new Vector3(0f, 0f, 0.2f);

		[InspectorDivider]
		public MonoBehaviour[] disabledScripts;
		public GameObject[] disabledGameObjects;


		[HideInInspector]
		public Vector3 CanvasPosition;
		protected Camera GuiCamera;

		private View _mainMenuView;

		protected void Awake()
		{
			CanvasPosition = CanvasPositionEditor;
			// Setup player
			var playerGo = GameObject.Find("Player (Player)");
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
			{
				CanvasPosition = CanvasPositionVislab;
				GuiInputType = InputType.Wand;
			}
			else
			{
				if (IOC.Core.Instance.Environment.HasDevice("Rift"))
				{
					GuiInputType = InputType.Head;
					CanvasPosition = CanvasPositionRift;
					var navigations = FindObjectsOfType<NavigationBase>();
					foreach (var navigation in navigations)
						navigation.DirectionReferenceNode = "HeadNode";
				}
				else
				{
					var camGo = GameObject.FindWithTag("MainCamera");
					camGo.GetComponent<SuperSampling_SSAA>().enabled = true;
				}
			}

			if (GuiInputType == InputType.Wand)
			{
				GuiCamera = wandGo.AddComponent<Camera>();
				GuiCamera.enabled = false;
				GuiCamera.cullingMask = LayerMask.GetMask("UI");
			}
			else
			{
				GuiCamera = GameObject.Find("HeadNode").GetComponentInChildren<Camera>();
				FindObjectOfType<VRManagerScript>().ShowWand = false;
				FindObjectOfType<VRRaySelection>().enabled = false;
			}

			if (GuiInputType == InputType.Head)
			{
				wandGo.GetComponent<VRAttachToNode>().VRParentNode = "HeadNode";
			}

			var canvases = GameObject.Find("Views").GetComponentsInChildren<Canvas>().Where(
				canvas => canvas.renderMode == RenderMode.WorldSpace);
			var enumerable = canvases as Canvas[] ?? canvases.ToArray();
			if (!enumerable.Any())
			{
				Debug.LogError("No Gui Canvas found!");
				return;
			}
			foreach (var canvas in enumerable)
			{
				RegisterUi(canvas, Vector3.zero);
				if (canvas.name == "UserInterface")
				{
					_mainMenuView = canvas.transform.FindChild("MainMenu").GetComponent<View>();
					if (IsGuiDisabledOnStart)
						_mainMenuView.Deactivate();
					continue;
				}
				if (canvas.name == "InfoCanvas")
				{
					foreach (var objectInfo in FindObjectsOfType<ObjectInfo>())
						objectInfo.Menu = canvas.GetComponentInChildren<InfoView>();
					canvas.transform.localPosition = new Vector3(-0.75f, 2f, 1f);

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
			Loom.Current.GetComponent<Loom>();

			FindObjectOfType<VRManagerScript>().TemplateCamera.SetActive(false);
		}

		public void Update()
		{
			var input = IOC.Core.Instance.Input;
			if (!input.WasCancelButtonPressed()) return;

			if (_mainMenuView == null)
				return;

			if (_mainMenuView.IsActive)
				_mainMenuView.Deactivate();
			else
				_mainMenuView.Activate();
		}

		public void RegisterUi(Canvas canvas, Vector3 position)
		{
			//canvas.transform.SetParent(
			//		GameObject.Find("Player").transform, false);
			//canvas.transform.localPosition = CanvasPosition + position;
			canvas.worldCamera = GuiCamera;
		}
	}
}
