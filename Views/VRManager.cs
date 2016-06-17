using UnityEngine;
using MarkLight;

[HideInPresenter]
public class VRManager : View
{
	public VRManagerScript VrManagerScript;
	public VRManagerPostFrame VrManagerPostFrame;
	public GUIText GuiText;
	public UFZ.Rendering.EyeDistance EyeDistance;

	[ChangeHandler("CenterNodeChanged")]
	public GameObject CenterNode;

	public override void Initialize()
	{
		base.Initialize();
		VrManagerScript.TemplateCamera = GameObject.Find("Main Camera");
		VrManagerScript.ConfigFile =
			"C:/Program Files (x86)/MiddleVR/data/Config/Misc/Default.vrx";
		VrManagerScript.DisableExistingCameras = false;
		VrManagerScript.ForceQualityIndex = 5;
		VrManagerScript.Fly = true;
	}

	public virtual void CenterNodeChanged()
	{
		VrManagerScript.VRSystemCenterNode = CenterNode;
	}
}
