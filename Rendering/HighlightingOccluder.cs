using UnityEngine;
using HighlightingSystem;

namespace UFZ.Rendering
{
	/// <summary>
	/// Activates Occluder on an Highlighter-script.
	/// </summary>
	[RequireComponent(typeof (Highlighter))]
	public class HighlightingOccluder : MonoBehaviour
	{
		private void Start()
		{
			GetComponent<Highlighter>().OccluderOn();
		}
	}
}
