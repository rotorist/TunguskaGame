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
	private Dictionary<Renderer, float> _fadeInSpeed;
	private Dictionary<Renderer, float> _fadeOutSpeed;


	public void Initialize()
	{
		_alteredRenderers = new List<Renderer>();
		_savedMaterials = new Dictionary<Renderer, Material[]>();
		_fadingMaterials = new Dictionary<Renderer, Material[]>();
		_fadeInSpeed = new Dictionary<Renderer, float>();
		_fadeOutSpeed = new Dictionary<Renderer, float>();
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
						float speed = 4;
						if(_fadeOutSpeed.ContainsKey(fadingMaterial.Key))
						{
							speed = _fadeOutSpeed[fadingMaterial.Key];
						}
						float alpha = Mathf.Clamp01(m.color.a - Time.fixedDeltaTime * speed);
						if(alpha < 0.05f)
						{
							alpha = 0;
							fadingMaterial.Key.enabled = false;
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
						float speed = 1;
						if(_fadeInSpeed.ContainsKey(fadingMaterial.Key))
						{
							speed = _fadeInSpeed[fadingMaterial.Key];
						}

						float alpha = Mathf.Clamp01(m.color.a + Time.fixedDeltaTime * speed);
						if(alpha > 0.95f)
						{
							alpha = 1;
							fadingMaterial.Key.enabled = true;
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

			if(_dupeObjects.ContainsKey(r))
			{
				GameObject.Destroy(_dupeObjects[r].gameObject);
				_dupeObjects.Remove(r);
			}
			_fadingMaterials.Remove(r);
			_fadingDirections.Remove(r);
			_savedMaterials.Remove(r);
			_alteredRenderers.Remove(r);
		}
			

		//Debug.Log("altered renderes count " + _alteredRenderers.Count + ", fading count " + _fadingMaterials.Count + " unfading count " + _unfadingMaterials.Count);
	}

	public void StartFadingMaterial(Renderer renderer, bool isInstant, bool isBuilding, float speed)
	{
		if(!_alteredRenderers.Contains(renderer))
		{
			_alteredRenderers.Add(renderer);
			if(isBuilding)
			{
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

		}

		bool needToChangeRender = true;
		if(_fadingMaterials.ContainsKey(renderer))
		{
			needToChangeRender = false;
		}

		if(!_fadeOutSpeed.ContainsKey(renderer))
		{
			_fadeOutSpeed.Add(renderer, speed);
		}
		else
		{
			_fadeOutSpeed[renderer] = speed;
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

	public void StartUnfadingMaterial(Renderer renderer, bool isInstant, bool isBuilding, float speed)
	{
		if(!_alteredRenderers.Contains(renderer))
		{
			return;
		}

		if(!_fadeInSpeed.ContainsKey(renderer))
		{
			_fadeInSpeed.Add(renderer, speed);
		}
		else
		{
			_fadeInSpeed[renderer] = speed;
		}

		renderer.enabled = true;
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
