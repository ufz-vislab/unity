using System;
using System.IO;
using System.Linq;
using System.Threading;
using DG.Tweening;
using MarkLight;
using MarkLight.Views.UI;
using UFZ.Interaction;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if UNITY_STANDALONE
using PygmyMonkey.AdvancedBuilder;
#endif
using UnityEngine;

namespace UFZ.Initialization
{
	/// <summary>
	/// Do global initialization stuff here.
	/// </summary>
	/// Is part of the VRBase scene.
#if UNITY_EDITOR
	[InitializeOnLoad]
#endif
	public class GlobalInits : MonoBehaviour
	{
		public enum InputType
		{
			Mouse,
			Wand,
			Head
		}

		[HideInInspector]
		public InputType GuiInputType = InputType.Mouse;

		//[InspectorDivider]
		public MonoBehaviour[] disabledScripts;
		public GameObject[] disabledGameObjects;


		[HideInInspector]
		public Vector3 CanvasPosition;
		protected Camera GuiCamera;

		private View _mainMenuView;

		protected void Awake()
		{

			var sceneSetup = FindObjectOfType<UFZ.Setup.SceneSetup>();
			if (sceneSetup != null)
				CanvasPosition = sceneSetup.CanvasPositionEditor;

			// Setup player
			var playerGo = GameObject.Find("Player (Player)");
			if (playerGo.transform.Find("HeadNode") == null)
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
				if (sceneSetup != null)
					CanvasPosition = sceneSetup.CanvasPositionVislab;
				GuiInputType = InputType.Wand;
				IOC.Core.Instance.Log.Info("GlobalInits: Cluster detected, using Wand input.");
			}
			else
			{
				if (IOC.Core.Instance.Environment.HasDevice("Rift"))
				{
					GuiInputType = InputType.Head;
					if (sceneSetup != null)
						CanvasPosition = sceneSetup.CanvasPositionRift;
					var navigations = FindObjectsOfType<NavigationBase>();
					foreach (var navigation in navigations)
						navigation.DirectionReferenceNode = "HeadNode";
					IOC.Core.Instance.Log.Info("GlobalInits: Rift Mode");
				}
				else
				{
					if (IOC.Core.Instance.Environment.HasDevice("Flystick"))
					{
						GuiInputType = InputType.Wand;
						IOC.Core.Instance.Log.Info("GlobalInits: Wand input");
					}
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
				canvas.worldCamera = GuiCamera;
				if (canvas.name == "UserInterface")
				{
					var view = canvas.GetComponent<UIView>();
					view.Position.Value = CanvasPosition;
					_mainMenuView = canvas.transform.Find("MainMenu").GetComponent<View>();
					if (sceneSetup != null && sceneSetup.IsGuiDisabledOnStart)
						_mainMenuView.Deactivate();
					continue;
				}

				canvas.gameObject.SetActive(false);
			}
		}

		public void Start()
		{
			DOTween.Init();
			Loom.Current.GetComponent<Loom>();

			FindObjectOfType<VRManagerScript>().TemplateCamera.SetActive(false);

			// Has to be set after everything is initialized
			// Would have been overwritten if setting in Awake()
			FindObjectOfType<UserInterface>().Position.Value = CanvasPosition;
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
	}
}
