using FullInspector;
using Kitware.VTK;
using UnityEngine;

public class VtkAlgorithm : BaseBehavior
{
	[SerializeField]
	private VtkMesh _vtkMesh;

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

	[SerializeField]
	public vtkAlgorithm Algorithm
	{
		get { return _algorithm; }
		set
		{
			_algorithm = value;
			if (_triangleFilter != null && _algorithm != null)
				_triangleFilter.SetInputConnection(_algorithm.GetOutputPort());
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

	[SerializeField]
	[HideInInspector]
	private vtkTriangleFilter _triangleFilter;

	void Reset()
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

		if(_triangleFilter == null)
			_triangleFilter = vtkTriangleFilter.New();

		if (_triangleFilter != null && _algorithm != null)
			_triangleFilter.SetInputConnection(_algorithm.GetOutputPort());
	}

	void Awake()
	{
		base.Awake();
		//Debug.Log("Awake!");
		//RestoreState();
	}

	[InspectorButton]
	void UpdateVtk()
	{
		if (_triangleFilter == null || _algorithm == null || _vtkMesh == null || _gameObject == null)
			return;
		_triangleFilter.Update();
		_vtkMesh.PolyDataToMesh(_triangleFilter.GetOutput());
		_gameObject.GetComponent<MeshFilter>().sharedMesh = _vtkMesh.Mesh;
	}
}
