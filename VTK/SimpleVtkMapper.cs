#if UNITY_STANDALONE_WIN
using System;
using UnityEngine;
using System.Collections.Generic;
using Kitware.VTK;
using UnityEngine.Rendering;

namespace UFZ.VTK
{
	public class SimpleVtkMapper : vtkPolyDataMapper
	{
		/*
		private enum ScalarMode
		{
			VTK_SCALAR_MODE_DEFAULT = 0,
			VTK_SCALAR_MODE_USE_POINT_DATA = 1,
			VTK_SCALAR_MODE_USE_CELL_DATA = 2,
			VTK_SCALAR_MODE_USE_POINT_FIELD_DATA = 3,
			VTK_SCALAR_MODE_USE_CELL_FIELD_DATA = 4,
			VTK_SCALAR_MODE_USE_FIELD_DATA = 5
		}
		*/

		//public vtkLookupTable LookupTable

		public VtkRenderer Renderer;

		public List<string> PointDataArrayNames;

		//private string _activePointDataColorArray;

		[HideInInspector, NonSerialized]
		public ComputeBuffer BufferPoints;
		[HideInInspector, NonSerialized]
		public ComputeBuffer BufferVerts;
		[HideInInspector, NonSerialized]
		public ComputeBuffer BufferLines;
		[HideInInspector, NonSerialized]
		public ComputeBuffer BufferTriangles;
		[HideInInspector, NonSerialized]
		public ComputeBuffer BufferNormals;
		[HideInInspector, NonSerialized]
		public ComputeBuffer BufferColors;
		
		private Material PointsMaterial;
		private Material LinesMaterial;
		private Material TrianglesMaterial;

		public CommandBuffer Buffer;

		public uint ActiveColorArrayIndex
		{
			get { return _activeColorArrayIndex; }
			set
			{
				Update();
				if (GetInput() == null)
					return;
				var pd = GetInput().GetPointData();
				if (value >= pd.GetNumberOfArrays())
					return;
				_activeColorArrayIndex = value;

				Modified();
			}
		}
		[SerializeField]
		private uint _activeColorArrayIndex;

		public bool ScalarVisibility
		{
			get { return _scalarVisibility; }
			set
			{
				if (value /* && GetInput().GetPointData().GetNumberOfArrays() > 0 */)
				{
					SetScalarModeToUsePointData();
					SetColorModeToMapScalars();
					ScalarVisibilityOn();
					_scalarVisibility = true;
				}
				else
				{
					ScalarVisibilityOff();
					_scalarVisibility = false;
				}
				Modified();
			}
		}
		private bool _scalarVisibility;

		private SimpleVtkMapper()
		{
			PointDataArrayNames = new List<string>();
			_activeColorArrayIndex = 0;
			
			PointsMaterial = new Material(Shader.Find("DX11/VtkPoints"));
			PointsMaterial.hideFlags = HideFlags.HideAndDontSave;
			LinesMaterial = new Material(Shader.Find("DX11/VtkPoints"));
			LinesMaterial.hideFlags = HideFlags.HideAndDontSave;
			TrianglesMaterial = new Material(Shader.Find("DX11/VtkTriangles"));
			TrianglesMaterial.hideFlags = HideFlags.HideAndDontSave;
		}

		~SimpleVtkMapper()
		{
			//UnityEngine.Object.DestroyImmediate(PointsMaterial);
			//UnityEngine.Object.DestroyImmediate(LinesMaterial);
			//UnityEngine.Object.DestroyImmediate(TrianglesMaterial);
		}

		public new static SimpleVtkMapper New()
		{
			var ret = vtkObjectFactory.CreateInstance("SimpleVtkMapper");
			if (ret != null)
				return (SimpleVtkMapper) ret;
			return new SimpleVtkMapper();
		}

		public override void RenderPiece(vtkRenderer ren, vtkActor act)
		{
			ReleaseBuffersImpl();
			Update();
			
			var input = GetInput();
			if (input == null)
			{
				Debug.LogWarning("Mapper has no input! " + Renderer.Algorithm.name);
				return;
			}


			var numPts = input.GetNumberOfPoints();
			if (numPts == 0)
			{
				Debug.LogWarning("Mapper got input with no points!");
				return;
			}

			PointDataArrayNames.Clear();
			for (var i = 0; i < input.GetPointData().GetNumberOfArrays(); i++)
				PointDataArrayNames.Add(input.GetPointData().GetArrayName(i));

			//if (GetLookupTable() == null)
			//	CreateDefaultLookupTable();

			// TODO: Texture mapping

			// TODO: if something has changed regenerate colors and display lists if required

			Draw(ren, act);
		}

