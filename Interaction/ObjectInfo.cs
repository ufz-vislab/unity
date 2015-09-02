using System.Runtime.Remoting;
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
			Activate();
		}

		protected void OnMouseDown()
		{
			Activate();
		}

		protected void OnTriggerEnter(Collider other)
		{
			// TODO Visualize this
		}

		protected void OnTriggerExit(Collider other)
		{
			// TODO
		}

		protected void OnTriggerStay(Collider other)
		{
			if (IOC.Core.Instance.Input.WasOkButtonPressed())
				Activate();
		}

		private void Activate()
		{
			Menu.Show();
			Menu.SetObjectInfo(this);
		}
	}
}
