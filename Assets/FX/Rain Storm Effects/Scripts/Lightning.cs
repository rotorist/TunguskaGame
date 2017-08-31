using UnityEngine;
using System.Collections;

public class Lightning : MonoBehaviour{
	
	public float offMin = 5.0f;
	public float offMax = 30.0f;
	public float onMin = 0.05f;
	public float onMax = 0.1f;
	public GameObject lightning;
	public AudioClip[] Thunder;
	public Light LightningLight;
	public Vector3 LightningCenter;
	public float LightningRange;
	
	void OnEnable()
	{
		LightningCenter = lightning.transform.position;
		StartCoroutine(lighter());
	}

	void Update()
	{
		LightningLight.intensity = UnityEngine.Random.Range(0f, 2f);
	}
	
	IEnumerator lighter(){
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(offMin, offMax));
			lightning.transform.position = new Vector3(UnityEngine.Random.Range(LightningCenter.x - LightningRange, LightningCenter.x + LightningRange),
														LightningCenter.y,
														UnityEngine.Random.Range(LightningCenter.z - LightningRange, LightningCenter.z + LightningRange));
			lightning.SetActive(true);
			yield return new WaitForSeconds(Random.Range(0.25f, 0.75f));
			GetComponent<AudioSource>().PlayOneShot(Thunder[Random.Range(0, Thunder.Length)]);
			yield return new WaitForSeconds(Random.Range(onMin, onMax));
			lightning.SetActive(false);
		}
	}
}