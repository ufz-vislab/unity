using Sirenix.OdinInspector;
using UnityEngine;

namespace UFZ.Misc
{
	public class GameObjectList : SerializedMonoBehaviour
	{
		[HideReferenceObjectPicker]
		public GameObject[] Objects;
	}
}
