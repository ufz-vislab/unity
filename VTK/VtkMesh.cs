using Kitware.VTK;
using UnityEngine;
using System.Collections;

namespace UFZ.VTK
{
	/// <summary>
	/// Storage class for creating a Unity mesh from a VTK poly data.
	/// </summary>
	public class VtkMesh
	{
		public Mesh Mesh;

		/// <summary>
		/// Generates the Unity Mesh.
		/// </summary>
		/// <param name="pd">The vtk poly data.</param>
		public void PolyDataToMesh(vtkPolyData pd)
		{
			if (pd == null)
			{
				Debug.LogWarning("No PolyData passed!");
				return;
			}

			var numVertices = pd.GetNumberOfPoints();
			if (numVertices == 0)
			{
				Debug.LogWarning("No vertices to convert!");
				return;
			}

			if (Mesh == null)
				Mesh = new Mesh();
			else
				Mesh.Clear();

			// Points / Vertices
			var vertices = new Vector3[numVertices];
			for (var i = 0; i < numVertices; ++i)
			{
				var pnt = pd.GetPoint(i);
				// Flip z-up to y-up
				vertices[i] = new Vector3(-(float) pnt[0], (float) pnt[2], (float) pnt[1]);
			}
			Mesh.vertices = vertices;

			// Normals
			var vtkNormals = pd.GetPointData().GetNormals();
			if (vtkNormals != null)
			{
				var numNormals = vtkNormals.GetNumberOfTuples();
				var normals = new Vector3[numNormals];
				for (var i = 0; i < numNormals; i++)
				{
					var normal = vtkNormals.GetTuple3(i);
					// flip normals ?
					normals[i] = new Vector3(-(float) normal[0], -(float) normal[1], -(float) normal[2]);
				}
				Mesh.normals = normals;
			}
			else
			{
				Debug.Log("No Normals!");
			}

			// Texture coordinates
			var vtkTexCoords = pd.GetPointData().GetTCoords();
			if (vtkTexCoords != null)
			{
				var numCoords = vtkTexCoords.GetNumberOfTuples();
				var uvs = new Vector2[numCoords];
				for (var i = 0; i < numCoords; ++i)
				{
					var texCoords = vtkTexCoords.GetTuple2(i);
					uvs[i] = new Vector2((float) texCoords[0], (float) texCoords[1]);
				}
				Mesh.uv = uvs;
			}

			// Vertex colors
//		if (numVertices > 0 && colorArray != null)
//		{
//			var colors = new Color32[numVertices];
//
//			for (var i = 0; i < numVertices; ++i)
//				colors[i] = GetColor32AtIndex(i);
//
//			Mesh.colors32 = colors;
//		}

			// Triangles / Cells
			var numTriangles = pd.GetNumberOfPolys();
			var polys = pd.GetPolys();
			if (polys.GetNumberOfCells() > 0)
			{
				var triangles = new int[numTriangles*3];
				var prim = 0;
				var pts = vtkIdList.New();
				polys.InitTraversal();
				while (polys.GetNextCell(pts) != 0)
				{
					for (var i = 0; i < pts.GetNumberOfIds(); ++i)
						triangles[prim*3 + i] = pts.GetId(i);

					++prim;
				}
				Mesh.SetTriangles(triangles, 0);
				//Mesh.RecalculateNormals();
				Mesh.RecalculateBounds();
				return;
			}

			// Lines
			var lines = pd.GetLines();
			if (lines.GetNumberOfCells() > 0)
			{
				var idList = new ArrayList();
				var pts = vtkIdList.New();
				lines.InitTraversal();
				while (lines.GetNextCell(pts) != 0)
				{
					for (var i = 0; i < pts.GetNumberOfIds() - 1; ++i)
					{
						idList.Add(pts.GetId(i));
						idList.Add(pts.GetId(i + 1));
					}
				}

				Mesh.SetIndices(idList.ToArray(typeof (int)) as int[], MeshTopology.Lines, 0);
				Mesh.RecalculateBounds();
				return;
			}

			// Points
			var points = pd.GetVerts();
			var numPointCells = points.GetNumberOfCells();
			if (numPointCells > 0)
			{
				var idList = new ArrayList();
				var pts = vtkIdList.New();
				points.InitTraversal();
				while (points.GetNextCell(pts) != 0)
				{
					for (int i = 0; i < pts.GetNumberOfIds(); ++i)
					{
						idList.Add(pts.GetId(i));
					}
				}

				Mesh.SetIndices(idList.ToArray(typeof (int)) as int[], MeshTopology.Points, 0);
				Mesh.RecalculateBounds();
			}
		}

//	private byte[] GetByteColorAtIndex(int i)
//	{
//		var scalar = colorArray.GetTuple1(i);
//		var dcolor = lut.GetColor(scalar);
//		var color = new byte[3];
//		for (uint j = 0; j < 3; j++)
//			color[j] = (byte)(255 * dcolor[j]);
//		return color;
//	}
//
//	private Color32 GetColor32AtIndex(int i)
//	{
//		var color = GetByteColorAtIndex(i);
//		return new Color32(color[0], color[1], color[2], 255);
//	}
//
//	private Color GetColorAtIndex(int i)
//	{
//		return GetColor32AtIndex(i);
//	}
	}
}
