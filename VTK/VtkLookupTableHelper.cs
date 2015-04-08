using Kitware.VTK;

namespace UFZ.VTK
{
	public enum LutPreset
	{
		BlueRed,
		RedBlue,
		Rainbow
	}

	public class VtkLookupTableHelper
	{
		public static vtkLookupTable Create(LutPreset preset, double rangeMin, double rangeMax)
		{
			var lut = new vtkLookupTable();
			lut.SetTableRange(rangeMin, rangeMax);
			switch (preset)
			{
				case LutPreset.BlueRed:
					lut.SetHueRange(0.66, 1.0);
					lut.SetNumberOfColors(128);
					break;
				case LutPreset.RedBlue:
					lut.SetHueRange(1.0, 0.66);
					lut.SetNumberOfColors(128);
					//lut.SetNumberOfTableValues(2);
					//lut.SetTableValue(0, 1.0, 0.0, 0.0, 1.0);
					//lut.SetTableValue(1, 0.0, 0.0, 1.0, 1.0);
					break;
				case LutPreset.Rainbow:
					lut.SetHueRange(0.0, 0.66);
					lut.SetNumberOfColors(256);
					break;
			}
			lut.Build();

			return lut;
		}
	}
}
