using UnityEngine;
using UFZ.Rendering;

namespace UFZ.Interaction
{
	// TODO: merge with ViewpointObjectVisibility?
	public class ObjectFader : MonoBehaviour
	{
		//CameraPathBezierAnimator path = null;
		public Vector2[] Values = {new Vector2(0,1), new Vector2(1,1)};
		public GameObject[] GameObjects;

		void Start()
		{
			//path = gameObject.GetComponent<CameraPathBezierAnimator>();
		}

		void Update()
		{
			//if(path == null)
			//	return;
			//if(path.isPlaying)
			//	SetTime(path.percentage);
		}

		public void SetTime(float time)
		{
			int index = 0;
			foreach(Vector2 val in Values)
			{
				if(val.x < time && index < (Values.Length - 1))
					++index;
				else
					break;
			}
			if(index == 0)
				return;

			float delta = Values[index].x - Values[index - 1].x;
			float factor = (Values[index].x - time) / delta;
			float alpha = Mathf.Lerp(Values[index].y, Values[index - 1].y, factor);

			foreach(GameObject go in GameObjects)
			{
				foreach(Renderer currentRenderer in go.GetComponentsInChildren<Renderer>())
				{
					if(currentRenderer.gameObject.GetComponent<BoundingBoxClipCorner>() ||
					   currentRenderer.gameObject.GetComponent<BoundingBoxClipSphere>())
						continue;

					MaterialProperties matProps = currentRenderer.gameObject.GetComponent<MaterialProperties>();
					if(matProps == null)
						matProps = currentRenderer.gameObject.AddComponent<MaterialProperties>();

					matProps.Opacity = alpha;
				}
			}
		}
	}
}
