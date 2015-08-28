using Zenject;
using UFZ.IOC;

namespace UFZ.Initialization
{
	/// <summary>
	/// IOC-bindings for MiddleVR.
	/// </summary>
	/// Is part of VRBase scene.
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
			Container.Bind<IInput>().ToSingle<UnityInput>();
			Container.Bind<IEnvironment>().ToSingle<UnityEnvironment>();
#else
			Container.Bind<ITime>().ToSingle<MiddleVrTime>();
			Container.Bind<IKeyboard>().ToSingle<MiddleVrKeyboard>();
			Container.Bind<IMouse>().ToSingle<MiddleVrMouse>();
			Container.Bind<ILogger>().ToSingle<MiddleVrLogger>();
			Container.Bind<IInput>().ToSingle<MiddleVrInput>();
			Container.Bind<IEnvironment>().ToSingle<MiddleVrEnvironment>();
#endif
		}
	}
}
