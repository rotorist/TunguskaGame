using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleAnomaly : MonoBehaviour 
{
	public float RadiusLow;
	public float RadiusHigh;
	public float ExpandSpeed;
	public AudioSource Audio;

	private float _currentRadiusTarget;

	private float _reEnableRendererTimer;
	private Renderer _myRenderer;
	
	// Update is called once per frame
	void Update () 
	{
		if(transform.localScale.x < _currentRadiusTarget - 0.1f && _reEnableRendererTimer > 2)
		{
			transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(_currentRadiusTarget, _currentRadiusTarget, _currentRadiusTarget), Time.deltaTime * ExpandSpeed);
		}
		else
		{
			//set new target and shrink back to low (pop the bubble)
			transform.localScale = new Vector3(RadiusLow, RadiusLow, RadiusLow);
			_currentRadiusTarget = UnityEngine.Random.Range(RadiusLow, RadiusHigh);

		}

		if(_reEnableRendererTimer < 2)
		{
			_reEnableRendererTimer += Time.deltaTime;
		}
		else
		{
			if(_myRenderer != null)
			{
				_myRenderer.enabled = true;
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Projectile")
		{
			return;
		}

		Character character = other.GetComponent<Character>();

		if(character != null && character.MyStatus.Health > 0)
		{
			Damage damage = new Damage();
			damage.Type = DamageType.Explosive;
			damage.BlastDamage = 40;
			damage.IsCritical = true;
			character.SendDamage(damage, (other.transform.position - transform.position).normalized, null, null);
			if(character.MyAI.ControlType == AIControlType.Player)
			{
				GameManager.Inst.CameraShaker.TriggerScreenShake(0.2f, 0.8f);
			}
			Explode(other.transform.position);
		}
		else
		{
			//destroy the object if it contains rigidbody
			if(other.GetComponent<Rigidbody>() != null)
			{
				Explode(other.transform.position);
				GameObject.Destroy(other.gameObject);
			}
		}
	}

	private void Explode(Vector3 loc)
	{
		transform.localScale = new Vector3(RadiusLow, RadiusLow, RadiusLow);
		_myRenderer = GetComponent<Renderer>();
		_myRenderer.enabled = false;
		_reEnableRendererTimer = 0;
		GameObject explosion = GameObject.Instantiate(Resources.Load("BubbleAnomalyExplosion")) as GameObject;
		explosion.transform.position = loc;
		Audio.PlayOneShot(GameManager.Inst.SoundManager.GetClip("airburst2"), 0.55f);


	}
}
