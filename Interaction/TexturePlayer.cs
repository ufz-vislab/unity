using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UFZ.Interaction
{

	public class TexturePlayer : IPlayable
	{
		public Texture2D FirstTexture;

		[SerializeField, HideInInspector]
		private Texture[] _textures;

		public override void SetStep(int step)
		{
			base.SetStep(step);

			var matProp = gameObject.GetComponent<UFZ.Rendering.MaterialProperties>();
			if (matProp == null)
				return;
			matProp.ColorBy = UFZ.Rendering.MaterialProperties.ColorMode.Texture;
			matProp.Texture = _textures[GetStep()];
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			var textureList = new List<Texture>();
			var baseName = FirstTexture.name;
			var rgx = new Regex("\\d+$");
			var matches = rgx.Matches(baseName);
			if (matches.Count <= 0)
				return;

			var match = matches[0].Value;
			var numDigits = match.Length;
			var textureName = baseName.Remove(baseName.Length - numDigits);

			var guids = AssetDatabase.FindAssets(textureName + " t:texture2D");
			foreach (var guid in guids)
			{
				var asset = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guid));
				if (asset)
					textureList.Add(asset);
			}
			NumSteps = textureList.Count;
			_textures = textureList.OrderBy(texture => texture.name).ToArray();
		}
#endif
	}
}
