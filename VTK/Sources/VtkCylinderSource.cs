#if UNITY_STANDALONE_WIN
using UnityEngine;
using Kitware.VTK;
using Sirenix.OdinInspector;

namespace UFZ.VTK
{
	public class VtkCylinderSource : VtkAlgorithm
	{
		private vtkCylinderSource _source;

		[ShowInInspector]
		public float Radius
		{
			get { return _radius; }
			set
			{
				if (value < 0.0001f)
					return;
				_radius = value;
				if(_source != null)
					_source.SetRadius(value);
			}
		}
		[SerializeField, HideInInspector]
		private float _radius = 1f;

		[SerializeField, HideInInspector]
		private float _height = 3f;

		[ShowInInspector]
		public int Resolution
		{
			get { return _resolution; }
			set
			{
				if (value <= 0)
					return;
				_resolution = value;
				if (_source != null)
					_source.SetResolution(value);
			}
		}
		[SerializeField, HideInInspector]
		private int _resolution = 6;

		protected override void Initialize()
		{
			base.Initialize();

			if (_source == null)
			{
				_source = vtkCylinderSource.New();

				// Insert triangle filter
				TriangleFilter.SetInputConnection(_source.GetOutputPort());
			}

			Algorithm = _source;
			AlgorithmOutput = TriangleFilter.GetOutputPort();

			// Make sure vtk filter options are setup (after serialization)
			_source.SetRadius(_radius);
			_source.SetHeight(_height);
			_source.SetResolution(_resolution);
			_source.SetCapping(1);
		}
	}
}
#endif
