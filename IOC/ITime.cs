using UnityEngine;
using System.Collections;

namespace UFZ.IOC
{
	public interface ITime
	{
		float DeltaTime();
		float Time();
	}

	public class UnityTime : ITime
	{
		public float DeltaTime()
		{
			return UnityEngine.Time.deltaTime;
		}

		public float Time()
		{
			return UnityEngine.Time.time;
		}
	}

	public class MiddleVrTime : ITime
	{
		public float DeltaTime()
		{
			return (float)MiddleVR.VRKernel.GetDeltaTime();
		}

		public float Time()
		{
			return (float)MiddleVR.VRKernel.GetTime();
		}
	}
}
