using UnityEngine;

namespace UFZ.Interaction
{
	public class BoundingBoxClipCorner : MonoBehaviour {

		public int Index;
		Vector3 _oldPosition;

		void Start () {
			_oldPosition = gameObject.transform.position;
		}

		void Update ()
		{
			Vector3 newPosition = gameObject.transform.position;
			if (newPosition != _oldPosition)
			{
				_oldPosition = gameObject.transform.position;
				GameObject parentObject = gameObject.GetComponent<VRWandGrab>().Parent.gameObject;
				parentObject.GetComponent<BoundingBoxClip> ().SetCornerPosition (Index);
			}
		}

		public void SetAsClipOctant()
		{
			GameObject parentObject = gameObject.GetComponent<VRWandGrab>().Parent.gameObject;
			parentObject.GetComponent<BoundingBoxClip> ().SetCutoutOctant(Index);
		}
	}
}
