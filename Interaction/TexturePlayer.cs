using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UFZ.Interaction;
using UFZ.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace UFZ.Interaction
{

	public class TexturePlayer : IPlayable
	{
		public Texture2D FirstTexture;

		[SerializeField, HideInInspector]
		private Texture[] _textures;
		public int NumTextures;

		public float Fps = 1f;

		private int _activeTexture;
		private float _elapsedTime;

		public override void Begin()
		{
			SetActiveTexture(0);
		}

		public override void End()
		{
			SetActiveTexture(NumTextures - 1);
		}

		public override void Forward()
		{
			SetActiveTexture(_activeTexture + 1);
		}

		public override void Back()
		{
			SetActiveTexture(_activeTexture - 1);
		}

		public override void Play()
		{
			IsPlaying = true;
		}

		public override void Stop()
		{
			IsPlaying = false;
			_elapsedTime = 0;
		}

		public override void TogglePlay()
		{
			IsPlaying = !IsPlaying;
		}

		protected void SetActiveTexture(int i)
		{
			if (i < 0)
				_activeTexture = NumTextures - 1;
			else if (i >= NumTextures)
				_activeTexture = 0;
			else
				_activeTexture = i;
			Percentage = (float)i / (NumTextures-1);
			TimeInfo = string.Format("{0:00}", _activeTexture);
			var matProp = gameObject.GetComponent<MaterialProperties>();
			if (matProp == null)
				return;
			matProp.ColorBy = MaterialProperties.ColorMode.Texture;
			matProp.Texture = _textures[_activeTexture];
		}
		
		private void Update()
		{
			if (!IsPlaying) return;
			_elapsedTime += IOC.Core.Instance.Time.DeltaTime();
			if (!(_elapsedTime > (1f / Fps)))
				return;
			_elapsedTime = 0;
			Forward();
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
			NumTextures = textureList.Count;
			_textures = textureList.OrderBy(texture => texture.name).ToArray();
		}
#endif
	}
}
