using System.Linq;
using UFZ.Interaction;
using UnityEngine;

/// <summary>
/// Basic flip-book like object switch.
/// </summary>
public abstract class ObjectSwitchBase : IPlayable
{
	public int ActiveChild;
	protected float ElapsedTime = 0f;

	public delegate void Callback(int index);
	public Callback ActiveChildCallback;

	void OnValidate()
	{
		SetActiveChild(ActiveChild);
	}

	protected virtual void Update ()
	{
		ElapsedTime += UFZ.IOC.Core.Instance.Time.DeltaTime();
	}

	public void SetActiveChild(int index)
	{
		var numChilds = transform.childCount;
		if (index >= numChilds)
			index = 0;
		if (index < 0)
			index = numChilds - 1;
		ActiveChild = index;

		var i = 0;
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
		ElapsedTime = 0f;

		if (ActiveChildCallback != null)
			ActiveChildCallback(ActiveChild);
	}

	public void NoActiveChild()
	{
		foreach (var ren in transform.GetComponentsInChildren<Renderer>())
				ren.enabled = false;
		ElapsedTime = 0f;
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
