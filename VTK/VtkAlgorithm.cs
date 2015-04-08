using System.Collections.Generic;
using System.Linq;
using FullInspector;
using Kitware.VTK;
using UnityEngine;
using tk = FullInspector.tk<UFZ.VTK.VtkAlgorithm>;

namespace UFZ.VTK
{
	public class VtkAlgorithm : BaseBehavior, tkCustomEditor
	{
		[SerializeField]
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
		private string _name;

		[SerializeField, InspectorDisabled]
		protected vtkAlgorithm Algorithm
		{
			get { return _algorithm; }
			set
			{
				_algorithm = value;
				_algorithm.ModifiedEvt += OnModifiedEvt;
			}
		}
		private vtkAlgorithm _algorithm;

		[SerializeField]
		public bool Visible
		{
			get { return _visible; }
			set
			{
				_visible = value;
				_gameObject.SetActive(value);
			}
		}
		private bool _visible = true;

		[SerializeField, HideInInspector]
		private vtkTriangleFilter _triangleFilter;
		
		public enum ColorBy
		{
			SolidColor,
			Array
		};

		[InspectorHeader("Coloring")]
		public ColorBy Coloring
		{
			get { return _coloring; }
			set
			{
				_coloring = value;
				if(MaterialProperties == null)
					return;
				switch (value)
				{
					case ColorBy.SolidColor:
						MaterialProperties.ColorBy = MaterialProperties.ColorMode.SolidColor;
						break;
					case ColorBy.Array:
						MaterialProperties.ColorBy = MaterialProperties.ColorMode.VertexColor;
						break;
				}
			}
		}
		private ColorBy _coloring;

		[SerializeField]
		public Color SolidColor
		{
			get { return _solidColor; }
			set
			{
				_solidColor = value;
				if (MaterialProperties != null)
					MaterialProperties.SolidColor = value;
			}
		}
		private Color _solidColor = Color.gray;
		
		[InspectorDisabled]
		public MaterialProperties MaterialProperties;

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
				meshRenderer.material = new Material(Shader.Find("Diffuse")) { color = _solidColor };
				MaterialProperties = _gameObject.AddComponent<MaterialProperties>();
				if(_coloring == ColorBy.SolidColor)
					MaterialProperties.ColorBy = MaterialProperties.ColorMode.SolidColor;
				MaterialProperties.SaveState();
			}

			if (_triangleFilter == null)
				_triangleFilter = vtkTriangleFilter.New();
		}

		[InspectorButton]
		private void UpdateVtk()
		{
			if (_triangleFilter == null || _algorithm == null || _vtkMesh == null || _gameObject == null)
				return;
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

		protected VtkAlgorithm OnSelectedArrayChange(VtkAlgorithm algorithm, tkEmptyContext context, int index)
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

		tkControlEditor tkCustomEditor.GetEditor()
		{
			return new tkControlEditor(
				new tk.VerticalGroup
				{
					//new tk.DefaultInspector(), // does not work yet
					new tk.PropertyEditor("Name"),
					new tk.PropertyEditor("Algorithm"),
					new tk.PropertyEditor("Visible"),
					new tk.PropertyEditor(new fiGUIContent("Color by"), "Coloring"),
					new tk.ShowIf(o => _coloring == ColorBy.SolidColor,
						new tk.PropertyEditor("SolidColor")),
					new tk.ShowIf(o => _coloring == ColorBy.Array,
						new tk.Popup(new fiGUIContent("Array"),
							tk.Val(o => o._arraylabels), tk.Val(o => o._selectedArrayIndex),
							OnSelectedArrayChange)),
				}
			);
		}
	}
}
