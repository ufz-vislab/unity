using System;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Threading;
#endif

namespace UFZ.VTK
{
#if UNITY_EDITOR
	[InitializeOnLoad]
#endif
	public class Init
	{
		// See http://stackoverflow.com/a/33124250/80480
		static Init()
		{
#if UNITY_EDITOR
			if (Thread.CurrentThread.ManagedThreadId != 1)
				return;

			var pluginPath = "UFZ" + Path.DirectorySeparatorChar + "VTK" + Path.DirectorySeparatorChar + "Plugins";
#endif
			var currentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
#if UNITY_EDITOR_32
			var dllPath = Application.dataPath + Path.DirectorySeparatorChar
				+ pluginPath
				+ Path.DirectorySeparatorChar + "x32";
#elif UNITY_EDITOR_64
			var dllPath = Application.dataPath + Path.DirectorySeparatorChar
			              + pluginPath
			              + Path.DirectorySeparatorChar + "x64";
#else
			var dllPath = Application.dataPath
				+ Path.DirectorySeparatorChar + "Plugins";

#endif
			if (currentPath != null
			    && currentPath.Contains(dllPath) == false)
				Environment.SetEnvironmentVariable("PATH", currentPath + Path.PathSeparator + dllPath,
					EnvironmentVariableTarget.Process);
			// Debug.Log("Set VTK Dll path to " + dllPath);
		}
	}
}
