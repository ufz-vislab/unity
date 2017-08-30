#if UNITY_STANDALONE_WIN
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
				if (value == _enabled)
					return;
				if (value == false)
					Cleanup();
				else
					_buffersUpToDate = false;
				_enabled = value;
			}
		}

		[SerializeField, HideInInspector]
		private bool _enabled = true;

		private SimpleVtkMapper _mapper;
		private vtkActor _actor;

		private Dictionary<Camera,CommandBuffer> _cameras = new Dictionary<Camera,CommandBuffer>();
		private Dictionary<Camera,bool> _camerasUpToDate = new Dictionary<Camera,bool>();

		[HideInInspector]
		private bool _buffersUpToDate;

		[SerializeField]
		public VtkAlgorithm Algorithm;

		[ShowInInspector, ShowIf("ScalarVisibility"),
		 InlineEditor(InlineEditorModes.GUIAndPreview, Expanded = true)]
		public VtkLookupTable LookupTable
		{
			get { return _lookupTable; }
			set
			{
				_lookupTable = value;
				if (value == null)
					return;
				value.OnChange -= RequestRenderUpdate;
				value.OnChange += RequestRenderUpdate;
				RequestRenderUpdate();
			}
		}

		[SerializeField, HideInInspector]
		private VtkLookupTable _lookupTable;

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
			
			if (_lookupTable != null)
				_lookupTable.OnChange += RequestRenderUpdate;
			
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
				_mapper.RenderPiece(null, _actor);
				_buffersUpToDate = true;

				foreach (var cam in _camerasUpToDate.Keys.ToList())
					_camerasUpToDate[cam] = false;
			}
		}


		private void OnRenderObject()
		{
			if (_cameras == null || _mapper == null)
				return;
			var act = gameObject.activeInHierarchy && enabled && Enabled;
			if (!act)
			{
				Cleanup();
				return;
			}

			var cam = Camera.current;
			if (!cam)
				return;

			// Did we already add the command buffer on this camera? Nothing to do then.
			if (_camerasUpToDate.ContainsKey(cam) && _camerasUpToDate[cam])
			{
#if UNITY_EDITOR
				SceneView.RepaintAll();
#endif
				return;
			}

			if (_camerasUpToDate.ContainsKey(cam) && !_camerasUpToDate[cam])
			{
				cam.RemoveCommandBuffer(CameraEvent.AfterForwardOpaque, _cameras[cam]);
				_cameras[cam].Release();
			}

			_camerasUpToDate[cam] = true;
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
			_camerasUpToDate.Clear();
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

		private void OnDestroy()
		{
			if (_mapper != null)
				_mapper.Cleanup();
		}

		public void RequestRenderUpdate()
		{
			_buffersUpToDate = false;
		}
	}
}
#endif
