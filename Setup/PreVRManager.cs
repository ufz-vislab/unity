using UnityEngine;

namespace UFZ.Initialization
{
	public class PreVRManager : MonoBehaviour
	{
		private void Awake()
		{
			var playerGo = GameObject.Find("Player");
			var vrManager = GameObject.Find("VRManager").GetComponent<VRManagerScript>();
			if(vrManager.VRSystemCenterNode == null)
				vrManager.VRSystemCenterNode = playerGo;
		}
	}
}
