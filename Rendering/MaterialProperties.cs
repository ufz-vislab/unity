using System.Diagnostics;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using FullInspector;
using System;
using System.Text.RegularExpressions;

namespace UFZ.Rendering
{
	/// <summary>Controls material properties and sets the right shader for it.</summary>
	/// <remarks>
	/// Can be added to any GameObject. Sets the properties on all materials in this
	/// object and child objects.
	/// </remarks>
	public class MaterialProperties : BaseBehavior<FullSerializerSerializer>
	{
		public enum VisibilityMode
		{
			/// <summary>Fully opaque</summary>
			Opaque,

			/// <summary>Transparent</summary>
			Transparent,

			/// <summary>Renderer is disabled<summary>
			Disabled
		}

		public enum ColorMode
		{
			/// <summary>A single color</summary>
			SolidColor,

			/// <summary>Colored by vertex colors (e.g. from VTK scalars)</summary>
			VertexColor,

			/// <summary>Texture mapping</summary>
			Texture,

			Invalid
		}

		public enum LightingMode
		{
			/// <summary>Lighting enabled</summary>
			Lit,

			/// <summary>Lighting disabled, useful for point and line rendering</summary>
			Unlit
		}

		public enum SideMode
		{
			/// <summary>Only the front side is rendered</summary>
			Front,

			/// <summary>Only the back side is rendered</summary>
			Back,
			TwoSided
		}

		protected const float disableThreshold = 0.01f;

		[SerializeField] protected Material[] _materials;

#if UNITY_EDITOR
		private void Reset()
		{
			UpdateProperties();
		}
#endif

		/// <summary>Retrieves properties from the currently set material(s).</summary>
		public void UpdateProperties()
		{
			RestoreState();
			Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
			if (renderers != null && renderers.Length > 0)
				GetSettingsFromMaterial(renderers[0].sharedMaterials);
			SaveState();
		}

		/// <summary>The opacity of the object.</summary>
		/// <value>Can be between 0 (fully transparent) and 1 (opaque)</value>
		public float Opacity
		{
			get { return _opacity; }
			set
			{
				if (Mathf.Approximately(_opacity, value))
					return;

				if (value < 0f) _opacity = 0f;
				else if (value > 1f) _opacity = 1f;
				else _opacity = value;
				UpdateShader();
			}
		}

		[SerializeField] private float _opacity = 1f;

		/// <summary>
		/// Returns if an object is fully opaque, transparent or completely disabled
		/// based on the <see cref="Opacity" />-value.
		/// </summary>
		[SerializeField]
		public VisibilityMode Visibility
		{
			get
			{
				if (Mathf.Approximately(_opacity, 1f))
					return VisibilityMode.Opaque;
				return _opacity < disableThreshold ? VisibilityMode.Disabled : VisibilityMode.Transparent;
			}
		}

		public ColorMode ColorBy
		{
			get { return _colorBy; }
			set
			{
				if (_colorBy == value)
					return;

				_colorBy = value;
				UpdateShader();
			}
		}

		[SerializeField] private ColorMode _colorBy = ColorMode.VertexColor;

		public Color SolidColor
		{
			get { return _solidColor; }
			set
			{
				_solidColor = value;
				foreach (var material in _materials)
				{
					if (!material.HasProperty("_Color"))
						continue;
					material.color = value;
				}
			}
		}

		[SerializeField] private Color _solidColor = Color.gray;

		public LightingMode Lighting
		{
			get { return _lit; }
			set
			{
				if (_lit == value)
					return;

				_lit = value;
				UpdateShader();
			}
		}

		[SerializeField] private LightingMode _lit = LightingMode.Lit;

		[InspectorMargin(5)]
		[InspectorComment("Set two sided mode with the 'UFZ / Two Sided Material' menu option!")]
		public SideMode Side
		{
			get { return _side; }
			set
			{
				if (_side == value)
					return;

				_side = value;
				UpdateShader();
			}
		}

		[SerializeField] private SideMode _side = SideMode.Front;

		/// <summary>Initializes class to an existing material</summary>
		protected void GetSettingsFromMaterial(Material[] mat)
		{
			if (mat == null || mat.Length < 1)
				return;
			var match = Regex.Match(mat[0].shader.name, @"UFZ/(.*)-(.*)-(.*)-(.*)");
			if (!match.Success)
			{
				// Shader will be set for the first time
				UpdateShader();
				GetSettingsFromMaterial(mat);
				return;
			}

			_opacity = match.Groups[1].Value == "Opaque" ? 1f : mat[0].color.a;

			_colorBy = (ColorMode) Enum.Parse(typeof (ColorMode), match.Groups[2].Value);
			_lit = (LightingMode) Enum.Parse(typeof (LightingMode), match.Groups[3].Value);
			if (mat.Length == 1)
				Side = (SideMode) Enum.Parse(typeof (SideMode), match.Groups[4].Value);
			else
				Side = SideMode.TwoSided;
			UpdateShader();
		}

		/// <summary>Sets the appropriate shader via string-kungfu.</summary>
		public void UpdateShader()
		{
			var stackTrace = new StackTrace();
			if (stackTrace.GetFrames().Any(stackFrame => stackFrame.GetMethod().Name.Contains("RestoreState")))
				return;

			if (gameObject == null)
				return;
			foreach (var localRenderer in gameObject.GetComponentsInChildren<Renderer>())
			{
				var transparent = Visibility.ToString("f");
				var colorBy = _colorBy.ToString("f");
				var lit = _lit.ToString("f");
				var side = _side.ToString("f");

				var materials = Application.isPlaying ? localRenderer.materials : localRenderer.sharedMaterials;

				if (Visibility == VisibilityMode.Disabled)
				{
					localRenderer.enabled = false;
				}
				else
				{
					// Object switches have precedence in enabling / disabling the renderer
					var switchBase = GetComponentInParent<ObjectSwitchBase>();
					if (switchBase)
						switchBase.SetActiveChild(switchBase.ActiveChild);
					else
						localRenderer.enabled = true;

					switch (materials.Length)
					{
						case 1:
							materials[0].shader = Shader.Find("UFZ/" + transparent + "-" + colorBy + "-" + lit + "-" + side);
							break;
						case 2:
							materials[0].shader =
								Shader.Find("UFZ/" + transparent + "-" + colorBy + "-" + lit + "-" + SideMode.Front.ToString("f"));
							materials[1].shader =
								Shader.Find("UFZ/" + transparent + "-" + colorBy + "-" + lit + "-" + SideMode.Back.ToString("f"));
							if (Side != SideMode.TwoSided)
							{
								var newMaterials = new Material[1];
								if (Side == SideMode.Front)
									newMaterials[0] = materials[0];
								else
									newMaterials[0] = materials[1];
								if (Application.isPlaying)
									localRenderer.materials = newMaterials;
								else
									localRenderer.sharedMaterials = newMaterials;

								materials = newMaterials;
							}
							break;
					}
				}

				foreach (var mat in materials)
				{
					if (!mat.HasProperty("_Color"))
						continue;
					var color = mat.color;
					color.a = _opacity;
					mat.color = color;
				}
				_materials = materials;
			}
		}
	}
}
