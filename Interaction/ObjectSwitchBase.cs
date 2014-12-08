using System.Linq;
using UnityEngine;
using System.Collections;

public class ObjectSwitchBase : MonoBehaviour
{
	public int ActiveChild;
	protected float _elapsedTime = 0f;

	public delegate void Callback(int index);
	public Callback ActiveChildCallback;

	void OnValidate()
	{
		SetActiveChild(ActiveChild);
	}

	protected virtual void Update ()
	{
		_elapsedTime += UFZ.IOC.Core.Instance.Time.DeltaTime();
	}

	public void SetActiveChild(int index)
	{
		if(index == ActiveChild)
			return;

		int numChilds = transform.childCount;
		if (index >= numChilds)
			index = 0;
		if (index < 0)
			index = numChilds - 1;
		ActiveChild = index;

		int i = 0;
		foreach (var child in transform.Cast<Transform>().OrderBy(t => t.name))
		{
			foreach (var childRenderer in child.gameObject.GetComponentsInChildren<Renderer>())
			{
				if (i == index || index < 0)
					childRenderer.enabled = true;
				else
					childRenderer.enabled = false;
			}
			++i;
		}
		_elapsedTime = 0f;

		if (ActiveChildCallback != null)
			ActiveChildCallback(ActiveChild);
	}

	public void NoActiveChild()
	{
		foreach (var ren in transform.GetComponentsInChildren<Renderer>())
				ren.enabled = false;
		_elapsedTime = 0f;
	}

	public virtual void Begin()
	{
		SetActiveChild(0);
	}

	public virtual void End()
	{
		SetActiveChild(transform.childCount - 1);
	}
}
