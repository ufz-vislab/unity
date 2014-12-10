using System.Linq;
using UnityEngine;
using System.Collections;

public abstract class ObjectSwitchBase : IPlayable
{
	public int ActiveChild;
	private int _activeChild;
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
		if(index == _activeChild)
			return;

		int numChilds = transform.childCount;
		if (index >= numChilds)
			index = 0;
		if (index < 0)
			index = numChilds - 1;
		_activeChild = index;
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

	public override vrValue Begin(vrValue iValue = null)
	{
		SetActiveChild(0);
		return iValue;
	}

	public override vrValue End(vrValue iValue = null)
	{
		SetActiveChild(transform.childCount - 1);
		return iValue;
	}
}
