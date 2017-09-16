using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnterableBuilding : MonoBehaviour 
{
	public int TopLevel;
	public List<BuildingComponent> Components;

	private bool _revealAll;
	private float _revealTimer;
	private bool _active;

	public bool IsActive 
	{
		get { return _active;}
	}

	public void NotifyHidingComponent(BuildingComponent component, float playerY)
	{
		int level = GetCurrentLevel(playerY);
		
		foreach(BuildingComponent c in Components)
		{
			//first level always shows
			if(c.Level == 0)
			{
				continue;
			}

			if(level >= c.Level)
			{
				RevealComponent(c, true);
			}
			else
			{
				if(_active)
				{
					HideComponent(c, true);
				}
				else
				{
					HideComponent(c, false);
				}
			}
		}
		
		/*
		if(component != null && playerY < component.YMin && !component.IsHidden)
		{
			if(_active)
			{
				HideComponent(component, true);
			}
			else
			{
				HideComponent(component, false);
			}
		}

		//reveal or hide other components
		foreach(BuildingComponent c in Components)
		{
			//Debug.Log("Checking building revealing component " + c.name);
			if(playerY > c.YMin && c.IsHidden)
			{
				
				RevealComponent(c, true);
			}
			else if(playerY < c.YMin && !c.IsHidden)
			{
				if(_active)
				{
					HideComponent(c, true);
				}
				else
				{
					HideComponent(c, false);
				}
			}
				
		}
		*/

		_revealTimer = 0;
		_active = true;
	}

	public int GetCurrentLevel(float y)
	{
		int level = 0;
		foreach(BuildingComponent component in Components)
		{
			if(y > component.YThreshold && component.Level >= level)
			{
				level = component.Level;
			}
		}

		return level;
	}

	public int GetCurrentNPCLevel(float y)
	{
		int level = 0;
		foreach(BuildingComponent component in Components)
		{
			if(y > component.YThresholdNPC && component.Level >= level)
			{
				level = component.Level;
			}
		}

		return level;
	}

	void Update()
	{
		//run reveal timer
		if(_active)
		{
			if(_revealTimer > 0.2f)
			{
				RevealAll();
				_active = false;
			}

			_revealTimer += Time.deltaTime;
		}


	}


	private void HideComponent(BuildingComponent component, bool isInstant)
	{
		if(component.IsHidden)
		{
			return;
		}

		Debug.Log("hiding component " + component.name);
		MeshRenderer renderer = component.GetComponent<MeshRenderer>();
		if(renderer != null)
		{
			//renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
			component.gameObject.layer = LayerMask.NameToLayer("IgnorePlayerRaycast");
			component.IsHidden = true;
			GameManager.Inst.MaterialManager.StartFadingMaterial(renderer, isInstant, true, 4);
		}

		Transform [] objects1 = component.transform.GetComponentsInChildren<Transform>();
		foreach(Transform t in objects1)
		{
			renderer = t.GetComponent<MeshRenderer>();
			if(renderer != null && renderer.gameObject != component.gameObject)
			{
				//renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
				renderer.gameObject.layer = LayerMask.NameToLayer("HiddenObjects");
				renderer.enabled = false;
			}

			Transform [] objects2  = t.transform.GetComponentsInChildren<Transform>();
			foreach(Transform t2 in objects2)
			{
				renderer = t2.GetComponent<MeshRenderer>();
				if(renderer != null && renderer.gameObject != component.gameObject)
				{
					//renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
					renderer.gameObject.layer = LayerMask.NameToLayer("HiddenObjects");
					renderer.enabled = false;
				}
			}
		}
	}

	private void RevealComponent(BuildingComponent component, bool isInstant)
	{
		if(!component.IsHidden)
		{
			return;
		}

		Debug.Log("revealing component " + component.name);
		MeshRenderer renderer = component.GetComponent<MeshRenderer>();
		if(renderer != null)
		{
			//renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
			component.gameObject.layer = LayerMask.NameToLayer("BuildingComponent");

			GameManager.Inst.MaterialManager.StartUnfadingMaterial(renderer, isInstant, true, 1);
		}

		Transform [] objects1 = component.transform.GetComponentsInChildren<Transform>();
		foreach(Transform t in objects1)
		{
			renderer = t.GetComponent<MeshRenderer>();
			if(renderer != null && renderer.gameObject != component.gameObject)
			{
				//renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
				renderer.gameObject.layer = LayerMask.NameToLayer("Default");
				renderer.enabled = true;
			}

			Transform [] objects2  = t.transform.GetComponentsInChildren<Transform>();
			foreach(Transform t2 in objects2)
			{
				renderer = t2.GetComponent<MeshRenderer>();
				if(renderer != null && renderer.gameObject != component.gameObject)
				{
					//renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
					renderer.gameObject.layer = LayerMask.NameToLayer("Default");
					renderer.enabled = true;
				}
			}
		}

		component.IsHidden = false;
	}

	private void RevealAll()
	{
		foreach(BuildingComponent c in Components)
		{
			RevealComponent(c, false);
		}
	}


}
