namespace UFZ.Interaction
{
	public class WandNavigation : NavigationBase
	{
		void Start()
		{
			DeadZone = 0.05f;
		}

		protected override void GetInputs()
		{
			var script = VrMgr.GetComponent<VRManagerScript>();
			if(script == null)
				return;

			Forward = script.WandAxisVertical;
			HorizontalRotation = script.WandAxisHorizontal;
		}
	}
}
