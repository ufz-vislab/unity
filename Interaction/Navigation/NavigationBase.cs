using UnityEngine;

namespace UFZ.Interaction
{
	public abstract class NavigationBase : MonoBehaviour
	{
		public string NodeToMove = "VRSystemCenterNode";
		public string DirectionReferenceNode = "HandNode";
		public string TurnAroundNode = "HeadNode";

		public float NavigationSpeed = 2.0f; // m/s
		public float RotationSpeed = 45.0f;  // degrees/s
		public float DeadZone = 0.1f;

		public float RunningSpeed = 5.0f;

		protected GameObject VrMgr = null;
		private GameObject _directionRefNode;
		private GameObject _nodeToMove;
		private GameObject _turnNode;

		private bool _searchedRefNode = false;
		private bool _searchedNodeToMove = false;
		private bool _searchedRotationNode = false;

		protected float Forward = 0.0f;
		protected float Upward = 0.0f;
		protected float Sideward = 0.0f;
		protected float HorizontalRotation = 0.0f;
		protected float VerticalRotation = 0.0f;
		protected float Running = 0.0f;

		void Update()
		{
			UFZ.IOC.ILogger _logger = UFZ.IOC.Core.Instance.Log;
			if (_directionRefNode == null)
				_directionRefNode = GameObject.Find(DirectionReferenceNode);
			if (_nodeToMove == null)
				_nodeToMove = GameObject.Find(NodeToMove);
			if (_turnNode == null)
				_turnNode = GameObject.Find(TurnAroundNode);

			if (_searchedRefNode == false && _directionRefNode == null)
			{
				_logger.Error("BaseNavigation: Couldn't find '" + DirectionReferenceNode + "'");
				_searchedRefNode = true;
			}

			if (_searchedNodeToMove == false && _nodeToMove == null)
			{
				_logger.Error("BaseNavigation: Couldn't find '" + NodeToMove + "'");
				_searchedNodeToMove = true;
			}

			if (_searchedRotationNode == false && TurnAroundNode.Length > 0 && _turnNode == null)
			{
				_logger.Error("BaseNavigation: Couldn't find '" + TurnAroundNode + "'");
				_searchedRotationNode = true;
			}

			if (_nodeToMove == null || _directionRefNode == null)
				return;

			Forward = Upward = Sideward = HorizontalRotation = VerticalRotation = Running = 0.0f;

			GetInputs();

			if (Mathf.Abs(Forward) < DeadZone)
				Forward = 0.0f;
			if (Mathf.Abs(Upward) < DeadZone)
				Upward = 0.0f;
			if (Mathf.Abs(Sideward) < DeadZone)
				Sideward = 0.0f;
			if (Mathf.Abs(HorizontalRotation) < DeadZone)
				HorizontalRotation = 0.0f;
			if (Mathf.Abs(VerticalRotation) < DeadZone)
				VerticalRotation = 0.0f;
			if (Mathf.Abs(Running) < DeadZone)
				Running = 0.0f;

			UFZ.IOC.ITime _time = UFZ.IOC.Core.Instance.Time;
			var translationVector = new Vector3(1, 1, 1);
			var tVec = translationVector * (NavigationSpeed + Running * (RunningSpeed - NavigationSpeed)) *
			           _time.DeltaTime();
			var nVec = new Vector3(tVec.x * Sideward, tVec.y * Upward, tVec.z * Forward);
			var mVec = _directionRefNode.transform.TransformDirection(nVec);
			_nodeToMove.transform.Translate(mVec, Space.World);

			var horizontalRotation = HorizontalRotation * RotationSpeed * _time.DeltaTime();
			var verticalRotation = VerticalRotation * RotationSpeed * _time.DeltaTime();

			if (_turnNode != null)
			{
				_nodeToMove.transform.RotateAround(_turnNode.transform.position, new Vector3(0, 1, 0), horizontalRotation);
				_nodeToMove.transform.RotateAround(_turnNode.transform.position, new Vector3(1, 0, 0), verticalRotation);
			}
			else
			{
				_nodeToMove.transform.Rotate(new Vector3(0, 1, 0), horizontalRotation);
				_nodeToMove.transform.Rotate(new Vector3(1, 0, 0), verticalRotation);
			}

		}

		protected abstract void GetInputs();
	}
}
