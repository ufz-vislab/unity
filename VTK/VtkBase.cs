#if UNITY_STANDALONE_WIN
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UFZ.VTK
{
#if UNITY_EDITOR
	[InitializeOnLoad]
#endif
	public class VtkBase : SerializedMonoBehaviour
	{
#if UNITY_EDITOR
		static VtkBase()
#else
		protected virtual void Awake()
#endif
		{
			SetDllPath.Init();
		}
	}
}
#endif