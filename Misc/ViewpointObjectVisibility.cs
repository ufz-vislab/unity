using System;
using Sirenix.OdinInspector;

namespace UFZ.Interaction
{
	/// <summary>
	/// Can be attached to a Viewpoint and controls fade in/out of objects
	/// upon view point arrival.
	/// </summary>
	public class ViewpointObjectVisibility : ObjectVisibility
	{
		/// <summary>
		/// Defines the visibility transition.
		/// </summary>
		public enum VisibilityTransition
		{
			/// <summary>Smoothly transitions over the viewpoint duration.</summary>
			Smooth,
			/// <summary>
			/// Fades in / out on transition to viewpoint start instantly.
			/// </summary>
			StepOnStart,
			/// <summary>Fades in / out on viewpoint arrival instantly.</summary>
			StepOnComplete
		}

		public VisibilityTransition Transition = VisibilityTransition.Smooth;

		private void Start()
		{
			var viewpoint = GetComponent<Viewpoint>();
			if (viewpoint == null)
			{
				Core.Warning("ViewpointObjectVisibility needs a Viewpoint script on the same GameObject!");
				return;
			}

			viewpoint.OnStart += OnStart;
			viewpoint.OnFinish += OnComplete;
			viewpoint.OnSet += OnSet;
		}

		private void OnStart(float duration)
		{
			switch (Transition)
			{
				case VisibilityTransition.StepOnStart:
					Do(true);
					break;

				case VisibilityTransition.Smooth:
					Do();
					break;
				case VisibilityTransition.StepOnComplete:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void OnComplete()
		{
			if(Transition == VisibilityTransition.StepOnComplete)
				Do(true);
		}

		private void OnSet()
		{
			Do(true);
		}
	}
}
