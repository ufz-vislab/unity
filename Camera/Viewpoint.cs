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
		public string NodeToMove = "Player";
		public vrCommand MoveToViewpointCommand;
		public vrCommand JumpToViewpointCommand;
		public bool StartHere = false;

		private GameObject _nodeToMove;

		private void Start()
		{
			_nodeToMove = GameObject.Find("Player");
			MoveToViewpointCommand = new vrCommand("Move To Viewpoint Command " + GetInstanceID(), MoveToViewpoint);
			JumpToViewpointCommand = new vrCommand("Jump To Viewpoint Command " + GetInstanceID(), JumpToViewpoint);

			if (StartHere)
				JumpToViewpoint(0);

			// Workaround to null exceptions when there is no subscriber to the event
			OnFinish += delegate { return; };
			OnStart += delegate(float duration) { return; };
			OnFinish += delegate { return; };
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
			OnSet();
			return true;
		}

		public delegate void OnSetEvent();
		public event OnSetEvent OnSet;

		public delegate void OnFinishEvent();
		public event OnFinishEvent OnFinish;

		public delegate void OnStartEvent(float duration);
		public event OnStartEvent OnStart;
	}
}