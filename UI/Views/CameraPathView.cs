using UnityEngine;
using DG.Tweening;
using MarkUX;
using MarkUX.Views;

[InternalView]
public class CameraPathView : View
{
	// public CheckBox LoopCheckBox;

	public CameraPathAnimator CameraPath;
	public float Position;
	public string ToggleButtonText = "Play";

	public void PositionChanged()
	{
		if (!CameraPath.isPlaying)
			CameraPath.Seek(Position);
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
		Position = 0f;
		SetChanged(() => Position);
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
		if (!Enabled || CameraPath == null)
			return;

		if (CameraPath.isPlaying)
		{
			SetValue(() => ToggleButtonText, "Pause");
			SetValue(() => Position, CameraPath.percentage);
		}
		else
		{
			SetValue(() => ToggleButtonText, "Play");
			SetValue(() => Position, CameraPath.percentage);
		}
	}
}
