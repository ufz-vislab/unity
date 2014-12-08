using UnityEngine;

public class EnableDisableObjectsAtStart : MonoBehaviour
{
	public GameObject[] Objects;
	public bool Enabled = true;
	void Start ()
	{
		foreach (var go in Objects)
			go.SetActive(Enabled);
	}
}
