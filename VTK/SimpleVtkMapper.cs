#if UNITY_STANDALONE_WIN
using UnityEngine;
using System.Collections.Generic;
using Kitware.VTK;

namespace UFZ.VTK
{
	//[Serializable]
	public class SimpleVtkMapper : vtkPolyDataMapper
	{
		private enum ScalarMode
		{
			VTK_SCALAR_MODE_DEFAULT = 0,
			VTK_SCALAR_MODE_USE_POINT_DATA = 1,
			VTK_SCALAR_MODE_USE_CELL_DATA = 2,
			VTK_SCALAR_MODE_USE_POINT_FIELD_DATA = 3,
			VTK_SCALAR_MODE_USE_CELL_FIELD_DATA = 4,
			VTK_SCALAR_MODE_USE_FIELD_DATA = 5
		}

		//public vtkLookupTable LookupTable

		public VtkRenderer Renderer;

		public List<string> PointDataArrayNames;

		//private string _activePointDataColorArray;


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
				pd.SetActiveAttribute((int) _activeColorArrayIndex, 0);

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
			var noAbort = 1;
			var input = GetInput();
			var prop = act.GetProperty();
			var opacity = prop.GetOpacity();

			//vtkUnsignedCharArray c = null;
			vtkDataArray n = null;
			vtkDataArray t = null;

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
			Renderer.BufferPoints = new ComputeBuffer(numPoints, 12);
			Renderer.BufferPoints.SetData(points);

			// Colors
			if (ScalarVisibility && GetInput().GetPointData().GetNumberOfArrays() > 0)
			{
				//var array = GetInput().GetPointData().GetArray((int) _activeColorArrayIndex);
				//var range = array.GetRange();
				//Debug.Log("Range for array " + _activeColorArrayIndex + " - " + array.GetName() + ": " + range[0] + " - " + range[1]);
				//SetScalarRange(range[0], range[1]);
				//GetLookupTable().SetRange(range[0], range[1]);
				SetScalarModeToUsePointData();
				SetColorModeToMapScalars();
				//var range2 = GetScalarRange();
				//Debug.Log("Mapper range: " + range2[0] + " - " + range2[1]);
				UpdateColorBuffer((float) act.GetProperty().GetOpacity());
			}


			// Normals
			n = input.GetPointData().GetNormals();
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
				Renderer.BufferNormals = new ComputeBuffer((int)numNormals, 12);
				Renderer.BufferNormals.SetData(normals);
			}

			DrawPoints(p, n, t, cellNum, noAbort, input.GetVerts(), ren);
			DrawLines(p, n, t, cellNum, noAbort, input.GetLines(), ren);
			DrawPolys(p, n, t, cellNum, noAbort, input.GetPolys(), ren);
			return noAbort;
		}

		private void UpdateColorBuffer(float opacity)
		{
			//int cellScalars;
			//ScalarVisibilityOn();

			//UseLookupTableScalarRangeOn();
			var Colors = MapScalars(opacity);

			if (Colors == null)
			{
				//ScalarVisibilityOff();
				return;
			}

			if (Colors.GetNumberOfTuples() <= 0)
				return;

			//if ((GetScalarMode() == (int) ScalarMode.VTK_SCALAR_MODE_USE_CELL_DATA ||
			//     GetScalarMode() == (int) ScalarMode.VTK_SCALAR_MODE_USE_CELL_FIELD_DATA ||
			//     GetScalarMode() == (int) ScalarMode.VTK_SCALAR_MODE_USE_FIELD_DATA ||
			//     input.GetPointData().GetScalars() == null)
			//    && GetScalarMode() != (int) ScalarMode.VTK_SCALAR_MODE_USE_POINT_FIELD_DATA)
			//	cellScalars = 1;

			var numColors = Colors.GetNumberOfTuples();
			//Debug.Log("Num colors: " + numColors);
			var colors = new Vector3[numColors];
			for (var i = 0; i < numColors; i++)
			{
				var color = Colors.GetTuple4(i);
				colors[i] = new Vector3((float) color[0]/255, (float) color[1]/255,
					(float) color[2]/255);
			}
			var buffer = Renderer.BufferColors;
			if (buffer == null || buffer.count != numColors)
			{
				buffer = new ComputeBuffer((int) numColors, 12);
				Renderer.BufferColors = buffer;
			}
			buffer.SetData(colors);
		}

		private void DrawPoints(vtkPoints p, vtkDataArray n, vtkDataArray t, int cellNum,
			int noAbort, vtkCellArray ca, vtkRenderer ren)
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

			Renderer.BufferVerts = new ComputeBuffer(verts.Count, sizeof(int));
			Renderer.BufferVerts.SetData(verts.ToArray());
			Renderer.PointsMaterial.SetBuffer("buf_Indices", Renderer.BufferVerts);
			Renderer.PointsMaterial.SetBuffer("buf_Points", Renderer.BufferPoints);
		}

		private void DrawLines(vtkPoints p, vtkDataArray n, vtkDataArray t, int cellNum,
			int noAbort, vtkCellArray ca, vtkRenderer ren)
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

			Renderer.BufferLines = new ComputeBuffer(verts.Count, sizeof(int));
			Renderer.BufferLines.SetData(verts.ToArray());
			Renderer.LinesMaterial.SetBuffer("buf_Indices", Renderer.BufferLines);
			Renderer.LinesMaterial.SetBuffer("buf_Points", Renderer.BufferPoints);
		}

		private void DrawPolys(vtkPoints p, vtkDataArray n, vtkDataArray t, int cellNum,
			int noAbort, vtkCellArray ca, vtkRenderer ren)
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

			var buffer = new ComputeBuffer(verts.Length, sizeof (int));
			Renderer.BufferTriangles = buffer;
			buffer.SetData(verts);
			Renderer.TrianglesMaterial.SetBuffer("buf_Indices", buffer);
			Renderer.TrianglesMaterial.SetBuffer("buf_Points", Renderer.BufferPoints);
			Renderer.TrianglesMaterial.SetBuffer("buf_Normals", Renderer.BufferNormals);
			Renderer.TrianglesMaterial.SetBuffer("buf_Colors", Renderer.BufferColors);
		}
	}
}
#endif
