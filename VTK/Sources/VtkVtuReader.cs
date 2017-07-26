#if UNITY_STANDALONE_WIN
using UnityEngine;
using System.Collections;
using System.IO;
using Kitware.VTK;
using Sirenix.OdinInspector;

namespace UFZ.VTK
{
	public class VtkVtuReader : VtkAlgorithm
	{
		private vtkXMLUnstructuredGridReader _source;

		[ShowInInspector]
		public string Filepath
		{
			get { return _filepath; }
			set
			{
				_filepath = value;
				if (_source != null)
				{
					if (_source.CanReadFile(AbsoluteFilePath()) == 1)
						_source.SetFileName(AbsoluteFilePath());
				}
			}
		}
		[SerializeField, HideInInspector]
		private string _filepath = "UFZ/VTK/Data/density.vtu";

		private vtkCellDataToPointData _cellToPointData;

		protected override void Initialize()
		{
			base.Initialize();

			if (_source == null)
			{
				_source = vtkXMLUnstructuredGridReader.New();
				_source.ModifiedEvt += (sender, args) => UpdateRenderer();
			}
			if (_cellToPointData == null)
			{
				_cellToPointData = vtkCellDataToPointData.New();
			}

			_cellToPointData.PassCellDataOn();
			_cellToPointData.SetInputConnection(_source.GetOutputPort());

			Algorithm = _source;
			AlgorithmOutput = _cellToPointData.GetOutputPort();

			if (_source.CanReadFile(AbsoluteFilePath()) == 1)
				_source.SetFileName(AbsoluteFilePath());

			_source.Update();

			// TODO: Wichtig:
			_source.GetOutput().GetCellData().SetActiveScalars("evap");
			//_source.GetOutput().GetCellData().SetScalars(_source.GetOutput().GetCellData().GetArray(0));
		}

		protected string AbsoluteFilePath()
		{
			return Path.GetFullPath(Application.dataPath + Path.DirectorySeparatorChar + _filepath);
		}
		
		[Button, ShowIf("ShowAddSurfaceFilter")]
		public void AddSurfaceFilter()
		{
			var filter = gameObject.AddComponent<VtkDataSetSurfaceFilter>();
			filter.InputAlgorithm = this;
		}

		protected bool ShowAddSurfaceFilter()
		{
			return true;
		}
	}
}
#endif
