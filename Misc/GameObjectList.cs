using Sirenix.OdinInspector;
using UnityEngine;

namespace UFZ.Misc
{
	public class GameObjectList : SerializedMonoBehaviour
	{
		[SceneObjectsOnly]
		public GameObject[] Objects;
	}
}
