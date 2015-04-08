using Kitware.VTK;
using UnityEngine;
using UFZ.VTK;

namespace UFZ.Tests
{
/*
 * Loads a vtkPolyData from an XML file (vtp).
 *
 **/

	public class TestVtkFileReader : MonoBehaviour
	{
		private void Start()
		{
			/*
			var filepath = System.IO.Path.Combine(Application.streamingAssetsPath, "VTK/Box.vtp");
				//Application.dataPath + "/" + "Vtk-Data/Box.vtp";
			//filepath = filepath.Replace("/", "\\");
			var reader = vtkXMLPolyDataReader.New();
			if (reader.CanReadFile(filepath) == 0)
			{
				Debug.Log(filepath + " could not be loaded by Vtk!");
				return;
			}
			reader.SetFileName(filepath);
			reader.Update();

			var vtkToUnity = new VtkToUnity(reader, "VTK/Box.vtp");
			vtkToUnity.ColorBy("Elevation", VtkToUnity.VtkColorType.PointData);
			vtkToUnity.SetLut(VtkToUnity.LutPreset.BlueRed);
			//vtkToUnity.ColorBy(Color.red);
			vtkToUnity.Update();
			vtkToUnity.gameObject.transform.Translate(-2f, 0f, 0f);

			var contours = vtkContourFilter.New();
			contours.SetInputConnection(vtkToUnity.TriangleFilter.GetOutputPort());
			contours.SetInputArrayToProcess(0, 0, 0, (int) Kitware.VTK.vtkDataObject.FieldAssociations.FIELD_ASSOCIATION_POINTS,
				"Elevation");
			for (int i = 0; i < 10; ++i)
				contours.SetValue(i, i/10.0);
			contours.ComputeScalarsOn();
			var vtkToUnityContours = new VtkToUnity(contours, "VTK/Box.vtp/Contours");
			vtkToUnityContours.ColorBy("Elevation", VtkToUnity.VtkColorType.PointData);
			vtkToUnityContours.SetLut(VtkToUnity.LutPreset.BlueRed);
			vtkToUnityContours.Update();
			vtkToUnityContours.gameObject.transform.Translate(-4f, 0f, 0f);

			// Points
			filepath = System.IO.Path.Combine(Application.streamingAssetsPath, "VTK/Points.vtp");
			if (reader.CanReadFile(filepath) == 0)
			{
				Debug.Log(filepath + " could not be loaded by Vtk!");
				return;
			}
			reader.SetFileName(filepath);
			reader.Update();

			var vtkToUnityPoints = new VtkToUnity(reader, "VTK/Points.vtp");
			vtkToUnityPoints.ColorBy("Elevation", VtkToUnity.VtkColorType.PointData);
			vtkToUnityPoints.SetLut(VtkToUnity.LutPreset.RedBlue);
			vtkToUnityPoints.Update();
			vtkToUnityPoints.gameObject.transform.Translate(2f, 0f, 0f);

			// Complex
			filepath = System.IO.Path.Combine(Application.streamingAssetsPath, "VTK/sand_with_vectors.vtu");
			var gridReader = vtkXMLUnstructuredGridReader.New();
			if (gridReader.CanReadFile(filepath) == 0)
			{
				Debug.Log(filepath + " could not be loaded by Vtk!");
				return;
			}
			gridReader.SetFileName(filepath);
			gridReader.Update();

			/*
		var geometryFilter = vtkGeometryFilter.New();
		geometryFilter.SetInputConnection(gridReader.GetOutputPort());

		var triangleFilter = vtkTriangleFilter.New();
		triangleFilter.SetInputConnection(geometryFilter.GetOutputPort());

		var decimateFilter = vtkDecimatePro.New();
		decimateFilter.SetInputConnection(triangleFilter.GetOutputPort());
		decimateFilter.SetTargetReduction(0.85);

		var vtkToUnityGeometry = new VtkToUnity(decimateFilter.GetOutputPort(), "VTK/sand_with_vectors.vtu");
		vtkToUnityGeometry.ColorBy("U", VtkToUnity.VtkColorType.POINT_DATA);
		vtkToUnityGeometry.SetLut(VtkToUnity.LutPreset.BLUE_RED);
		vtkToUnityGeometry.Update();
		return;
		*/

			/*
			var contours2 = vtkContourFilter.New();
			contours2.SetInputConnection(gridReader.GetOutputPort());
			contours2.SetInputArrayToProcess(0, 0, 0, (int) Kitware.VTK.vtkDataObject.FieldAssociations.FIELD_ASSOCIATION_POINTS,
				"p");
			for (int i = 1; i < 1; ++i)
				contours2.SetValue(i, i*1000.0);
			contours2.ComputeScalarsOn();

			var vtkToUnityContours2 = new VtkToUnity(contours2, "VTK/sand_with_vectors.vtu");
			vtkToUnityContours2.ColorBy("U", VtkToUnity.VtkColorType.PointData);
			vtkToUnityContours2.SetLut(VtkToUnity.LutPreset.BlueRed);
			vtkToUnityContours2.Update();
			* */
		}
	}
}