		protected virtual int Draw(vtkRenderer ren, vtkActor act)
		{
			// TODO: possible optimizations:
			// - Use data directly, e.g. input.GetPoints().GetData().GetVoidPointer();
			// - do not convert doubles to floats and longs to ints
			const int noAbort = 1;
			var input = GetInput();
			var prop = act.GetProperty();
			var opacity = prop.GetOpacity();

			//var cellScalars = 0;
			var cellNum = 0;
			int cellNormals;
			//int resolve = 0, zResolve = 0;
			//double zRes = 0.0;

			if (opacity <= 0.0)
				return noAbort;

			//var rep = prop.GetRepresentation();
			//var interpolation = prop.GetInterpolation();

			// Points
			var p = input.GetPoints();
			var numPoints = (int) p.GetNumberOfPoints();
			//Debug.Log("Num points: " + numPoints);
			if (numPoints == 0)
				return noAbort;

			var points = new Vector3[numPoints];
			for (int i = 0; i < numPoints; i++)
			{
				var pt = p.GetPoint(i);
				points[i] = new Vector3((float) pt[0], (float) pt[1], (float) pt[2]);
			}
			BufferPoints = new ComputeBuffer(numPoints, 12);
			BufferPoints.SetData(points);
			PointsMaterial.SetBuffer("buf_Points", BufferPoints);
			LinesMaterial.SetBuffer("buf_Points", BufferPoints);
			TrianglesMaterial.SetBuffer("buf_Points", BufferPoints);

			// Colors
			if (ScalarVisibility && GetInput().GetPointData().GetNumberOfArrays() > 0)
			{
				//var array = GetInput().GetPointData().GetArray((int) _activeColorArrayIndex);
				//var range = array.GetRange();
				//Debug.Log("Range for array " + _activeColorArrayIndex + " - " + array.GetName() + ": " + range[0] + " - " + range[1]);
				//SetScalarRange(range[0], range[1]);
				//GetLookupTable().SetRange(range[0], range[1]);
				
				//var range2 = GetScalarRange();
				//Debug.Log("Mapper range: " + range2[0] + " - " + range2[1]);
				UpdateColorBuffer((float) act.GetProperty().GetOpacity());
			}


			// Normals
			var n = input.GetPointData().GetNormals();
			//if (interpolation == VTK_FLAT)
			//	n = null;

			cellNormals = 0;
			if (n == null && input.GetCellData().GetNormals() != null)
			{
				cellNormals = 1;
				n = input.GetCellData().GetNormals();
			}

			if (n != null && cellNormals == 0)
			{
				var numNormals = n.GetNumberOfTuples();
				//Debug.Log("Num normals: " + numNormals);
				var normals = new Vector3[numNormals];
				for (int i = 0; i < numNormals; i++)
				{
					var normal = n.GetTuple3(i);
					normals[i] = new Vector3((float)normal[0], (float)normal[1], (float)normal[2]);
				}
				BufferNormals = new ComputeBuffer((int)numNormals, 12);
				BufferNormals.SetData(normals);
				TrianglesMaterial.SetBuffer("buf_Normals", BufferNormals);
			}

			Buffer = new CommandBuffer {name = "VTK rendering"};
			DrawPoints(input.GetVerts());
			DrawLines(input.GetLines());
			DrawPolys(input.GetPolys());
			
			return noAbort;
		}

		private void UpdateColorBuffer(float opacity)
		{
			//int cellScalars;

			Update();
			if (GetInput() == null)
				return;
			GetInput().GetPointData().SetActiveAttribute((int) _activeColorArrayIndex, 0);
			
			var vtkColors = MapScalars(opacity);

			if (vtkColors == null)
				return;

			if (vtkColors.GetNumberOfTuples() <= 0)
				return;

			//if ((GetScalarMode() == (int) ScalarMode.VTK_SCALAR_MODE_USE_CELL_DATA ||
			//     GetScalarMode() == (int) ScalarMode.VTK_SCALAR_MODE_USE_CELL_FIELD_DATA ||
			//     GetScalarMode() == (int) ScalarMode.VTK_SCALAR_MODE_USE_FIELD_DATA ||
			//     input.GetPointData().GetScalars() == null)
			//    && GetScalarMode() != (int) ScalarMode.VTK_SCALAR_MODE_USE_POINT_FIELD_DATA)
			//	cellScalars = 1;

			var numColors = vtkColors.GetNumberOfTuples();
			//Debug.Log("Num colors: " + numColors);
			var colors = new Vector3[numColors];
			for (var i = 0; i < numColors; i++)
			{
				var color = vtkColors.GetTuple4(i);
				colors[i] = new Vector3((float) color[0]/255, (float) color[1]/255,
					(float) color[2]/255);
			}
			var buffer = BufferColors;
			if (buffer == null || buffer.count != numColors)
			{
				buffer = new ComputeBuffer((int) numColors, 12);
				BufferColors = buffer;
				TrianglesMaterial.SetBuffer("buf_Colors", BufferColors);
			}
			buffer.SetData(colors);
		}

