using UnityEngine;
using System.Collections;
using MiddleVR_Unity3D;

public class FlyCamController : MonoBehaviour {

	/*
	wasd : basic movement
	shift : Makes camera accelerate
	space : Moves camera on X and Z axis only.  So camera doesn't gain any height*/

	string NodeToMove = "";
	vrKeyboard keyb = null;
	public float mainSpeed = 10.0f; // regular speed
	public float shiftAdd = 25.0f;  // multiplied by how long shift is held.  Basically running
	public float maxShift = 100.0f; // Maximum speed when holdin gshift
	public float camSens = 0.2f;    // How sensitive it with mouse
	private Vector3 lastMouse = new Vector3(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)
	private float totalRun= 1.0f;

	void Start()
	{
		keyb = MiddleVR.VRDeviceMgr.GetKeyboard();
		NodeToMove = "CenterNode";
	}

	void Update ()
	{
		if (keyb == null)
			return;
		GameObject nodeToMove = null;
		nodeToMove = GameObject.Find(NodeToMove);
		if (nodeToMove == null)
			return;

		lastMouse = Input.mousePosition - lastMouse ;
		lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0 );
		lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x , transform.eulerAngles.y + lastMouse.y, 0);
		nodeToMove.transform.eulerAngles = lastMouse;
		lastMouse =  Input.mousePosition;

		Vector3 p = GetBaseInput();
		if(keyb.IsKeyToggled(MiddleVR.VRK_LSHIFT))
		{
			totalRun += UFZ.IOC.Core.Instance.Time.DeltaTime();
			p  = p * totalRun * shiftAdd;
			p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
			p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
			p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
		}
		else
		{
			totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
			p = p * mainSpeed;
		}

		p = p * UFZ.IOC.Core.Instance.Time.DeltaTime();

		Vector3 newPosition = nodeToMove.transform.position;
		if(keyb.IsKeyToggled(MiddleVR.VRK_SPACE))
		{ //If player wants to move on X and Z axis only
			nodeToMove.transform.Translate(p);
			newPosition.x = nodeToMove.transform.position.x;
			newPosition.z = nodeToMove.transform.position.z;
			nodeToMove.transform.position = newPosition;
		}
		else
			nodeToMove.transform.Translate(p);
	}

	private Vector3 GetBaseInput()
	{
		Vector3 p_Velocity = new Vector3();
		if(keyb.IsKeyPressed(MiddleVR.VRK_W))
			p_Velocity += new Vector3(0, 0 , 1);

		if(keyb.IsKeyPressed(MiddleVR.VRK_S))
			p_Velocity += new Vector3(0, 0, -1);

		if(keyb.IsKeyPressed(MiddleVR.VRK_A))
			p_Velocity += new Vector3(-1, 0, 0);

		if(keyb.IsKeyPressed(MiddleVR.VRK_D))
			p_Velocity += new Vector3(1, 0, 0);

		return p_Velocity;
	}
}
