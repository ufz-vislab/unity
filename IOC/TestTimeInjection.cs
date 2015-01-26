using Zenject;
using UnityEngine;
using UFZ.IOC;

public class TestTimeInjection : MonoBehaviour
{
	[Inject]
	private ITime _time;

	[Inject]
	private IKeyboard keyboard;

	[Inject]
	private ILogger logger;
	
	void Update ()
	{
		if(keyboard.IsKeyPressed(KeyCode.A))
			logger.Info(_time.DeltaTime().ToString());
	}
}
