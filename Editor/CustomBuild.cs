using System;
using System.Diagnostics;
using PygmyMonkey.AdvancedBuilder;

public class CustomBuild : IAdvancedCustomBuild
{
	public void OnPreBuild(Configuration configuration, DateTime buildDate)
	{
		// Do nothing
	}

	public void OnPostBuild(Configuration configuration, DateTime buildDate)
	{
		var process = new Process
		{
			StartInfo =
			{
				FileName = @"C:\tools\cwRsync_5.5.0_x86_Free\cwrsync.cmd"
			}
		};
		process.Start();
	}
}
