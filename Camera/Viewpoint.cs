using System.Collections;
using DG.Tweening;
using UFZ.IOC;
using UnityEngine;
#if MVR
using MiddleVR_Unity3D;
#endif

namespace UFZ.Interaction
{
	/// <summary>
	/// Creates a menu navigatable camera viewpoint. It is important that the GameObject where
	/// this script is attached to is a child of a GameObject named "Viewpoints"!
	/// </summary>
	public class Viewpoint : MonoBehaviour
	{
		// TODO: Better set globally or based on scene size
		public float Speed = 1.5f;

		public float MaxTransitionTime = 5f;
		[HideInInspector]
		public string NodeToMove = "Player (Player)";
		[HideInInspector]
		public string Name = "Viewpoint";

		private GameObject _nodeToMove;
		private Tweener _moveTweener;
		private Tweener _rotateTweener;

		public void Awake()
		{
			Name = name;
		}

		public void Start()
		{
			_nodeToMove = GameObject.Find("Player (Player)");

			// Workaround to null exceptions when there is no subscriber to the event
			OnFinish += delegate { return; };
			OnStart += delegate (float duration) { return; };
			OnFinish += delegate { return; };
		}

		/// <summary>
		/// Moves a GameObject smoothly to this viewpoint.
		/// </summary>
		public void Move()
		{
			//const float speed = 1.5f; // units per seconds
			var vec = transform.position - _nodeToMove.transform.position;
			var length = vec.magnitude;
			var duration = length / Speed;
			if (duration > MaxTransitionTime)
				duration = MaxTransitionTime;

			var vps = FindObjectsOfType<Viewpoint>();
			foreach (var vp in vps)
				vp.Stop();

			//LeanTween.move(_nodeToMove, transform.position, duration);
			//LeanTween.rotate(_nodeToMove, transform.rotation.eulerAngles, duration)
			//	.setOnStart(() => OnStart(duration))
			//	.setOnComplete(() => OnFinish());
			_moveTweener = _nodeToMove.transform.DOMove(transform.position, duration)
				.OnStart(() => OnStart(duration))
				.OnComplete(() => OnFinish());
			_rotateTweener = _nodeToMove.transform.DORotate(transform.rotation.eulerAngles, duration);
		}

		/// <summary>
		/// Translates a GameObject to a viewpoint instantly.
		/// </summary>
		public void Jump()
		{
			_nodeToMove.transform.position = transform.position;
			_nodeToMove.transform.rotation = transform.rotation;
			if (OnSet != null) OnSet();
		}

		public void Stop()
		{
			if (_moveTweener != null)
				_moveTweener.Kill();
			if (_rotateTweener != null)
				_rotateTweener.Kill();
		}

		public delegate void OnSetEvent();
		public event OnSetEvent OnSet;

		public delegate void OnFinishEvent();
		public event OnFinishEvent OnFinish;

		public delegate void OnStartEvent(float duration);
		public event OnStartEvent OnStart;
	}
}