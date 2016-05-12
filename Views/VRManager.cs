using UnityEngine;
using System.Collections;
using MarkLight;

[HideInPresenter]
public class VRManager : View
{
	public VRManagerScript VrManagerScriptComponent;
	public VRManagerPostFrame VrManagerPostFrameComponent;
	public GUIText GuiTextComponent;

	[ChangeHandler("CenterNodeChanged")]
	public GameObject CenterNode;

	public override void Initialize()
	{
		base.Initialize();
		VrManagerScriptComponent.TemplateCamera = UnityEngine.GameObject.Find("Main Camera");
		VrManagerScriptComponent.ConfigFile =
			"C:/Program Files (x86)/MiddleVR/data/Config/Misc/Default.vrx";
		VrManagerScriptComponent.DisableExistingCameras = false;
		VrManagerScriptComponent.ForceQualityIndex = 5;
	}

	public virtual void CenterNodeChanged()
	{
		Debug.Log(CenterNode);
		VrManagerScriptComponent.VRSystemCenterNode = CenterNode;
	}
}
