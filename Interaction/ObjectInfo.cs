using UnityEngine;

namespace UFZ.Interaction
{
	public class ObjectInfo : ClickableObject
	{
		public Texture2D[] Images;

		public InfoView Menu;

		protected override void OnValidate()
		{
			base.OnValidate();

			Menu = FindObjectOfType<InfoView>();
		}

		protected override void Activate()
		{
			Menu.Show();
			Menu.SetObjectInfo(this);
		}
	}
}
