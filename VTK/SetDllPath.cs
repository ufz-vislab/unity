using System;
using System.IO;
using System.Threading;
using UnityEngine;

public class SetDllPath {

	public static void Init()
	{
		var currentPath = Environment.GetEnvironmentVariable("PATH",
			EnvironmentVariableTarget.Process);
		if (currentPath != null &&  currentPath.Contains("VTK-Init"))
			return;
			
		if (Thread.CurrentThread.ManagedThreadId != 1)
			return;
#if UNITY_EDITOR
		var pluginPath = Path.Combine(Application.dataPath, "UFZ/VTK/Plugins");
#endif
#if UNITY_EDITOR_32
			var dllPath = Path.Combine(pluginPath, "x86");
#elif UNITY_EDITOR_64
		var dllPath = Path.Combine(pluginPath, "x86_64");
#elif UNITY_STANDALONE
			var dllPath = Path.Combine(Application.dataPath, "Plugins");
			//var dllPath = "./" + AppParameters.Get.productName + "_Data/Plugins";
#endif
		Environment.SetEnvironmentVariable("PATH", currentPath +
		                                           Path.PathSeparator + dllPath +
		                                           Path.PathSeparator + "VTK-Init",
			EnvironmentVariableTarget.Process);
		Debug.LogWarning("Set VTK Dll path to " + dllPath);
	}
}
