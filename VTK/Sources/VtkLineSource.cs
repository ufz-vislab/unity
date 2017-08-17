#if UNITY_STANDALONE_WIN
using UnityEngine;
using Kitware.VTK;
using Sirenix.OdinInspector;

namespace UFZ.VTK
{
	public class VtkLineSource : VtkAlgorithm
	{
		private vtkLineSource _source;

		[ShowInInspector]
		public Vector3 Point1
		{
			get { return _point1; }
			set
			{
				_point1 = value;
				if (_source != null)
					_source.SetPoint1(value.x, value.y,value.z);
			}
		}
		[SerializeField, HideInInspector]
		private Vector3 _point1 = Vector3.zero;

		[ShowInInspector]
		public Vector3 Point2
		{
			get { return _point2; }
			set
			{
				_point2 = value;
				if (_source != null)
					_source.SetPoint2(value.x, value.y, value.z);
			}
		}
		[SerializeField, HideInInspector]
		private Vector3 _point2 = Vector3.up;

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
		private int _resolution = 1;

		protected override void Initialize()
		{
			base.Initialize();

			if (_source == null)
				_source = vtkLineSource.New();

			Algorithm = _source;
			AlgorithmOutput = _source.GetOutputPort();

			// Make sure vtk filter options are setup (after serialization)
			_source.SetPoint1(_point1.x, _point1.y, _point1.z);
			_source.SetPoint2(_point2.x, _point2.y, _point2.z);
			_source.SetResolution(_resolution);
		}
	}
}
#endif
