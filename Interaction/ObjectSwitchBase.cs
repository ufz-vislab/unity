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
	protected float ElapsedTime;
	protected GameObject ActiveChildGo;

	public delegate void Callback(int index);
	public Callback ActiveChildCallback;

	public Ordering Order = Ordering.Alphanumeric;

	public bool Active
	{
		get { return _active; }
		set
		{
			foreach (var childRenderer in ActiveChildGo.GetComponentsInChildren<Renderer>(true))
				childRenderer.enabled = value;
			_active = value;
		}
	}
	private bool _active = true;

#if MVR
	private vrCommand _activeChildCommand;
	protected void Start()
	{
		_activeChildCommand = new vrCommand("", ActiveChildCommandHandler);
	}

	private void OnDestroy()
	{
		MiddleVR.DisposeObject(ref _activeChildCommand);
	}
	
	private vrValue ActiveChildCommandHandler(vrValue index)
	{
		SetActiveChildInternal(index.GetInt());
		return true;
	}
#endif
	
	protected virtual void OnValidate()
	{
		SetActiveChildInternal(ActiveChild);
		Name = gameObject.name;
	}

	protected virtual void Update ()
	{
		ElapsedTime += UFZ.IOC.Core.Instance.Time.DeltaTime();
	}

	public void SetActiveChild(float percentage)
	{
		SetActiveChild((int)(percentage * (transform.childCount - 1)));
	}

	public void SetActiveChild(int index)
	{
#if MVR
		if (_activeChildCommand != null)
			_activeChildCommand.Do(index);
		else
			SetActiveChildInternal(index);
#else
			SetActiveChildInternal(index);
#endif
	}

	private void SetActiveChildInternal(int index)
	{
		var numChilds = transform.childCount;
		if (index >= numChilds)
			index = 0;
		if (index < 0)
			index = numChilds - 1;
		ActiveChild = index;
		Percentage = (float)index/(numChilds-1);
		TimeInfo = string.Format("{0:00}", index);

		ElapsedTime = 0f;

		var i = 0;
		var transforms = Order == Ordering.Alphanumeric
			? transform.Cast<Transform>().ToArray().OrderBy(t => t.name, new AlphanumComparatorFast()).ToArray()
			: transform.Cast<Transform>().ToArray();

		ActiveChildGo = transforms[index].gameObject;
		if (!_active)
			return;

		foreach (var child in transforms)
		{
			var enable = i == index || index < 0;
			SetVisible(child, enable);
			++i;
		}

		if (ActiveChildCallback != null)
			ActiveChildCallback(ActiveChild);
	}

	protected void SetVisible(Transform currentTransform, bool enable)
	{
		var objectSwitch = currentTransform.GetComponent<ObjectSwitchBase>();

		if (objectSwitch != null && enable)
		{
			objectSwitch.SetActiveChild(objectSwitch.ActiveChild);
			return;
		}
		foreach (var ren in currentTransform.GetComponents<Renderer>())
			ren.enabled = enable;

		for (var i = 0; i < currentTransform.childCount; i++)
			SetVisible(currentTransform.GetChild(i), enable);
	}

	public void NoActiveChild()
	{
		foreach (var ren in transform.GetComponentsInChildren<Renderer>(true))
				ren.enabled = false;
		ElapsedTime = 0f;
	}

	public override void Begin()
	{
		SetActiveChild(0);
	}

	public override void End()
	{
		SetActiveChild(transform.childCount - 1);
	}
}
