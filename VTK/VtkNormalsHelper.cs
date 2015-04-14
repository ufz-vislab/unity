using Kitware.VTK;

namespace UFZ.VTK
{
	public class VtkNormalsHelper
	{
		public static bool GetPointNormals(vtkDataSet polydata)
		{
			var normalDataDouble = vtkDoubleArray.SafeDownCast(
				polydata.GetPointData().GetArray("Normals"));
			if (normalDataDouble != null)
				return true;

			var normalDataFloat = vtkFloatArray.SafeDownCast(
				polydata.GetPointData().GetArray("Normals"));
			if (normalDataFloat != null)
				return true;

			var normalsDouble = vtkDoubleArray.SafeDownCast(
				polydata.GetPointData().GetNormals());
			if (normalsDouble != null)
				return true;

			var normalsFloat = vtkFloatArray.SafeDownCast(
				polydata.GetPointData().GetNormals());
			if (normalsFloat != null)
				return true;

			var normalsGeneric = polydata.GetPointData().GetNormals();
			if (normalsGeneric != null)
				return true;

			return false;
		}

		public static bool GetCellNormals(vtkDataSet polydata)
		{
			var normalDataDouble = vtkDoubleArray.SafeDownCast(
				polydata.GetCellData().GetArray("Normals"));
			if (normalDataDouble != null)
				return true;

			var normalDataFloat = vtkFloatArray.SafeDownCast(
				polydata.GetCellData().GetArray("Normals"));
			if (normalDataFloat != null)
				return true;

			var normalsDouble = vtkDoubleArray.SafeDownCast(
				polydata.GetCellData().GetNormals());
			if (normalsDouble != null)
				return true;

			var normalsFloat = vtkFloatArray.SafeDownCast(
				polydata.GetCellData().GetNormals());
			if (normalsFloat != null)
				return true;

			var normalsGeneric = polydata.GetCellData().GetNormals();
			if (normalsGeneric != null)
				return true;

			return false;
		}
	}
}
