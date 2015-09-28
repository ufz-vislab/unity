using UnityEngine;
using MarkUX;
using System.Collections.Generic;
using UFZ.Misc;
using UFZ.Rendering;

[InternalView]
public class VisibilityView : View
{
	public List<VisibilityStruct> Objects;

	protected GameObjectList GoList;

	public override void Initialize()
	{
		base.Initialize();

		Objects = new List<VisibilityStruct>();
		/*
		var objectVisibilities = FindObjectsOfType<UFZ.Interaction.ObjectVisibility>();
		foreach (var objectVisibility in objectVisibilities)
		{
			objectVisibility.RestoreState();
			if (objectVisibility.Entries == null || objectVisibility.Entries.Length == 0)
				continue;

			foreach (var objectVisibilityInfo in objectVisibility.Entries)
			{
				var materialProperties = objectVisibilityInfo.GameObject.GetComponentsInChildren<MaterialProperties>();
				if (materialProperties == null || materialProperties.Length == 0)
					continue;

				Objects.Add(new VisibilityStruct
				{
					Name = objectVisibilityInfo.GameObject.name,
					Opacity = materialProperties[0].Opacity,
					Enabled = materialProperties[0].Enabled,
					MatProps = materialProperties
				});
			}
		}
		*/
		var go = GameObject.Find("Visibilities");
		if (go == null)
			return;
		GoList = go.GetComponentInChildren<GameObjectList>();

		UpdateVisibilities();
	}

	private void UpdateVisibilities()
	{
		if (GoList == null)
			return;
		if (GoList.Objects == null)
			return;

		Objects.Clear();
		foreach (var o in GoList.Objects)
		{
			if (o == null)
				continue;

			var materialProperties = o.GetComponentsInChildren<MaterialProperties>();
			if (materialProperties == null || materialProperties.Length == 0)
				continue;

			foreach (var materialPropertiese in materialProperties)
			{
				materialPropertiese.RestoreState();
			}
			Objects.Add(new VisibilityStruct
			{
				Name = o.name,
				Opacity = materialProperties[0].Opacity,
				Enabled = materialProperties[0].Enabled,
				MatProps = materialProperties
			});
		}
		SetChanged(() => Objects);
	}

	public void EnabledHandler()
	{
		if (!Enabled)
			return;

		// TODO Does not work yet
		//UpdateVisibilities();
	}

	public void Update()
	{
		// TODO: Update only when UI has changed
		if (!Enabled)
			return;

		foreach (var visibilityStruct in Objects)
		{
			foreach (var matProp in visibilityStruct.MatProps)
			{
				matProp.RestoreState();
				matProp.Opacity = visibilityStruct.Opacity;
				matProp.Enabled = visibilityStruct.Enabled;
			}
		}
	}
}

public class VisibilityStruct
{
	public string Name;
	public float Opacity;
	public bool Enabled;
	public MaterialProperties[] MatProps;
}
