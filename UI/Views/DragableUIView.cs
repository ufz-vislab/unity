using UnityEngine;
using MarkLight.Views.UI;

public class DragableUIView : UIView
{
	public string Caption = "Caption";
	private bool dragging;
	private Vector3 lastPointerPos;

	public void DragStart()
	{
		dragging = true;
		if (UFZ.Core.HasDevice("Flystick"))
			lastPointerPos = transform.InverseTransformPoint(FindObjectOfType<WandInputModule>().cursor.position);
		else
			lastPointerPos = UFZ.Core.Position();
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
		if (UFZ.Core.HasDevice("Flystick"))
			currentPointerPos = transform.InverseTransformPoint(FindObjectOfType<WandInputModule>().cursor.position);
		else
			currentPointerPos = UFZ.Core.Position();
		var deltaPointerPos = currentPointerPos - lastPointerPos;
		transform.localPosition += deltaPointerPos;
		lastPointerPos = currentPointerPos;
		// Take movement of UI into account for last pointer position
		if (UFZ.Core.HasDevice("Flystick"))
			lastPointerPos -= deltaPointerPos;
	}

	public void Close()
	{
		if (Parent)
			Parent.Deactivate();
		else
			Deactivate();
	}
}
