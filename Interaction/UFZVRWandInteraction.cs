/* UFZVRWandInteraction
 * MiddleVR
 * (c) i'm in VR
 */

using UnityEngine;
using MiddleVR_Unity3D;

namespace UFZ.Interaction
{
	public class UFZVRWandInteraction : MonoBehaviour {

		public float RayLength = 2;

		public bool Highlight = true;
		public Color HighlightColor = new Color();
		public Color GrabColor = new Color();

		public bool RepeatAction = false;

		GameObject _mObjectInHand;
		GameObject _mCurrentObject;


		bool _mObjectWasKinematic = true;

		private vrButtons _mButtons;
		private bool      _mSearchedButtons = false;

		private GameObject _mRay = null;

		// Use this for initialization
		void Start ()
		{
			_mRay = GameObject.Find("WandRay");

			if (_mRay == null) return;
			_mRay.transform.localScale = new Vector3( 0.01f, RayLength / 2.0f, 0.01f );
			_mRay.transform.localPosition = new Vector3( 0,0, RayLength / 2.0f );
		}

		private Collider GetClosestHit()
		{
			// Detect objects
			Vector3 dir = transform.localToWorldMatrix * Vector3.forward;

			RaycastHit[] hits = Physics.RaycastAll(transform.position, dir, RayLength);

			int i = 0;
			Collider closest = null;
			float distance = Mathf.Infinity;

			while (i < hits.Length)
			{
				RaycastHit hit = hits[i];

				//print("HIT : " + i + " : " + hit.collider.name);

				if( hit.distance < distance && hit.collider.name != "VRWand" && hit.collider.GetComponent<UFZVRActor>() != null )
				{
					distance = hit.distance;
					closest = hit.collider;
				}

				i++;
			}

			return closest;
		}

		private void HighlightObject( GameObject obj, bool state )
		{
			HighlightObject(obj, state, HighlightColor);
		}

		private void HighlightObject( GameObject obj, bool state, Color hCol )
		{
			GameObject hobj = _mRay;

			if (hobj == null || hobj.renderer == null || !Highlight) return;
			hobj.renderer.material.color = state ? hCol : Color.white;
		}

		private void Grab( GameObject iObject )
		{
			//MiddleVRTools.Log("Take :" + m_CurrentObject.name);

			_mObjectInHand = iObject;
			_mObjectInHand.transform.parent = transform.parent;

			if (_mObjectInHand.rigidbody != null)
			{
				_mObjectWasKinematic = _mObjectInHand.rigidbody.isKinematic;
				_mObjectInHand.rigidbody.isKinematic = true;
			}

			HighlightObject(_mObjectInHand, true, GrabColor);
		}

		private void Ungrab()
		{
			//MiddleVRTools.Log("Release : " + m_ObjectInHand);

			_mObjectInHand.transform.parent = null;

			if (_mObjectInHand.rigidbody != null)
			{
				if (!_mObjectWasKinematic)
					_mObjectInHand.rigidbody.isKinematic = false;
			}

			_mObjectInHand = null;

			HighlightObject(_mCurrentObject, false, HighlightColor);

			_mCurrentObject = null;
		}

		// Update is called once per frame
		void Update () {
			if (_mButtons == null)
			{
				_mButtons = MiddleVR.VRDeviceMgr.GetWandButtons();
			}

			if( _mButtons == null )
			{
				if (_mSearchedButtons == false)
				{
					//MiddleVRTools.Log("[~] VRWandInteraction: Wand buttons undefined. Please specify Wand Buttons in the configuration tool.");
					_mSearchedButtons = true;
				}
			}

			Collider hit = GetClosestHit();

			if( hit != null )
			{
				//print("Closest : " + hit.name);

				if( _mCurrentObject != hit.gameObject &&  _mObjectInHand == null )
				{
					//MiddleVRTools.Log("Enter other : " + hit.name);
					HighlightObject( _mCurrentObject, false );
					_mCurrentObject = hit.gameObject;
					HighlightObject(_mCurrentObject, true );
					//MiddleVRTools.Log("Current : " + m_CurrentObject.name);
				}
			}
				// Else
			else
			{
				//MiddleVRTools.Log("No touch ! ");

				if (_mCurrentObject != null && _mCurrentObject != _mObjectInHand)
				{
					HighlightObject(_mCurrentObject, false, HighlightColor );
					_mCurrentObject = null;
				}
			}

			//MiddleVRTools.Log("Current : " + m_CurrentObject);

			if (_mButtons != null && _mCurrentObject != null )
			{
				uint mainButton = MiddleVR.VRDeviceMgr.GetWandButton0();
				uint oneButton = MiddleVR.VRDeviceMgr.GetWandButton2();

				UFZVRActor script = _mCurrentObject.GetComponent<UFZVRActor>();

				//MiddleVRTools.Log("Trying to take :" + m_CurrentObject.name);
				if (script == null) return;
				// Grab
				if (_mButtons.IsToggled(mainButton))
				{
					if (script.Grabable)
						Grab(_mCurrentObject);
				}

				// Clip
				if(_mButtons.IsToggled(oneButton) && script.Clipable)
				{
					BoundingBoxClip clip = _mCurrentObject.GetComponent<BoundingBoxClip>();
					clip.enabled = !clip.enabled;
				}



				// Release
				if (_mButtons.IsToggled(mainButton, false) && _mObjectInHand != null)
					Ungrab();

				// Action
				if (((!RepeatAction && _mButtons.IsToggled(mainButton)) || (RepeatAction&& _mButtons.IsPressed(mainButton))))
					_mCurrentObject.SendMessage("VRAction", SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
