using UnityEngine;
using System.Collections;

public class FlashLight : MonoBehaviour 
{
	public MeshRenderer LightCone;
	public Light Light;
	public bool IsOn;

	public void Toggle(bool isOn)
	{
		LightCone.enabled = isOn;
		Light.enabled = isOn;
		IsOn = isOn;
	}


}
