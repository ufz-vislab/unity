using UnityEngine;
using System.Linq;

namespace UFZ.Interaction
{
	[RequireComponent (typeof (MasterObjectSwitchController))]
	public class MasterObjectSwitchController : MonoBehaviour
	{
		public MasterObjectSwitch MasterObjectSwitch;
		public ObjectSwitch CurrentObjectSwitch;
		//public MyVRGUI gui;

		void Start ()
		{
			CurrentObjectSwitch = SwitchFromIndex(0);
			MasterObjectSwitch.SetActiveChild(0);
			//gui = GameObject.Find("OVRPlayerController").GetComponentInChildren<MyVRGUI>();
		}

		ObjectSwitch SwitchFromIndex(int index)
		{
			int i = 0;
			foreach(var child in MasterObjectSwitch.transform.Cast<Transform>().OrderBy(t=>t.name))
			{
				if(i == index)
					return child.GetComponentInChildren<ObjectSwitch>();
				++i;
			}
			return null;
		}

		void Update ()
		{
			int upOrDown = 0;
			if(Input.GetKey(KeyCode.DownArrow) || Input.GetButtonUp("Fire4"))
				upOrDown = -1;
			else if(Input.GetKey(KeyCode.UpArrow) || Input.GetButtonUp("Fire3"))
				upOrDown = 1;

			if(upOrDown == -1)
				MasterObjectSwitch.back();
			else if (upOrDown == 1)
				MasterObjectSwitch.forward();
			CurrentObjectSwitch = SwitchFromIndex(MasterObjectSwitch.activeChild);
			if(!CurrentObjectSwitch)
			{
				//gui.setLabel("");
				return;
			}
			//if(currentObjectSwitch is AnnotatedObjectSwitch)
			//{
			//var sw = currentObjectSwitch as AnnotatedObjectSwitch;
			//if(sw.annotations.Length > sw.activeChild)
			//	gui.setLabel(sw.annotations[sw.activeChild]);
			//else
			//	gui.setLabel("");
			//}
			//else
			//	gui.setLabel("");

			int leftOrRight = 0;
			if(Input.GetKey(KeyCode.LeftArrow) || Input.GetButtonUp("Fire1"))
				leftOrRight = -1;
			else if(Input.GetKey(KeyCode.RightArrow) || Input.GetButtonUp("Fire2"))
				leftOrRight = 1;

			if(leftOrRight == -1)
				CurrentObjectSwitch.Back();
			else if (leftOrRight == 1)
				CurrentObjectSwitch.Forward();

		}
	}
}
