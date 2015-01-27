using DG.Tweening;
using UnityEngine;

namespace UFZ
{
	/// <summary>
	/// Creates a menu navigatable camera viewpoint. It is important that the GameObject where
	/// this script is attached to is a child of a GameObject named "Viewpoints"!
	/// </summary>
	public class Viewpoint : MonoBehaviour
	{
		public string NodeToMove = "VRSystemCenterNode";
		public vrCommand MoveToViewpointCommand;
		public vrCommand JumpToViewpointCommand;

		private GameObject _nodeToMove;
		private bool _searchedNodeToMove = false;

		private void Start()
		{
			MoveToViewpointCommand = new vrCommand("Move To Viewpoint Command " + GetInstanceID(), MoveToViewpoint);
			JumpToViewpointCommand = new vrCommand("Jump To Viewpoint Command " + GetInstanceID(), JumpToViewpoint);

			// Workaround to null exceptions when there is no subscriber to the event
			OnStart += delegate(float duration) { return; };
			OnFinish += delegate { return; };
		}

		private void Update()
		{
			var logger = UFZ.IOC.Core.Instance.Log;
			if (_nodeToMove == null)
				_nodeToMove = GameObject.Find(NodeToMove);
			if (_searchedNodeToMove != false || _nodeToMove != null)
				return;
			logger.Error("BaseNavigation: Couldn't find '" + NodeToMove + "'");
			_searchedNodeToMove = true;
		}

		public vrValue MoveToViewpoint(vrValue ivalue)
		{
			// TODO calculate speed
			const float duration = 5;

			_nodeToMove.transform.DOMove(transform.position, duration)
				.OnStart(() => OnStart(duration)).OnComplete(() => OnFinish());
			_nodeToMove.transform.DORotate(transform.rotation.eulerAngles, duration);
			return true;
		}

		public vrValue JumpToViewpoint(vrValue ivalue)
		{
			_nodeToMove.transform.position = transform.position;
			_nodeToMove.transform.rotation = transform.rotation;
			return true;
		}

		public delegate void OnFinishEvent();

		public event OnFinishEvent OnFinish;

		public delegate void OnStartEvent(float duration);

		public event OnStartEvent OnStart;
	}
}