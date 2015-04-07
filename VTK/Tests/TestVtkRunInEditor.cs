using UnityEngine;

namespace UFZ.Tests
{
	public class TestVtkRunInEditor : MonoBehaviour
	{
		public int resolution = 8;
		private int oldResolution;

		private Kitware.VTK.vtkSphereSource SphereSource;
		private UFZ.VTK.VtkToUnity vtkToUnity = null;

		private void Start()
		{
			Generate();
		}

		[ContextMenu("Reset Vtk")]
		private void Reset()
		{
			DestroyImmediate(vtkToUnity.gameObject);
			vtkToUnity = null;
		}

		[ContextMenu("Generate Vtk")]
		private void Generate()
		{
			if (SphereSource == null)
				SphereSource = Kitware.VTK.vtkSphereSource.New();
			if (vtkToUnity == null)
			{
				vtkToUnity = new UFZ.VTK.VtkToUnity(SphereSource, "VTK Run In Editor");
				vtkToUnity.ColorBy(Color.green);
			}
			SphereSource.SetPhiResolution(resolution);
			SphereSource.SetThetaResolution(resolution);
			SphereSource.SetRadius(1);
			SphereSource.Update();

			vtkToUnity.Update();
		}

		private void Update()
		{
			if (resolution != oldResolution)
			{
				SphereSource.SetPhiResolution(resolution);
				SphereSource.SetThetaResolution(resolution);
				vtkToUnity.Update();
				oldResolution = resolution;
			}
		}
	}
}
