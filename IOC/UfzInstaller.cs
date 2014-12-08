using ModestTree.Zenject;
using UnityEngine;
using MiddleVR_Unity3D;
using UFZ.IOC;

public class UfzInstaller : MonoInstaller
{
	public override void InstallBindings()
	{
		//Container.Bind<IDependencyRoot>().ToSingle<DependencyRootStandard>();
		//Container.Bind<IInstaller>().ToSingle<StandardUnityInstaller>();
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		Container.Bind<ITime>().ToSingle<UnityTime>();
		Container.Bind<IKeyboard>().ToSingle<UnityKeyboard>();
		Container.Bind<IMouse>().ToSingle<UnityMouse>();
		Container.Bind<ILogger>().ToSingle<UnityLogger>();
#else
		Container.Bind<ITime>().ToSingle<MiddleVrTime>();
		Container.Bind<IKeyboard>().ToSingle<MiddleVrKeyboard>();
		Container.Bind<IMouse>().ToSingle<MiddleVrMouse>();
		Container.Bind<ILogger>().ToSingle<MiddleVrLogger>();
#endif
	}
}
