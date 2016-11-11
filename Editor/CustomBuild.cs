using System;
using System.Diagnostics;
using System.IO;
using PygmyMonkey.AdvancedBuilder;

public class CustomBuild : IAdvancedCustomBuild
{
	public void OnPreBuild(Configuration configuration, DateTime buildDate)
	{
		// Do nothing
	}

	public void OnPostBuild(Configuration configuration, DateTime buildDate)
	{
		// Do nothing
	}

	public void OnEveryBuildDone()
	{
		var hostname = Environment.MachineName;
		if (!hostname.Equals("vismaster"))
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
	}
}
