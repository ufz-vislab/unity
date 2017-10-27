using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UFZ.Rendering
{
	public abstract class MaterialPropertiesBase : SerializedMonoBehaviour
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

		[ShowInInspector, BoxGroup("Visibility")]
		public bool Enabled
		{
			get { return _enabled; }
			set { _enabled = value; UpdateProperties(); }
		}
		[SerializeField, HideInInspector] private bool _enabled = true;

		/// <summary>
		/// Returns if an object is fully opaque, transparent or completely disabled
		/// based on the <see cref="Opacity" />-value.
		/// </summary>
		[BoxGroup("Visibility"), ReadOnly, ShowIf("Enabled")]
		public VisibilityMode Visibility = VisibilityMode.Opaque;

		/// <summary>The opacity of the object.</summary>
		/// <value>Can be between 0 (fully transparent) and 1 (opaque)</value>
		[ShowInInspector, BoxGroup("Visibility"), ShowIf("Enabled")] // Range(0f, 1f)
		public float Opacity
		{
			get { return _opacity; }
			set { _opacity = value; UpdateProperties(); }
		}

		[SerializeField, HideInInspector] private float _opacity = 1f;

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
			UpdateProperties();
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
		protected void Awake()
		{
			InitUnityApi();
		}

#if UNITY_EDITOR
		protected virtual void OnValidate()
		{
			if (Application.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode)
				return;
			InitUnityApi();

			UpdateProperties();
		}
#endif
		
		// Init stuff with can only be called from the main thread
		private void InitUnityApi()
		{
			if (PropertyBlock != null)
				return;

#if UNITY_EDITOR
			if (Thread.CurrentThread.ManagedThreadId != 1)
				return;
#endif

			ColorPropId = Shader.PropertyToID("_Color");
			WireframeColorPropId = Shader.PropertyToID("_V_WIRE_Color");
			TexturePropId =  Shader.PropertyToID("_MainTex");
			
			PropertyBlock = new MaterialPropertyBlock();
		}

		[Button]
		protected virtual void UpdateProperties()
		{
			if (PropertyBlock == null)
				return;

			var color = PropertyBlock.GetVector(ColorPropId);
			color.w = Opacity;
			PropertyBlock.SetVector(ColorPropId, color);
			PropertyBlock.SetColor(WireframeColorPropId, new Color(color.x, color.y, color.z, Opacity));

			if (Opacity > 0.99f)
				Visibility = VisibilityMode.Opaque;
			else
				Visibility = Opacity < DisableThreshold ?
					VisibilityMode.Disabled : VisibilityMode.Transparent;
		}

		protected virtual void Update()
		{
			//UpdateProperties();
		}
	}
}
