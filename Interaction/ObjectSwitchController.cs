using UnityEngine;
using MiddleVR_Unity3D;

namespace UFZ.Interaction
{
	public class ObjectSwitchController : MonoBehaviour
	{
		public ObjectSwitch ObjectSwitch;

		void Update ()
		{
			if(!ObjectSwitch)
				return;

			var leftOrRight = (int)IOC.Core.Instance.Input.GetHorizontalAxis();
			switch (leftOrRight)
			{
				case -1:
					ObjectSwitch.Back();
					break;
				case 1:
					ObjectSwitch.Forward();
					break;
			}
		}

		public void ObjectSwitchPlayToggle()
		{
			ObjectSwitch.IsPlaying = !ObjectSwitch.IsPlaying;
		}
	}
}
