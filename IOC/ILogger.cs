using UnityEngine;
using System.Collections;
using MiddleVR_Unity3D;

namespace UFZ.IOC
{
	public interface ILogger
	{
		void Info(string message);
		void Warning(string message);
		void Error(string message);
	}

	public class UnityLogger : ILogger
	{
		public void Info(string message)
		{
			Debug.Log(message);
		}

		public void Warning(string message)
		{
			Debug.LogWarning(message);
		}

		public void Error(string message)
		{
			Debug.LogError(message);
		}
	}

	public class MiddleVrLogger : ILogger
	{
		public void Info(string message)
		{
			MiddleVRTools.Log(2, "[ ] " + message);
		}

		public void Warning(string message)
		{
			MiddleVRTools.Log(1, "[-] " + message);
		}

		public void Error(string message)
		{
			MiddleVRTools.Log(0, "[X] " + message);
		}
	}
}
