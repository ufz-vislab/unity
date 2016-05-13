using UnityEngine;
using System.Collections;
using MarkLight;
using UFZ.Interaction;

[HideInPresenter]
public class NavigationManager : View
{
	[ChangeHandler("CenterNodeChanged")]
	public GameObject CenterNode;

	[ChangeHandler("SpeedChanged")]
	public float Speed = 0.5f;
	[ChangeHandler("RunningChanged")]
	public float Running = 3f;
	[ChangeHandler("RotationSpeedChanged")]
	public float RotationSpeed = 45f;

	public GamepadNavigation GamepadNavigation;
	public KeyboardNavigation KeyboardNavigation;
	public SpaceMouseNavigation SpaceMouseNavigation;

	protected virtual void CenterNodeChanged()
	{
		GamepadNavigation.NodeToMove = CenterNode;
		KeyboardNavigation.NodeToMove = CenterNode;
		SpaceMouseNavigation.NodeToMove = CenterNode;
	}

	protected virtual void SpeedChanged()
	{
		GamepadNavigation.NavigationSpeed = Speed;
		KeyboardNavigation.NavigationSpeed = Speed;
		SpaceMouseNavigation.NavigationSpeed = Speed;
	}

	protected virtual void RunningChanged()
	{
		GamepadNavigation.RunningSpeed = Running;
		KeyboardNavigation.RunningSpeed = Running;
		SpaceMouseNavigation.RunningSpeed = Running;
	}

	protected virtual void RotationSpeedChanged()
	{
		GamepadNavigation.RotationSpeed = RotationSpeed;
		KeyboardNavigation.RotationSpeed = RotationSpeed;
		SpaceMouseNavigation.RotationSpeed = RotationSpeed;
	}

}
