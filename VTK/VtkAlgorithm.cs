using FullInspector;
using Kitware.VTK;
using UnityEngine;

namespace UFZ.VTK
{
	public class VtkAlgorithm : BaseBehavior
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
			FieldData
		};

		[InspectorHeader("Coloring")]
		public ColorBy Coloring;

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

		private void Reset()
		{
			Initialize();

//			var currentDomain = AppDomain.CurrentDomain;
//			var aName = new AssemblyName("TempAssembly");
//			var ab = currentDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.RunAndSave);
//			var mb = ab.DefineDynamicModule(aName.Name, aName.Name + ".dll");
//			var eb = mb.DefineEnum("ColorBy", TypeAttributes.Public, typeof (int));
//
//			eb.DefineLiteral("Solid Color", 0);
//			eb.DefineLiteral("Point Array: Elevation", 1);
//
//			var finished = eb.CreateType();
//			
//			Coloring = ();
//			
//			ab.Save(aName.Name + ".dll");
//			ColorBy  = (ColorBy) 1;
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
				if(Coloring == ColorBy.SolidColor)
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
			_vtkMesh.PolyDataToMesh(_triangleFilter.GetOutput());
			_gameObject.GetComponent<MeshFilter>().sharedMesh = _vtkMesh.Mesh;
		}

		protected void OnModifiedEvt(vtkObject sender, vtkObjectEventArgs objectEventArgs)
		{
			UpdateVtk();
		}
	}
}
