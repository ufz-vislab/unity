#if UNITY_STANDALONE_WIN
using UnityEngine;
using Kitware.VTK;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
#if UNITY_EDITOR
using UnityEditor;
using System.Threading;
#endif

namespace UFZ.VTK
{
	public abstract class VtkAlgorithm : SerializedMonoBehaviour
	{
		public bool HasInput { get { return _hasInput; } }
		protected bool _hasInput;

		[ShowInInspector, HideIf("HasOutput")]
		public bool HasOutput
		{
			get
			{
				if (Algorithm == null)
					return false;
				Algorithm.Update();
				var output = Algorithm.GetOutputDataObject(0) as vtkDataSet;
				var numPoints = output.GetNumberOfPoints();
				if (output != null && numPoints > 0)
					return true;
				return false;
			}
		}

		[ShowInInspector]
		public string OutputDataType
		{
			get
			{
				if (!HasOutput)
					return "";
				var output = Algorithm.GetOutputDataObject(0) as vtkDataSet;
				return output.GetType().ToString();
			}
		}

		[ShowInInspector, ShowIf("HasInput")]
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
		[SerializeField, HideInInspector]
		private VtkAlgorithm _inputAlgorithm;

		[HideInInspector]
		protected vtkAlgorithm Algorithm;

		[SerializeField]
		protected VtkRenderer ren;

		[OdinSerialize, HideInInspector]
		protected vtkAlgorithmOutput AlgorithmOutput;

		protected vtkTriangleFilter TriangleFilter;

		private void Reset()
		{
			Initialize();
		}

		protected void Awake()
		{
			Initialize();
		}

		protected void OnValidate()
		{
			Initialize();
		}

		public vtkAlgorithmOutput OutputPort()
		{
			return AlgorithmOutput;
		}

		[Button, ShowIf("ShowAddRenderer")]
		public void AddRenderer()
		{
			ren = gameObject.AddComponent<VtkRenderer>();
			ren.Algorithm = this;
			Initialize();
		}

		protected bool ShowAddRenderer()
		{
			return HasOutput && ren == null;
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
				ren.UpdateBuffers();
		}
	}
}
#endif