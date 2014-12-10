using UnityEngine;
using System.Collections;

namespace UFZ.IOC
{
	// all values in seconds
	public interface ITime
	{
		float DeltaTime();
		float Time();
		float RealtimeSinceStartup();
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

		public float RealtimeSinceStartup()
		{
			return UnityEngine.Time.realtimeSinceStartup;
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
			return (float)(MiddleVR.VRKernel.GetTime() / 1000.0);
		}

		public float RealtimeSinceStartup()
		{
			return Time();
		}
	}
}
