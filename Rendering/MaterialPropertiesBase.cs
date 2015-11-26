using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
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

		// IMaterialProperties Interface
		public void SetRendererEnabled(bool enabled)
		{
			Enabled = enabled;
		}

		public bool GetRendererEnabled()
		{
			return Enabled;
		}

		public void SetOpacity(float opacity)
		{
			Opacity = opacity;
		}

		public float GetOpacity()
		{
			return Opacity;
		}

		[SerializeField, InspectorHeader("Visibility"), InspectorDivider]
		public bool Enabled {
			get { return _enabled; }
			set {
				_enabled = value;
				UpdateShader();
			}
		}

		/// <summary>
		/// Returns if an object is fully opaque, transparent or completely disabled
		/// based on the <see cref="Opacity" />-value.
		/// </summary>
		[SerializeField, InspectorShowIf("Enabled")]
		public VisibilityMode Visibility {
			get {
				if (Mathf.Approximately(_opacity, 1f))
					return VisibilityMode.Opaque;
				return _opacity < disableThreshold ? VisibilityMode.Disabled : VisibilityMode.Transparent;
			}
		}

		protected bool _enabled = true;

		/// <summary>The opacity of the object.</summary>
		/// <value>Can be between 0 (fully transparent) and 1 (opaque)</value>
		[SerializeField, InspectorShowIf("Enabled"), InspectorRange(0f, 1f, Step = 0.01f)]
		public float Opacity {
			get { return _opacity; }
			set {
				if (Mathf.Approximately(_opacity, value))
					return;

				_opacity = value;

				#if UNITY_EDITOR
				if (!FullInspector.Internal.fiUtility.IsMainThread)
					return;

				var colorId = Shader.PropertyToID("_Color");
				var color = PropertyBlock.GetVector(colorId);
				color.w = _opacity;
				PropertyBlock.SetVector(colorId, color);
				UpdateShader();
				UpdateRenderers();
				#else

				Loom.QueueOnMainThread(() =>
				{
				var colorId = Shader.PropertyToID("_Color");
				var color = PropertyBlock.GetVector(colorId);
				color.w = _opacity;
				PropertyBlock.SetVector(colorId, color);
				UpdateShader();
				UpdateRenderers();
				});
				#endif
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
	}
}
