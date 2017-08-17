#if UNITY_STANDALONE_WIN
using System;
using Kitware.VTK;
using Sirenix.OdinInspector;
using UFZ.VTK;
using UnityEngine;

public class VtkReverseSense : VtkAlgorithm
{
	private vtkReverseSense _filter;

	[ShowInInspector]
	public bool ReverseNormals
	{
		get { return _reverseNormals; }
		set
		{
			_reverseNormals = value;
			if (_filter != null)
				_filter.SetReverseNormals(Convert.ToInt32(value));
		}
	}

	[SerializeField, HideInInspector] private bool _reverseNormals;
	
	protected override void Initialize()
	{
		base.Initialize();

		if (_filter == null)
		{
			_filter = vtkReverseSense.New();
			_filter.ModifiedEvt += (sender, args) => UpdateRenderer();

			Algorithm = _filter;
			AlgorithmOutput = _filter.GetOutputPort();
		}
	}
}
#endif