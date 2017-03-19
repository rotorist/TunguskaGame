using UnityEngine;
using System.Collections;

public class FootKickCollider : MonoBehaviour 
{
	public Character Attacker;
	public bool IsActive;


	public void SetActive(bool isActive)
	{
		transform.GetComponent<BoxCollider>().enabled = isActive;
		IsActive = isActive;
	}

	void OnCollisionEnter(Collision collision) 
	{
		if(!IsActive)
		{
			return;
		}

		Character hitCharacter = collision.collider.GetComponent<Character>();
		Vector3 pos = collision.contacts[0].point;
		Vector3 normal = collision.contacts[0].normal;

		//Debug.Log("KICKING hit collider is " + collision.collider.name);
		if(hitCharacter == Attacker)
		{
			return;
		}
			

		if(hitCharacter != null && Attacker.MyAI.IsCharacterFriendly(hitCharacter))
		{
			//return;
		}

		if(hitCharacter != null)
		{
			//Debug.Log("Kicked somebody! " + hitCharacter.name);
			Vector3 fakeNormal = (hitCharacter.transform.position - Attacker.transform.position).normalized;
			hitCharacter.SendMeleeDamage(new Damage(), fakeNormal, Attacker, 1);


		}
	}
}
