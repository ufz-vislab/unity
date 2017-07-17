using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Diagnostics;
using System.Linq;
using Debug = UnityEngine.Debug;

namespace UFZ.Rendering
{

	public class DefaultMaterialProperties : MaterialPropertiesBase
	{
		public void Reset()
		{
			#if UNITY_EDITOR
			//if (!FullInspector.Internal.fiUtility.IsMainThread)
			//	return;

			PropertyBlock.SetColor(Shader.PropertyToID("_Color"), new Color(1f, 1f, 1f, Opacity));
			UpdateRenderers();
			#else
			Loom.QueueOnMainThread(() =>
			{
			PropertyBlock.SetColor(Shader.PropertyToID("_Color"), new Color(1f, 1f, 1f, _opacity));
			UpdateRenderers();
			});
			#endif
		}

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

			foreach (var meshRenderer in GetComponentsInChildren<MeshRenderer>())
			{
				meshRenderer.enabled = Enabled;
				if (!Enabled)
					continue;

				if (Visibility == VisibilityMode.Disabled)
				{
					meshRenderer.enabled = false;
				} else
				{
					// Object switches have precedence in enabling / disabling the renderer
					var switchBase = GetComponentInParent<ObjectSwitchBase>();
					if (switchBase)
						switchBase.SetActiveChild(switchBase.ActiveChild);
					else
						meshRenderer.enabled = true;

					var mat = meshRenderer.sharedMaterial;
					var shaderName = mat.shader.name;
					var shaderFamily = shaderName.Split('/').Last();
					Shader shader = null;
					if (Visibility == VisibilityMode.Opaque)
						shader = Shader.Find("Legacy Shaders/" + shaderFamily);
					else
						shader = Shader.Find("Legacy Shaders/Transparent/" + shaderFamily);

					if (shader != null)
						mat.shader = shader;
					else
						Debug.LogWarning("Shader not found: " + shaderFamily);
				}
			}
		}
	}

}