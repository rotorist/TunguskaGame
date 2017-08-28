using UnityEngine;

public class FireLightScript : MonoBehaviour
{
	public float minIntensity = 0.25f;
	public float maxIntensity = 0.5f;

	public Light fireLight;

	float random;

	void Update()
	{
		if(GameManager.Inst == null || !GameManager.Inst.PlayerControl.IsGamePaused)
		{
			random = Random.Range(0.0f, 150.0f);
			float noise = Mathf.PerlinNoise(random, Time.time);
			fireLight.GetComponent<Light>().intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
			//fireLight.GetComponent<Light>().intensity = UnityEngine.Random.Range(minIntensity, maxIntensity);
		}
	}
}