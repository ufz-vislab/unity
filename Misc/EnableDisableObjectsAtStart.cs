using UnityEngine;

namespace UFZ.Initialization
{
	/// <summary>
	/// Enables or disables an array of objects at start.
	/// </summary>
	public class EnableDisableObjectsAtStart : MonoBehaviour
	{
		public GameObject[] Objects;
		public bool Enabled = true;

		private void Start()
		{
			foreach (var go in Objects)
				go.SetActive(Enabled);
		}
	}
}
