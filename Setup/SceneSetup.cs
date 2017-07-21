using System.Collections;
using Sirenix.OdinInspector;
using UFZ.Interaction;
using UnityEngine;

namespace UFZ.Setup
{
	public class SceneSetup : MonoBehaviour
	{
		public enum VrConfigs
		{
			Default,
			Flystick
		}
		public static SceneSetup Instance;

		[EnumToggleButtons]
		public VrConfigs VrConfig = VrConfigs.Default;

		[FoldoutGroup("GUI")]
		public Vector3 CanvasPositionEditor = new Vector3(0.5f, 0f, 1f);
		[FoldoutGroup("GUI")]
		public Vector3 CanvasPositionVislab = new Vector3(0.75f, 2f, 1f);
		[FoldoutGroup("GUI")]
		public Vector3 CanvasPositionRift = new Vector3(0f, 0f, 0.2f);
		[FoldoutGroup("GUI")]
		public bool IsGuiDisabledOnStart = true;

		[BoxGroup("Start")]
		public Viewpoint Viewpoint;

		//[BoxGroup("Navigation")]
		//public float Speed = 2f;

		public string GetVrConfig()
		{
			switch (VrConfig)
			{
					case VrConfigs.Default:
						return "C:/Program Files (x86)/MiddleVR/data/Config/Misc/Default.vrx";
					case VrConfigs.Flystick:
						return "Y:/vislab/unity/configs/Local/Flystick.vrx";
				default:
					return "C:/Program Files (x86)/MiddleVR/data/Config/Misc/Default.vrx";
			}
		}

		private void Awake ()
		{
			if (Instance == null)
				Instance = this;
			else if (Instance != this)
				Destroy(gameObject);
		}

		private void Start()
		{
			if (Viewpoint != null)
				Viewpoint.Jump();
		}
	}
}
