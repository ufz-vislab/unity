using UnityEngine;
using System.Collections;

namespace UFZ.Interaction
{
	public class CommonNavigation : MonoBehaviour
	{

		public GameObject Map;
		string _nodeToMove = "";

		// Use this for initialization
		void Start ()
		{
			_nodeToMove = "CenterNode";
		}

		//public void FlyToMap(Vector3 pos)
		//{
		//	FlyTo(Map.GetComponent<MapCamera>().FlyTo(pos) + new Vector3(0,1,0));
		//}

		public void FlyTo(Vector3 pos)
		{
			if(pos == new Vector3(-99f, -99f, -99f))
				return;
			//const float speed = 2f;
			GameObject nodeToMove = GameObject.Find(_nodeToMove);
			if (nodeToMove == null)
				return;
			//iTween.MoveTo(nodeToMove, new Hashtable {
			//	{"position", pos},
			//	{"time", speed},
			//	{"orienttopath", false},
			//	{"onstart", "IncreaseTweenCounter"},
			//	{"oncomplete", "DecreaseTweenCounter"}
			//});
		}
	}
}
