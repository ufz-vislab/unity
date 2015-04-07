using UnityEngine;

namespace UFZ.Tools
{
	/// <summary>
	/// Reverses normals in a Mesh on start.
	/// </summary>
	/// Can be attached to a GameObject with a MeshFilter-component.
	[RequireComponent(typeof (MeshFilter))]
	public class ReverseNormals : MonoBehaviour
	{
		private void Start()
		{
			var filter = GetComponent(typeof (MeshFilter)) as MeshFilter;
			if (filter == null) return;
			var mesh = filter.mesh;

			var normals = mesh.normals;
			for (var i = 0; i < normals.Length; i++)
				normals[i] = -normals[i];
			mesh.normals = normals;

			for (var m = 0; m < mesh.subMeshCount; m++)
			{
				var triangles = mesh.GetTriangles(m);
				for (var i = 0; i < triangles.Length; i += 3)
				{
					var temp = triangles[i + 0];
					triangles[i + 0] = triangles[i + 1];
					triangles[i + 1] = temp;
				}
				mesh.SetTriangles(triangles, m);
			}
		}
	}
}
