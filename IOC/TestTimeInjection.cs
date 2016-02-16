using Zenject;
using UnityEngine;
using UFZ.IOC;

namespace UFZ.Tests
{
	public class TestTimeInjection : MonoBehaviour
	{
#pragma warning disable 649
		[Inject] private ITime _time;

		[Inject] private IKeyboard _keyboard;

		[Inject] private UFZ.IOC.ILogger _logger;
#pragma warning restore 649

		private void Update()
		{
			if (_keyboard.IsKeyPressed(KeyCode.A))
				_logger.Info(_time.DeltaTime().ToString());
		}
	}
}
