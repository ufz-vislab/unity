#if UNITY_STANDALONE_WIN
using System;
using System.Collections;
using UnityEngine;
using Kitware.VTK;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UFZ.VTK
{
	[ExecuteInEditMode]
	public class VtkRenderer : SerializedMonoBehaviour
	{
		[HideInInspector]
		public Material PointsMaterial;
		[HideInInspector]
		public Material LinesMaterial;
		[HideInInspector]
		public Material TrianglesMaterial;

		private SimpleVtkMapper _mapper;
		private vtkActor _actor;

		[HideInInspector]
		public bool BuffersUpToDate;

		[HideInInspector, NonSerialized]
		public ComputeBuffer BufferPoints;
		[HideInInspector, NonSerialized]
		public ComputeBuffer BufferVerts;
		[HideInInspector, NonSerialized]
		public ComputeBuffer BufferLines;
		[HideInInspector, NonSerialized]
		public ComputeBuffer BufferTriangles;
		[HideInInspector, NonSerialized]
		public ComputeBuffer BufferNormals;
		[HideInInspector, NonSerialized]
		public ComputeBuffer BufferColors;

		[SerializeField]
		public VtkAlgorithm Algorithm;

		[ShowInInspector]
		public bool ScalarVisibility
		{
			get { return _scalarVisibility; }
			set
			{
				_scalarVisibility = value;
				_mapper.ScalarVisibility = value;
			}
		}
		[SerializeField, HideInInspector]
		private bool _scalarVisibility;

		[ShowInInspector]
		public uint ActiveColorArrayIndex
		{
			get
			{
				if (_mapper == null)
					return 9999;
				return _mapper.ActiveColorArrayIndex;
			}
			set { _mapper.ActiveColorArrayIndex = value; }
		}

		[ShowInInspector]
		public Vector2 Range
		{
			get { return _range; }
			set
			{
				_range = value;
				_mapper.SetScalarRange(value.x, value.y);
			}
		}
		[SerializeField, HideInInspector]
		private Vector2 _range;

		private void Initialize()
		{
			if(Algorithm == null)
				return;

			_mapper = SimpleVtkMapper.New();
			_mapper.Renderer = this;
			_mapper.SetInputConnection(Algorithm.OutputPort());
			_mapper.ScalarVisibility = _scalarVisibility;
			_mapper.ActiveColorArrayIndex = ActiveColorArrayIndex;

			_actor = vtkActor.New();
			_actor.SetMapper(_mapper);

			var property = vtkProperty.New();
			property.SetColor(1.0, 0.0, 0.0);
			_actor.SetProperty(property);
			if(PointsMaterial == null)
				PointsMaterial = new Material(Shader.Find("DX11/VtkPoints"));
			if (LinesMaterial == null)
				LinesMaterial = new Material(Shader.Find("DX11/VtkPoints"));
			if (TrianglesMaterial == null)
				TrianglesMaterial = new Material(Shader.Find("DX11/VtkTriangles"));

			_mapper.ModifiedEvt += delegate { BuffersUpToDate = false; };

			ScalarVisibility = _scalarVisibility;
			Range = _range;
		}

		// Hack for regenerating buffers after startup; see OnDestroy()
		private void Start()
		{
			StartCoroutine(TriggerBufferUpdate());
		}

		private IEnumerator TriggerBufferUpdate()
		{
			yield return new WaitForSeconds(0.01f);
			BuffersUpToDate = false;
		}

		// Is called somehow when pressing start button. Stack trace is not shown
		// Buffers are destroyed and not regenerated. Introduced coroutine-hack in
		// Start()/.
		private void OnDestroy()
		{
			//ReleaseBuffersImpl();
			//Debug.Log(StackTraceUtility.ExtractStackTrace());
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
			if (!BuffersUpToDate)
				UpdateBuffers();

			// Apply transformation
			PointsMaterial.SetMatrix("_MATRIX_M", transform.localToWorldMatrix);
			LinesMaterial.SetMatrix("_MATRIX_M", transform.localToWorldMatrix);
			TrianglesMaterial.SetMatrix("_MATRIX_M", transform.localToWorldMatrix);
		}

		private void OnRenderObject()
		{
			if (_mapper == null)
				return;

			if (BufferPoints == null || BufferPoints.count == 0)
				return;

			if (BufferVerts != null && BufferVerts.count > 0)
			{
				PointsMaterial.SetPass(0);
				Graphics.DrawProcedural(MeshTopology.Points, BufferVerts.count);
			}
			if (BufferLines != null && BufferLines.count > 0)
			{
				LinesMaterial.SetPass(0);
				Graphics.DrawProcedural(MeshTopology.Lines, BufferLines.count);
			}
			if (BufferPoints.count > 2 &&
				BufferTriangles != null && BufferTriangles.count > 2)
			{
				TrianglesMaterial.SetPass(0);
				Graphics.DrawProcedural(MeshTopology.Triangles, BufferTriangles.count);
			}
		}

		[Button]
		public void UpdateBuffers()
		{
			if (_mapper == null)
				Initialize();

			ReleaseBuffersImpl();
			_mapper.Update();
			_mapper.RenderPiece(null, _actor);
			BuffersUpToDate = true;
#if UNITY_EDITOR
			SceneView.RepaintAll();
#endif
		}

		private void ReleaseBuffersImpl()
		{
			if (BufferPoints != null)
			{
				BufferPoints.Release();
				BufferPoints = null;
			}
			if (BufferVerts != null)
			{
				BufferVerts.Release();
				BufferVerts = null;
			}
			if (BufferLines != null)
			{
				BufferLines.Release();
				BufferLines = null;
			}
			if (BufferTriangles != null)
			{
				BufferTriangles.Release();
				BufferTriangles = null;
			}
			if (BufferNormals != null)
			{
				BufferNormals.Release();
				BufferNormals = null;
			}
			if (BufferColors != null)
			{
				BufferColors.Release();
				BufferColors = null;
			}
			BuffersUpToDate = false;
		}
	}
}
#endif
