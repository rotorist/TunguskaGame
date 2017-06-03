using UnityEngine;
using System.Collections;

public class ScrollUV : MonoBehaviour {
	public Vector2 direction = new Vector2(1,0);
	public float   speed = 1;
	
	void Update () 
	{
		Renderer renderer = GetComponent<Renderer>();
		renderer.material.mainTextureOffset += direction * speed * Time.deltaTime;
	}
}
