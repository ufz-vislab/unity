using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UFZ.Interaction;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
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
		public enum ColorMode { SolidColor, VertexColor, Texture }

		protected static Dictionary<ColorMode, string> ColorModeDict
		{
			get { return new Dictionary<ColorMode, string>()
			{
				{ ColorMode.SolidColor, "SolidColor" },
				{ ColorMode.VertexColor, "VertexColor" },
				{ ColorMode.Texture, "Texture" }
			}; }
		}

		public enum LightingMode { Lit, Unlit }

		protected static Dictionary<LightingMode, string> LightingModeDict
		{
			get { return new Dictionary<LightingMode, string>()
			{
				{ LightingMode.Lit, "Lit" },
				{ LightingMode.Unlit, "Unlit" }
			}; }
		}

		public enum SideMode { Front, Back, TwoSided, Wireframe }

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

		[OdinSerialize, HideInInspector]
		protected Material[] Materials;

		protected override void Start()
		{
			base.Start();
			UpdateShaderInternal();
		}

		[OdinSerialize, BoxGroup("Coloring")]
		public ColorMode ColorBy = ColorMode.VertexColor;

		[OdinSerialize, BoxGroup("Coloring"), ShowIf("IsSolidColorMode")]
		public Color SolidColor = Color.gray;
		private bool IsSolidColorMode() { return ColorBy == ColorMode.SolidColor; }

		[BoxGroup("Rendering")]
		public LightingMode Lighting = LightingMode.Lit;

		[BoxGroup("Rendering")]
		public SideMode Side = SideMode.Front;

		[BoxGroup("Coloring"), ShowIf("IsTextureColorMode")]
		public Texture Texture
		{
			get { return _texture; }
			set
			{
				_texture = value;
				UpdateProperties();
			}
		}
		[SerializeField, HideInInspector] private Texture _texture;
		private bool IsTextureColorMode() { return ColorBy == ColorMode.Texture; }

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
				var colorBy = ColorModeDict[ColorBy];
				var lit = LightingModeDict[Lighting];
				var side = SideModeDict[Side];

				var materials = localRenderer.sharedMaterials;

				if (Visibility == VisibilityMode.Disabled)
				{
					localRenderer.enabled = false;
				} else
				{
					// Object switches have precedence in enabling / disabling the renderer
					var switchBase = GetComponentInParent<ObjectSwitch>();
					if (switchBase)
						switchBase.SetActiveChild(switchBase.GetStep());
					else
						localRenderer.enabled = true;

					if (materials.Length == 0 || Side == SideMode.Wireframe)
						materials = new Material[1];
					if (materials.Length != 2 && Side == SideMode.TwoSided)
						materials = new Material[2];

					if (materials.Length == 1)
					{
						if (Side == SideMode.Wireframe)
						{
							var matName = "Materials/Wireframe";
							if (Opacity < 1f)
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
					}
					else if (materials.Length == 2)
					{
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
					}
				}
				if (Application.isPlaying)
					localRenderer.materials = materials;
				else
					localRenderer.sharedMaterials = materials;
				Materials = materials;
			}
		}

		protected override void UpdateProperties()
		{
			base.UpdateProperties();

			if (PropertyBlock == null || Thread.CurrentThread.ManagedThreadId != 1)
				return;

			PropertyBlock.SetColor(Shader.PropertyToID("_Color"),
				new Color(SolidColor.r, SolidColor.g, SolidColor.b, Opacity));
			PropertyBlock.SetFloat(WireframeColorPropId, ColorBy == ColorMode.VertexColor ? 1f : 0f);
			if (Texture != null)
				PropertyBlock.SetTexture(TexturePropId, Texture);

			UpdateShader();
			UpdateRenderers();
		}

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();

			UpdateProperties();
		}

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
