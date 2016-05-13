using System.Collections;
using MarkLight;
using ModestTree;
using UFZ.Initialization;
using UFZ.IOC;
using Zenject;

public class InjectionManager : View
{
	[ChangeHandler("InstallerChanged")]
	public UfzInstaller UfzInstaller;
	public SceneCompositionRoot SceneCompositionRoot;
	public Core Core;

	protected virtual void InstallerChanged()
	{
		SceneCompositionRoot.OnlyInjectWhenActive = true;
		SceneCompositionRoot.Installers = new MonoInstaller[1];
		SceneCompositionRoot.Installers[0] = UfzInstaller;
	}
}
