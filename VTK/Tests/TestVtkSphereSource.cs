using UnityEngine;

public class VtkSphereSourceWrapper : MonoBehaviour
{
	public vrCommand SetRadiusCommand;
	public Kitware.VTK.vtkSphereSource Source;

	public VtkSphereSourceWrapper()
	{
		SetRadiusCommand = new vrCommand("Set Radius Command - ", SetRadius);
	}

	public double GetRadius()
	{
		return Source.GetRadius();
	}

	private vrValue SetRadius(vrValue iValue)
	{
		Source.SetRadius(iValue.GetDouble());
		return null;
	}
}

/*
 * Creates a vtkSphereSource and converts it to a Unity GameObject.
 * Pressing up/down keyboard arrows changes the sphere resolution.
 *
 **/
public class TestVtkSphereSource : MonoBehaviour
{
	Kitware.VTK.vtkSphereSource _sphereSource;
	UFZ.VTK.VtkToUnity _vtkToUnity;

	void Start()
	{
		_sphereSource = Kitware.VTK.vtkSphereSource.New();
		_sphereSource.SetRadius(1);
		_sphereSource.Update();
		var wrapper = gameObject.AddComponent<VtkSphereSourceWrapper>();
		wrapper.Source = _sphereSource;

		_vtkToUnity = new UFZ.VTK.VtkToUnity(_sphereSource, "VTK Sphere Source");
		_vtkToUnity.ColorBy(Color.grey);
		_vtkToUnity.Update();
		_vtkToUnity.gameObject.transform.parent = transform;
		_vtkToUnity.gameObject.transform.localPosition = new Vector3();
	}
}
