using UnityEngine;

namespace UFZ.Interaction
{
	public class BoundingBoxClip : MonoBehaviour
	{
		public GameObject[] Boxes;
		public GameObject Sphere;
		Shader _oldShader;
		Vector3[][] lineArray = new Vector3[3][];

		void OnEnable ()
		{
			_oldShader = gameObject.renderer.material.shader;
			gameObject.renderer.material.shader = Shader.Find ("UFZ/Bounding Box Clip");

			if (Boxes == null)
			{

				// Calculate gizmo sizes
				Bounds bounds = gameObject.renderer.bounds;
				float boxSizeMin = Mathf.Max(bounds.size[0], bounds.size[2]) / 20f;

				// Check and fix for zeros in bounds sizes
				bool expanded = false;
				Vector3 expandVector = Vector3.zero;
				for(int i = 0; i < 3; ++i)
				{
					if(Mathf.Approximately(bounds.size[i], 0.0f))
					{
						expandVector[i] += boxSizeMin / 2f;
						expanded = true;
					}
				}
				bounds.Expand(expandVector);

				float boxSize;
				if(expanded)
					boxSize = boxSizeMin / 4f;
				else
				{
					float boxSizeMax = Mathf.Min(bounds.size[0], bounds.size[1], bounds.size[2]) / 2.1f;
					boxSize = Mathf.Min(boxSizeMin, boxSizeMax);
				}
				boxSize *= 1f / gameObject.transform.lossyScale.x;

				Boxes = new GameObject[8];
				for (int i = 0; i < 8; i++)
				{
					Boxes [i] = GameObject.CreatePrimitive (PrimitiveType.Cube);
					Boxes [i].transform.parent = gameObject.transform;
					Boxes [i].transform.localScale = new Vector3 (boxSize, boxSize, boxSize);
					Boxes[i].renderer.material.shader = Shader.Find("Diffuse");
					UFZVRActor vrActor = Boxes[i].AddComponent<UFZVRActor>();
					vrActor.Clipable = false;
					Boxes [i].AddComponent ("UIButtonScale");
					BoundingBoxClipCorner cornerScript = Boxes [i].AddComponent<BoundingBoxClipCorner>();
					cornerScript.Index = i;
				}
				Sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
				Sphere.transform.parent = gameObject.transform;
				Sphere.transform.localPosition = new Vector3 (0f, 0f, 0f);
				Sphere.transform.localScale = new Vector3 (boxSize, boxSize, boxSize);
				UFZVRActor vrActorSphere = Sphere.AddComponent<UFZVRActor>();
				vrActorSphere.Clipable = false;
				Sphere.AddComponent ("UIButtonScale");
				Sphere.AddComponent ("BoundingBoxClipSphere");

				ResetClipping (bounds);

				// Line drawing
				lineArray[0] = new Vector3[5];
				lineArray[1] = new Vector3[5];
				lineArray[2] = new Vector3[8];

				SetLinePositions();
			}
			ShowGizmos (true);
		}

		void OnDisable ()
		{
			ShowGizmos (false);

			gameObject.renderer.material.shader = _oldShader;
		}

		public void ResetClipping (Bounds bounds)
		{
			Sphere.transform.localPosition = new Vector3(0f, 0f, 0f);
			SetCurrentBounds (bounds);
			SetCutoutPosition(Sphere.transform.position);
			SetCutoutOctant(4);
		}

		public void ShowGizmos (bool showGizmos)
		{
			foreach (GameObject box in Boxes)
				box.renderer.enabled = showGizmos;

			Sphere.renderer.enabled = showGizmos;

			// Disable collider
			gameObject.collider.enabled = !showGizmos;
		}

		public void SetCornerPosition (int index)
		{
			Bounds newBounds;
			switch (index) {
				case 0:
				case 6:
					newBounds = new Bounds (Boxes [0].transform.position, Vector3.zero);
					newBounds.Encapsulate (Boxes [6].transform.position);
					break;
				case 1:
				case 7:
					newBounds = new Bounds (Boxes [1].transform.position, Vector3.zero);
					newBounds.Encapsulate (Boxes [7].transform.position);
					break;
				case 2:
				case 4:
					newBounds = new Bounds (Boxes [2].transform.position, Vector3.zero);
					newBounds.Encapsulate (Boxes [4].transform.position);
					break;
				case 3:
				case 5:
					newBounds = new Bounds (Boxes [3].transform.position, Vector3.zero);
					newBounds.Encapsulate (Boxes [5].transform.position);
					break;
				default:
					newBounds = new Bounds ();
					break;
			}

			SetCurrentBounds (newBounds);
			SetLinePositions();
		}

		public void SetCutoutPosition (Vector3 pos)
		{
			gameObject.renderer.material.SetVector ("cutout_pos", (pos - gameObject.transform.position) * (1f/gameObject.transform.lossyScale.x));
		}

		public void SetCutoutOctant(int index)
		{
			var cutoutVector = new Vector3(1, 1, 1);
			if(index < 4)
				cutoutVector.y = 0;
			if((index % 4) == 0 ||
			   (index % 4) == 1)
				cutoutVector.z = 0;
			if((index % 4) == 0 ||
			   (index % 4) == 3)
				cutoutVector.x = 0;

			gameObject.renderer.material.SetVector("cutout_octant", cutoutVector);
		}

		void SetCurrentBounds (Bounds bounds)
		{
			// Boxes are arranged counter-clockwise, bottom-first
			Boxes [0].transform.position = bounds.min;
			Boxes [1].transform.position = bounds.min + new Vector3 (bounds.size.x, 0, 0);
			Boxes [2].transform.position = bounds.min + new Vector3 (bounds.size.x, 0, bounds.size.z);
			Boxes [3].transform.position = bounds.min + new Vector3 (0, 0, bounds.size.z);
			Boxes [4].transform.position = bounds.min + new Vector3 (0, bounds.size.y, 0);
			Boxes [5].transform.position = bounds.min + new Vector3 (bounds.size.x, bounds.size.y, 0);
			Boxes [6].transform.position = bounds.min + new Vector3 (bounds.size.x, bounds.size.y, bounds.size.z);
			Boxes [7].transform.position = bounds.min + new Vector3 (0, bounds.size.y, bounds.size.z);

			Bounds shaderBounds = new Bounds (bounds.center - gameObject.transform.position, bounds.size);
			Vector3 minVector = shaderBounds.min * (1f/gameObject.transform.lossyScale.x);
			Vector3 maxVector = shaderBounds.max * (1f/gameObject.transform.lossyScale.x);
			gameObject.renderer.material.SetVector ("minima", minVector);
			gameObject.renderer.material.SetVector ("maxima", maxVector);
		}

		void SetLinePositions()
		{
			for(int i = 0; i < 5; i++)
				lineArray[0][i] = Boxes[i % 4].transform.position;

			for(int i = 0; i < 5; i++)
				lineArray[1][i] = Boxes[(i % 4) + 4].transform.position;

			for(int i = 0; i < 4; i++)
			{
				lineArray[2][i * 2] = Boxes[i].transform.position;
				lineArray[2][i * 2 + 1] = Boxes[i + 4].transform.position;
			}
		}
	}
}
