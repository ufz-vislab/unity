using System.Web.UI.WebControls;
//using PygmyMonkey.AdvancedBuilder;

namespace UFZ.CI
{
	public class Build
	{
		[UnityEditor.MenuItem("UFZ/CI/Build active")]
		public static void RunSingleBuild()
		{
			//var builder = AdvancedBuilder.Get();
			//var index = builder.getProjectConfigurations().selectedConfigurationIndex;
			//var config = builder.getProjectConfigurations().configurationList[index];
			//AdvancedBuilder.PerformBuild(config);
		}

		[UnityEditor.MenuItem("UFZ/CI/Build all")]
		public static void RunAllBuilds()
		{
			//AdvancedBuilder.PerformBuild();
		}
	}
}
