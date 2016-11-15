using UnityEngine;
using Kitware.VTK;

namespace UFZ.VTK
{
	public class VtkElevationFilter : VtkAlgorithm
	{
		private vtkElevationFilter _filter;

		[SerializeField]
		public Vector3 Point1
		{
			get { return _point1; }
			set
			{
				_point1 = value;
				if (_filter != null)
					_filter.SetLowPoint(value.x, value.y, value.z);
			}
		}
		private Vector3 _point1 = Vector3.zero;

		[SerializeField]
		public Vector3 Point2
		{
			get { return _point2; }
			set
			{
				_point2 = value;
				if (_filter != null)
					_filter.SetHighPoint(value.x, value.y, value.z);
			}
		}
		private Vector3 _point2 = Vector3.up;

		protected override void Initialize()
		{
			base.Initialize();
			_hasInput = true;

			if (_filter == null)
			{
				_filter = vtkElevationFilter.New();
				_filter.ModifiedEvt += (sender, args) => UpdateRenderer();
			}

			Algorithm = _filter;
			AlgorithmOutput = _filter.GetOutputPort();

			_filter.SetLowPoint(_point1.x, _point1.y, _point1.z);
			_filter.SetHighPoint(_point2.x, _point2.y, _point2.z);
		}

	}
}