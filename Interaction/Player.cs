using UnityEngine;

namespace UFZ.Interaction
{
	/// <summary>
	/// The Player represents the user / viewer of the application.
	/// </summary>
	public class Player : MonoBehaviour
	{
#if MVR
		/// <summary>
		/// vrCommands which resets the players orientation.
		/// </summary>
		public vrCommand ResetRotationCommand;
		private void Awake()
		{
			ResetRotationCommand = new vrCommand("Reset Rotation Command", ResetRotation);
		}

		public vrValue ResetRotation(vrValue iValue)
		{
			var node = MiddleVR.VRDisplayMgr.GetNode("VRSystemCenterNode");
			node.SetOrientationLocal(new vrQuat(node.GetYawLocal(), 0, 0));
			return 0;
		}
#endif
	}
}
