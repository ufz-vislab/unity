
using System;
using System.Diagnostics;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.SceneManagement;
#endif
using System.IO;

namespace UFZ.Build
{
	public class Builder
	{
		private const string SyncPath = "Y:\\vislab\\unity\\Player-x64\\";
#if UNITY_EDITOR
		static void BuildDemo(string name, bool absolute = false)
		{
			var buildPlayerOptions = new BuildPlayerOptions();
			var sceneDirectory = GetSceneShortDirectory(name);
			var sceneShortName = GetSceneShortName(name);

			//if (!absolute)
			//{
			//	sceneDirectory = "Assets/_project/Scenes/" + sceneShortName;
			//	name = sceneDirectory + ".scene";
			//}

			buildPlayerOptions.scenes = new[]
			{
				name,
				"Assets/Plugins/UFZ/Scenes/VRBase.unity"
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

			var shortName = sceneShortName.Substring(0, sceneShortName.Length - ".unity".Length);
			UnityEngine.Debug.Log("Syncing to " + SyncPath + shortName + "_Data");

			Sync(dest + "_Data", SyncPath + sceneShortName.Substring(0, sceneShortName.Length - ".unity".Length) + "_Data");
			File.Copy(buildPlayerOptions.locationPathName, SyncPath + shortName + ".exe", true);
		}

		[MenuItem("UFZ/Build current scene")]
		static void BuildCurrent()
		{
			BuildDemo(GetCurrentScene(), true);
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

		private static void Sync(string source, string dest)
		{
			var hostname = Environment.MachineName;
			if (!hostname.Equals("VISMASTER"))
				return;

			var process = new Process
			{
				StartInfo =
				{
					FileName = "\"C:\\Program Files\\FreeFileSync\\FreeFileSync.exe\"",
					Arguments = "..\\..\\..\\sync.ffs_batch -leftDir " + source + " -rightDir " + dest
				}
			};
			process.Start();
		}
	}
}
