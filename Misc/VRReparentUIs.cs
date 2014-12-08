using UnityEngine;
using System.Collections;
#if MVR
using MiddleVR_Unity3D;
#endif // MVR

public class VRReparentUIs : MonoBehaviour {

	public GameObject ui;

	void Start ()
	{
		// = GameObject.Find("UI Root");
#if MVR
		// Reparent UI Root to MiddleVRs center node to have the UI
		// statically displayed relative to the viewer.
		GameObject screenGo = GameObject.Find("Screen0");
        if (screenGo)
        {
            ui.transform.parent = screenGo.transform;

            vrScreen screen = MiddleVR.VRDisplayMgr.GetScreen("Screen0");
            ui.transform.localScale *= screen.GetHeight();
            ui.transform.localPosition = new Vector3(-screen.GetWidth()*2, 0, 0);
            ui.transform.localRotation = Quaternion.identity; ;
        }
        else
            MiddleVRTools.Log("[X] Screen0 not found!");
#else
		ui.transform.localPosition = new Vector3(ui.transform.localPosition.x,
		                                        ui.transform.localPosition.y,
		                                        1.5f);
#endif
	}
}
