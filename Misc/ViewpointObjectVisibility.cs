using System.Linq;
using DG.Tweening;
using FullInspector;
using UnityEngine;

namespace UFZ
{
	public class ViewpointObjectVisibility : BaseBehavior<FullSerializerSerializer>
	{
		public enum VisibilityTransition
		{
			Smooth,
			StepOnStart,
			StepOnComplete
		}
		public struct ObjectVisibility
		{
			public GameObject GameObject;
			public float Opacity;
			public VisibilityTransition TransitionType;
		}


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
					if (entry.TransitionType == VisibilityTransition.Smooth)
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
				if(entry.TransitionType != VisibilityTransition.StepOnComplete)
					continue;

				var matProps = entry.GameObject.GetComponent<MaterialProperties>();
				matProps.Opacity = entry.Opacity;
			}
		}
	}
}
