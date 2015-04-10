using System.Collections.Generic;
using System.Linq;
using FullInspector;
using Kitware.VTK;
using UnityEngine;
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

	public class VtkAlgorithm : BaseBehavior, tkCustomEditor
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
				_algorithm.ModifiedEvt += OnModifiedEvt;
			}
		}
		[SerializeField]
		private vtkAlgorithm _algorithm;

		// TODO: Modify opacity on MaterialProperties
		public bool Visible
		{
			get { return _visible; }
			set
			{
				_visible = value;
				_gameObject.SetActive(value);
			}
		}
		[SerializeField]
		private bool _visible = true;

		[SerializeField, HideInInspector]
		private vtkTriangleFilter _triangleFilter;

		[InspectorHeader("Coloring")]
		public MaterialProperties.ColorMode ColorBy
		{
			get { return MaterialProperties.ColorBy; }
			set { MaterialProperties.ColorBy = value; }
		}

		public Color SolidColor
		{
			get { return MaterialProperties.SolidColor; }
			set { MaterialProperties.SolidColor = value; }
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
				UpdateVtk();
			}
		}
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
		private GUIContent[] _arraylabels;
		[SerializeField]
		private int _selectedArrayIndex;

		private void Reset()
		{
			Initialize();
		}

		protected virtual void Initialize()
		{
			if (_vtkMesh == null)
				_vtkMesh = new VtkMesh();
			if (_gameObject == null)
			{
				_gameObject = new GameObject(Name);
				_gameObject.transform.parent = transform;
				_gameObject.transform.localPosition = new Vector3();
				_gameObject.AddComponent<MeshFilter>();
				var meshRenderer = _gameObject.AddComponent<MeshRenderer>();
				meshRenderer.material =
					new Material(Shader.Find("Diffuse")) { color = Color.gray };
				MaterialProperties = _gameObject.AddComponent<MaterialProperties>();
			}

			if (_triangleFilter == null)
				_triangleFilter = vtkTriangleFilter.New();
		}

		[InspectorButton]
		private void UpdateVtk()
		{
			if (_triangleFilter == null || _algorithm == null || _vtkMesh == null ||
				_gameObject == null)
				return;
			if(_input)
				_algorithm.SetInputConnection(_input.Algorithm.GetOutputPort());
			_algorithm.Update();
			// Input connection has to be set here because _algorithm address changes somehow
			// because of FullInspector serialization
			_triangleFilter.SetInputConnection(_algorithm.GetOutputPort());
			_triangleFilter.Update();
			var polyData = _triangleFilter.GetOutput();

			_arrayNames = new List<string>();
			var pointData = polyData.GetPointData();
			var cellData = polyData.GetCellData();
			for (var i = 0; i < pointData.GetNumberOfArrays(); i++)
				_arrayNames.Add("P-" + pointData.GetArrayName(i));
			for (var i = 0; i < cellData.GetNumberOfArrays(); i++)
				_arrayNames.Add("C-" + cellData.GetArrayName(i));
			_arraylabels = _arrayNames.Select(t => new GUIContent(t)).ToArray();
			
			_vtkMesh.PolyDataToMesh(polyData);
			UpdateMeshColors(_selectedArrayIndex);
			_gameObject.GetComponent<MeshFilter>().sharedMesh = _vtkMesh.Mesh;
		}

		protected void OnModifiedEvt(vtkObject sender, vtkObjectEventArgs objectEventArgs)
		{
			UpdateVtk();
		}

		protected VtkAlgorithm OnSelectedArrayChange(VtkAlgorithm algorithm,
			tkEmptyContext context, int index)
		{
			Debug.Log("Selected array: " + _arrayNames[index]);
			_selectedArrayIndex = index;
			UpdateMeshColors(index);
			return algorithm;
		}

		private void UpdateMeshColors(int index)
		{
			var isPointArray = _arrayNames[index].StartsWith("P-");
			var arrayName = _arrayNames[index].Substring(2);
			vtkDataArray dataArray;
			if (isPointArray)
				dataArray = _triangleFilter.GetOutput().GetPointData().GetArray(arrayName);
			else
				dataArray = _triangleFilter.GetOutput().GetCellData().GetArray(arrayName);
			var range = dataArray.GetRange(0);
			var lut = VtkLookupTableHelper.Create(LutPreset.Rainbow, range[0], range[1]);
			_vtkMesh.SetColors(dataArray, lut);
		}

		public virtual tkControlEditor GetEditor()
		{
			return new tkControlEditor(
				new tk.VerticalGroup
				{
					//new tk.DefaultInspector(), // TODO: does not work yet
					new tk.PropertyEditor("Name"),
					new tk.PropertyEditor("Algorithm"),
					new tk.ShowIf(o => InputDataType != DataType.None,
						new tk.PropertyEditor("Input")),
					new tk.PropertyEditor("Visible"),
					new tk.PropertyEditor("ColorBy"),
					new tk.PropertyEditor("SolidColor"),
					new tk.Popup(new fiGUIContent("Array"),
						tk.Val(o => o._arraylabels), tk.Val(o => o._selectedArrayIndex),
							OnSelectedArrayChange)
					// TODO: ShowIf does not work after play mode
//					new tk.ShowIf(o => _coloring == ColorBy.SolidColor,
//						new tk.PropertyEditor("SolidColor")),
//					new tk.ShowIf(o => _coloring == ColorBy.Array,
//						new tk.Popup(new fiGUIContent("Array"),
//							tk.Val(o => o._arraylabels), tk.Val(o => o._selectedArrayIndex),
//							OnSelectedArrayChange)),
//					new tk.PropertyEditor("Radius")
				}
			);
		}
	}
}
