#if UNITY_STANDALONE_WIN
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kitware.VTK;
using Sirenix.OdinInspector;
using UFZ.Rendering;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UFZ.VTK
{
	[ExecuteInEditMode]
	public class VtkRenderer : SerializedMonoBehaviour, IHideable
	{
		[ShowInInspector]
		public bool Enabled {
			get { return _enabled; }
			set
			{
				_enabled = value;
				_buffersUpToDate = false;
			}
		}

		[SerializeField, HideInInspector]
		private bool _enabled = true;

		private SimpleVtkMapper _mapper;
		private vtkActor _actor;
		
		private Dictionary<Camera,CommandBuffer> _cameras = new Dictionary<Camera,CommandBuffer>();

		[HideInInspector]
		private bool _buffersUpToDate;

		[SerializeField]
		public VtkAlgorithm Algorithm;

		[ShowInInspector, ShowIf("Enabled")]
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

		[ShowInInspector, ShowIf("Enabled")]
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

		[ShowInInspector, ShowIf("Enabled")]
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

		public int NumPointDataArrays
		{
			get { return _mapper.PointDataArrayNames.Count; }
		}

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

			ScalarVisibility = _scalarVisibility;
			Range = _range;
			
			_mapper.ModifiedEvt += delegate { RequestRenderUpdate(); };
			Algorithm.Algorithm.ModifiedEvt += delegate { RequestRenderUpdate(); };
		}

		// Hack for regenerating buffers after startup
		private void Start()
		{
			StartCoroutine(TriggerBufferUpdate());
		}

		private IEnumerator TriggerBufferUpdate()
		{
			yield return new WaitForSeconds(0.01f);
			_buffersUpToDate = false;
		}

		[Button]
		private void Update()
		{
			if (_mapper == null)
				Initialize();
#if UNITY_EDITOR
			if (EditorApplication.isCompiling)
			{
				_buffersUpToDate = false;
				return;
			}
#endif
			// Apply transformation
			_mapper.SetTransformMatrix(transform.localToWorldMatrix);

			if (!_buffersUpToDate)
			{
				Cleanup();
				_mapper.RenderPiece(null, _actor);
				_buffersUpToDate = true;
			
				OnRenderObject();
			}
		}

		private void OnRenderObject()
		{
			if (_cameras == null)
				return;
			var act = gameObject.activeInHierarchy && enabled && Enabled;
			if (!act)
			{
				Cleanup();
				return;
			}
			
			// _mapper.SetTransformMatrix(transform.localToWorldMatrix);

			var cam = Camera.current;
			if (!cam)
				return;

			// Did we already add the command buffer on this camera? Nothing to do then.
			if (_cameras.ContainsKey(cam))
			{
#if UNITY_EDITOR
				SceneView.RepaintAll();
#endif
				return;
			}

			_cameras[cam] = _mapper.Buffer;
			cam.AddCommandBuffer(CameraEvent.AfterForwardOpaque, _mapper.Buffer);
#if UNITY_EDITOR
			SceneView.RepaintAll();
#endif
		}
		
		private void Cleanup()
		{
			foreach (var cam in _cameras)
			{
				if (cam.Key)
					cam.Key.RemoveCommandBuffer (CameraEvent.AfterForwardOpaque, cam.Value);
			}
			_cameras.Clear();
			_buffersUpToDate = false;
		}
		
		public void OnEnable()
		{
			Cleanup();
#if UNITY_EDITOR
			EditorApplication.update += Update;
#endif
		}

		public void OnDisable()
		{
			Cleanup();
#if UNITY_EDITOR
			EditorApplication.update = null;
#endif
		}

		public void RequestRenderUpdate()
		{
			_buffersUpToDate = false;
		}
	}
}
#endif
