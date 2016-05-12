using UnityEngine;
using System.Collections;
using MarkLight;
using UFZ.Interaction;

namespace UFZ.Views
{
	[HideInPresenter]
	public class Player : View
	{
		public VRShareTransform VrShareTransformComponent;
		public Rigidbody RigidbodyComponent;
		public CapsuleCollider CapsuleColliderComponent;

		public override void Initialize()
		{
			base.Initialize();

			RigidbodyComponent.useGravity = false;
			CapsuleColliderComponent.radius = 0.25f;
			CapsuleColliderComponent.isTrigger = true;

			//var vrManager = FindObjectOfType<VRManagerScript>();
			//if (vrManager == null)
			//	Debug.LogError("VRManagerScript not found!");
			//else
			//	vrManager.VRSystemCenterNode = gameObject;
		}

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
