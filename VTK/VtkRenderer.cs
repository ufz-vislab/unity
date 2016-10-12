using FullInspector;
using UnityEngine;
using Kitware.VTK;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UFZ.VTK
{
	[ExecuteInEditMode]
	public class VtkRenderer : BaseBehavior
	{
		[HideInInspector]
		public Material PointsMaterial;
		[HideInInspector]
		public Material LinesMaterial;
		[HideInInspector]
		public Material TrianglesMaterial;

		protected SimpleVtkMapper mapper;
		protected vtkActor actor;

		[HideInInspector]
		public bool BuffersUpToDate;

		[HideInInspector]
		public ComputeBuffer bufferPoints;
		[HideInInspector]
		public ComputeBuffer bufferVerts;
		[HideInInspector]
		public ComputeBuffer bufferLines;
		[HideInInspector]
		public ComputeBuffer bufferTriangles;
		[HideInInspector]
		public ComputeBuffer bufferNormals;
		[HideInInspector]
		public ComputeBuffer bufferColors;

		public VtkAlgorithm Algorithm;

		[ShowInInspector]
		public bool ScalarVisibility
		{
			get { return _scalarVisibility; }
			set
			{
				_scalarVisibility = value;
				mapper.ScalarVisibility = value;
			}
		}
		[SerializeField, HideInInspector]
		private bool _scalarVisibility;

		[ShowInInspector]
		public uint ActiveColorArrayIndex
		{
			get { return mapper.ActiveColorArrayIndex; }
			set { mapper.ActiveColorArrayIndex = value; }
		}

		[ShowInInspector]
		public Vector2 Range
		{
			get { return _range; }
			set
			{
				_range = value;
				mapper.SetScalarRange(value.x, value.y);
			}
		}
		[SerializeField, HideInInspector]
		private Vector2 _range;

#if UNITY_EDITOR
		private void Reset()
		{
			Initialize();
		}
#endif

		private void Initialize()
		{
			if(Algorithm == null)
				return;

			mapper = SimpleVtkMapper.New();
			mapper.Renderer = this;
			mapper.SetInputConnection(Algorithm.OutputPort());
			mapper.ScalarVisibility = _scalarVisibility;
			mapper.ActiveColorArrayIndex = ActiveColorArrayIndex;
			//if(actor == null)
			actor = vtkActor.New();
			actor.SetMapper(mapper);

			var property = vtkProperty.New();
			property.SetColor(1.0, 0.0, 0.0);
			actor.SetProperty(property);
			if(PointsMaterial == null)
				PointsMaterial = new Material(Shader.Find("DX11/VtkPoints"));
			if (LinesMaterial == null)
				LinesMaterial = new Material(Shader.Find("DX11/VtkPoints"));
			if (TrianglesMaterial == null)
				TrianglesMaterial = new Material(Shader.Find("DX11/VtkTriangles"));
			UpdateBuffers();
			mapper.ModifiedEvt += delegate { UpdateBuffers(); };

			ScalarVisibility = _scalarVisibility;
			Range = _range;

			_initialized = true;
		}

		private bool _initialized;

		private void Awake()
		{
			if (Application.isPlaying)
				Initialize();
		}

		private void OnEnable()
		{
#if UNITY_EDITOR
			EditorApplication.playmodeStateChanged += delegate
			{
				if (!EditorApplication.isPlaying)
					Initialize();
			};
#endif
		}

		private void OnDisable()
		{
#if UNITY_EDITOR
			EditorApplication.playmodeStateChanged = null;
#endif
		}

		private void OnDestroy()
		{
#if UNITY_EDITOR
			DestroyImmediate(PointsMaterial);
			DestroyImmediate(LinesMaterial);
			DestroyImmediate(TrianglesMaterial);
#else
			Destroy(PointsMaterial);
			Destroy(LinesMaterial);
			Destroy(TrianglesMaterial);
#endif
			ReleaseBuffers();
		}

		private void Update()
		{
#if UNITY_EDITOR
			if (EditorApplication.isCompiling)
			{
				BuffersUpToDate = false;
				return;
			}
#endif
			if (!_initialized)
				Initialize();

			if (!BuffersUpToDate)
				UpdateBuffers();

			// Apply transformation
			PointsMaterial.SetMatrix("_MATRIX_M", transform.localToWorldMatrix);
			LinesMaterial.SetMatrix("_MATRIX_M", transform.localToWorldMatrix);
			TrianglesMaterial.SetMatrix("_MATRIX_M", transform.localToWorldMatrix);
		}

		private void OnRenderObject()
		{
			if (mapper == null)
				return;

			if (bufferPoints == null || bufferPoints.count == 0)
				return;

			if (bufferVerts != null && bufferVerts.count > 0)
			{
				PointsMaterial.SetPass(0);
				Graphics.DrawProcedural(MeshTopology.Points, bufferVerts.count);
			}
			if (bufferLines != null && bufferLines.count > 0)
			{
				LinesMaterial.SetPass(0);
				Graphics.DrawProcedural(MeshTopology.Lines, bufferLines.count);
			}
			if (bufferPoints.count > 2 &&
				bufferTriangles != null && bufferTriangles.count > 2)
			{
				TrianglesMaterial.SetPass(0);
				Graphics.DrawProcedural(MeshTopology.Triangles, bufferTriangles.count);
			}

			//var center = actor.GetCenter();
			//Debug.Log(name + ": [" + center[0] + ", " + center[1] + ", " + center[2] + "]");
		}

		public void UpdateBuffers()
		{
			if (mapper == null)
			{
				Initialize();
				return;
			}

			ReleaseBuffers();
			mapper.Update();
			mapper.RenderPiece(null, actor);
			BuffersUpToDate = true;
		}

		protected void ReleaseBuffers()
		{
#if UNITY_EDITOR
			if (!FullInspector.Internal.fiUtility.IsMainThread)
				return;
			ReleaseBuffersImpl();
#else
			Loom.QueueOnMainThread(() => { ReleaseBuffersImpl(); });
#endif
		}

		private void ReleaseBuffersImpl()
		{
			if (bufferPoints != null)
			{
				bufferPoints.Release();
				bufferPoints = null;
			}
			if (bufferVerts != null)
			{
				bufferVerts.Release();
				bufferVerts = null;
			}
			if (bufferLines != null)
			{
				bufferLines.Release();
				bufferLines = null;
			}
			if (bufferTriangles != null)
			{
				bufferTriangles.Release();
				bufferTriangles = null;
			}
			if (bufferNormals != null)
			{
				bufferNormals.Release();
				bufferNormals = null;
			}
			if (bufferColors != null)
			{
				bufferColors.Release();
				bufferColors = null;
			}
		}
	}
}