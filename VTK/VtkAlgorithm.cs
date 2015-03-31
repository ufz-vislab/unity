using FullInspector;
using Kitware.VTK;
using UnityEngine;

namespace UFZ.VTK
{
	public class VtkAlgorithm : BaseBehavior
	{
		[SerializeField] private VtkMesh _vtkMesh;

		[SerializeField] private GameObject _gameObject;

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
				_gameObject.AddComponent<MeshRenderer>();
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
