using UnityEngine;
using MiddleVR_Unity3D;

[AddComponentMenu("MiddleVR/Interactions/Attach UI to Node")]
public class VRAttachUIToNode : MonoBehaviour
{
	public string VRParentNode = "HeadNode";

	private bool m_Attached;
	private bool m_Searched;

	protected void Update()
	{
		if (m_Attached) return;
		var node = GameObject.Find(VRParentNode);

		if (VRParentNode.Length == 0)
		{
			MVRTools.Log(0, "[X] AttachToNode: Please specify a valid VRParentNode name.");
		}

		if (node != null)
		{
			var oldPos = transform.localPosition;
			var oldRot = transform.localRotation;
			var oldScale = transform.localScale;

			// Setting new parent
			transform.SetParent(node.transform, false);
			transform.localPosition = oldPos;
			transform.localRotation = oldRot;
			transform.localScale = oldScale;

			MVRTools.Log(2, "[+] AttachToNode: " + name + " attached to : " + node.name);
			m_Attached = true;

			// Stop this component now.
			enabled = false;
		}
		else
		{
			if (m_Searched) return;
			MVRTools.Log(0, "[X] AttachToNode: Failed to find Game object '" + VRParentNode + "'");
			m_Searched = true;

			// Stop this component now.
			enabled = false;
		}
	}
}
