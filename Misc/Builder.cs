
using System;
using System.Diagnostics;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.SceneManagement;
#endif
using UnityEngine;
using System.IO;

namespace UFZ.Build
{
	public class Builder
	{
		private const string SyncPath = "Y:\\vislab\\unity\\Player-x64\\";
#if UNITY_EDITOR

		static void BuildFromCLI()
		{
			var scene = GetArg ("-scene");
			BuildDemo(scene);
		}
		static void BuildDemo(string name, bool sync = false)
		{
			var buildPlayerOptions = new BuildPlayerOptions();
			var sceneDirectory = GetSceneShortDirectory(name);
			var sceneShortName = GetSceneShortName(name);

			buildPlayerOptions.scenes = new[]
			{
				name,
				"Assets/UFZ/Scenes/VRBase.unity"
			};
			var buildPath = Application.dataPath + "/../Builds/Windows/x64";
			var dest = buildPath + "/" + sceneDirectory;
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

			if (sync)
			{
				Sync(dest + "_Data", SyncPath + sceneShortName.Substring(0, sceneShortName.Length - ".unity".Length) + "_Data");
				Sync(buildPath + "/Mono", SyncPath + "/Mono");
				File.Copy(buildPlayerOptions.locationPathName, SyncPath + shortName + ".exe", true);
				File.Copy(buildPath + "/UnityPlayer.dll", SyncPath + "/UnityPlayer.dll", true);
			}
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

		private static void Sync(string source, string dest)
		{
			var hostname = Environment.MachineName;
			if (!hostname.Equals("VISMASTER"))
				return;

			UnityEngine.Debug.Log("Sync " + source + " to " + dest);
			var process = new Process
			{
				StartInfo =
				{
					FileName = "\"C:\\Program Files\\FreeFileSync\\FreeFileSync.exe\"",
					Arguments = Application.dataPath + "\\..\\sync.ffs_batch -leftDir " + source + " -rightDir " + dest
				}
			};
			process.Start();
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

		private static string GetArg(string name)
		{
			var args = System.Environment.GetCommandLineArgs();
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] == name && args.Length > i + 1)
				{
					return args[i + 1];
				}
			}
			return null;
		}
	}
}
