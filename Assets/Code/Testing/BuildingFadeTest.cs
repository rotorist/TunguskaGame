using UnityEngine;
using System.Collections;

public class BuildingFadeTest : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
	{
		//_mesh = this.gameObject.GetComponent<MeshFilter>().mesh;
	}

	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.I))
		{
			SetOpacity(0.5f);
		}
	}

	protected void SetOpacity (float opacity) 
	{
		Material material = gameObject.GetComponent<Renderer>().material;

		Color color = material.color;
		color = new Color(color.a - 0.1f, color.r, color.g, color.b);
		material.color = color;
	}
}
