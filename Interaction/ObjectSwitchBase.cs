using System.Linq;
using UFZ.Interaction;
using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Basic flip-book like object switch.
/// </summary>
public abstract class ObjectSwitchBase : IPlayable
{
	public enum Ordering
	{
		Alphanumeric,
		Transform
	}

	public int ActiveChild;
	protected float ElapsedTime = 0f;

	public delegate void Callback(int index);
	public Callback ActiveChildCallback;

	public Ordering Order = Ordering.Alphanumeric;

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
		Transform[] transforms;
		if(Order == Ordering.Alphanumeric)
			transforms = transform.Cast<Transform>().ToArray().OrderBy(t => t.name, new AlphanumComparatorFast()).ToArray();
		else
			transforms = transform.Cast<Transform>().ToArray();
		foreach (var child in transforms)
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

#if MVR
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
#else
	public override void Begin()
	{
		SetActiveChild(0);
	}

	public override void End()
	{
		SetActiveChild(transform.childCount - 1);
	}
#endif
}
