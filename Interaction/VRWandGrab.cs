using UnityEngine;
using System.Collections;

namespace UFZ.Interaction
{
	public class VRWandGrab : MonoBehaviour {

		public Transform Parent;
		public bool IgnoreRotation = true;
		Vector3 _localPosition;


		void OnEnable()
		{
			Parent = gameObject.transform.parent;
			gameObject.transform.parent = GameObject.Find("VRWand").transform;
			_localPosition = gameObject.transform.localPosition;
		}

		void OnDisable()
		{
			gameObject.transform.parent = Parent;
		}

		void Update ()
		{
			if(IgnoreRotation)
				gameObject.transform.rotation = Quaternion.identity;
			gameObject.transform.localPosition = _localPosition;
		}
	}
}
