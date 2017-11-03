#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.SceneManagement;
#endif
using System.IO;

namespace UFZ.Build
{
	public class Builder
	{
#if UNITY_EDITOR
		static void BuildDemo(string name, bool absolute = false)
		{
			var buildPlayerOptions = new BuildPlayerOptions();
			var sceneDirectory = GetSceneShortDirectory(name);
			var sceneShortName = GetSceneShortName(name);
			if (!absolute)
			{
				sceneDirectory = "Assets/_project/Scenes/" + sceneShortName;
				name = sceneDirectory + ".scene";
			}

			buildPlayerOptions.scenes = new[]
			{
				name,
				"Assets/UFZ/Scenes/VRBase.unity"
			};
			var dest = "Builds/Windows/x64/" + sceneDirectory;
			buildPlayerOptions.locationPathName = dest + ".exe";
			buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
			buildPlayerOptions.options = BuildOptions.None;
			BuildPipeline.BuildPlayer(buildPlayerOptions);
			
			// Check VTK
			var path = Path.Combine(GetSceneDirectory(name), "VTK");
			if (Directory.Exists(path))
			{
				FileUtil.ReplaceDirectory(path, dest + "_Data/VTK");
			}
		}

		[MenuItem("UFZ/Build current scene")]
		static void BuildCurrent()
		{
			BuildDemo(GetCurrentScene(), true);
		}

		//[MenuItem("UFZ/Build/Chaohu2")]
		static void BuildChaohu2()
		{
			BuildDemo("Chaohu2");
		}

		//[MenuItem("UFZ/Build/Gross Schoenebeck")]
		static void BuildGrossSchoenebeck()
		{
			BuildDemo("GrossSchoenebeck");
		}
		
		public static string GetCurrentScene()
		{
			return SceneManager.GetSceneAt(0).path;
		}
#endif

		public static string GetSceneDirectory(string name)
		{
			var sceneShortName = Path.GetFileNameWithoutExtension(name);
			var path = "Assets/_project/" + sceneShortName;
			return Directory.Exists(path) ? path : "";
		}

		public static string GetSceneShortDirectory(string name)
		{
			return Path.GetFileNameWithoutExtension(name);
		}

		public static string GetSceneShortName(string name)
		{
			return Path.GetFileName(name);
		}
	}
}