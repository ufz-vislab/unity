#if UNITY_STANDALONE_WIN
using UnityEngine;
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
					//Debug.LogWarning("VtuReader: Set Filepath to " + value);
					var absPath = AbsoluteFilePath();
					if (_source.GetFileName() == absPath ||
					    _source.CanReadFile(absPath) != 1)
						return;

					Debug.LogWarning("VtuReader: Try to read file " + absPath);
					_source.SetFileName(absPath);
				}
			}
		}
		[SerializeField, HideInInspector]
		private string _filepath = "UFZ/VTK/Data/density.vtu";

		//public TextAsset VtuFile;

		private vtkCellDataToPointData _cellToPointData;

		protected override void Initialize()
		{
			base.Initialize();

			if (_source == null)
				_source = vtkXMLUnstructuredGridReader.New();
			if (_cellToPointData == null)
				_cellToPointData = vtkCellDataToPointData.New();

			_cellToPointData.PassCellDataOn();
			_cellToPointData.SetInputConnection(_source.GetOutputPort());

			Filepath = _filepath;

			Algorithm = _source;
			AlgorithmOutput = _cellToPointData.GetOutputPort();

			_source.Update();

			// TODO: Wichtig:
			_source.GetOutput().GetCellData().SetActiveScalars("evap");
			//_source.GetOutput().GetCellData().SetScalars(_source.GetOutput().GetCellData().GetArray(0));
		}

		protected string AbsoluteFilePath()
		{
#if UNITY_EDITOR
			if (_filepath.StartsWith("./VTK"))
			{
				var sceneDirectory = Misc.Builder.GetSceneDirectory(Misc.Builder.GetCurrentScene());
				return Path.GetFullPath(sceneDirectory + Path.DirectorySeparatorChar + _filepath);
			}
#endif
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
