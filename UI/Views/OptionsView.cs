using MarkLight.Views.UI;
using UFZ.Interaction;
using UnityEngine;
using UFZ.Rendering;

//[InternalView]
public class OptionsView : UIView
{
	public void StereoEnabledClick(CheckBox source)
	{
		var eye = FindObjectOfType<EyeDistance>();
		eye.Distance = source.IsChecked ? 0.063f : 0f;
	}

	public void SuperelevatedButtonClick()
	{
		// var simAnim = FindObjectOfType<BathySimAnimation>();
		// simAnim.Toggle();
	}

	public void TemperatureButtonClick()
	{
		var sim = GameObject.Find("Sim Transform");
		var temp = sim.transform.Find("Temperature");
		var vel = sim.transform.Find("Velocity");

		temp.gameObject.SetActive(true);
		vel.gameObject.SetActive(false);
	}

	public void VelocityButtonClick()
	{
		var sim = GameObject.Find("Sim Transform");
		var temp = sim.transform.Find("Temperature");
		var vel = sim.transform.Find("Velocity");

		temp.gameObject.SetActive(false);
		vel.gameObject.SetActive(true);
	}

	public void SateliteButtonClick()
	{
		var rappbode = GameObject.Find("Rappbode-Surface");
		var rappProps = rappbode.GetComponent<MaterialProperties>();
		if (rappProps.ColorBy == MaterialProperties.ColorMode.VertexColor)
			rappProps.ColorBy = MaterialProperties.ColorMode.Texture;
		else
			rappProps.ColorBy = MaterialProperties.ColorMode.VertexColor;

		var koenig = GameObject.Find("Koenigshuette-Surface");
		var koenigProps = koenig.GetComponent<MaterialProperties>();
		if (koenigProps.ColorBy == MaterialProperties.ColorMode.VertexColor)
			koenigProps.ColorBy = MaterialProperties.ColorMode.Texture;
		else
			koenigProps.ColorBy = MaterialProperties.ColorMode.VertexColor;
	}

	public void OrientationButtonClick()
	{
		var player = GameObject.Find("Player (Player)").GetComponent<Player>();
#if MVR
		player.ResetRotation(0);
#endif
	}
}
