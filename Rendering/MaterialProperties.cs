using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
using FullInspector;
using Debug = UnityEngine.Debug;
using UFZ.Tools.Extensions;

namespace UFZ.Rendering
{
	/// <summary>Controls material properties and sets the right shader for it.</summary>
	/// <remarks>
	/// Can be added to any GameObject. Sets the properties on all materials in this
	/// object and child objects.
	/// </remarks>
	public class MaterialProperties : MaterialPropertiesBase
	{
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

		protected static Dictionary<ColorMode, string> ColorModeDict
		{
			get { return new Dictionary<ColorMode, string>()
			{
				{ ColorMode.SolidColor, "SolidColor" },
				{ ColorMode.VertexColor, "VertexColor" },
				{ ColorMode.Texture, "Texture" },
				{ ColorMode.Invalid, "Invaild" }
			}; }
		}

		public enum LightingMode
		{
			/// <summary>Lighting enabled</summary>
			Lit,

			/// <summary>Lighting disabled, useful for point and line rendering</summary>
			Unlit
		}

		protected static Dictionary<LightingMode, string> LightingModeDict
		{
			get { return new Dictionary<LightingMode, string>()
			{
				{ LightingMode.Lit, "Lit" },
				{ LightingMode.Unlit, "Unlit" }
			}; }
		}

		public enum SideMode
		{
			/// <summary>Only the front side is rendered</summary>
			Front,

			/// <summary>Only the back side is rendered</summary>
			Back,
			TwoSided,
		    Wireframe
		}

		protected static Dictionary<SideMode, string> SideModeDict
		{
			get { return new Dictionary<SideMode, string>()
			{
				{ SideMode.Front, "Front" },
				{ SideMode.Back, "Back" },
				{ SideMode.TwoSided, "TwoSided" },
				{ SideMode.Wireframe, "Wireframe" }
			}; }
		}

		[SerializeField]
		[HideInInspector]
		protected Material[] Materials;


		#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();

			// Workaround, otherwise color and texture is lost when exiting playmode
			if (PropertyBlock == null || !FullInspector.Internal.fiUtility.IsMainThread)
				return;

				PropertyBlock.SetColor(Shader.PropertyToID("_Color"),
				new Color(_solidColor.r, _solidColor.g, _solidColor.b, _opacity));
			if (_texture != null)
				PropertyBlock.SetTexture(Shader.PropertyToID("_MainTex"), _texture);
			UpdateRenderers();
		}
		#endif

		private void Start()
		{
			RestoreState();
			UpdateShaderInternal();
		}

		[SerializeField, InspectorHeader("Coloring"), InspectorDivider]
		public ColorMode ColorBy {
			get { return _colorBy; }
			set
			{
				if (_colorBy == value)
					return;

				_colorBy = value;

				if (PropertyBlock == null || !FullInspector.Internal.fiUtility.IsMainThread)
					return;

				PropertyBlock.SetFloat(WireframeColorPropId,
					_colorBy == ColorMode.VertexColor ? 1f : 0f);
				UpdateShader();
			}
		}

		private ColorMode _colorBy = ColorMode.VertexColor;

		[SerializeField]
		public Color SolidColor {
			get { return _solidColor; }
			set
			{
				_solidColor = value;

				if (PropertyBlock == null || !FullInspector.Internal.fiUtility.IsMainThread)
					return;
				PropertyBlock.SetColor(ColorPropId, new Color(value.r, value.g, value.b, _opacity));
			    PropertyBlock.SetColor(WireframeColorPropId, new Color(value.r, value.g, value.b, _opacity));
				UpdateRenderers();
			}
		}

		private Color _solidColor = Color.gray;

		[SerializeField]
		public LightingMode Lighting {
			get { return _lit; }
			set {
				if (_lit == value)
					return;

				_lit = value;
				UpdateShader();
			}
		}

		private LightingMode _lit = LightingMode.Lit;

		[SerializeField]
		public SideMode Side {
			get { return _side; }
			set {
				if (_side == value)
					return;

				_side = value;
				UpdateShader();
			}
		}

		private SideMode _side = SideMode.Front;

