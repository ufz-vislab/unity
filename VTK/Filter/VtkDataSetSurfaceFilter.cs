using System;
using UnityEngine;
using Kitware.VTK;

namespace UFZ.VTK
{
	public class VtkDataSetSurfaceFilter : VtkAlgorithm
	{
		[SerializeField]
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
				Algorithm = _filter;
				_filter.ModifiedEvt += (sender, args) => UpdateRenderer();

				if (_normals == null)
				{
					_normals = vtkPolyDataNormals.New();
					_normals.SetInputConnection(_filter.GetOutputPort());
					_normals.ModifiedEvt += (sender, args) => UpdateRenderer();
				}

				TriangleFilter.SetInputConnection(_normals.GetOutputPort());
				AlgorithmOutput = TriangleFilter.GetOutputPort();
			}
		}
	}
}
