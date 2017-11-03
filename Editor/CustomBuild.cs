using System;
using System.Diagnostics;

public class CustomBuild // : IAdvancedCustomBuild
{
	public void OnEveryBuildDone()
	{
#if CUSTOM_POST_BUILD
		var hostname = Environment.MachineName;
		if (!hostname.Equals("VISMASTER"))
			return;

		var process = new Process
		{
			StartInfo =
			{
				FileName = "\"C:\\Program Files\\FreeFileSync\\FreeFileSync.exe\"",
				Arguments = "FileSync\\win-x32.ffs_batch"
			}
		};
		process.Start();

		var process64 = new Process
		{
			StartInfo =
			{
				FileName = "\"C:\\Program Files\\FreeFileSync\\FreeFileSync.exe\"",
				Arguments = "FileSync\\win-x64.ffs_batch"
			}
		};
		process64.Start();
#endif
	}
}
