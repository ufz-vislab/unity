using System;
using FullInspector;
using Kitware.VTK;
using UnityEngine;
using tk = FullInspector.tk<UFZ.VTK.ClipPolyData>;

namespace UFZ.VTK
{
	public class ClipPolyData : VtkAlgorithm
	{
		public Vector3 PlanePosition
		{
			get { return _planePosition; }
			set
			{
				// TODO: snap to input bounding box
				_planePosition = value;
				_plane.SetOrigin(-value.x, value.z, value.y);
			}
		}

		public Axis PlaneAxis
		{
			get { return _planeAxis; }
			set
			{
				_planeAxis = value;
				if (value == Axis.X) _plane.SetNormal(1.0, 0.0, 0.0);
				if (value == Axis.Y) _plane.SetNormal(0.0, 0.0, 1.0);
				if (value == Axis.Z) _plane.SetNormal(0.0, 1.0, 0.0);
				Algorithm.Modified();
			}
		}

		public bool Flip
		{
			get { return _flip; }
			set
			{
				_flip = value;
				((vtkClipDataSet)Algorithm).SetInsideOut(value ? 1 : 0);
			}
		}

		//public Transform PlaneTransform;

		[SerializeField]
		private Vector3 _planePosition;
		[SerializeField]
		private Axis _planeAxis = Axis.X;
		[SerializeField]
		private bool _flip;

		private vtkPlane _plane;

		protected override bool IsInitialized()
		{
			return true;
		}

		protected override void Initialize()
		{
			base.Initialize();

			if (Algorithm == null)
				Algorithm = vtkClipDataSet.New();
			Flip = _flip;

			if (_plane == null)
				_plane = vtkPlane.New();
			PlanePosition = _planePosition;
			PlaneAxis = _planeAxis;

			var clip = (vtkClipDataSet)Algorithm;
			clip.GenerateClipScalarsOn();
			clip.SetClipFunction(_plane);

			Name = "ClipPolyData";
			InputDataType = DataType.vtkPolyData;
			OutputDataDataType = (DataType)Enum.Parse(typeof(DataType),
				Algorithm.GetOutputPortInformation(0).Get(vtkDataObject.DATA_TYPE_NAME()));

			InitializeFinish();
		}

		public override tkControlEditor GetEditor()
		{
			var parentEditor = base.GetEditor();
			return new tkControlEditor(
				new tk.VerticalGroup {
					new tkTypeProxy<VtkAlgorithm, tkDefaultContext, ClipPolyData, tkDefaultContext>(
						(tkControl<VtkAlgorithm, tkDefaultContext>)parentEditor.Control),
					new tk.PropertyEditor("PlanePosition"),
					new tk.PropertyEditor("PlaneAxis"),
					new tk.PropertyEditor("Flip")
				});
		}
	}
}
