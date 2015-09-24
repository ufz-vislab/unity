using UnityEngine;
using FullInspector;

namespace UFZ.Rendering
{
	[fiInspectorOnly]
	public class MaterialPropertiesHelper : MonoBehaviour
	{
		[InspectorComment("This will apply the selected properties to all child GameObjects!")]
		public MaterialProperties.SideMode SideMode = MaterialProperties.SideMode.Front;
		public MaterialProperties.LightingMode LightingMode = MaterialProperties.LightingMode.Lit;
		public MaterialProperties.ColorMode ColorMode = MaterialProperties.ColorMode.SolidColor;

		[InspectorButton]
		public void Apply()
		{
			var matProps = gameObject.GetComponentsInChildren<MaterialProperties>();
			if (matProps.Length == 0)
			{
				Debug.LogWarning("No MaterialProperties found!");
				return;
			}

			foreach (var matProp in matProps)
			{
				matProp.Side = SideMode;
				matProp.ColorBy = ColorMode;
				matProp.Lighting = LightingMode;
			}
		}
	}
}