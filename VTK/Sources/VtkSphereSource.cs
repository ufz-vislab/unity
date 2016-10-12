using UnityEngine;
using Kitware.VTK;
using FullInspector;
using tk = FullInspector.tk<UFZ.VTK.VtkSphereSource>;

namespace UFZ.VTK
{
	// Derive from VtkAlgorithm
	// Optional: tkCustomEditor if you have custom handles
	public class VtkSphereSource : VtkAlgorithm, tkCustomEditor
	{
		// A pointer to the actual algorithm type for convenience
		private vtkPointSource _source;

		// An exposed algorithm property
		[SerializeField]
		public int NumPoints
		{
			get { return _numPoints; }
			set
			{
				// Range checking
				if (value <= 0 || value > 10000000) return;
				_numPoints = value;
				if (_source == null)
					return;
				_source.SetNumberOfPoints(_numPoints);
			}
		}
		private int _numPoints = 100;

		[SerializeField]
		public float Radius
		{
			get { return _radius;  }
			set
			{
				if (value <= 0f) return;
				_radius = value;
				if (_source == null)
					return;
				_source.SetRadius(_radius);
			}
		}
		private float _radius = 1f;

		// Has to be implemented
		protected override void Initialize()
		{
			// Call base class Initialize
			base.Initialize();

			// Create actual algorithm
			if (_source == null)
			{
				_source = vtkPointSource.New();

				// Set base class algorithm pointer
				Algorithm = _source;

				// Update renderer on modified event
				_source.ModifiedEvt +=
					(sender, args) => UpdateRenderer();

				// Specify output port, insert e.g. triangle filter here
				AlgorithmOutput = _source.GetOutputPort();
			}

			// Set algorithm default values
			_source.SetNumberOfPoints(_numPoints);
			_source.SetRadius(_radius);
		}

		// Optinal: draw gizmos
		protected void OnDrawGizmosSelected()
		{
			// These are read only Gizmos, for interactive Gizmos (Handles)
			// see Editor/VtkSphereSourceEditor
			
			//Gizmos.color = Color.white;
			//Gizmos.DrawWireSphere(transform.position, _radius);
		}

		// Optional for custom handles, see Editor/VtkSphereSourceEditor
		public tkControlEditor GetEditor()
		{
			//var parentEditor = base.GetEditor();
			return new tkControlEditor(
				new tk.VerticalGroup {
					new tk.PropertyEditor("NumPoints"),
					new tk.PropertyEditor("Radius"),
					new tk.Button("Add Renderer", (source, context) => AddRenderer())
				});
		}
	}
}