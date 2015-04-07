using UFZ.Helper;
using UnityEngine;
using MiddleVR_Unity3D;

namespace UFZ.Interaction
{
	[AddComponentMenu("UFZ/VR Actor")]
	public class VRActor : MonoBehaviour
	{
		public bool Grabable = true;
		public bool Clipable = true;
		public bool CollidersOnChilds = false;

		// Use this for initialization
		void Start()
		{
			if (gameObject.GetComponent<Rigidbody>() == null)
			{
				var body = gameObject.AddComponent<Rigidbody>();
				body.isKinematic = true;
			}

			AddCollider(gameObject);
			if (CollidersOnChilds)
				IterateChildren.Iterate(gameObject, AddCollider, true);

			if (Clipable)
			{
				var clip = gameObject.AddComponent<BoundingBoxClip>();
				clip.enabled = false;
			}

			if (Grabable)
			{
				var grab = gameObject.AddComponent<VRWandGrab>();
				grab.enabled = false;
			}
		}

		static void AddCollider(GameObject go)
		{
			if (go.GetComponent<Collider>() == null)
			{
				if (go.GetComponent<MeshFilter>())
					go.AddComponent<MeshCollider>().sharedMesh =
						go.GetComponent<MeshFilter>().mesh;
				else
					go.AddComponent<BoxCollider>();
			}

			if (go.GetComponent<Collider>() == null)
			{
				MiddleVRTools.Log("[X] Actor object " + go.name + " has no collider !! Put one or it won't act. " + go.name);
			}
		}
	}
}