		[SerializeField]
		public Texture Texture {
			get { return _texture; }
			set {
				_texture = value;

				if (value == null || !FullInspector.Internal.fiUtility.IsMainThread || PropertyBlock == null)
					return;
				PropertyBlock.SetTexture(TexturePropId, _texture);
				UpdateRenderers();
			}
		}

		private Texture _texture;


		public override void UpdateRenderers()
		{
			foreach (var localRenderer in gameObject.GetComponentsInChildren<Renderer>())
				localRenderer.SetPropertyBlock(PropertyBlock);
		}

		/// <summary>Sets the appropriate shader via string-kungfu.</summary>
		public override void UpdateShader()
		{
			var stackTrace = new StackTrace();
			var frames = stackTrace.GetFrames();
			if (frames != null && frames.Any(stackFrame => stackFrame.GetMethod().Name.Contains("RestoreState")))
				return;

			UpdateShaderInternal();
		}

		private void UpdateShaderInternal()
		{
			foreach (var localRenderer in gameObject.GetComponentsInChildren<Renderer>())
			{
				localRenderer.enabled = Enabled;
				if (!Enabled)
					continue;

				var transparent = VisibilityModeDict[Visibility];
				var colorBy = ColorModeDict[_colorBy];
				var lit = LightingModeDict[_lit];
				var side = SideModeDict[_side];

				var materials = localRenderer.sharedMaterials;

				if (Visibility == VisibilityMode.Disabled)
				{
					localRenderer.enabled = false;
				} else
				{
					// Object switches have precedence in enabling / disabling the renderer
					var switchBase = GetComponentInParent<ObjectSwitchBase>();
					if (switchBase)
						switchBase.SetActiveChild(switchBase.ActiveChild);
					else
						localRenderer.enabled = true;

					if (materials.Length == 0 || _side == SideMode.Wireframe)
						materials = new Material[1];
					if (materials.Length != 2 && _side == SideMode.TwoSided)
						materials = new Material[2];

					switch (materials.Length)
					{
					case 1:
					    if (_side == SideMode.Wireframe)
					    {
					        var matName = "Materials/Wireframe";
					        if (_opacity < 1f)
					            matName = "Materials/WireframeAdditive";

					        var mat = Resources.Load(matName, typeof(Material)) as Material;
					        materials[0] = mat;
					    }
					    else
					    {
					        var matName = transparent + colorBy + lit + side;
					        var mat = Resources.Load("Materials/" + matName, typeof(Material)) as Material;
					        if (mat == null)
					            Debug.LogWarning(gameObject.name + ": Material " + matName + " not found.");
					        materials[0] = mat;
					    }
					    break;
					case 2:
						var matNameFront = transparent + colorBy + lit + SideModeDict[SideMode.Front];
						var matNameBack = transparent + colorBy + lit + SideModeDict[SideMode.Back];
						var matFront = Resources.Load("Materials/" + matNameFront, typeof(Material)) as Material;
						var matBack = Resources.Load("Materials/" + matNameBack, typeof(Material)) as Material;
						materials[0] = matFront;
						materials[1] = matBack;
						if (Side != SideMode.TwoSided)
						{
							var newMaterials = new Material[1];
							if (Side == SideMode.Front)
								newMaterials[0] = materials[0];
							else
								newMaterials[0] = materials[1];

							materials = newMaterials;
						}
						break;
					}
				}
				if (Application.isPlaying)
					localRenderer.materials = materials;
				else
					localRenderer.sharedMaterials = materials;
				Materials = materials;
			}
		}

		#if UNITY_EDITOR
		[MenuItem("UFZ/Consolidate MaterialProperties")]
		private static void Consolidate()
		{
			var gos = Selection.gameObjects;
			if (gos.Length == 0)
				Debug.LogWarning("No GameObjects selected!");

			foreach (var go in gos)
			{
				var matProps = go.GetComponentsInChildren<MaterialProperties>();
				if (matProps.Length == 0)
				{
					Debug.LogWarning("No MaterialProperties found!");
					return;
				}

				go.AddComponent(matProps[0]);

				foreach (var matProp in matProps)
					DestroyImmediate(matProp);
			}
		}
		#endif
	}
}
