using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class MasterObjectSwitch : MonoBehaviour
{
	public int activeChild;
	public int fps;
	float elapsedTime = 0f;

	public delegate void Callback();
	public Callback ActiveChildCallback;

	void Reset()
	{
		activeChild = 0;
		fps = 5;
	}

	void Update()
	{
		elapsedTime += UFZ.IOC.Core.Instance.Time.DeltaTime();
	}

	void OnValidate()
	{
		SetActiveChild(activeChild);
	}

	public void forward()
	{
		if(elapsedTime > (1f / fps))
			SetActiveChild(activeChild + 1);
	}

	public void back()
	{
		if(elapsedTime > (1f / fps))
				SetActiveChild(activeChild - 1);
	}

	public void SetActiveChild(int index)
	{
		int numChilds = transform.childCount;
		if(index >= numChilds)
			index = 0;
		if(index < 0)
			index = numChilds - 1;
		activeChild = index;

		int i = 0;
		foreach(var child in transform.Cast<Transform>().OrderBy(t=>t.name))
		{
			if(i == index)
				child.gameObject.SetActive(true);
			else
				child.gameObject.SetActive(false);
			++i;
		}

		elapsedTime = 0f;

		if(ActiveChildCallback != null)
			ActiveChildCallback();
	}
}
