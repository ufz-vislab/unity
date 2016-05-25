using UnityEngine;
using MarkLight.Views.UI;
using MiddleVR_Unity3D;

public class MainMenu : UIView
{
	public string Caption = "Main Menu";
	public ViewSwitcher ContentViewSwitcher;

	private bool dragging = false;
	private Vector3 lastPointerPos;

	public void SectionSelected(ItemSelectionActionData eventData)
	{
		ContentViewSwitcher.SwitchTo(eventData.ItemView.Text + "View");
	}

	public void Close()
	{
		Deactivate();
	}

	public void DragStart()
	{
		dragging = true;
		if (UFZ.IOC.Core.Instance.Environment.HasDevice("Flystick"))
			lastPointerPos = FindObjectOfType<WandInputModule>().cursor.position;
		else
			lastPointerPos = UFZ.IOC.Core.Instance.Mouse.Position();
	}

	public void DragEnd()
	{
		dragging = false;
	}

	public void Update()
	{
		if (!dragging)
			return;

		Vector3 currentPointerPos;
		var scalingFactor = 1f;
		if (UFZ.IOC.Core.Instance.Environment.HasDevice("Flystick"))
		{
			currentPointerPos = FindObjectOfType<WandInputModule>().cursor.position;
			scalingFactor = 1f / transform.lossyScale.x;
		}
		else
			currentPointerPos = UFZ.IOC.Core.Instance.Mouse.Position();
		var deltaPointerPos = currentPointerPos - lastPointerPos;
		transform.localPosition += deltaPointerPos * scalingFactor;
		lastPointerPos = currentPointerPos;
	}
}
