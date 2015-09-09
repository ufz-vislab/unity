namespace UFZ.IOC
{

	public interface IEnvironment
	{
		bool IsCluster();
		
		bool HasDevice(string name);
	}

	public class UnityEnvironment : IEnvironment
	{
		public bool IsCluster()
		{
			return false;
		}
		
		public bool HasDevice(string name)
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
		
		public bool HasDevice(string name)
		{
			for (uint index = 0; index < MiddleVR.VRDeviceMgr.GetDevicesNb(); index++)
			{
				var device = MiddleVR.VRDeviceMgr.GetDeviceByIndex(index);
				if (device.GetName().Contains(name))
					return true;
			}
			return false;
		}
	}
}
