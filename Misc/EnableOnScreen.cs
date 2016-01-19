using UnityEngine;
using System.Linq;

public class EnableOnScreen : MonoBehaviour
{
	public string[] Screens;

	void Start()
	{
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
		var screenName = MiddleVR.VRClusterMgr.IsCluster() ?
			MiddleVR.VRClusterMgr.GetMyClusterNode().GetViewport().GetCamera().GetScreen().GetName() :
			MiddleVR.VRDisplayMgr.GetActiveViewport(0).GetCamera().GetScreen().GetName();
		var active = Screens.ToArray().Contains(screenName);
		gameObject.SetActive(active);
		vrScreen screen;
		vrViewport Vp;
#endif
	}
}
