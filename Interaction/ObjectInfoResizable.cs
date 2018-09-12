using MarkLight;
using MarkLight.Views.UI;
using UFZ.UI.Views;
using UnityEngine;

namespace UFZ.Interaction
{
	public class ObjectInfoResizable : UFZ.Interaction.ObjectInfo
	{
		public int Width = 800;
        public int Height = 600;


		public void Start()
		{
			base.Start();
            _menu.Width.Value = new ElementSize(Width, ElementSizeUnit.Pixels);
            _menu.Height.Value = new ElementSize(Height, ElementSizeUnit.Pixels);
		}
	}
}
