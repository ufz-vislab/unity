using FullInspector;
using MarkUX;
using UnityEngine;

namespace UFZ.Interaction
{
	[RequireComponent(typeof(VRActor))]
	public class ObjectInfo : BaseBehavior
	{
		//public string[] Urls;
		public Texture2D[] Images;

		public InfoView Menu;

		protected override void Awake()
		{
			base.Awake();

			var vrActor = GetComponent<VRActor>();
			vrActor.Grabable = false;
		}

		protected void VRAction(VRSelection iSelection)
		{
			Menu.gameObject.transform.parent.gameObject.SetActive(true);
			Menu.SetObjectInfo(this);
		}

		protected void OnMVRWandEnter(VRSelection iSelection)
		{
			Menu.gameObject.transform.parent.parent.gameObject.SetActive(true);
			Menu.SetObjectInfo(this);
		}

		protected void OnMVRWandExit(VRSelection iSelection)
		{
			Menu.gameObject.transform.parent.parent.gameObject.SetActive(false);
		}
	}
}
