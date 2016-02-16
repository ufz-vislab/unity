using System;
using UnityEngine;
using MarkUX;
using System.Collections.Generic;
using UFZ.Misc;
using UFZ.Rendering;
using MarkUX.Views;

[InternalView]
public class VisibilityView : View
{
	public List<VisibilityStruct> Objects;
	//public int AdditionalInfo;

	protected GameObjectList GoList;

	public override void Initialize()
	{
		base.Initialize();

		Objects = new List<VisibilityStruct>();

		var objectVisibilities = FindObjectsOfType<UFZ.Interaction.ObjectVisibility>();
		foreach (var objectVisibility in objectVisibilities)
		{
			objectVisibility.RestoreState();
			if (objectVisibility.Entries == null || objectVisibility.Entries.Length == 0)
				continue;

			var index = 0;
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
					Index = index,
					MatProps = materialProperties
				});
				index++;
			}
		}

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

	public void EnabledClick(CheckBox source)
	{
		var visibilityStruct = Objects[int.Parse(source.Id) - 1];

		foreach (var matProp in visibilityStruct.MatProps)
			matProp.SetEnabled(visibilityStruct.Enabled);
	}

	public void OpacityChanged(Slider source)
	{
		var visibilityStruct = Objects[int.Parse(source.Id) - 1];
		Debug.Log("Opacity: " + visibilityStruct.Opacity);
		foreach (var matProp in visibilityStruct.MatProps)
			matProp.SetOpacity(visibilityStruct.Opacity);
	}

	public void TextClick(HyperLink source)
	{
		// TODO: does not sync checkbox
		var visibilityStruct = Objects[int.Parse(source.Id) - 1];
		visibilityStruct.Enabled = !visibilityStruct.Enabled;
		SetChanged(() => Objects);

		foreach (var matProp in visibilityStruct.MatProps)
			matProp.SetEnabled(visibilityStruct.Enabled);
	}
}

public class VisibilityStruct
{
	public string Name;
	public float Opacity;
	public bool Enabled;
	public int Index;
	public MaterialProperties[] MatProps;
}
