#if UNITY_STANDALONE_WIN
using UnityEngine;
using Kitware.VTK;

namespace UFZ.VTK
{
	public class VtkCylinderSource : VtkAlgorithm
	{
		private vtkCylinderSource _source;

		[SerializeField]
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
		private float _radius = 1f;

		private float _height = 3f;

		[SerializeField]
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
		private int _resolution = 6;

		protected override void Initialize()
		{
			base.Initialize();

			if (_source == null)
			{
				_source = vtkCylinderSource.New();
				_source.ModifiedEvt += (sender, args) => UpdateRenderer();

				// Insert triangle filter
				TriangleFilter.SetInputConnection(_source.GetOutputPort());
			}

			Algorithm = _source;
			AlgorithmOutput = TriangleFilter.GetOutputPort();

			_source.SetRadius(_radius);
			_source.SetHeight(_height);
			_source.SetResolution(_resolution);
			_source.SetCapping(1);
		}
	}
}
#endif