		private void DrawPoints(vtkCellArray ca)
		{
			var numVerts = ca.GetNumberOfCells();
			if (numVerts == 0)
				return;

			var verts = new List<int>();
			var pts = vtkIdList.New();
			ca.InitTraversal();
			while (ca.GetNextCell(pts) != 0)
			{
				var cellVerts = new int[pts.GetNumberOfIds()];
				for (var i = 0; i < pts.GetNumberOfIds(); i++)
					cellVerts[i] = (int) (pts.GetId(i));
				verts.AddRange(cellVerts);
			}

			BufferVerts = new ComputeBuffer(verts.Count, sizeof(int));
			BufferVerts.SetData(verts.ToArray());
			PointsMaterial.SetBuffer("buf_Indices", BufferVerts);
			Buffer.DrawProcedural(Matrix4x4.identity, PointsMaterial, -1,
				MeshTopology.Points, BufferPoints.count);
		}

		private void DrawLines(vtkCellArray ca)
		{
			// TODO: VtkLineSource with uneven resolution sometimes draw a striped pattern
			if(ca.GetNumberOfCells() == 0)
				return;

			var verts = new List<int>();
			var pts = vtkIdList.New();
			ca.InitTraversal();
			while (ca.GetNextCell(pts) != 0)
			{
				for (var i = 0; i < pts.GetNumberOfIds() - 1; i++)
				{
					verts.Add((int)pts.GetId(i));
					verts.Add((int)pts.GetId(i + 1));
				}
			}

			BufferLines = new ComputeBuffer(verts.Count, sizeof(int));
			BufferLines.SetData(verts.ToArray());
			PointsMaterial.SetBuffer("buf_Indices", BufferLines);
			Buffer.DrawProcedural(Matrix4x4.identity, LinesMaterial, -1,
				MeshTopology.Lines, BufferLines.count);
		}

		private void DrawPolys(vtkCellArray ca)
		{
			var numTriangles = ca.GetNumberOfCells();
			if (numTriangles == 0)
				return;

			var verts = new int[numTriangles * 3];
			var prim = 0;
			var pts = vtkIdList.New();
			ca.InitTraversal();
			while (ca.GetNextCell(pts) != 0)
			{
				for (var i = 0; i < pts.GetNumberOfIds(); ++i)
					verts[prim*3 + i] = (int)(pts.GetId(i));
				++prim;
			}

			BufferTriangles = new ComputeBuffer(verts.Length, sizeof (int));
			BufferTriangles.SetData(verts);
			TrianglesMaterial.SetBuffer("buf_Indices", BufferTriangles);
			Buffer.DrawProcedural(Matrix4x4.identity, TrianglesMaterial, -1,
				MeshTopology.Triangles, BufferTriangles.count);
		}
		
		private void ReleaseBuffersImpl()
		{
			if (BufferPoints != null)
			{
				BufferPoints.Release();
				BufferPoints = null;
			}
			if (BufferVerts != null)
			{
				BufferVerts.Release();
				BufferVerts = null;
			}
			if (BufferLines != null)
			{
				BufferLines.Release();
				BufferLines = null;
			}
			if (BufferTriangles != null)
			{
				BufferTriangles.Release();
				BufferTriangles = null;
			}
			if (BufferNormals != null)
			{
				BufferNormals.Release();
				BufferNormals = null;
			}
			if (BufferColors != null)
			{
				BufferColors.Release();
				BufferColors = null;
			}
			if (Buffer != null)
			{
				Buffer.Release();
				Buffer = null;
			}
		}

		public void SetTransformMatrix(Matrix4x4 matrix)
		{
			if (PointsMaterial)
				PointsMaterial.SetMatrix("_MATRIX_M", matrix);
			if (LinesMaterial)
				LinesMaterial.SetMatrix("_MATRIX_M", matrix);
			if (TrianglesMaterial)
				TrianglesMaterial.SetMatrix("_MATRIX_M", matrix);
		}
	}
}
#endif
