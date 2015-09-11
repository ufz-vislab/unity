#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace UFZ.Interaction
{
	/// <summary>
	/// Base class for fly-like navigation.
	/// </summary>
	///
	/// Subclasses have to implement GetInputs().
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
	public abstract class NavigationBase : VRInteraction
#else
	public abstract class NavigationBase : MonoBehaviour
#endif
	{
		/// <summary>
		/// The name of the GameObject which will be moved.
		/// </summary>
		public string NodeToMove = "Player";
		/// <summary>
		/// The name of the GameObject which rotation determines the fly direction.
		/// </summary>
		public string DirectionReferenceNode = "HandNode";
		/// <summary>
		/// The name of the GameObject which will be the center of the rotation.
		/// </summary>
		public string TurnAroundNode = "HeadNode";

		/// <summary>
		/// The navigation speed in m/s.
		/// </summary>
		public float NavigationSpeed = 2.0f;
		/// <summary>
		/// The rotation speed in degress/s
		/// </summary>
		public float RotationSpeed = 45.0f;
		/// <summary>
		/// The dead zone in which inputs are ignored.
		/// </summary>
		public float DeadZone = 0.1f;

		/// <summary>
		/// The running speed in m/s
		/// </summary>
		public float RunningSpeed = 5.0f;

		protected GameObject VrMgr = null;
		private GameObject _directionRefNode;
		private GameObject _nodeToMove;
		private GameObject _turnNode;

		private bool _searchedRefNode;
		private bool _searchedNodeToMove;
		private bool _searchedRotationNode;

		/// <summary>
		/// The strength of forward translation, range [-1,1].
		/// </summary>
		protected float Forward = 0.0f;
		/// <summary>
		/// The strength of upward (vertical) translation, range [-1,1].
		/// </summary>
		protected float Upward = 0.0f;
		/// <summary>
		/// The strength of sideward (horizontal) translation, range [-1,1].
		/// </summary>
		protected float Sideward = 0.0f;
		protected float HorizontalRotation = 0.0f;
		protected float VerticalRotation = 0.0f;
		/// <summary>
		/// The running factor, range [0,1].
		/// </summary>
		protected float Running = 0.0f;

		void Update()
		{
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
			if (!IsActive())
				return;
#endif


#if UNITY_EDITOR
			// Disable input when GameView is not focussed
#if UNITY_4_0
			var window = EditorWindow.focusedWindow;
			if (window != null && window.title != "UnityEditor.GameView")
				return;
#endif
#if UNITY_5_0
			var window = EditorWindow.focusedWindow;
			if (window != null && window.titleContent.text != "Game")
				return;
#endif
#endif

			var logger = IOC.Core.Instance.Log;
			if (_directionRefNode == null)
				_directionRefNode = GameObject.Find(DirectionReferenceNode);
			if (_nodeToMove == null)
				_nodeToMove = GameObject.Find(NodeToMove);
			if (_turnNode == null)
				_turnNode = GameObject.Find(TurnAroundNode);

			if (_searchedRefNode == false && _directionRefNode == null)
			{
				logger.Error("BaseNavigation: Couldn't find '" + DirectionReferenceNode + "'");
				_searchedRefNode = true;
			}

			if (_searchedNodeToMove == false && _nodeToMove == null)
			{
				logger.Error("BaseNavigation: Couldn't find '" + NodeToMove + "'");
				_searchedNodeToMove = true;
			}

			if (_searchedRotationNode == false && TurnAroundNode.Length > 0 && _turnNode == null)
			{
				logger.Error("BaseNavigation: Couldn't find '" + TurnAroundNode + "'");
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
			if (Mathf.Abs(Running) < DeadZone)
				Running = 0.0f;

			var time = IOC.Core.Instance.Time;

			if (!(Mathf.Approximately(Forward, 0f)
				  && Mathf.Approximately(Upward, 0f)
				  && Mathf.Approximately(Sideward, 0f)))
			{
				var translationVector = new Vector3(1, 1, 1);
				var tVec = translationVector * (NavigationSpeed + Running * (RunningSpeed - NavigationSpeed)) *
						   time.DeltaTime();
				var nVec = new Vector3(tVec.x * Sideward, tVec.y * Upward, tVec.z * Forward);
				if (!Mathf.Approximately(nVec.magnitude, 0f))
				{
					var mVec = _directionRefNode.transform.TransformDirection(nVec);
					_nodeToMove.transform.Translate(mVec, Space.World);
				}
			}

			if (Mathf.Abs(HorizontalRotation) > DeadZone)
			{
				var horizontalRotation = HorizontalRotation * RotationSpeed * time.DeltaTime();
				if (_turnNode != null)
					_nodeToMove.transform.RotateAround(_turnNode.transform.position, new Vector3(0, 1, 0), horizontalRotation);
				else
					_nodeToMove.transform.Rotate(new Vector3(0, 1, 0), horizontalRotation);
			}
			if (Mathf.Abs(VerticalRotation) > DeadZone)
			{
				var verticalRotation = VerticalRotation * RotationSpeed * time.DeltaTime();

				if (_turnNode != null)
					_nodeToMove.transform.RotateAround(_turnNode.transform.position, new Vector3(1, 0, 0), verticalRotation);
				else
					_nodeToMove.transform.Rotate(new Vector3(1, 0, 0), verticalRotation);
			}
		}

		/// <summary>
		/// Inputs should be handled here. The following member should be set:
		/// Forward, Upward, Sideward, HorizontalRotation and VerticalRotation.
		/// </summary>
		protected abstract void GetInputs();
	}
}
