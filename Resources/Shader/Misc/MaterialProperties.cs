using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using FullInspector;
using System;
using System.Text.RegularExpressions;

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
		Texture
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

	protected float _opacity = 1f;
	protected ColorMode _colorBy = ColorMode.VertexColor;
	protected LightingMode _lit = LightingMode.Lit;
	protected SideMode _side = SideMode.Front;
	protected const float disableThreshold = 0.01f;

	#if UNITY_EDITOR
	void Reset()
	{
		UpdateProperties();
	}
	#endif

	/// <summary>Retrieves properties from the currently set material(s).</summary>
	public void UpdateProperties()
	{
		RestoreState();
		Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
		if(renderers != null && renderers[0] != null)
			getSettingsFromMaterial(renderers[0].sharedMaterials);
		SaveState();
	}

	/// <summary>The opacity of the object.</summary>
	/// <value>Can be between 0 (fully transparent) and 1 (opaque)</value>
	[ShowInInspector]
	public float Opacity
	{
		get { return this._opacity; }
		set
		{
			if(Mathf.Approximately(_opacity, value))
				return;

			if(value < 0f) _opacity = 0f;
			else if(value > 1f) _opacity = 1f;
			else _opacity = value;
			UpdateShader();
		}
	}

	/// <summary>
	/// Returns if an object is fully opaque, transparent or completely disabled
	/// based on the <see cref="Opacity" />-value.
	/// </summary>
	[ShowInInspector]
	public VisibilityMode Visibility
	{
		get
		{
			if(Mathf.Approximately(_opacity, 1f))
				return VisibilityMode.Opaque;
			else if(_opacity < disableThreshold)
				return VisibilityMode.Disabled;
			return VisibilityMode.Transparent;
		}
	}

	[ShowInInspector]
	public ColorMode ColorBy
	{
		get { return this._colorBy; }
		set
		{
			if(_colorBy == value)
				return;

			_colorBy = value;
			UpdateShader();
		}
	}

	[ShowInInspector]
	public LightingMode Lighting
	{
		get { return _lit; }
		set
		{
			if(_lit == value)
				return;

			_lit = value;
			UpdateShader();
		}
	}

	[InspectorMargin(5)]
	[InspectorComment("Set two sided mode with the 'UFZ / Two Sided Material' menu option!")]
	[ShowInInspector]
	public SideMode Side
	{
		get { return _side; }
		set
		{
			if(_side == value)
				return;

			_side = value;
			UpdateShader();
		}
	}

	/// <summary>Initializes class to an existing material</summary>
	protected void getSettingsFromMaterial(Material[] mat)
	{
		if(mat == null || mat.Length < 1)
			return;
		Match match = Regex.Match(mat[0].shader.name, @"UFZ/(.*)-(.*)-(.*)-(.*)");
		if(!match.Success)
		{
			// Shader will be set for the first time
			UpdateShader();
			getSettingsFromMaterial(mat);
			return;
		}

		if(match.Groups[1].Value == "Opaque")
			_opacity = 1f;
		else
			_opacity = mat[0].color.a;

		_colorBy = (ColorMode) Enum.Parse(typeof(ColorMode), match.Groups[2].Value);
		_lit = (LightingMode) Enum.Parse(typeof(LightingMode), match.Groups[3].Value);
		if(mat.Length == 1)
			_side = (SideMode) Enum.Parse(typeof(SideMode), match.Groups[4].Value);
		else
			_side = SideMode.TwoSided;
		UpdateShader();
	}

	/// <summary>Sets the appropriate shader via string-kungfu.</summary>
	public void UpdateShader()
	{
		if(gameObject == null)
			return;
		foreach(Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
		{
			string transparent = Visibility.ToString("f");
			string color_by = _colorBy.ToString("f");
			string lit = _lit.ToString("f");
			string side = _side.ToString("f");

			Material[] mats;
			if(Application.isPlaying)
				mats = renderer.materials;
			else
				mats = renderer.sharedMaterials;

			if(Visibility != VisibilityMode.Disabled)
			{
				if(mats.Length == 1)
					mats[0].shader = Shader.Find("UFZ/" + transparent + "-" + color_by + "-" + lit + "-" + side);
				else if(mats.Length == 2)
				{
					mats[0].shader = Shader.Find("UFZ/" + transparent + "-" + color_by + "-" + lit + "-" + SideMode.Front.ToString("f"));
					mats[1].shader = Shader.Find("UFZ/" + transparent + "-" + color_by + "-" + lit + "-" + SideMode.Back.ToString("f"));

					if(Side != SideMode.TwoSided)
					{
						Material[] newMats = new Material[1];
						if(Side == SideMode.Front)
							newMats[0] = mats[0];
						else
							newMats[0] = mats[1];
						if(Application.isPlaying)
							renderer.materials = newMats;
						else
							renderer.sharedMaterials = newMats;
					}
				}
			}
			if(Visibility == VisibilityMode.Transparent)
			{
				foreach(Material mat in mats)
				{
					Color color = mat.color;
					color.a = _opacity;
					mat.color = color;
				}
			}
//			if(_opacity < disableThreshold)
//				renderer.enabled = false;
//			else
//				renderer.enabled = true;
		}
	}
}
