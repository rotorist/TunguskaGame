using UnityEngine;
using System.Collections;

public class FlashLight : MonoBehaviour 
{
	public MeshRenderer LightCone;
	public Light Light;
	//public Light SecondaryLight;
	public bool IsOn;

	public void Toggle(bool isOn)
	{
		//LightCone.enabled = false;
		Light.enabled = isOn;
		IsOn = isOn;
		if(LightCone != null)
		{
			GameObject.Destroy(LightCone.gameObject);
		}
	}

	public void TogglePlayerLight(bool isOn)
	{
		//LightCone.enabled = false;
		Light.enabled = isOn;
		//SecondaryLight.enabled = isOn;
		IsOn = isOn;
		if(LightCone != null)
		{
			GameObject.Destroy(LightCone.gameObject);
		}
	}

}
