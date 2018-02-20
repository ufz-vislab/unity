using UnityEngine;

namespace UFZ.Initialization
{
	public class PreVRManager : MonoBehaviour
	{
		private void Awake()
		{
			var vrManager = GameObject.Find("VRManager").GetComponent<VRManagerScript>();
			var sceneSetup = FindObjectOfType<UFZ.Setup.SceneSetup>();
			if (sceneSetup != null)
			{
				Core.Info("Using custom MVR config");
				vrManager.ConfigFile = sceneSetup.GetVrConfig();
			}
		}
	}
}
