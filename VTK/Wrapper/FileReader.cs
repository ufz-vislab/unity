using FullInspector;
using Kitware.VTK;
using UnityEditor;
using UnityEngine;
using tk = FullInspector.tk<UFZ.VTK.FileReader>;

namespace UFZ.VTK
{
	public class FileReader : VtkAlgorithm
	{
		[InspectorDisabled]
		public string Filename
		{
			get { return _filename; }
			set
			{
				var reader = ((vtkXMLGenericDataObjectReader)Algorithm);
				_filename = value;
				Name = value;
				reader.SetFileName(value);

				reader.GetPointDataArraySelection().EnableAllArrays();
				reader.GetCellDataArraySelection().EnableAllArrays();

				reader.Update();
				byte parallel = 0;
				var vtkType = reader.ReadOutputType(value, ref parallel);

				// From Common/vtkType.h:
				if (vtkType == 0) OutputDataDataType = DataType.vtkPolyData;
				if (vtkType == 2) OutputDataDataType = DataType.vtkStructuredGrid;
				if (vtkType == 4) OutputDataDataType = DataType.vtkUnstructuredGrid;
				if (vtkType == 6) OutputDataDataType = DataType.vtkImageData;

				SaveState();
			}
		}
		[SerializeField]
		private string _filename = "";

		public void PickFile(FileReader reader, tkDefaultContext context)
		{
			var path = EditorUtility.OpenFilePanel("Load a VTK file",
				Application.dataPath + "/StreamingAssets", "vt*");
			if(path.Length == 0)
				return;
			Filename = path;
		}

		protected override bool IsInitialized()
		{
			return Filename.Length > 0;
		}

		protected override void Initialize()
		{
			base.Initialize();

			if (Algorithm == null)
				Algorithm = vtkXMLGenericDataObjectReader.New();
			if(_filename.Length > 0)
				Filename = _filename;
			else
				Name = "FileReader";

			InputDataType = DataType.None;

			InitializeFinish();
		}

		public override tkControlEditor GetEditor()
		{
			var parentEditor = base.GetEditor();
			return new tkControlEditor(
				new tk.VerticalGroup {
					new tkTypeProxy<VtkAlgorithm, tkDefaultContext, FileReader, tkDefaultContext>(
						(tkControl<VtkAlgorithm, tkDefaultContext>)parentEditor.Control),
					new tk.Button(new fiGUIContent("Choose file"), PickFile),
					new tk.PropertyEditor("Filename")
				});
		}
	}
}
