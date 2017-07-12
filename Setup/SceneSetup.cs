using System;
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

		public VrConfigs VrConfig = VrConfigs.Default;

		public Vector3 CanvasPositionEditor = new Vector3(0.5f, 0f, 1f);
		public Vector3 CanvasPositionVislab = new Vector3(0.75f, 2f, 1f);
		public Vector3 CanvasPositionRift = new Vector3(0f, 0f, 0.2f);

		public bool IsGuiDisabledOnStart = true;

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

			// DontDestroyOnLoad(gameObject);
		}
	}
}
