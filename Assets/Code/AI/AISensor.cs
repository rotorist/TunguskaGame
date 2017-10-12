using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AISensor 
{
	public float PersonalThreatCritical;
	public float PersonalThreatHigh;
	public float PersonalThreatLow;

	private Character _parentCharacter;
	private WorkingMemory _workingMemory;




	public void Initialize(Character parent)
	{
		_parentCharacter = parent;
		_workingMemory = _parentCharacter.MyAI.WorkingMemory;
		_parentCharacter.MyEventHandler.OnOneSecondTimer -= UpdatePerSecond;
		_parentCharacter.MyEventHandler.OnOneSecondTimer += UpdatePerSecond;

		PersonalThreatCritical = 1;
		PersonalThreatHigh = 0.6f;
		PersonalThreatLow = 0.3f;
	}


	public void UpdatePerFrame()
	{

	}

	public void UpdatePerSecond()
	{
		if(_parentCharacter.MyAI.ControlType != AIControlType.Player)
		{
			
			UpdateWorkingMemoryCharacters();

			DetectDisturbance();

			DetectExplosive();

			DetectCorpse();

			
		}


	}

	/*
	public bool GetTargetHittability(Character target)
	{
		GameObject myEyes = _parentCharacter.MyReference.Eyes;
		RaycastHit hit;
		float colliderHeight = target.GetComponent<CapsuleCollider>().height;
		Vector3 rayTarget = target.transform.position + Vector3.up * colliderHeight * 0.75f;
		Ray ray = new Ray(myEyes.transform.position, rayTarget - myEyes.transform.position);
		if(Physics.Raycast(ray, out hit))
		{
			//Debug.Log("raycast hit in sensor: " + hit.collider.name);
			Character hitCharacter = hit.collider.GetComponent<Character>();
			if(hitCharacter == target)
			{
				return true;
			}
		}
		else
		{
			return false;
		}

		return false;
	}
	*/

	public void OnTakingDamage(Character attacker)
	{
		if(_parentCharacter.MyAI.ControlType == AIControlType.Player)
		{
			return;
		}

		CsDebug.Inst.CharLog(_parentCharacter, "Taking damage! " + _parentCharacter.name);
		//ignore friendly fire
		if(_parentCharacter.MyAI.IsCharacterFriendly(attacker))
		{
			return;
		}

		WorkingMemoryFact fact = _workingMemory.FindExistingFact(FactType.PersonalThreat, attacker);
		if(fact == null)
		{

			fact = _workingMemory.AddFact(FactType.PersonalThreat, attacker, attacker.transform.position, 1, 0.1f);

			fact.ThreatLevel = PersonalThreatHigh;

			fact.ThreatDropRate = 0.1f;
		}
		else
		{
			fact.Confidence = 1;
			if(fact.ThreatLevel < PersonalThreatHigh)
			{
				fact.ThreatLevel = PersonalThreatHigh;
			}
			fact.LastKnownPos = attacker.transform.position;
		}

		//raise threat to critical if health is low
		if(_parentCharacter.MyStatus.Health <= _parentCharacter.MyStatus.MaxHealth / 3 ||
			(_parentCharacter.MyAI.BlackBoard.TargetEnemy == null && _parentCharacter.MyAI.BlackBoard.InvisibleEnemy == null))
		{
			fact.ThreatLevel = PersonalThreatCritical;
		}

		if(attacker != null && attacker.MyAI.ControlType == AIControlType.Player)
		{
			if(_parentCharacter.MyAI.IsCharacterEnemy(attacker) > 0)
			{
				//reduce relationship
				float reduction = 0.25f;
				if(_parentCharacter.MyAI.BlackBoard.TargetEnemy != null)
				{
					reduction = 0.07f;
				}
				GameManager.Inst.NPCManager.GetFactionData(Faction.Player).ReduceRelationshipByID(_parentCharacter.Faction, reduction);
				GameManager.Inst.NPCManager.GetFactionData(_parentCharacter.Faction).ReduceRelationshipByID(Faction.Player, reduction);
			}
		}

		//check if there's already a known enemy matching the shooter
		WorkingMemoryFact enemyFact = _workingMemory.FindExistingFact(FactType.KnownEnemy, attacker);
		if(enemyFact != null)
		{
			enemyFact.ThreatLevel = 1;
		}
		else
		{
			//if no existing known enemy fact from this attacker, add one
			fact = _workingMemory.AddFact(FactType.KnownEnemy, attacker, attacker.transform.position, 0.5f, 0.01f);
			fact.ThreatLevel = 1f;
			fact.ThreatDropRate = 0.01f;

			_parentCharacter.MyAI.BlackBoard.InvisibleEnemy = attacker;
			_parentCharacter.MyAI.BlackBoard.LastKnownEnemyPosition = attacker.transform.position;

			_parentCharacter.MyAI.CallForHelp(attacker);
		}

		//check if there is currently invisible enemy; if not add it
		if(_parentCharacter.MyAI.BlackBoard.TargetEnemy == null && _parentCharacter.MyAI.BlackBoard.InvisibleEnemy == null)
		{
			_parentCharacter.MyAI.BlackBoard.InvisibleEnemy = attacker;
			_parentCharacter.MyAI.BlackBoard.LastKnownEnemyPosition = attacker.transform.position;
		}

		//raise guard level
		_parentCharacter.MyAI.BlackBoard.GuardLevel = 3;
		_parentCharacter.MyAI.BlackBoard.TargetEnemyThreat = 1;

		//only raise important event when the highest personal threat is lower than new threat, in order to damp the rate of replanning
		_parentCharacter.MyAI.BlackBoard.AvgPersonalThreatDir = (fact.LastKnownPos - _parentCharacter.transform.position).normalized;
		if(fact.ThreatLevel > _parentCharacter.MyAI.BlackBoard.HighestPersonalThreat)
		{
			//Debug.Log("VERY HIGH personal threat! new: " + fact.ThreatLevel + " old: " + _parentCharacter.MyAI.BlackBoard.HighestPersonalThreat + _parentCharacter.name);
			_parentCharacter.MyAI.BlackBoard.HighestPersonalThreat = fact.ThreatLevel;

			_parentCharacter.MyAI.OnImportantEvent(1);
		}
	}

	public void OnReceiveDisturbance(float threat, object source, Vector3 location, Character sourceChar)
	{
		if(_parentCharacter.MyAI.ControlType == AIControlType.Player)
		{
			return;
		}

		if(_parentCharacter.MyAI.IsCharacterInParty(sourceChar) && threat < 1)
		{
			return;
		}
		
		//first check if the memory fact exists already
		WorkingMemoryFact fact = _workingMemory.FindExistingFact(FactType.Disturbance, source);

		if(fact == null)
		{
			//Debug.Log("adding new disturbance fact");
			//confidence drop rate depends on the threat level
			float dropRate = 0.1f - 0.05f * threat;
			fact = _workingMemory.AddFact(FactType.Disturbance, source, location, 1, dropRate);
			fact.ThreatLevel = threat;
			fact.ThreatDropRate = 0.0f;

		}
		else
		{
			//Debug.Log("Found existing disturbance memory " + _parentCharacter.name);
			fact.Confidence = 1;
			fact.LastKnownPos = location;
			fact.ThreatLevel += threat;
			if(fact.ThreatLevel > 1)
			{
				fact.ThreatLevel = 1;
			}
		}

		//if higher than old threat then trigger an important event; don't do this when there's active enemy target or personal threat
		//Debug.Log("new threat level " + fact.ThreatLevel + _parentCharacter.name);
		if(_parentCharacter.MyAI.BlackBoard.HighestDisturbanceThreat <= fact.ThreatLevel 
			&& _parentCharacter.MyAI.BlackBoard.TargetEnemy == null
			&& _parentCharacter.MyAI.BlackBoard.HighestPersonalThreat <= 0)
		{
			//Debug.Log("Received higher disturbance " + fact.ThreatLevel + ", raising important event, I am " + _parentCharacter.name);
			_parentCharacter.MyAI.BlackBoard.HighestDisturbanceThreat = fact.ThreatLevel;
			//if threat greater than 1 then use thrower's location
			if(fact.ThreatLevel < 1)
			{
				_parentCharacter.MyAI.BlackBoard.HighestDisturbanceLoc = location;
			}
			else
			{
				Vector3 destination = Vector3.zero;
				if(AI.RandomPoint(sourceChar.transform.position, new Vector3(3, 1, 3), out destination))
				{
					_parentCharacter.MyAI.BlackBoard.HighestDisturbanceLoc = destination;
				}
				else
				{
					_parentCharacter.MyAI.BlackBoard.HighestDisturbanceLoc = location;
				}
			}

			_parentCharacter.MyAI.BlackBoard.HightestDisturbanceSource = source;
			_parentCharacter.MyAI.OnImportantEvent(fact.ThreatLevel);
		}
	}

	public void OnDeath()
	{
		_parentCharacter.MyEventHandler.OnOneSecondTimer -= UpdatePerSecond;
	}







	private void UpdateWorkingMemoryCharacters()
	{
		
		//set the field of view and view range to a number for now; these will be part of char attributes
		float fov = 170;
		float range = 60;
		GameObject myEyes = _parentCharacter.MyReference.Eyes;

		foreach(Character c in GameManager.Inst.NPCManager.AllCharacters)
		{
			
			if(c == _parentCharacter || _parentCharacter.MyAI.IsCharacterEnemy(c) >= 3)
			{
				continue;
			}

			if(c.MyStatus.Health <= 0)
			{
				//ignore dead
				continue;
			}

			bool isSeen = false;


			//adjust distance according to guardlevel
			if(_parentCharacter.MyAI.BlackBoard.GuardLevel == 2)
			{
				range = Mathf.Clamp(range, 20, range);
				fov = 200;
			}
			else if(_parentCharacter.MyAI.BlackBoard.GuardLevel == 3)
			{
				range = Mathf.Clamp(range, 30, range);
				fov = 240;
			}
			else
			{
				range = c.Stealth.Visibility * _parentCharacter.MyStatus.EyeSight;
			}

			float baseRange = range;

			//add 8 to range for player
			if(c.MyAI.ControlType == AIControlType.Player)
			{
				//Debug.Log("Update working memory checking player");
				range += 8; //make a buffer zone to notify player with flashing vignette when enemy is too near
			}


			//check if within range and fov
			float distance = Vector3.Distance(c.transform.position, _parentCharacter.transform.position);
			if(distance <= 3 && _parentCharacter.MyAI.BlackBoard.GuardLevel > 2)
			{
				fov = 360;
			}


			if(distance <= range && Vector3.Angle(myEyes.transform.forward, (c.transform.position - _parentCharacter.transform.position)) <= fov / 2)
			{
				//Debug.Log(_parentCharacter.name + " sensor range/fov check passed for " + c.name);		
				//now do a raycast check if this character is behind walls. 
				RaycastHit hit;
				float colliderHeight = 0; 
				CapsuleCollider collider = c.GetComponent<CapsuleCollider>();
				if(collider.direction == 2)
				{
					colliderHeight = collider.center.y;
				}
				else if(collider.direction == 1)
				{
					colliderHeight = collider.height;
				}
				Vector3 rayTarget = c.transform.position + Vector3.up * colliderHeight * 0.7f;
				Ray ray = new Ray(myEyes.transform.position, rayTarget - myEyes.transform.position);
				//Debug.DrawRay(myEyes.transform.position, rayTarget - myEyes.transform.position, Color.red, 0.9f);
				if(Physics.Raycast(ray, out hit, 200))
				{
					//Debug.Log(_parentCharacter.name + " raycast hit in sensor: " + hit.collider.name);
					Character hitCharacter = hit.collider.GetComponent<Character>();
					if(hitCharacter != null && hitCharacter == c)
					{
						//Debug.Log("sensor ray check passed");
						if(c.MyAI.ControlType != AIControlType.Player)
						{
							
							isSeen = true;
							c.Stealth.SetDetectedVisibilityBoost(5);
						}
						else
						{
							if(distance <= baseRange)
							{
								isSeen = true;
								c.Stealth.SetDetectedVisibilityBoost(5);
								c.Stealth.AlmostDetected = 0;
							}
							else
							{
								//notify player of imminent detection
								c.Stealth.AlmostDetected = 1;
							}
						}
					}
					else
					{
						
					}
				}

			}
			else if(c.MyAI.ControlType == AIControlType.Player)
			{
				c.Stealth.AlmostDetected = 0;
			}
				
			//process each character found
			//determine confidence based on whether seen or heard
			float confidence = 0;
			if(isSeen)
			{
				confidence = 1;
			}
			else
			{
				//not seen nor heard, continue
				continue;
			}

			int relationship = _parentCharacter.MyAI.IsCharacterEnemy(c);
			if(relationship < 2 || c == _parentCharacter.Killer)
			{
				


				//add/update enemy fact
				WorkingMemoryFact fact = _workingMemory.FindExistingFact(FactType.KnownEnemy, c);

				Vector3 position = c.transform.position;

				//check if the neutral is within patrol range
				float dist = Vector3.Distance(_parentCharacter.MyAI.BlackBoard.DefensePoint, c.transform.position);
				float highThreat = 1;
				int originalGuardLevel = _parentCharacter.MyAI.BlackBoard.GuardLevel;
				//Debug.Log("AI Sensor update enemy relationship " + relationship);
				if(relationship > 0 && c != _parentCharacter.Killer)
				{
					if(dist > _parentCharacter.MyAI.BlackBoard.DefenseRadius * 2f)
					{
						highThreat = 0f;
						_parentCharacter.MyAI.BlackBoard.GuardLevel = originalGuardLevel;
					}
					else if(dist > _parentCharacter.MyAI.BlackBoard.DefenseRadius)
					{
						highThreat = 0.5f;
						if(originalGuardLevel >= 2)
						{
							_parentCharacter.MyAI.BlackBoard.GuardLevel = originalGuardLevel;
						}
						else
						{
							_parentCharacter.MyAI.BlackBoard.GuardLevel = 2;
						}
					}
					else
					{
						highThreat = 1;
						_parentCharacter.MyAI.BlackBoard.GuardLevel = 3;
					}
				}

				if(fact == null)
				{
					
					//didn't find it in working memory, create a new fact
					fact = _workingMemory.AddFact(FactType.KnownEnemy, c, c.transform.position, confidence, 0.03f);
					//Debug.LogError("adding known enemy fact " + c.Faction + " " + _parentCharacter.Faction + " " + c.name + " I am " + _parentCharacter.name);
					fact.ThreatLevel = highThreat;
					fact.ThreatDropRate = 0.03f;

				}
				else
				{
					//found it in working memory, refresh confidence level
					fact.Confidence = confidence;
					fact.LastKnownPos = c.transform.position;
					fact.ThreatLevel = highThreat;
					//Debug.Log("refreshing enemy fact");
				}

				//if current blackboard has personal threat, mark this target high threat level
				if(_parentCharacter.MyAI.BlackBoard.HighestPersonalThreat > 0)
				{
					//Debug.Log("Since there's  personal threat " + _parentCharacter.MyAI.BlackBoard.HighestPersonalThreat + ", marking enemy threat as high");
					fact.ThreatLevel = 1;
				}

				//if we consider this character is posing personal threat then his threat level is maxed
				float personalThreat = CheckPersonalThreat(c);
				if(personalThreat >= PersonalThreatLow)
				{
					//Debug.Log("enemy is aiming at me, marking enemy threat as high");
					fact.ThreatLevel = 1;
				}


			}
			else
			{
				//add/update friend fact
			}


		}


	}

	private float CheckPersonalThreat(Character c)
	{
		

		//check if enemy is aiming at me. Add personal threat memory.
		if(c.MyReference.CurrentWeapon != null)
		{
			if(c.MyReference.CurrentWeapon.GetComponent<Weapon>().IsRanged)
			{
				float aimAngle = Vector3.Angle(_parentCharacter.transform.position - c.transform.position, c.MyReference.CurrentWeapon.transform.forward);
				if(aimAngle < 15)
				{
					WorkingMemoryFact fact = _workingMemory.FindExistingFact(FactType.PersonalThreat, c);
					if(fact == null)
					{
						fact = _workingMemory.AddFact(FactType.PersonalThreat, c, c.transform.position, 1, 0.2f);
						fact.ThreatLevel = PersonalThreatLow;
						fact.ThreatDropRate = 0.1f;
					}
					else
					{
						fact.Confidence = 1;
						if(fact.ThreatLevel < PersonalThreatLow)
						{
							fact.ThreatLevel = PersonalThreatLow;
						}
						fact.LastKnownPos = c.transform.position;
					}

					_parentCharacter.MyAI.BlackBoard.AvgPersonalThreatDir = (fact.LastKnownPos - _parentCharacter.transform.position).normalized;
					if(PersonalThreatLow > _parentCharacter.MyAI.BlackBoard.HighestPersonalThreat)
					{
						_parentCharacter.MyAI.BlackBoard.HighestPersonalThreat = PersonalThreatLow;
					}

					return PersonalThreatLow;
				}
			}

		}



		return 0;
	}

	private void DetectDisturbance()
	{
		//go through all characters and find enemies within hearing range
		foreach(Character c in GameManager.Inst.NPCManager.AllCharacters)
		{
			if(c == _parentCharacter || _parentCharacter.MyAI.IsCharacterEnemy(c) >= 2 || c.MyStatus.Health <= 0)
			{
				continue;
			}

			float distance = Vector3.Distance(c.transform.position, _parentCharacter.transform.position);

			if(c.Stealth.NoiseLevel > distance)
			{
				
				//first check if the memory fact exists already
				WorkingMemoryFact fact = _workingMemory.FindExistingFact(FactType.Disturbance, c);
				if(fact == null)
				{
					//confidence drop rate depends on the threat level
					float dropRate = 0.1f - 0.095f * c.Stealth.NoiseThreat;
					fact = _workingMemory.AddFact(FactType.Disturbance, c, c.transform.position, 1, dropRate);
					fact.ThreatLevel = c.Stealth.NoiseThreat;
					fact.ThreatDropRate = dropRate;

				}
				else
				{
					fact.Confidence = 1;
					fact.LastKnownPos = c.transform.position;
					fact.ThreatLevel = c.Stealth.NoiseThreat;
				}
			}

		}

		//now look through all disturbance and put the highest in black board
		float oldThreat = _parentCharacter.MyAI.BlackBoard.HighestDisturbanceThreat;
		float tempThreat = 0;
		Vector3 tempLoc = Vector3.zero;
		object tempSource = null;
		List<WorkingMemoryFact> disturbances = _workingMemory.FindExistingFactOfType(FactType.Disturbance);
		foreach(WorkingMemoryFact f in disturbances)
		{
			if(f.ThreatLevel > tempThreat)
			{
				tempThreat = f.ThreatLevel;
				tempLoc = f.LastKnownPos;
				tempSource = f.Target;
			}


		}

		//Debug.Log("updating HighestDisturbance threat to " + tempThreat);
		_parentCharacter.MyAI.BlackBoard.HighestDisturbanceThreat = tempThreat;

		//if higher than old threat then trigger an important event; don't do this when there's active enemy target or personal threat
		if(tempThreat > oldThreat 
			&& _parentCharacter.MyAI.BlackBoard.TargetEnemy == null
			&& _parentCharacter.MyAI.BlackBoard.HighestPersonalThreat <= 0)
		{
			
			_parentCharacter.MyAI.BlackBoard.HighestDisturbanceLoc = tempLoc;
			_parentCharacter.MyAI.BlackBoard.HightestDisturbanceSource = tempSource;
			//Debug.Log("Found higher disturbance, raising important event " + _parentCharacter.name);
			_parentCharacter.MyAI.OnImportantEvent(tempThreat);
		}
	}

	private void DetectCorpse()
	{
		
		//skip this if there's threat
		if(_parentCharacter.MyAI.BlackBoard.InvisibleEnemy != null ||
			_parentCharacter.MyAI.BlackBoard.TargetEnemy != null)
		{
			return;
		}

		foreach(Character c in GameManager.Inst.NPCManager.AllCharacters)
		{
			if(c.MyStatus.Health > 0)
			{
				continue;
			}

			float fov = 170;
			float range = c.Stealth.Visibility;
			GameObject myEyes = _parentCharacter.MyReference.Eyes;

			bool isKnown = false;
			WorkingMemoryFact fact = _workingMemory.FindExistingFact(FactType.NearbyCorpse, c);
			if(fact != null)
			{
				isKnown = true;
			}

			float distance = Vector3.Distance(c.transform.position, _parentCharacter.transform.position);
			if(distance <= range && Vector3.Angle(myEyes.transform.forward, (c.transform.position - _parentCharacter.transform.position)) <= fov / 2 && !isKnown)
			{
				//now do a raycast check if this character is behind walls. 
				RaycastHit hit;
				float colliderHeight = c.GetComponent<CapsuleCollider>().height;
				Vector3 rayTarget = c.transform.position;
				Ray ray = new Ray(myEyes.transform.position, rayTarget - myEyes.transform.position);
				Debug.DrawRay(myEyes.transform.position, rayTarget - myEyes.transform.position);
				if(Physics.Raycast(ray, out hit))
				{
					//Debug.Log("raycast hit in sensor for corpse: " + hit.collider.name);
					Character hitCharacter = hit.collider.GetComponent<Character>();
					if(hitCharacter != null && hitCharacter == c)
					{
						Debug.Log("Found new corpse! " + _parentCharacter.name);
						fact = _workingMemory.AddFact(FactType.NearbyCorpse, c, c.transform.position, 1, 0);
						fact.ThreatLevel = 1;
						fact.ThreatDropRate = 0.1f;


						if(_parentCharacter.MyAI.BlackBoard.TargetCorpse == null)
						{
							_parentCharacter.MyAI.BlackBoard.TargetCorpse = fact;
						}
						else
						{
							if((Vector3.Distance(_parentCharacter.MyAI.BlackBoard.TargetCorpse.LastKnownPos, _parentCharacter.transform.position) <
								Vector3.Distance(c.transform.position, _parentCharacter.transform.position)) || _parentCharacter.MyAI.BlackBoard.TargetCorpse.ThreatLevel <= 0)
							{
								_parentCharacter.MyAI.BlackBoard.TargetCorpse = fact;
							}
						}

						_parentCharacter.MyAI.OnImportantEvent(0.3f);
					}

				}

			}
		}
	}

	private void DetectExplosive()
	{	
		//check if there's live explosive near me
		GameObject [] explosives = GameObject.FindGameObjectsWithTag("Explosive");
		foreach(GameObject e in explosives)
		{
			
			Explosive explosive = e.GetComponent<Explosive>();
			if(explosive != null && explosive.IsEnabled)
			{
				Vector3 dist = e.transform.position - _parentCharacter.transform.position;
				if(dist.magnitude <= 7)// && UnityEngine.Random.Range(0, 100) > 60)
				{
					WorkingMemoryFact fact = _workingMemory.FindExistingFact(FactType.PersonalThreat, e);
					if(fact == null)
					{
						fact = _workingMemory.AddFact(FactType.PersonalThreat, e, e.transform.position, 1, 0.3f);
						fact.ThreatLevel = PersonalThreatCritical;
						fact.ThreatDropRate = 0.1f;
					}
					else
					{
						fact.Confidence = 1;
						if(fact.ThreatLevel < PersonalThreatCritical)
						{
							fact.ThreatLevel = PersonalThreatCritical;
						}
						fact.LastKnownPos = e.transform.position;
					}

					_parentCharacter.MyAI.BlackBoard.AvgPersonalThreatDir = (fact.LastKnownPos - _parentCharacter.transform.position).normalized;
					if(PersonalThreatCritical > _parentCharacter.MyAI.BlackBoard.HighestPersonalThreat)
					{
						_parentCharacter.MyAI.BlackBoard.HighestPersonalThreat = PersonalThreatCritical;
					}
					Debug.Log("Detected grenade! ");
					_parentCharacter.MyAI.OnImportantEvent(1f);
				}
			}
		}
	}
}
