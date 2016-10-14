using System;
using UnityEngine;
using System.IO;
using FullInspector;
using Kitware.VTK;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UFZ.VTK
{
#if UNITY_EDITOR // Calls the static constructor on load
	[InitializeOnLoad]
#endif
	public abstract class VtkAlgorithm : BaseBehavior
	{
		public bool HasInput { get { return _hasInput; } }
		protected bool _hasInput;

		[ShowInInspector, InspectorHidePrimary, InspectorHideIf("HasOutput"),
		 InspectorComment(CommentType.Error, "No valid output!")]
		public bool HasOutput
		{
			get
			{
				if (Algorithm == null)
					return false;
				Algorithm.Update();
				var output = Algorithm.GetOutputDataObject(0) as vtkDataSet;
				if (output != null && output.GetNumberOfPoints() > 0)
					return true;
				return false;
			}
		}

		[SerializeField, InspectorShowIf("HasInput")]
		public VtkAlgorithm InputAlgorithm
		{
			get { return _inputAlgorithm; }
			set
			{
				if (Algorithm == null)
					return;
				if (value == null)
					return;
				if (!HasInput || value.AlgorithmOutput == null)
					return;
				_inputAlgorithm = value;
				Algorithm.SetInputConnection(value.AlgorithmOutput);
				UpdateRenderer();
			}
		}

		private VtkAlgorithm _inputAlgorithm;

		[HideInInspector]
		protected vtkAlgorithm Algorithm;

		[SerializeField]
		protected VtkRenderer ren;

		[SerializeField]
		protected vtkAlgorithmOutput AlgorithmOutput;

		protected vtkTriangleFilter TriangleFilter;

		private void Reset()
		{
			Initialize();
		}

		protected override void Awake()
		{
			base.Awake();
			Initialize();
		}

		protected override void OnValidate()
		{
			base.OnValidate();
			Initialize();
		}

		public vtkAlgorithmOutput OutputPort()
		{
			return AlgorithmOutput;
		}

		[InspectorButton, InspectorShowIf("HasOutput")]
		public void AddRenderer()
		{
			ren = gameObject.AddComponent<VtkRenderer>();
			ren.Algorithm = this;
			Initialize();
		}

		protected virtual void Initialize()
		{
			if (_inputAlgorithm != null)
				_inputAlgorithm.Initialize();
			if (TriangleFilter == null)
				TriangleFilter = vtkTriangleFilter.New();
		}

		protected void UpdateRenderer()
		{
			if (ren)
				ren.BuffersUpToDate = false;
		}

		// See http://stackoverflow.com/a/33124250/80480
		static VtkAlgorithm()
		{
			const string pluginPath = "UFZ/VTK/Plugins";
#if UNITY_EDITOR
			if (!FullInspector.Internal.fiUtility.IsMainThread)
				return;
#endif
			var currentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
#if UNITY_EDITOR_32
			var dllPath = Application.dataPath
				+ pluginPath
				+ Path.DirectorySeparatorChar + "x32";
#elif UNITY_EDITOR_64
			var dllPath = Application.dataPath
						  + pluginPath
						  + Path.DirectorySeparatorChar + "x64";
#else
			var dllPath = Application.dataPath
				+ Path.DirectorySeparatorChar + "Plugins";

#endif
			if (currentPath != null
				&& currentPath.Contains(dllPath) == false)
				Environment.SetEnvironmentVariable("PATH", currentPath + Path.PathSeparator + dllPath,
					EnvironmentVariableTarget.Process);
		}
	}
}
