using MarkLight.Views.UI;
using UFZ.Rendering;
using UFZ.Views;
using UnityEngine;

namespace UFZ.UI.Views
{
	public class OptionsView : UIView
	{
		public void StereoEnabledClick(CheckBox source)
		{
			var eye = FindObjectOfType<EyeDistance>();
			eye.Distance = source.IsChecked ? 0.063f : 0f;
		}

		public void OrientationButtonClick()
		{
			var player = GameObject.Find("Player (Player)").GetComponent<Player>();
#if MVR
			player.ResetRotationCommand.Do(0);
#endif
		}
	}
}
