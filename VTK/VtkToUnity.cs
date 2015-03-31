using Kitware.VTK;
using UnityEngine;

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

		private VtkMesh _vtkMesh = new VtkMesh();

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
			_triangleFilter.Update();
			_vtkMesh.PolyDataToMesh(_triangleFilter.GetOutput());
			_go.GetComponent<MeshFilter>().sharedMesh = _vtkMesh.Mesh;
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
