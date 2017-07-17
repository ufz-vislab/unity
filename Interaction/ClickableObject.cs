using MiddleVR_Unity3D;
using UFZ.Rendering;
using UnityEngine;

namespace UFZ.Interaction
{
	[RequireComponent(typeof(VRActor))]
	public abstract class ClickableObject : MonoBehaviour
	{
		protected void OnValidate()
		{
			var vrActor = GetComponent<VRActor>();
			vrActor.Grabable = false;
			vrActor.SyncDirection = MVRNodesMapper.ENodesSyncDirection.NoSynchronization;

			if (GetComponent<Collider>())
				return;
			var matProps = GetComponentInChildren<MaterialProperties>();
			if (!matProps) return;
			var meshFilter = GetComponentsInChildren<MeshFilter>();
			if (meshFilter == null || meshFilter.Length <= 0) return;

			foreach (var filter in meshFilter)
			{
				var meshCollider = gameObject.AddComponent<MeshCollider>();
				meshCollider.sharedMesh = filter.sharedMesh;
			}
		}

		/// <summary>
		/// Is triggered when Wand button 0 was
		/// </summary>
		protected void VRAction(VRSelection iSelection)
		{
			Activate();
		}

		protected void OnMouseDown()
		{
			Debug.Log("OnMouseDown");
			Activate();
		}

		protected void OnTriggerEnter(Collider other)
		{
			// TODO Visualize this
			Debug.Log("OnTriggerEnter");
		}

		protected void OnTriggerExit(Collider other)
		{
			Debug.Log("OnOnTriggerExit");
		}

		protected void OnTriggerStay(Collider other)
		{
			if (IOC.Core.Instance.Input.WasOkButtonPressed())
			{
				Debug.Log("OkButtonWasPressed");
				Activate();
			}
		}

		protected abstract void Activate();
	}
}
