using UnityEngine;
using System.Collections;
using FullInspector;
using Kitware.VTK;
using UFZ.VTK;

public class VtkStreamTracer : VtkAlgorithm
{
	private vtkStreamTracer _filter;

	/*
	[SerializeField]
	public Vector3 Point
	{
		get { return _point; }
		set
		{
			_point = value;
			if (_filter != null)
			{
				// TODO set input point
			}
		}
	}
	private Vector3 _point = Vector3.zero;
	*/

	[SerializeField,ShowInInspector]
	public VtkAlgorithm Source
	{
		get { return _source; }
		set
		{
			if (_filter != null)
			{
				_filter.SetSourceConnection(value.OutputPort());
				_source = value;
			}
		}
	}
	private VtkAlgorithm _source;

	private vtkRungeKutta4 _integrator;

	protected override void Initialize()
	{
		base.Initialize();
		_hasInput = true;

		if (_filter == null)
		{
			_filter = vtkStreamTracer.New();
			Algorithm = _filter;
			_filter.ModifiedEvt += (sender, args) => UpdateRenderer();
			AlgorithmOutput = _filter.GetOutputPort();
		}

		if (Source != null)
		{
			_filter.SetSourceConnection(Source.OutputPort());
		}

		if (_integrator == null)
			_integrator = vtkRungeKutta4.New();

		_filter.SetIntegrator(_integrator);
		_filter.SetMaximumPropagation(0.1);
		//_filter.SetMaximumPropagationUnitToTimeUnit();
		_filter.SetInitialIntegrationStep(0.1);
		_filter.SetIntegrationStepUnit(2);
		//_filter.SetInitialIntegrationStepUnitToCellLengthUnit();
		_filter.SetIntegrationDirectionToBackward();
		//_filter.SetIntegrationDirectionToBoth();
	}
}
