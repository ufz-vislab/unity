using System;
using System.Collections.Generic;
using System.Linq;
using FullInspector;
using Kitware.VTK;
using UnityEngine;
using UFZ.Rendering;
using tk = FullInspector.tk<UFZ.VTK.VtkAlgorithm>;

namespace UFZ.VTK
{
	public enum DataType
	{
		None,
		vtkUnstructuredGrid,
		vtkStructuredGrid,
		vtkPolyData,
		vtkImageData
	}

	public abstract class VtkAlgorithm : BaseBehavior, tkCustomEditor
	{
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				if (_gameObject)
					_gameObject.name = value;
			}
		}
		[SerializeField]
		private string _name;

		protected vtkAlgorithm Algorithm
		{
			get { return _algorithm; }
			set
			{
				_algorithm = value;
				InitAlgorithmModifiedEvent();
			}
		}
		private vtkAlgorithm _algorithm;

		public float Opacity
		{
			get {
				return MaterialProperties == null ?
					-1.0f : MaterialProperties.Opacity;
			}
			set
			{
				if(MaterialProperties != null)
					MaterialProperties.Opacity = value;
			}
		}

		private vtkGeometryFilter _geometryFilter;

		private vtkTriangleFilter _triangleFilter;

		private vtkPolyDataNormals _normalsFilter;

		public MaterialProperties.ColorMode ColorBy
		{
			get {
				return MaterialProperties == null ?
					MaterialProperties.ColorMode.Invalid : MaterialProperties.ColorBy;
			}
			set
			{
				if(MaterialProperties != null)
					MaterialProperties.ColorBy = value;
			}
		}

		public Color SolidColor
		{
			get
			{
				return MaterialProperties == null ?
					Color.magenta : MaterialProperties.SolidColor;
			}
			set
			{
				if(MaterialProperties != null)
					MaterialProperties.SolidColor = value;
			}
		}
		
		[InspectorDisabled]
		public MaterialProperties MaterialProperties;

		public VtkAlgorithm Input
		{
			get { return _input; }
			set
			{
				if(value.OutputDataDataType != InputDataType ||
					InputDataType == DataType.None)
					return;
				_input = value;
				UpdateVtk(this, null);
			}
		}
		[SerializeField]
		private VtkAlgorithm _input;

		[SerializeField]
		protected DataType InputDataType;
		[SerializeField]
		protected DataType OutputDataDataType;

		[SerializeField]
		private VtkMesh _vtkMesh;
		[SerializeField]
		private GameObject _gameObject;
		[SerializeField]
		private List<string> _arrayNames;
		[SerializeField]
		private GUIContent[] _arrayLabels;
		[SerializeField]
		private List<string> _inputArrayNames;
		[SerializeField]
		private GUIContent[] _inputArrayLabels;

		private vtkPolyData _polyDataOutput;

		public vtkDataSet Output
		{
			get
			{
				if(IsInitialized())
					_algorithm.Update();
				return _output;
			}
		}

		private vtkDataSet _output;

		public int SelectedArrayIndex
		{
			get { return _selectedArrayIndex; }
			set
			{
				_selectedArrayIndex = value;
				UpdateMeshColors(value);
			}
		}
		[SerializeField]
		private int _selectedArrayIndex;

		public int ArrayToProcessIndex
		{
			get { return _arrayToProcessIndex; }
			set
			{
				_arrayToProcessIndex = value;
				_algorithm.SetInputArrayToProcess(0, 0, 0,
					(int)vtkDataObject.FieldAssociations.FIELD_ASSOCIATION_POINTS,
					_inputArrayNames[value].Substring(2));
			}
		}
		[SerializeField]
		private int _arrayToProcessIndex;

		public bool GenerateMesh = true;
		public bool GenerateNormals = true;

		abstract protected bool IsInitialized();

		private void Reset()
		{
			Initialize();
		}

		protected override void OnValidate()
		{
			base.OnValidate();
			Initialize();
		}

		private void InitAlgorithmModifiedEvent()
		{
			if (_algorithm == null) return;
			_algorithm.RemoveAllHandlersForAllEvents();
			_algorithm.ModifiedEvt += OnModifiedEvt;
		}

		protected virtual void Initialize()
		{
			//InitAlgorithmModifiedEvent();

			if (_vtkMesh == null)
				_vtkMesh = new VtkMesh();
			if (_gameObject == null)
			{
				_gameObject = new GameObject(Name);
				_gameObject.transform.parent = transform;
				_gameObject.transform.localPosition = new Vector3();
//				_gameObject.AddComponent<MeshFilter>();
//				var meshRenderer = _gameObject.AddComponent<MeshRenderer>();
			}

			if (_triangleFilter == null)
				_triangleFilter = vtkTriangleFilter.New();

			if (_polyDataOutput == null)
				_polyDataOutput = _triangleFilter.GetOutput();
		}

		protected void InitializeFinish()
		{
			if (_input != null)
			{
				_inputArrayNames = GetArrayNames(_input.Output);
				_inputArrayLabels = _inputArrayNames.Select(t => new GUIContent(t)).ToArray();
			}
			if (_inputArrayNames != null && _inputArrayNames.Count > 0)
				ArrayToProcessIndex = _arrayToProcessIndex;

			SaveState();
		}

		private void UpdateVtk(VtkAlgorithm algorithm, tkDefaultContext context)
		{
			if (!IsInitialized())
				return;
			if (_input)
			{
				_algorithm.SetInputConnection(_input.Algorithm.GetOutputPort());
				_input.UpdateVtk(_input, null);
			}

			if (_triangleFilter == null || _algorithm == null || _vtkMesh == null ||
				_gameObject == null)
				return;
			_algorithm.Update();
			_output = (vtkDataSet)_algorithm.GetOutputDataObject(0);
			// Input connection has to be set here because _algorithm address changes somehow
			// because of FullInspector serialization
			if (OutputDataDataType != DataType.vtkPolyData)
			{
				if (_geometryFilter == null)
					_geometryFilter = vtkGeometryFilter.New();
				//_geometryFilter.MergingOff();
				_geometryFilter.SetInputConnection(_algorithm.GetOutputPort());
				_triangleFilter.SetInputConnection(_geometryFilter.GetOutputPort());
			}
			else
				_triangleFilter.SetInputConnection(_algorithm.GetOutputPort());
			_triangleFilter.PassVertsOn();
			_triangleFilter.PassLinesOn();
			_triangleFilter.Update();
			_polyDataOutput = _triangleFilter.GetOutput();

			if (_polyDataOutput == null ||
				_polyDataOutput.GetNumberOfPoints() == 0 ||
				_polyDataOutput.GetNumberOfCells() == 0)
			{
				// Debug.Log("Polydata output empty!");
				return;
			}

			if (GenerateNormals && !VtkNormalsHelper.GetPointNormals(_polyDataOutput))
			{
				if (_normalsFilter == null)
					_normalsFilter = vtkPolyDataNormals.New();
				_normalsFilter.SetInputConnection(_triangleFilter.GetOutputPort());
				_normalsFilter.ComputePointNormalsOn();
				_normalsFilter.ComputeCellNormalsOff();
				_normalsFilter.Update();
				_polyDataOutput = _normalsFilter.GetOutput();
			}

			_arrayNames = GetArrayNames(_polyDataOutput);
			_arrayLabels = _arrayNames.Select(t => new GUIContent(t)).ToArray();

			if(!GenerateMesh)
				return;

			_vtkMesh.Update(_polyDataOutput);
			UpdateMeshColors(_selectedArrayIndex);
			DestroyImmediate(_gameObject.GetComponent<MeshRenderer>());
			DestroyImmediate(_gameObject.GetComponent<MeshFilter>());
			if (_vtkMesh.Meshes.Count == 1)
			{
				_gameObject.AddComponent<MeshFilter>().sharedMesh = _vtkMesh.Meshes[0];
				var meshRenderer = _gameObject.AddComponent<MeshRenderer>();
				meshRenderer.material =
					new Material(Shader.Find("Diffuse")) { color = Color.gray };
				for(var i = 0; i < _gameObject.transform.childCount; i++)
					DestroyImmediate(_gameObject.transform.GetChild(i));
			}
			else
			{
				for (var i = 0; i < _vtkMesh.Meshes.Count; i++)
				{
					var currentName = Name + "-" + i;
					GameObject child;
					var childTransform = _gameObject.transform.FindChild(currentName);
					if (childTransform == null)
					{
						child = new GameObject(currentName);
						child.transform.parent = _gameObject.transform;
						child.transform.localPosition = new Vector3();
						child.AddComponent<MeshFilter>();
						var meshRenderer = child.AddComponent<MeshRenderer>();
						meshRenderer.material =
							new Material(Shader.Find("Diffuse")) {color = Color.gray};
					}
					else
						child = childTransform.gameObject;
					child.GetComponent<MeshFilter>().sharedMesh = _vtkMesh.Meshes[i];
				}
				while (_vtkMesh.Meshes.Count < _gameObject.transform.childCount)
					DestroyImmediate(_gameObject.transform.GetChild(
						_gameObject.transform.childCount - 1));
			}
			if(MaterialProperties == null)
				MaterialProperties = _gameObject.AddComponent<MaterialProperties>();
		}

		private static List<string> GetArrayNames(vtkDataSet dataSet)
		{
			var arrayNames = new List<string>();
			var pointData = dataSet.GetPointData();
			var cellData = dataSet.GetCellData();
			for (var i = 0; i < pointData.GetNumberOfArrays(); i++)
				arrayNames.Add("P-" + pointData.GetArrayName(i));
			for (var i = 0; i < cellData.GetNumberOfArrays(); i++)
				arrayNames.Add("C-" + cellData.GetArrayName(i));
			return arrayNames;
		}

		protected void OnModifiedEvt(vtkObject sender, vtkObjectEventArgs objectEventArgs)
		{
			UpdateVtk(this, null);
		}

		protected static VtkAlgorithm OnSelectedArrayChange(VtkAlgorithm algorithm,
			tkDefaultContext context, int index)
		{
			algorithm.SelectedArrayIndex = index;
			return algorithm;
		}

		protected static VtkAlgorithm OnArrayToProcessChange(VtkAlgorithm algorithm,
			tkDefaultContext context, int index)
		{
			algorithm.ArrayToProcessIndex = index;
			return algorithm;
		}

		private void UpdateMeshColors(int index)
		{
			var isPointArray = _arrayNames[index].StartsWith("P-");
			var arrayName = _arrayNames[index].Substring(2);
			vtkDataArray dataArray;
			if (isPointArray)
				dataArray = _polyDataOutput.GetPointData().GetArray(arrayName);
			else
				dataArray = _polyDataOutput.GetCellData().GetArray(arrayName);
			var range = dataArray.GetRange(0);
			var lut = VtkLookupTableHelper.Create(LutPreset.Rainbow, range[0], range[1]);
			_vtkMesh.SetColorArray(arrayName, isPointArray, lut);
		}

		public virtual tkControlEditor GetEditor()
		{
			return new tkControlEditor(
				new tk.VerticalGroup
				{
					//new tk.DefaultInspector(), // TODO: does not work yet
					new tk.PropertyEditor("Name"),
					new tk.ShowIf(o => InputDataType != DataType.None,
						new tk.PropertyEditor("Input")),
					new tk.PropertyEditor("Algorithm")
					{
						Style = new tk.ReadOnly()
					},
					new tk.PropertyEditor("OutputDataDataType")
					{
						Style = new tk.ReadOnly()
					},
					new tk.PropertyEditor("GenerateMesh"),
					new tk.PropertyEditor("GenerateNormals"),
					new tk.Button(new fiGUIContent("Update VTK"), UpdateVtk),
					new tk.Label("Rendering")
					{
						Style = new tk.Color(Color.magenta)
					},
					new tk.Slider(
						new fiGUIContent("Opacity"),
						0, 1, (o,c) => o.Opacity, (o,c,v) => o.Opacity = v),
					new tk.PropertyEditor("ColorBy"),
					new tk.PropertyEditor("SolidColor"),
					new tk.ShowIf(o => _arrayLabels != null,
						new tk.Popup(new fiGUIContent("Color Array"),
						tk.Val(o => o._arrayLabels), tk.Val(o => o._selectedArrayIndex),
								OnSelectedArrayChange)
						),
					new tk.Label("Algorithm Properties")
					{
						Style = new tk.Color(Color.green)
					},
					new tk.ShowIf(o => _algorithm != null && _algorithm.GetOutputDataObject(0) != null,
						new tk.Foldout(new fiGUIContent("VTK information"),
							new tk.VerticalGroup{
								new tk.Label(tk.Val(o =>
									new fiGUIContent("Points: " + o.Algorithm.GetOutputDataObject(0).GetNumberOfElements(0).ToString())
								)),
								new tk.Label(tk.Val(o =>
									new fiGUIContent("Cells: " + o.Algorithm.GetOutputDataObject(0).GetNumberOfElements(1).ToString())
								))
								}
							)
						),
					new tk.ShowIf(o => _inputArrayLabels != null,
						new tk.Popup(new fiGUIContent("Active array"),
						tk.Val(o => o._inputArrayLabels), tk.Val(o => o._arrayToProcessIndex),
								OnArrayToProcessChange)
						)
				}
			);
		}
	}
}
