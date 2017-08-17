#if UNITY_STANDALONE_WIN
using Kitware.VTK;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UFZ.VTK.Sources
{
	// Derive from VtkAlgorithm
	public class VtkPointSource : VtkAlgorithm
	{
		// A pointer to the actual algorithm type for convenience
		private vtkPointSource _source;

		// An exposed algorithm property
		[ShowInInspector]
		public int NumPoints
		{
			get { return _numPoints; }
			set
			{
				// Range checking
				if (value <= 0 || value > 10000000) return;
				_numPoints = value;
				if (_source == null)
					return;
				_source.SetNumberOfPoints(_numPoints);
			}
		}
		[SerializeField, HideInInspector]
		private int _numPoints = 100;

		[ShowInInspector]
		public float Radius
		{
			get { return _radius;  }
			set
			{
				if (value <= 0f) return;
				_radius = value;
				if (_source == null)
					return;
				_source.SetRadius(_radius);
			}
		}
		[SerializeField, HideInInspector]
		private float _radius = 1f;

		// Has to be implemented
		protected override void Initialize()
		{
			// Call base class Initialize
			base.Initialize();

			// Create actual algorithm
			if (_source == null)
				_source = vtkPointSource.New();

			// Set base class algorithm pointer
			Algorithm = _source;

			// Specify output port, insert e.g. triangle filter here
			AlgorithmOutput = _source.GetOutputPort();

			// Make sure vtk filter options are setup (after serialization)
			_source.SetNumberOfPoints(_numPoints);
			_source.SetRadius(_radius);
		}
	}
}
#endif
