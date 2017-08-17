#if UNITY_STANDALONE_WIN
using System;
using UnityEngine;
using Kitware.VTK;
using Sirenix.OdinInspector;

namespace UFZ.VTK
{
	public class VtkDataSetSurfaceFilter : VtkAlgorithm
	{
		[ShowInInspector]
		public bool FlipNormals
		{
			get { return _flipNormals; }
			set
			{
				if (_normals == null)
					return;
				_normals.SetFlipNormals(Convert.ToInt32(value));
				_flipNormals = value;
			}
		}
		[SerializeField, HideInInspector]
		private bool _flipNormals;

		private vtkDataSetSurfaceFilter _filter;
		private vtkPolyDataNormals _normals;

		protected override void Initialize()
		{
			base.Initialize();
			_hasInput = true;

			if (_filter == null)
			{
				_filter = vtkDataSetSurfaceFilter.New();;

				if (_normals == null)
				{
					_normals = vtkPolyDataNormals.New();
					_normals.ModifiedEvt += delegate { UpdateRenderer(); };
				}

				TriangleFilter.SetInputConnection(_normals.GetOutputPort());
			}

			_normals.SetInputConnection(_filter.GetOutputPort());
			
			// Make sure vtk filter options are setup (after serialization)
			_normals.SetFlipNormals(Convert.ToInt32(_flipNormals));

			Algorithm = _filter;
			AlgorithmOutput = TriangleFilter.GetOutputPort();
		}
	}
}
#endif
