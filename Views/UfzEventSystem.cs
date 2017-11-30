using MarkLight;
using UFZ.Input;
using UFZ.Setup;
using UnityEngine;

[ExcludeComponent("UnityEventSystem")]
public class UfzEventSystem : MarkLight.Views.EventSystem
{
	public WandInputModule WandInputModule;

	public void Awake()
	{
		UFZ.IOC.Core.Instance.Log.Info("UfzEventSystem: Creating cursor");
		var cursor = SRResources.WandCursor.Instantiate();
		cursor.transform.SetParent(transform, false);
		WandInputModule.cursor = cursor.GetComponent<RectTransform>();

		var globalInits = FindObjectOfType<GlobalInits>();
		if (globalInits.GuiInputType == GlobalInits.InputType.Mouse)
		{
			UFZ.IOC.Core.Instance.Log.Info("UfzEventSystem: Mouse input, disabling cursor");
			WandInputModule.enabled = false;
			WandInputModule.cursor.gameObject.SetActive(false);
		}
	}
}
