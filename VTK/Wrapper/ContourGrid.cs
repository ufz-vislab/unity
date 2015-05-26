using System;
using System.Collections.Generic;
using FullInspector;
using Kitware.VTK;
using UnityEngine;
using tk = FullInspector.tk<UFZ.VTK.ContourGrid>;

namespace UFZ.VTK
{
	public class ContourGrid : VtkAlgorithm
	{
		public List<double> Values
		{
			get { return _values; }
			set
			{
				_values = value;
				var contour = (vtkContourGrid) Algorithm;
				contour.SetNumberOfContours(value.Count);
				for (var index = 0; index < value.Count; index++)
					contour.SetValue(index, value[index]);
				SaveState();
			}
		}
		[SerializeField]
		private List<double> _values;

		protected override bool IsInitialized()
		{
			return _values.Count > 0;
		}

		protected override void Initialize()
		{
			base.Initialize();

			if (Algorithm == null)
				Algorithm = vtkContourGrid.New();
			//var contour = (vtkContourGrid)Algorithm;
			

			Name = "ContourGrid";
			InputDataType = DataType.vtkUnstructuredGrid;
//			OutputDataDataType = (DataType)Enum.Parse(typeof(DataType),
//				Algorithm.GetOutputPortInformation(0).Get(vtkDataObject.DATA_TYPE_NAME()));
			OutputDataDataType = DataType.vtkPolyData;

			InitializeFinish();
		}

		public override tkControlEditor GetEditor()
		{
			var parentEditor = base.GetEditor();
			return new tkControlEditor(
				new tk.VerticalGroup {
					new tkTypeProxy<VtkAlgorithm, tkDefaultContext, ContourGrid, tkDefaultContext>(
						(tkControl<VtkAlgorithm, tkDefaultContext>)parentEditor.Control),
					new tk.PropertyEditor("Values")
				});
		}
	}
}
