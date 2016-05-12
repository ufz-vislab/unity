using System.Runtime.Remoting;
using FullInspector;
using MiddleVR_Unity3D;
using UnityEngine;

namespace UFZ.Interaction
{
	[RequireComponent(typeof(VRActor))]
	public abstract class ClickableObject : BaseBehavior
	{
		protected override void OnValidate()
		{
			base.OnValidate();

			var vrActor = GetComponent<VRActor>();
			vrActor.Grabable = false;
			vrActor.SyncDirection = MVRNodesMapper.ENodesSyncDirection.NoSynchronization;
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
