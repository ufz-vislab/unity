/* VRWandInteraction
 * MiddleVR
 * (c) i'm in VR
 */

using UnityEngine;
using System;

namespace UFZ.Interaction
{
	public class NavigationController : MonoBehaviour
	{
		private const string NodeToMove = "CenterNode";
		private const string DirectionReferenceNode = "HandNode";
		private const string TurnAroundNode = "HeadNode";

		public float NavigationSpeed = 1.0f;
		public float RotationSpeed = 1.0f;

		public bool Fly = true;

		private bool _mSearchedRefNode = false;
		private bool _mSearchedNodeToMove = false;
		private bool _mSearchedRotationNode = false;

		// Update is called once per frame
		void Update()
		{
			GameObject directionRefNode = GameObject.Find(DirectionReferenceNode);
			GameObject nodeToMove       = GameObject.Find(NodeToMove);
			GameObject turnNode         = GameObject.Find(TurnAroundNode);

			if ( _mSearchedRefNode == false && directionRefNode == null )
			{
				Debug.Log("[X] VRWandNavigation: Couldn't find '" + DirectionReferenceNode + "'");
				_mSearchedRefNode = true;
			}

			if (_mSearchedNodeToMove == false && nodeToMove == null)
			{
				Debug.Log("[X] VRWandNavigation: Couldn't find '" + NodeToMove + "'");
				_mSearchedNodeToMove = true;
			}

			if (_mSearchedRotationNode == false && TurnAroundNode.Length > 0 && turnNode == null)
			{
				Debug.Log("[X] VRWandNavigation: Couldn't find '" + TurnAroundNode + "'");
				_mSearchedRotationNode = true;
			}

			if (directionRefNode != null && nodeToMove != null )
			{
				float speed = 0.0f;
				float speedR = 0.0f;
				float forward = 0f; // Z-axis
				float rotation = 0f; // around Y-axis

				// Flystick via MVR
				GameObject vrmgr = GameObject.Find("VRManager");
				if( vrmgr != null )
				{
					VRManagerScript script = vrmgr.GetComponent<VRManagerScript>();

					if( script != null )
					{
						forward = script.WandAxisVertical;
						rotation = script.WandAxisHorizontal;

					}
				}

				// Razor Hydra via Sixense SDK
				//SixenseInput.Controller leftHandController = SixenseInput.GetController(SixenseHands.LEFT);
				//SixenseInput.Controller rightHandController = SixenseInput.GetController(SixenseHands.RIGHT);

				//if(rightHandController != null)
				//{
				//	if(Mathf.Approximately(forward,0f))
				//		forward = rightHandController.JoystickY;
				//	if(Mathf.Approximately(rotation, 0f))
				//		rotation = rightHandController.JoystickX;
				//}

				// Keyboard
				if(Mathf.Approximately(forward, 0f))
				{
					vrKeyboard keyb = MiddleVR.VRDeviceMgr.GetKeyboard();
					if(keyb.IsKeyPressed(MiddleVR.VRK_W))
						forward = 1f;
					else if(keyb.IsKeyPressed(MiddleVR.VRK_S))
						forward = -1f;
				}

				// TODO Spacenav

				// Common navigation calculation
				float deltaTime = UFZ.IOC.Core.Instance.Time.DeltaTime();

				if (Math.Abs(forward) > 0.1) speed = forward * NavigationSpeed * deltaTime;
				if (Math.Abs(rotation) > 0.1) speedR = rotation * RotationSpeed * deltaTime;

				// Computing direction
				Vector3 translationVector = new Vector3(0, 0, 1);
				Vector3 tVec = translationVector;
				Vector3 nVec = new Vector3(tVec.x * speed, tVec.y * speed, tVec.z * speed );

				Vector3 mVec = directionRefNode.transform.TransformDirection(nVec);

				if( Fly == false )
					mVec.y = 0.0f;

				nodeToMove.transform.Translate(mVec,Space.World);

				if (turnNode != null)
					nodeToMove.transform.RotateAround(turnNode.transform.position, new Vector3(0, 1, 0), speedR);
				else
					nodeToMove.transform.Rotate(new Vector3(0, 1, 0), speedR);
			}
		}
	}
}
