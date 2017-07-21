using MarkLight.Views.UI;
using UFZ.UI.Views;
using UnityEngine;

namespace UFZ.Interaction
{
	public class ObjectInfo : ClickableObject
	{
		public Texture2D[] Images;

		private InfoView _menu;

		public void Start()
		{
			_menu = FindObjectOfType<UserInterface>().CreateView<InfoView>();
			_menu.ObjectInfo = this;
			_menu.InitializeViews();
			_menu.Deactivate();
		}

		protected override void Activate()
		{
			_menu.Activate();
		}
	}
}
