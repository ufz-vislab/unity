using UnityEngine;
using HighlightingSystem;

[RequireComponent(typeof(Highlighter))]
public class HighlightingOccluder : MonoBehaviour
{
	void Start ()
	{
		GetComponent<Highlighter>().OccluderOn();
	}
}
