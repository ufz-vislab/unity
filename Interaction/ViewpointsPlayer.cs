using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UFZ.Interaction
{
	public class ViewpointsPlayer : MonoBehaviour
	{
		public struct ViewpointData
		{
			public Viewpoint Viewpoint;
			public bool Override;
			public float Speed;
			public float MaxTransitionTime;
		}
		public bool PlayAtStart;
		public bool Loop = true;
		public float WaitTime = 5;
		public List<ViewpointData> ViewpointsData;

		private int _currentViewpoint;

		void Start()
		{
			if (PlayAtStart)
				ViewpointsData[0].Viewpoint.Move();

			foreach (var viewpoint in ViewpointsData)
			{
				if (viewpoint.Override)
				{
					viewpoint.Viewpoint.MaxTransitionTime = viewpoint.MaxTransitionTime;
					viewpoint.Viewpoint.Speed = viewpoint.Speed;
				}
				viewpoint.Viewpoint.OnFinish += delegate
				{
					if (_currentViewpoint == ViewpointsData.Count - 1)
					{
						if (Loop)
							_currentViewpoint = 0;
						else
							return;
					}
					else
						_currentViewpoint++;
					
					StartCoroutine(Move(ViewpointsData[_currentViewpoint].Viewpoint));
				};
			}
		}

		private IEnumerator Move(Viewpoint viewpoint)
		{
			yield return new WaitForSeconds(WaitTime);
			viewpoint.Move();
		}

		[Button]
		public void Play()
		{
			ViewpointsData[_currentViewpoint].Viewpoint.Move();
		}
	}
}