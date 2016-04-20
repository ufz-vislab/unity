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
	public class ObjectVisibility : BaseBehavior
	{
		/// <summary>
		/// Struct for storing transition data for one GameObject.
		/// </summary>
		public struct ObjectVisibilityInfo
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
			/// The transition duration
			/// </summary>
			public float Duration;
		}

		/// <summary>
		/// An array of object transition data.
		/// </summary>
		public ObjectVisibilityInfo[] Entries;

		[InspectorHeader("Global settings")]
		public float Opacity = 0f;
		public float Duration = 5f;

		[InspectorButton]
		public void OverwriteWithGlobalSettings()
		{
			for (var index = 0; index < Entries.Length; index++)
			{
				Entries[index].Opacity = Opacity;
				Entries[index].Duration = Duration;
			}
		}

		[InspectorOrder(0.0)]
		[InspectorButton]
		private void SetVisibilitiesInScene()
		{
			Do(true);
		}

		public void Do(bool immediate = false)
		{
			foreach (var entry in Entries)
			{
				if(entry.GameObject == null)
					continue;

				var matProps = entry.GameObject.GetComponentsInChildren<MaterialProperties>();
				foreach (var matProp in matProps)
				{
					// This assignment is necessary due to the old version of NET that Unity uses,
					// where the target of a foreach is not considered unique as it should
					// From http://dotween.demigiant.com/support.php
					var thisProp = matProp;
					if (entry.Duration > 0f && !immediate)
						DOTween.To(() => thisProp.Opacity, x => thisProp.Opacity = x, entry.Opacity, entry.Duration);
					else
						thisProp.Opacity = entry.Opacity;
				}
			}
		}
	}
}
