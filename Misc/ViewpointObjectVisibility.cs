using DG.Tweening;
using FullInspector;
using UnityEngine;
using UFZ.Rendering;

namespace UFZ.Interaction
{
	/// <summary>
	/// Can be attached to a Viewpoint and controls fade in/out of objects
	/// upon view point arrival.
	/// </summary>
	public class ViewpointObjectVisibility : BaseBehavior
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

		/// <summary>
		/// Struct for storing transition data for one GameObject.
		/// </summary>
		public struct ObjectVisibility
		{
			/// <summary>
			/// The game object which will be fade in / out.
			/// </summary>
			public GameObject GameObject;
			/// <summary>
			/// The target opacity.
			/// </summary>
			public float Opacity;
			/// <summary>
			/// The transition type
			/// </summary>
			public VisibilityTransition TransitionType;
		}

		/// <summary>
		/// An array of object transition data.
		/// </summary>
		public ObjectVisibility[] Entries;

		private void Start()
		{
			var viewpoint = GetComponent<Viewpoint>();
			if (viewpoint == null)
			{
				IOC.Core.Instance.Log.Warning("ViewpointObjectVisibility needs a Viewpoint script on the same GameObject!");
				return;
			}

			viewpoint.OnStart += OnStart;
			viewpoint.OnFinish += OnComplete;
			viewpoint.OnSet += OnSet;
		}

		private void OnStart(float duration)
		{
			foreach (var entry in Entries)
			{
				if(entry.TransitionType == VisibilityTransition.StepOnComplete)
					continue;

				var matProps = entry.GameObject.GetComponentsInChildren<MaterialProperties>();
				foreach (var matProp in matProps)
				{
					if (entry.TransitionType == VisibilityTransition.Smooth && duration > 0f)
						DOTween.To(() => matProp.Opacity, x => matProp.Opacity = x, entry.Opacity, duration);
					else
						matProp.Opacity = entry.Opacity;
				}
			}
		}

		private void OnComplete()
		{
			foreach (var entry in Entries)
			{
				if (entry.TransitionType != VisibilityTransition.StepOnComplete)
					continue;

				var matProps = entry.GameObject.GetComponentsInChildren<MaterialProperties>();
				foreach (var matProp in matProps)
					matProp.Opacity = entry.Opacity;
			}
		}

		private void OnSet()
		{
			foreach (var entry in Entries)
			{
				var matProps = entry.GameObject.GetComponentsInChildren<MaterialProperties>();
				foreach (var matProp in matProps)
					matProp.Opacity = entry.Opacity;
			}
		}
	}
}
