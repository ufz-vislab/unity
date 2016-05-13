using UnityEngine;
using System.Collections;

namespace UFZ
{
	[RequireComponent (typeof(BoxCollider))]
	public class CutsceneZoneTrigger : MonoBehaviour
	{
		public bool ShowGizmo = true;
		public Collider Collider;
		public Slate.Cutscene[] Cutscenes;

		public void Awake()
		{
			if (Collider == null)
				Collider = GameObject.Find("Player (Player)").GetComponent<Collider>();
		}

		void OnTriggerEnter (Collider other)
		{
			Debug.Log("Collision: " + other.name);
			if (other != Collider)
				return;

			foreach (var cutscene in Cutscenes) {
				if (cutscene.isActive)
					cutscene.Pause ();
				cutscene.Play ();
			}
		}

		void OnTriggerExit (Collider other)
		{
			if (other != Collider)
				return;

			foreach (var cutscene in Cutscenes) {
				if (cutscene.isActive) {
					cutscene.Pause ();
					cutscene.PlayReverse ();
				} else
					cutscene.PlayReverse (cutscene.playTimeMax, cutscene.playTimeMin);
			}
		}

		void OnDrawGizmos ()
		{
			if (!ShowGizmo)
				return;
			Gizmos.color = new Color (1f, 0f, 0f, 0.25f);
			Gizmos.DrawCube (transform.position, transform.lossyScale);
		}

		void OnDrawGizmosSelected ()
		{
			if (ShowGizmo)
				return;
			Gizmos.color = new Color (1f, 0f, 0f, 0.25f);
			Gizmos.DrawCube (transform.position, transform.lossyScale);
		}
	}
}
