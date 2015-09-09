using UnityEngine;

namespace UFZ.Interaction
{
	/// <summary>
	/// The Player represents the user / viewer of the application.
	/// </summary>
	public class Player : MonoBehaviour
	{
		/// <summary>
		/// vrCommands which resets the players orientation.
		/// </summary>
		public vrCommand ResetRotationCommand;

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
		private void Awake()
		{
			ResetRotationCommand = new vrCommand("Reset Rotation Command", ResetRotation);
		}

		private vrValue ResetRotation(vrValue iValue)
		{
			var node = MiddleVR.VRDisplayMgr.GetNode("VRSystemCenterNode");
			node.SetOrientationLocal(new vrQuat(node.GetYawLocal(), 0, 0));
			return 0;
		}
#endif
	}
}
