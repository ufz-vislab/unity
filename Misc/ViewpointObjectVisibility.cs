using System.Linq;
using DG.Tweening;
using FullInspector;
using UnityEngine;

namespace UFZ
{
	public class ViewpointObjectVisibility : BaseBehavior<FullSerializerSerializer>
	{
		public struct ObjectVisibility
		{
			public GameObject GameObject;
			public bool Visible;
			public float Opacity;
		}


		public ObjectVisibility[] Entries;

		private void Start()
		{
			var viewpoint = GetComponent<Viewpoint>();
			if (viewpoint == null)
			{
				UFZ.IOC.Core.Instance.Log.Warning("ViewpointObjectVisibility needs a Viewpoint script on the same GameObject!");
				return;
			}

			viewpoint.OnStart += Execute;

			foreach (var entry in Entries.Where(entry => entry.GameObject.GetComponent<MaterialProperties>() == null))
				entry.GameObject.AddComponent<MaterialProperties>();
		}

		private void Execute(float duration)
		{
			foreach (var entry in Entries)
			{
				var matProps = entry.GameObject.GetComponent<MaterialProperties>();
				if (entry.Visible)
				{
					if (Mathf.Approximately(entry.Opacity, 0))
						DOTween.To(() => matProps.Opacity, x => matProps.Opacity = x, 1f, duration);
					else
						DOTween.To(() => matProps.Opacity, x => matProps.Opacity = x, entry.Opacity, duration);
				}
				else
					DOTween.To(() => matProps.Opacity, x => matProps.Opacity = x, 0f, duration);
			}
		}
	}
}
