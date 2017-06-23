using System.Collections;
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

	[ChangeHandler("ConfigFileChanged")]
	public string ConfigFile;

	public override void Initialize()
	{
		base.Initialize();
		VrManagerScript.TemplateCamera = GameObject.Find("Main Camera");
		VrManagerScript.ConfigFile = ConfigFile;
			// "C:/Program Files (x86)/MiddleVR/data/Config/Misc/Default.vrx";
		VrManagerScript.DisableExistingCameras = false;
		VrManagerScript.ForceQualityIndex = 5;
		StartCoroutine(SetFly(true)); // Delay as VRWand is not yet initialized
	}

	public virtual void CenterNodeChanged()
	{
		VrManagerScript.VRSystemCenterNode = CenterNode;
	}

	public virtual void ConfigFileChanged()
	{
		VrManagerScript.ConfigFile = ConfigFile;
	}

	private IEnumerator SetFly(bool fly, float delay = 1f)
	{
		yield return new WaitForSeconds(delay);
		VrManagerScript.Fly = fly;
	}
}
