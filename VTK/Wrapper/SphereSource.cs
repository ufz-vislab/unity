using FullInspector;
using Kitware.VTK;
using UnityEngine;

namespace UFZ.VTK
{
	public class SphereSource : VtkAlgorithm
	{
		[InspectorHeader("Algorithm Properties")]
		[SerializeField]
		public double Radius
		{
			get { return _radius; }
			set { SetRadius(value); }
		}
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

		protected override void Initialize()
		{
			base.Initialize();

			if (Algorithm == null)
				Algorithm = vtkSphereSource.New();

			Radius = 1.0;
			Name = "SphereSource";

			SaveState();
		}
	}
}
