using UnityEngine;
using System.Collections;
using ModestTree.Zenject;

namespace UFZ.IOC
{
	// Should be added to 'Composition Root" object
	public class Core : UFZ.Misc.Singleton<Core>
	{
		protected Core () {}

		[Inject]
		public ITime Time;

		[Inject]
		public ILogger Log;

		[Inject]
		public IKeyboard Keyboard;

		[Inject]
		public IMouse Mouse;
	}
}
