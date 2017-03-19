using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MaterialManager 
{


	private List<Renderer> _alteredRenderers;
	private Dictionary<Renderer, Material[]> _savedMaterials;
	private Dictionary<Renderer, Material[]> _fadingMaterials;
	private Dictionary<Renderer, GameObject> _dupeObjects;
	private Dictionary<Renderer, int> _fadingDirections;


	public void Initialize()
	{
		_alteredRenderers = new List<Renderer>();
		_savedMaterials = new Dictionary<Renderer, Material[]>();
		_fadingMaterials = new Dictionary<Renderer, Material[]>();
		_fadingDirections = new Dictionary<Renderer, int>();
		_dupeObjects = new Dictionary<Renderer, GameObject>();
	}

	public void FixedUpdate()
	{
		List<Renderer> toBeRemoved = new List<Renderer>();

		//loop through all materials that are supposed to fade in/out
		foreach(KeyValuePair<Renderer, Material[]> fadingMaterial in _fadingMaterials)
		{
			if(_fadingDirections[fadingMaterial.Key] < 0)
			{
				foreach(Material m in fadingMaterial.Value)
				{
					if(m.color.a > 0f)
					{
						float alpha = Mathf.Clamp01(m.color.a - Time.fixedDeltaTime * 4);
						if(alpha < 0.05f)
						{
							alpha = 0;

						}

						m.color = new Color(m.color.r, m.color.g, m.color.b, alpha);
						fadingMaterial.Key.materials = fadingMaterial.Value;
					}
					else
					{
						//fadingMaterial.Key.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
						//m.color = new Color(m.color.r, m.color.g, m.color.b, 1);
						_fadingDirections[fadingMaterial.Key] = 0;
					}
				}
			}
			else if(_fadingDirections[fadingMaterial.Key] > 0)
			{
				foreach(Material m in fadingMaterial.Value)
				{
					if(m.color.a < 1)
					{
						float alpha = Mathf.Clamp01(m.color.a + Time.fixedDeltaTime * 1);
						if(alpha > 0.95f)
						{
							alpha = 1;

						}
						//Debug.Log(alpha);
						m.color = new Color(m.color.r, m.color.g, m.color.b, alpha);
						fadingMaterial.Key.materials = fadingMaterial.Value;
					}
					else
					{
						//if(fadingMaterial.Key.shadowCastingMode != UnityEngine.Rendering.ShadowCastingMode.On)
						{
							//switch back to saved material (shared) and destroy instanced material
							fadingMaterial.Key.materials = _savedMaterials[fadingMaterial.Key];

							if(!toBeRemoved.Contains(fadingMaterial.Key))
							{
								toBeRemoved.Add(fadingMaterial.Key);
							}
							//fadingMaterial.Key.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
							_fadingDirections[fadingMaterial.Key] = 0;
						}
					}

				}
			}
		}

		foreach(Renderer r in toBeRemoved)
		{
			if(_fadingMaterials.ContainsKey(r))
			{
				foreach(Material m in _fadingMaterials[r])
				{
					GameObject.Destroy(m);
				}
			}

			GameObject.Destroy(_dupeObjects[r].gameObject);
			_dupeObjects.Remove(r);
			_fadingMaterials.Remove(r);
			_fadingDirections.Remove(r);
			_savedMaterials.Remove(r);
			_alteredRenderers.Remove(r);
		}
			

		//Debug.Log("altered renderes count " + _alteredRenderers.Count + ", fading count " + _fadingMaterials.Count + " unfading count " + _unfadingMaterials.Count);
	}

	public void StartFadingMaterial(Renderer renderer, bool isInstant)
	{
		if(!_alteredRenderers.Contains(renderer))
		{
			_alteredRenderers.Add(renderer);
			//make a duplicate object to block light
			GameObject dupe = GameObject.Instantiate(renderer.gameObject, renderer.transform.parent) as GameObject;
			//remove children
			foreach(Transform child in dupe.transform)
			{
				GameObject.Destroy(child.gameObject);
			}
			dupe.transform.position = renderer.gameObject.transform.position;
			dupe.transform.localScale = renderer.transform.localScale;
			_dupeObjects.Add(renderer, dupe);
			dupe.GetComponent<Collider>().enabled = false;
			dupe.GetComponent<BuildingComponent>().enabled = false;
			dupe.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
		}

		bool needToChangeRender = true;
		if(_fadingMaterials.ContainsKey(renderer))
		{
			needToChangeRender = false;
		}



		//first save the renderer's current shared material
		if(!_savedMaterials.ContainsKey(renderer))
		{
			_savedMaterials.Add(renderer, renderer.sharedMaterials);
		}

		//create instance materials and save the reference for destruction later
		if(!_fadingMaterials.ContainsKey(renderer))
		{
			
			_fadingMaterials.Add(renderer, renderer.materials);
		}

		if(!_fadingDirections.ContainsKey(renderer))
		{
			_fadingDirections.Add(renderer, -1);
		}
		else
		{
			_fadingDirections[renderer] = -1;
		}

		if(needToChangeRender)
		{
			//for each instanced material, set shader to transparent
			foreach(Material m in _fadingMaterials[renderer])
			{
				StaticUtility.ChangeRenderMode(m, BlendMode.Fade);
				if(isInstant)
				{
					m.color = new Color(m.color.r, m.color.g, m.color.b, 0f);
				}
				else
				{
					m.color = new Color(m.color.r, m.color.g, m.color.b, 1f);
				}
			}

		}

		if(isInstant)
		{
			FixedUpdate();
		}

	}

	public void StartUnfadingMaterial(Renderer renderer, bool isInstant)
	{
		if(!_alteredRenderers.Contains(renderer))
		{
			return;
		}


		if(!_fadingDirections.ContainsKey(renderer))
		{
			_fadingDirections.Add(renderer, 1);
		}
		else
		{
			//set fading direction to false
			_fadingDirections[renderer] = 1;
			if(isInstant)
			{
				foreach(Material m in _fadingMaterials[renderer])
				{
					m.color = new Color(m.color.r, m.color.g, m.color.b, 1f);
				}
				FixedUpdate();
			}
		}

	
	}
}
