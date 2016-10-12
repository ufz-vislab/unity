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

		[SerializeField, InspectorHeader("Visibility"), InspectorDivider]
		public bool Enabled {

			get { return _enabled; }
			set
			{
				_enabled = value;
				UpdateShader();
			}
		}
		protected bool _enabled = true;

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
				return _opacity < disableThreshold ? VisibilityMode.Disabled : VisibilityMode.Transparent;
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

				var colorId = Shader.PropertyToID("_Color");
				var color = PropertyBlock.GetVector(colorId);
				color.w = _opacity;
				PropertyBlock.SetVector(colorId, color);
				UpdateShader();
				UpdateRenderers();
			}
		}
		protected float _opacity = 1f;

		[SerializeField, HideInInspector]
		protected MaterialPropertyBlock PropertyBlock {
			get { return _propertyBlock ?? (_propertyBlock = new MaterialPropertyBlock()); }
		}

		protected MaterialPropertyBlock _propertyBlock;

		abstract public void UpdateShader();

		abstract public void UpdateRenderers();

		protected const float disableThreshold = 0.01f;

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

#if MVR
		protected void Start()
		{
			_enabledCommand = new vrCommand("", EnabledCommandHandler);
			_opacityChangedCommand = new vrCommand("", OpacityChangedCommandHandler);
		}

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
	}
}
