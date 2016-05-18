using UnityEngine;
using DG.Tweening;
using MarkLight;
using MarkLight.Views.UI;

[HideInPresenter]
public class CameraPathView : UIView
{

	public CameraPathAnimator CameraPath;
	public float PathPosition;
	public string ToggleButtonText = "Play";

	public void PositionChanged()
	{
		if (!CameraPath.isPlaying)
			CameraPath.Seek(PathPosition);
	}

	public void TogglePlay()
	{
		if (CameraPath.isPlaying)
			CameraPath.Pause();
		else
			CameraPath.Play();
	}

	public void ResetPath()
	{
		CameraPath.Stop();
		CameraPath.Seek(0f);
		PathPosition = 0f;
	}

	public void FlyToStart()
	{
		var nodeToMove = CameraPath.animationObject;
		var pathPoint = CameraPath.cameraPath.GetPoint(0);
		var orientationPoint = CameraPath.cameraPath.orientationList[0];
		nodeToMove.transform.DOMove(pathPoint.worldPosition, 5);
		nodeToMove.transform.DORotate(orientationPoint.rotation.eulerAngles, 5);
	}

	/*
	public void LoopClick(CheckBoxClickActionData data)
	{
		if (CameraPath == null)
			return;
		// BUG: data is always null
		Debug.Log("Checked: " + LoopCheckBox.Checked);
		CameraPath.animationMode = LoopCheckBox.Checked ?
			CameraPathAnimator.animationModes.loop :
			CameraPathAnimator.animationModes.once;
	}
	*/

	public void Update()
	{
		if (!IsActive || CameraPath == null)
			return;

		if (CameraPath.isPlaying)
		{
			SetValue(() => ToggleButtonText, "Pause");
			SetValue(() => PathPosition, CameraPath.percentage);
		}
		else
		{
			SetValue(() => ToggleButtonText, "Play");
			SetValue(() => PathPosition, CameraPath.percentage);
		}
	}
}
