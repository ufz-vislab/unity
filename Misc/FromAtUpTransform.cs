using UnityEngine;
using System.Collections;

public class FromAtUpTransform : MonoBehaviour
{
	public bool ZUp = false;
	public Vector3 From;
	public Vector3 At;
	public Vector3 Up;

	void OnValidate ()
	{
		if (ZUp)
		{
			gameObject.transform.position = new Vector3(From.x, From.z, From.y);
			gameObject.transform.LookAt(new Vector3(At.x, At.z, At.y), new Vector3(Up.x, Up.z, Up.y));
		}
		else
		{
			gameObject.transform.position = From;
			gameObject.transform.LookAt(At, Up);
		}
	}
}
