using Kitware.VTK;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UFZ.VTK
{
	/// <summary>
	/// Storage class for creating a Unity mesh from a VTK poly data.
	/// </summary>
	public class VtkMesh
	{
		public List<Mesh> Meshes;

		private const int MaxVertices = 65500;
		private List<vtkPolyData> _pds; 

		public void Update(vtkPolyData pd)
		{
			_pds = Subdivide(pd);
			Meshes = new List<Mesh>(_pds.Count);
			foreach (var subPd in _pds)
			{
				var subMesh = PolyDataToMesh(subPd);
				if (subMesh == null)
				{
					Debug.LogWarning("Submesh null!");
					continue;
				}
				Meshes.Add(subMesh);
			}
		}

		public void SetColorArray(string name, bool pointData, vtkLookupTable lut)
		{
			for(var i = 0; i < Meshes.Count; i++)
			{
				vtkDataArray dataArray;
				if (pointData)
					dataArray = _pds[i].GetPointData().GetArray(name);
				else
					dataArray = _pds[i].GetCellData().GetArray(name);
				SetColors(Meshes[i], dataArray, lut);
			}
		}

		/// <summary>
		/// Generates a Unity Mesh from a vtkPolyData.
		/// </summary>
		/// <param name="pd">The vtk poly data.</param>
		/// <returns>The Unity Mesh (without colors).</returns>
		private static Mesh PolyDataToMesh(vtkPolyData pd)
		{
			if (pd == null)
			{
				Debug.LogWarning("No PolyData passed!");
				return null;
			}

			var numVertices = pd.GetNumberOfPoints();
			if (numVertices == 0)
			{
				Debug.LogWarning("No vertices to convert!");
				return null;
			}

			var mesh = new Mesh();
			

			// Points / Vertices
			var vertices = new Vector3[numVertices];
			for (var i = 0; i < numVertices; ++i)
			{
				var pnt = pd.GetPoint(i);
				// Flip z-up to y-up
				vertices[i] = new Vector3(-(float) pnt[0], (float) pnt[2], (float) pnt[1]);
			}
			mesh.vertices = vertices;

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
				mesh.normals = normals;
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
				mesh.uv = uvs;
			}

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
				mesh.SetTriangles(triangles, 0);
				//Mesh.RecalculateNormals();
				mesh.RecalculateBounds();
				return mesh;
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

				mesh.SetIndices(idList.ToArray(typeof (int)) as int[], MeshTopology.Lines, 0);
				mesh.RecalculateBounds();
				return mesh;
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

				mesh.SetIndices(idList.ToArray(typeof (int)) as int[], MeshTopology.Points, 0);
				mesh.RecalculateBounds();
			}

			return mesh;
		}

		public void SetColors(Mesh mesh, vtkDataArray colorArray, vtkLookupTable lut)
		{
			var numVertices = mesh.vertexCount;
			if (numVertices <= 0 || colorArray == null)
				return;
			var colors = new Color32[numVertices];
			for (var i = 0; i < numVertices; ++i)
			{
				var scalar = colorArray.GetTuple1(i);
				var dcolor = lut.GetColor(scalar);
				var color = new byte[3];
				for (uint j = 0; j < 3; j++)
					color[j] = (byte)(255 * dcolor[j]);
				colors[i] = new Color32(color[0], color[1], color[2], 255);
			}
			mesh.colors32 = colors;
		}

		/// <summary>
		/// Subdivides a vtkPolyData into pieces containing max. MaxVertices.
		/// </summary>
		/// <param name="pd">The pd.</param>
		/// <returns>A list of vtkPolyData</returns>
		private static List<vtkPolyData> Subdivide(vtkPolyData pd)
		{
			var pds = new List<vtkPolyData>();
			if (pd.GetNumberOfPoints() <= MaxVertices)
			{
//				Debug.Log("No subdivide neccessary. " + pd.GetNumberOfPoints());
				pds.Add(pd);
				return pds;
			}

			var dicer = vtkOBBDicer.New();
			dicer.SetInput(pd);
			dicer.SetNumberOfPointsPerPiece(MaxVertices);
			dicer.SetDiceModeToNumberOfPointsPerPiece();
			dicer.Update();
//			Debug.Log("Subdivided into " + dicer.GetNumberOfActualPieces() + " pieces.");

			var threshold = vtkThreshold.New();
			pd = vtkPolyData.SafeDownCast(dicer.GetOutput());
			threshold.SetInput(pd);
			threshold.SetInputArrayToProcess(0, 0, 0,
				(int)vtkDataObject.FieldAssociations.FIELD_ASSOCIATION_POINTS,
				"vtkOBBDicer_GroupIds");
			var geometry = vtkGeometryFilter.New();
			geometry.SetInputConnection(threshold.GetOutputPort());

			for(var i = 0; i < dicer.GetNumberOfActualPieces(); i++)
			{
				threshold.ThresholdBetween(i, i);
				geometry.Update();
				// Last submesh needs not to be copied
				if (i == dicer.GetNumberOfActualPieces() - 1)
					pds.Add(geometry.GetOutput());
				else
				{
					var copiedOutput = new vtkPolyData();
					copiedOutput.DeepCopy(geometry.GetOutput());
					pds.Add(copiedOutput);
				}
			}

			return pds;
		}
	}
}
