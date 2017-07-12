using System.Collections;
using UnityEngine;
using MarkLight;
using UFZ.Initialization;

[HideInPresenter]
public class VRManager : View
{
	public PreVRManager PreVrManager;
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
		//	VrManagerScript.ConfigFile is configured in PreVRManager.cs
		VrManagerScript.DisableExistingCameras = false;
		VrManagerScript.ForceQualityIndex = 5;
		StartCoroutine(SetFly(true)); // Delay as VRWand is not yet initialized
	}

	public virtual void CenterNodeChanged()
	{
		VrManagerScript.VRSystemCenterNode = CenterNode;
	}

	private IEnumerator SetFly(bool fly, float delay = 1f)
	{
		yield return new WaitForSeconds(delay);
		VrManagerScript.Fly = fly;
	}
}
