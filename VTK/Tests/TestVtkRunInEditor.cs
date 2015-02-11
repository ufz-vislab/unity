using UnityEngine;

public class TestVtkRunInEditor : MonoBehaviour
{
	public int resolution = 8;
	int oldResolution;

	Kitware.VTK.vtkSphereSource SphereSource;
	UFZ.VTK.VtkToUnity vtkToUnity = null;

	void Start()
	{
		Generate();
	}

	[ContextMenu("Reset Vtk")]
	void Reset()
	{
		DestroyImmediate(vtkToUnity.gameObject);
		vtkToUnity = null;
	}

	[ContextMenu("Generate Vtk")]
	void Generate()
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

	void Update()
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