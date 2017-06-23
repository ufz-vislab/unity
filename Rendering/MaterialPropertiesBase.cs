using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using FullInspector;

namespace UFZ.Rendering
{
	public abstract class MaterialPropertiesBase : BaseBehavior
	{
		public enum VisibilityMode
		{
			/// <summary>Fully opaque</summary>
			Opaque,

			/// <summary>Transparent</summary>
			Transparent,

			/// <summary>Renderer is disabled</summary>
			Disabled
		}
		
		protected static Dictionary<VisibilityMode, string> VisibilityModeDict
		{
			get { return new Dictionary<VisibilityMode, string>()
			{
				{ VisibilityMode.Opaque, "Opaque" },
				{ VisibilityMode.Transparent, "Transparent" },
				{ VisibilityMode.Disabled, "Disabled" }
			}; }
		}

		[SerializeField, InspectorHeader("Visibility"), InspectorDivider]
		public bool Enabled {

			get { return _enabled; }
			set
			{
				_enabled = value;
				UpdateShader();
			}
		}
		private bool _enabled = true;

		/// <summary>
		/// Returns if an object is fully opaque, transparent or completely disabled
		/// based on the <see cref="Opacity" />-value.
		/// </summary>
		[SerializeField, InspectorShowIf("Enabled")]
		public VisibilityMode Visibility {
			get
			{
				if (_opacity > 0.99f)
					return VisibilityMode.Opaque;
				return _opacity < DisableThreshold ? VisibilityMode.Disabled : VisibilityMode.Transparent;
			}
		}

		/// <summary>The opacity of the object.</summary>
		/// <value>Can be between 0 (fully transparent) and 1 (opaque)</value>
		[SerializeField, InspectorShowIf("Enabled"), InspectorRange(0f, 1f, Step = 0.01f)]
		public float Opacity {
			get { return _opacity; }
			set
			{
				if (Mathf.Approximately(_opacity, value))
					return;

				_opacity = value;
				
				if (PropertyBlock == null)
					return;
				
				var color = PropertyBlock.GetVector(ColorPropId);
				color.w = _opacity;
				PropertyBlock.SetVector(ColorPropId, color);
				PropertyBlock.SetColor(WireframeColorPropId, new Color(color.x, color.y, color.z, _opacity));
				UpdateShader();
				UpdateRenderers();
			}
		}
		// ReSharper disable once InconsistentNaming
		protected float _opacity = 1f;

		[HideInInspector]
		protected MaterialPropertyBlock PropertyBlock { get; private set; }

		public abstract void UpdateShader();

		public abstract void UpdateRenderers();

		protected const float DisableThreshold = 0.01f;

		public void SetEnabled(bool enable)
		{
#if MVR
			if (_enabledCommand != null)
				_enabledCommand.Do(enable);
#else
			Enabled = enable;
#endif
		}

		public void SetOpacity(float opacity)
		{
#if MVR
			if (_opacityChangedCommand != null)
				_opacityChangedCommand.Do(opacity);
#else
			Opacity = opacity;
#endif
		}

		protected virtual void Start()
		{
#if MVR
			_enabledCommand = new vrCommand("", EnabledCommandHandler);
			_opacityChangedCommand = new vrCommand("", OpacityChangedCommandHandler);
#endif
		}

#if MVR
		private void OnDestroy()
		{
			MiddleVR.DisposeObject(ref _enabledCommand);
			MiddleVR.DisposeObject(ref _opacityChangedCommand);
		}

		private vrCommand _enabledCommand;
		private vrCommand _opacityChangedCommand;

		private vrValue EnabledCommandHandler(vrValue enable)
		{
			Enabled = enable.GetBool();
			return true;
		}

		private vrValue OpacityChangedCommandHandler(vrValue opacity)
		{
			Opacity = opacity.GetFloat();
			return true;
		}
#endif

		protected int ColorPropId;
		protected int WireframeColorPropId;
		protected int TexturePropId;
		protected override void Awake()
		{
			base.Awake();
			InitUnityApi();
		}

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();

			if (Application.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode)
				return;
			InitUnityApi();
		}
#endif
		
		// Init stuff with can only be called from the main thread
		private void InitUnityApi()
		{
			if (PropertyBlock != null)
				return;

#if UNITY_EDITOR
			if (!FullInspector.Internal.fiUtility.IsMainThread)
				return;
#endif

			ColorPropId = Shader.PropertyToID("_Color");
			WireframeColorPropId = Shader.PropertyToID("_V_WIRE_Color");
			TexturePropId =  Shader.PropertyToID("_MainTex");
			
			PropertyBlock = new MaterialPropertyBlock();
		}
	}
}
