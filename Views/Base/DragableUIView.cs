using UnityEngine;
using MarkLight.Views.UI;

namespace UFZ.Views
{
	public class DragableUIView : UIView
	{
		public string Caption = "Caption";
		private bool dragging;
		private Vector3 lastPointerPos;

		public void DragStart()
		{
			dragging = true;
			if (IOC.Core.Instance.Environment.HasDevice("Flystick"))
				lastPointerPos = transform.InverseTransformPoint(FindObjectOfType<UFZ.Input.WandInputModule>().cursor.position);
			else
				lastPointerPos = IOC.Core.Instance.Mouse.Position();
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
			if (IOC.Core.Instance.Environment.HasDevice("Flystick"))
				currentPointerPos = transform.InverseTransformPoint(FindObjectOfType<UFZ.Input.WandInputModule>().cursor.position);
			else
				currentPointerPos = IOC.Core.Instance.Mouse.Position();
			var deltaPointerPos = currentPointerPos - lastPointerPos;
			transform.localPosition += deltaPointerPos;
			lastPointerPos = currentPointerPos;
			// Take movement of UI into account for last pointer position
			if (IOC.Core.Instance.Environment.HasDevice("Flystick"))
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
}
