using UnityEngine;
using System.Collections;

public class FlashLight : MonoBehaviour 
{
	public MeshRenderer Body;
	public MeshRenderer LightCone;
	public Light Light;
	//public Light SecondaryLight;
	public bool IsOn;

	public void Toggle(bool isOn)
	{
		//LightCone.enabled = false;
		Light.enabled = isOn;
		if(isOn)
		{
			Body.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		}
		else
		{
			Body.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
		}
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
		if(isOn)
		{
			Body.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		}
		else
		{
			Body.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
		}
		//SecondaryLight.enabled = isOn;
		IsOn = isOn;
		if(LightCone != null)
		{
			GameObject.Destroy(LightCone.gameObject);
		}
	}

}
