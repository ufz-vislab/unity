using DG.Tweening;
using FullInspector;
using UnityEngine;

namespace UFZ.Interaction
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

		// TODO for MarkUX
		public string Name = "Viewpoint";

		private GameObject _nodeToMove;

		public void Awake()
		{
			Name = name;
		}

		public void Start()
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

		/// <summary>
		/// Moves a GameObject smoothly to this viewpoint.
		/// </summary>
		/// <param name="ivalue">Not used.</param>
		/// <returns></returns>
		public vrValue MoveToViewpoint(vrValue ivalue)
		{
			// TODO calculate speed
			const float duration = 5;

			_nodeToMove.transform.DOMove(transform.position, duration)
				.OnStart(() => OnStart(duration)).OnComplete(() => OnFinish());
			_nodeToMove.transform.DORotate(transform.rotation.eulerAngles, duration);
			return true;
		}

		/// <summary>
		/// Translates a GameObject to a viewpoint instantly.
		/// </summary>
		/// <param name="ivalue">Not used.</param>
		/// <returns></returns>
		public vrValue JumpToViewpoint(vrValue ivalue)
		{
			_nodeToMove.transform.position = transform.position;
			_nodeToMove.transform.rotation = transform.rotation;
			if(OnSet != null) OnSet();
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
