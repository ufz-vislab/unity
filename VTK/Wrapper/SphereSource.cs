using System;
using FullInspector;
using Kitware.VTK;
using UnityEngine;
using tk = FullInspector.tk<UFZ.VTK.SphereSource>;

namespace UFZ.VTK
{
	public class SphereSource : VtkAlgorithm
	{
		public double Radius
		{
			get { return _radius; }
			set { SetRadius(value); }
		}
		[SerializeField]
		private double _radius;

		[HideInInspector, NotSerialized]
		public vrCommand SetRadiusCommand;

		public SphereSource()
		{
			SetRadiusCommand = new vrCommand("Set Radius Command - ", SetRadius);
		}

		public vrValue SetRadius(vrValue iValue)
		{
			if (Algorithm == null || !iValue.IsNumber())
				return null;
			_radius = iValue.GetDouble();
			((vtkSphereSource) Algorithm).SetRadius(_radius);
			return null;
		}

		protected override bool IsInitialized()
		{
			return true;
		}

		protected override void Initialize()
		{
			base.Initialize();

			if (Algorithm == null)
				Algorithm = vtkSphereSource.New();

			Radius = 1.0;
			Name = "SphereSource";
			InputDataType = DataType.None;
			OutputDataDataType = (DataType) Enum.Parse(typeof (DataType),
				Algorithm.GetOutputPortInformation(0).Get(vtkDataObject.DATA_TYPE_NAME()));

			SaveState();
		}

		public override tkControlEditor GetEditor()
		{
			var parentEditor = base.GetEditor();
			return new tkControlEditor(
				new tk.VerticalGroup {
					new tkTypeProxy<VtkAlgorithm, tkDefaultContext, SphereSource, tkDefaultContext>(
						(tkControl<VtkAlgorithm, tkDefaultContext>)parentEditor.Control),
					new tk.PropertyEditor("Radius")
				});
		}
	}
}
