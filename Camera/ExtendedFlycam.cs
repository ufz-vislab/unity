using UnityEngine;
using System.Collections;

public class ExtendedFlycam : MonoBehaviour
{
	public Transform directionNode = null;
	public float rotationSensitivity = 90;
	public float normalMoveSpeed = 10;
	public float deadZone = 0.01f;

	private float rotationX = 0.0f;

	void Start ()
	{
	}

	void Update ()
	{
		float deltaTime = UFZ.IOC.Core.Instance.Time.DeltaTime();
		//Debug.Log("Rot: " + Input.GetAxis("Mouse X"));
		if(Mathf.Abs(Input.GetAxis("Mouse X")) > deadZone)
		{
			rotationX += Input.GetAxis("Mouse X") * rotationSensitivity * deltaTime;
			transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
		}

		if(directionNode != null)
		{
	 		transform.position += directionNode.forward * normalMoveSpeed * Input.GetAxis("Vertical") * deltaTime;
			transform.position += directionNode.right * normalMoveSpeed * Input.GetAxis("Horizontal") * deltaTime;
		}
		else
		{
			transform.position += transform.forward * normalMoveSpeed * Input.GetAxis("Vertical") * deltaTime;
			transform.position += transform.right * normalMoveSpeed * Input.GetAxis("Horizontal") * deltaTime;
		}
	}
}
