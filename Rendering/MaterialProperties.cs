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

		[SerializeField]
		[HideInInspector]
		protected Material[] _materials;


		#if UNITY_EDITOR
		private void Reset()
		{
			RestoreState();
			UpdateShader();
		}

		protected override void OnValidate()
		{
			base.OnValidate();

			// Workaround, otherwise color and texture is lost when exiting playmode
			PropertyBlock.SetColor(Shader.PropertyToID("_Color"),
				new Color(_solidColor.r, _solidColor.g, _solidColor.b, _opacity));
			if (_texture != null)
				PropertyBlock.SetTexture(Shader.PropertyToID("_MainTex"), _texture);
			UpdateRenderers();
		}
		#endif

		[SerializeField, InspectorHeader("Coloring"), InspectorDivider]
		public ColorMode ColorBy {
			get { return _colorBy; }
			set
			{
				if (_colorBy == value)
					return;

				_colorBy = value;
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

				PropertyBlock.SetColor(Shader.PropertyToID("_Color"), new Color(value.r, value.g, value.b, _opacity));
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

				if (value == null)
					return;
				PropertyBlock.SetTexture(Shader.PropertyToID("_MainTex"), _texture);
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
			if (stackTrace.GetFrames().Any(stackFrame => stackFrame.GetMethod().Name.Contains("RestoreState")))
				return;

			if (gameObject == null)
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

				var transparent = Visibility.ToString("f");
				var colorBy = _colorBy.ToString("f");
				var lit = _lit.ToString("f");
				var side = _side.ToString("f");

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

					if (materials.Length == 0)
						materials = new Material[1];
					if (materials.Length != 2 && _side == SideMode.TwoSided)
						materials = new Material[2];

					switch (materials.Length)
					{
					case 1:
						var matName = transparent + colorBy + lit + side;
						var mat = Resources.Load("Materials/" + matName, typeof(Material)) as Material;
						if (mat == null)
							Debug.LogWarning("Material " + matName + " not found.");
						materials[0] = mat;
						break;
					case 2:
						var matNameFront = transparent + colorBy + lit + SideMode.Front.ToString("f");
						var matNameBack = transparent + colorBy + lit + SideMode.Back.ToString("f");
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
				_materials = materials;
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

				go.AddComponent<MaterialProperties>(matProps[0]);

				foreach (var matProp in matProps)
					DestroyImmediate(matProp);
			}
		}
		#endif
	}
}
