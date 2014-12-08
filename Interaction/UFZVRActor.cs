/* UFZVRActor
 * MiddleVR
 * (c) i'm in VR
 */
using UnityEngine;
using MiddleVR_Unity3D;

namespace UFZ.Interaction
{
	[AddComponentMenu("UFZ/VR Actor")]
	public class UFZVRActor : MonoBehaviour
	{
		public bool Grabable = true;
		public bool Clipable = true;
		public bool CollidersOnChilds = false;

		// Use this for initialization
		void Start()
		{
			if (gameObject.rigidbody == null)
			{
				Rigidbody body = gameObject.AddComponent<Rigidbody>();
				body.isKinematic = true;
			}

			AddCollider(gameObject);
			if (CollidersOnChilds)
				IterateChildren.Iterate(gameObject, delegate(GameObject go)
				{ AddCollider(go); }, true);

			if (Clipable)
			{
				BoundingBoxClip clip = gameObject.AddComponent<BoundingBoxClip>();
				clip.enabled = false;
			}

			if (Grabable)
			{
				VRWandGrab grab = gameObject.AddComponent<VRWandGrab>();
				grab.enabled = false;
			}
		}

		static void AddCollider(GameObject go)
		{
			if (go.collider == null)
			{
				if (go.GetComponent<MeshFilter>())
					go.AddComponent<MeshCollider>().sharedMesh =
						go.GetComponent<MeshFilter>().mesh;
				else
					go.AddComponent<BoxCollider>();
			}

			if (go.collider == null)
			{
				MiddleVRTools.Log("[X] Actor object " + go.name + " has no collider !! Put one or it won't act. " + go.name);
			}
		}
	}
}
