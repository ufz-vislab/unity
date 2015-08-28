namespace UFZ.IOC
{

	public interface IEnvironment
	{
		bool IsCluster();
	}

	public class UnityEnvironment : IEnvironment
	{
		public bool IsCluster()
		{
			return false;
		}
	}

	public class MiddleVrEnvironment : IEnvironment
	{
		public bool IsCluster()
		{
			return MiddleVR.VRClusterMgr.IsCluster();
		}
	}
}
