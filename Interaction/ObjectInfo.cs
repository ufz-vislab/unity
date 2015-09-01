using FullInspector;
using MarkUX;
using UnityEngine;

namespace UFZ.Interaction
{
	[RequireComponent(typeof(VRActor))]
	public class ObjectInfo : BaseBehavior
	{
		public Texture2D[] Images;

		public InfoView Menu;

		protected override void Awake()
		{
			base.Awake();

			var vrActor = GetComponent<VRActor>();
			vrActor.Grabable = false;
		}

		/// <summary>
		/// Is triggered when Wand button 0 was pressed and shows the info canvas
		/// </summary>
		protected void VRAction(VRSelection iSelection)
		{
			Menu.Show();
			Menu.SetObjectInfo(this);
		}

		protected void OnMouseDown()
		{
			Menu.Show();
			Menu.SetObjectInfo(this);
		}
	}
}
