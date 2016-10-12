using UnityEngine;
using System.Collections;
using System.IO;
using Kitware.VTK;

namespace UFZ.VTK
{
	public class VtkVtuReader : VtkAlgorithm
	{
		private vtkXMLUnstructuredGridReader _source;

		[SerializeField]
		public string Filepath
		{
			get { return _filepath; }
			set
			{
				_filepath = value;
				if(_source != null)
					_source.SetFileName(value);
			}
		}

		private string _filepath = "X:\\minimal_mesh.vtu";

		private vtkDataSetSurfaceFilter _geometryFilter;
		private vtkCellDataToPointData _cellToPointData;
		private vtkPolyDataNormals _normals;

		protected override void Initialize()
		{
			base.Initialize();

			if (_source == null)
			{
				_source = vtkXMLUnstructuredGridReader.New();
				Algorithm = _source;
				_source.ModifiedEvt += (sender, args) => UpdateRenderer();
				if (_cellToPointData == null)
				{
					_cellToPointData = vtkCellDataToPointData.New();
					_cellToPointData.PassCellDataOn();
					_cellToPointData.SetInputConnection(_source.GetOutputPort());
				}
				if (_geometryFilter == null)
				{
					_geometryFilter = vtkDataSetSurfaceFilter.New();
					_geometryFilter.SetInputConnection(_cellToPointData.GetOutputPort());
				}
				if (_normals == null)
				{
					_normals = vtkPolyDataNormals.New();
					_normals.FlipNormalsOn();
					_normals.SetInputConnection(_geometryFilter.GetOutputPort());
				}
				TriangleFilter.SetInputConnection(_normals.GetOutputPort());
				AlgorithmOutput = TriangleFilter.GetOutputPort();
			}

			_source.SetFileName(_filepath);
			_source.Update();

			// Wichtig:
			_source.GetOutput().GetCellData().SetActiveScalars("evap");
			//_source.GetOutput().GetCellData().SetScalars(_source.GetOutput().GetCellData().GetArray(0));
		}
	}
}

