using Kitware.VTK;
using UnityEngine;
using System.Collections;

namespace UFZ.VTK
{
	[System.Serializable]
	public class VtkToUnity
	{
		public GameObject gameObject
		{
			get { return _go; }
		}

		public vtkTriangleFilter TriangleFilter
		{
			get { return _triangleFilter; }
		}

		public Mesh mesh = new Mesh();
		public string name;
		public string colorFieldName = "";
		public VtkColorType colorDataType = VtkColorType.PointData;
		public vtkDataArray colorArray = null;
		public vtkLookupTable lut = vtkLookupTable.New();
		public Material mat;
		public Color solidColor;
		public bool ComputePointNormals = true;

		private GameObject _go;
		private vtkTriangleFilter _triangleFilter;

		public enum VtkColorType
		{
			PointData,
			CellData,
			SolidColor
		}

		public enum LutPreset
		{
			BlueRed,
			RedBlue,
			Rainbow
		}

		public VtkToUnity(vtkAlgorithm algorithm, GameObject newGo)
		{
			Initialize(algorithm, newGo);
		}

		protected void OnModifiedEvt(vtkObject sender, vtkObjectEventArgs objectEventArgs)
		{
			Update();
		}

		public VtkToUnity(vtkAlgorithm algorithm, string name)
		{
			_go = new GameObject(name);
			Initialize(algorithm, gameObject);
		}

		private void Initialize(vtkAlgorithm algorithm, GameObject go)
		{
			_go = go;
			name = go.name;

			_triangleFilter = vtkTriangleFilter.New();
			if (ComputePointNormals)
			{
				var computeNormals = vtkPolyDataNormals.New();
				computeNormals.SetInputConnection(algorithm.GetOutputPort());
				computeNormals.ComputePointNormalsOn();
				_triangleFilter.SetInputConnection(computeNormals.GetOutputPort());
			}
			else
				_triangleFilter.SetInputConnection(algorithm.GetOutputPort());

			algorithm.ModifiedEvt += OnModifiedEvt;

			var meshFilter = go.AddComponent<MeshFilter>();
			meshFilter.sharedMesh = mesh;
			go.AddComponent<MeshRenderer>();
		}

		~VtkToUnity()
		{
			/*
		foreach (Material mat in go.GetComponent<Renderer>().materials)
			Object.DestroyImmediate(mat);			
		*/
		}

		public void Update()
		{
			PolyDataToMesh();
		}

		private void PolyDataToMesh()
		{
			mesh.Clear();

			_triangleFilter.Update();
			var pd = _triangleFilter.GetOutput();

			// Points / Vertices
			var numVertices = pd.GetNumberOfPoints();
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
				Debug.Log("No Normals: " + gameObject.name);
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

			// Vertex colors
			if (numVertices > 0 && colorArray != null)
			{
				var colors = new Color32[numVertices];

				for (var i = 0; i < numVertices; ++i)
					colors[i] = GetColor32AtIndex(i);

				mesh.colors32 = colors;
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
				//mesh.RecalculateNormals();
				mesh.RecalculateBounds();
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

				mesh.SetIndices(idList.ToArray(typeof (int)) as int[], MeshTopology.Lines, 0);
				mesh.RecalculateBounds();
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

				mesh.SetIndices(idList.ToArray(typeof (int)) as int[], MeshTopology.Points, 0);
				mesh.RecalculateBounds();
			}
		}

		private byte[] GetByteColorAtIndex(int i)
		{
			var scalar = colorArray.GetTuple1(i);
			var dcolor = lut.GetColor(scalar);
			var color = new byte[3];
			for (uint j = 0; j < 3; j++)
				color[j] = (byte) (255*dcolor[j]);
			return color;
		}

		private Color32 GetColor32AtIndex(int i)
		{
			var color = GetByteColorAtIndex(i);
			return new Color32(color[0], color[1], color[2], 255);
		}

		private Color GetColorAtIndex(int i)
		{
			return GetColor32AtIndex(i);
		}

		public void ColorBy(Color color)
		{
			colorFieldName = "";
			colorDataType = VtkColorType.SolidColor;
			solidColor = color;

			mat = new Material(Shader.Find("Diffuse")) {color = color};
			gameObject.GetComponent<Renderer>().material = mat;
		}

		public void ColorBy(string fieldname, VtkColorType type)
		{
			colorFieldName = fieldname;
			colorDataType = type;

			if (colorFieldName != "")
			{
				_triangleFilter.Update();
				var pd = _triangleFilter.GetOutput();

				switch (colorDataType)
				{
					case VtkColorType.PointData:
						colorArray = pd.GetPointData().GetScalars(colorFieldName);
						break;
					case VtkColorType.CellData:
						colorArray = pd.GetCellData().GetScalars(colorFieldName);
						break;
				}

				// TODO: Use MaterialProperties script
				// TODO: Unlit for points (or anything without normals)
				gameObject.GetComponent<Renderer>().materials = new Material[2]
				{
					new Material(Shader.Find("UFZ/Opaque-VertexColor-Lit-Front")),
					new Material(Shader.Find("UFZ/Opaque-VertexColor-Lit-Back"))
				};
			}
			else
			{
				colorArray = null;
				mat = new Material(Shader.Find("Diffuse")) {color = Color.magenta};
				gameObject.GetComponent<Renderer>().material = mat;
				Debug.Log("Color array " + fieldname + " not found!");
			}
		}

		public void SetLut(LutPreset preset)
		{
			double[] range = {0.0, 1.0};
			if (colorArray != null)
				range = colorArray.GetRange();
			else
				Debug.Log("VtkToUnity.SetLut(): No color array set!");
			SetLut(preset, range[0], range[1]);
		}

		public void SetLut(LutPreset preset, double rangeMin, double rangeMax)
		{
			lut.SetTableRange(rangeMin, rangeMax);
			switch (preset)
			{
				case LutPreset.BlueRed:
					lut.SetHueRange(0.66, 1.0);
					lut.SetNumberOfColors(128);
					break;
				case LutPreset.RedBlue:
					lut.SetHueRange(1.0, 0.66);
					lut.SetNumberOfColors(128);
					//lut.SetNumberOfTableValues(2);
					//lut.SetTableValue(0, 1.0, 0.0, 0.0, 1.0);
					//lut.SetTableValue(1, 0.0, 0.0, 1.0, 1.0);
					break;
				case LutPreset.Rainbow:
					lut.SetHueRange(0.0, 0.66);
					lut.SetNumberOfColors(256);
					break;
			}
			lut.Build();
		}
	}
}
