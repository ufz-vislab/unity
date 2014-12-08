using UnityEngine;
using System.Collections;

// Creates a LineRenderer and adds all vertices as points
// Always creates one closed line.
[ExecuteInEditMode]
public class MyLineRenderer : MonoBehaviour {

	void Start ()
	{
		LineRenderer lineRenderer = gameObject.AddComponent("LineRenderer") as LineRenderer;
		Vector3[] vertices = GetComponent<MeshFilter> ().sharedMesh.vertices;
		lineRenderer.SetVertexCount (vertices.Length);
		int i = 0;
		foreach (Vector3 vertex in vertices) {
			lineRenderer.SetPosition (i, vertex);
			i++;
		}
		
			
	}
}
