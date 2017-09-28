using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//this class selects from working memory a enemy target and feed to the black board
//also controls the looking direction and aiming

public class AITargeting 
{
	public AITargetingModes Mode;
	private Vector3 _lookAroundDir;

	private Character _parentCharacter;
	private float _lookAroundAngle;


	public void Initialize(Character c)
	{
		_parentCharacter = c;
		Mode = AITargetingModes.LookAhead;
		_parentCharacter.MyEventHandler.OnOneSecondTimer -= UpdatePerSecond;
		_parentCharacter.MyEventHandler.OnOneSecondTimer += UpdatePerSecond;

		_parentCharacter.MyEventHandler.OnHalfSecondTimer -= UpdatePerHalfSecond;
		_parentCharacter.MyEventHandler.OnHalfSecondTimer += UpdatePerHalfSecond;
	}

	public void OnDeath()
	{
		_parentCharacter.MyEventHandler.OnOneSecondTimer -= UpdatePerSecond;
		_parentCharacter.MyEventHandler.OnHalfSecondTimer -= UpdatePerHalfSecond;
	}


	public void UpdatePerFrame()
	{
		
		Character currentTarget = _parentCharacter.MyAI.BlackBoard.TargetEnemy;

		//update looking and aiming position
		if(_parentCharacter.CharacterType == CharacterType.Human && currentTarget != null && ((HumanCharacter)_parentCharacter).UpperBodyState == HumanUpperBodyStates.Aim 
			&& _parentCharacter.MyReference.CurrentWeapon != null)
		{
			//is aiming

			Vector3 lookPos = currentTarget.GetComponent<Character>().MyReference.Eyes.transform.position;
			_parentCharacter.LookTarget.transform.position = Vector3.Lerp(_parentCharacter.LookTarget.transform.position, lookPos, 8 * Time.deltaTime);

			Vector3 gunPos = _parentCharacter.MyReference.CurrentWeapon.transform.position;
			_parentCharacter.AimTargetRoot.position = gunPos;
			_parentCharacter.MyAI.BlackBoard.AimPoint = GetAimPointOnTarget(currentTarget);

			if(_parentCharacter.MyReference.CurrentWeapon.GetComponent<Weapon>().AimPosition == 1)
			{
				_parentCharacter.MyAI.BlackBoard.AimPoint += new Vector3(0, 0.5f, 0);
			}
			Vector3 aimDir = _parentCharacter.MyAI.BlackBoard.AimPoint - _parentCharacter.MyReference.CurrentWeapon.transform.position;
			Quaternion rotation = Quaternion.LookRotation(aimDir);
			_parentCharacter.AimTargetRoot.transform.rotation = Quaternion.Lerp(_parentCharacter.AimTargetRoot.transform.rotation, rotation, Time.deltaTime * 9);


		}
		else
		{
			
			//is not aiming
			float aimHeight = 1.5f;

			_parentCharacter.AimTargetRoot.position = _parentCharacter.transform.position + Vector3.up * 1.5f;
			Vector3 velocity = _parentCharacter.GetCharacterVelocity();

			Vector3 aimDir = velocity.normalized;
			/*
			if(_parentCharacter.MyAI.BlackBoard.InvisibleEnemy != null)
			{
				if(Mode == AITargetingModes.LookAheadAround)
				{
					aimDir = Quaternion.Euler(0, _lookAroundAngle, 0) * (velocity.magnitude > 0 ? velocity : _parentCharacter.transform.forward);
				}
				else if(Mode == AITargetingModes.LookAround)
				{
					aimDir = Quaternion.Euler(0, _lookAroundAngle, 0) * (_lookAroundDir != Vector3.zero ? _lookAroundDir : _parentCharacter.transform.forward);
				}
				else
				{
					aimDir = _parentCharacter.MyAI.BlackBoard.LastKnownEnemyPosition - _parentCharacter.transform.position;
				}
			}
			else if(velocity.magnitude < 0.1f)
			{
				aimDir = _parentCharacter.transform.forward;
			}
			*/

			if(Mode == AITargetingModes.LookAheadAround)
			{
				aimDir = Quaternion.Euler(0, _lookAroundAngle, 0) * (velocity.magnitude > 0 ? velocity : _parentCharacter.transform.forward);
			}
			else if(Mode == AITargetingModes.LookAround)
			{
				aimDir = Quaternion.Euler(0, _lookAroundAngle, 0) * (_lookAroundDir != Vector3.zero ? _lookAroundDir : _parentCharacter.transform.forward);
			}
			else if(Mode == AITargetingModes.LookAhead)
			{
				aimDir = _parentCharacter.transform.forward;
			}
			else
			{
				aimDir = _parentCharacter.MyAI.BlackBoard.LastKnownEnemyPosition - _parentCharacter.transform.position;
			}


			Quaternion rotation = Quaternion.LookRotation(aimDir);
			_parentCharacter.AimTargetRoot.transform.rotation = Quaternion.Lerp(_parentCharacter.AimTargetRoot.transform.rotation, rotation, Time.deltaTime * 6);

			_parentCharacter.MyAI.BlackBoard.AimPoint = _parentCharacter.AimTarget.position;

			Vector3 lookPos;
			/*
			if(velocity.magnitude > 1)
			{
				lookPos = _parentCharacter.transform.position + new Vector3(velocity.normalized.x, 0, velocity.normalized.z) * 2 + new Vector3(0, aimHeight, 0);
			}
			else
			{
				lookPos = _parentCharacter.transform.position + new Vector3(aimDir.x, 0, aimDir.z) * 2 + new Vector3(0, aimHeight, 0);

			}
			*/

			if(_parentCharacter.MyAI.BlackBoard.TargetEnemy != null)
			{
				//Vector3 lookDir = _parentCharacter.MyAI.BlackBoard.TargetEnemy.transform.position - _parentCharacter.transform.position;
				//lookPos = _parentCharacter.transform.position + lookDir.normalized * 2 + new Vector3(0, aimHeight, 0);
				lookPos = _parentCharacter.MyAI.BlackBoard.TargetEnemy.transform.position + new Vector3(0, aimHeight, 0);
			}
			else
			{
				lookPos = _parentCharacter.transform.position + new Vector3(aimDir.x, 0, aimDir.z) * 2 + new Vector3(0, aimHeight, 0);
			}

			_parentCharacter.LookTarget.transform.position = Vector3.Lerp(_parentCharacter.LookTarget.transform.position, lookPos, 8 * Time.deltaTime);

		}


	}

	public void UpdatePerHalfSecond()
	{
		//if(_parentCharacter.name != "HumanCharacter")
		//	Debug.Log("AITargeting Update half second " + _parentCharacter.name);
		
		//from working memory look for biggest enemy threat
		//if no threat present, look for nearest uninvestigated item
		List<WorkingMemoryFact> enemyFacts = _parentCharacter.MyAI.WorkingMemory.FindExistingFactOfType(FactType.KnownEnemy);
		//Debug.Log("Update Half Second for " + _parentCharacter.name + " enemyfacts " + enemyFacts.Count);
		if(enemyFacts.Count > 0)
		{
			//for now just get the closest enemy in sight
			WorkingMemoryFact selected = null; 


			float minDist = 1000;
			foreach(WorkingMemoryFact f in enemyFacts)
			{
				float dist = Vector3.Distance(f.LastKnownPos, _parentCharacter.transform.position);
				if(dist < minDist && f.Confidence >= 1)
				{
					minDist = dist;
					selected = f;
				}
			}
				
			if(selected != null)
			{ 
				if(_parentCharacter.MyAI.BlackBoard.TargetEnemy != null)
				{
					//Debug.Log("Updating half second character targeting; current blackboard target is " + _parentCharacter.MyAI.BlackBoard.TargetEnemy.name);
				}
				else
				{
					//Debug.Log("Updating half second character targeting; current blackboard target is null");
				}

				bool isNewTarget = (_parentCharacter.MyAI.BlackBoard.TargetEnemy != (Character)selected.Target && selected.Confidence >= 1f);

				//if targetEnemy is not null and target is locked, then we will not switch to new enemy
				if(_parentCharacter.MyAI.BlackBoard.IsTargetLocked && _parentCharacter.MyAI.BlackBoard.TargetEnemy != null)
				{
					isNewTarget = false;
				}

				if(isNewTarget)
				{
					//if not player faction and no previously known enemy then bark
					bool willCallForHelp = false;

					if(_parentCharacter.MyAI.BlackBoard.TargetEnemy == null && _parentCharacter.MyAI.BlackBoard.InvisibleEnemy == null 
						&& selected.ThreatLevel > 0.1f && _parentCharacter.MyAI.ControlType != AIControlType.Player)
					{
						//_parentCharacter.MyAI.Bark("Got intruder!");
						_parentCharacter.PlayVocal(VocalType.Surprise);
						willCallForHelp = true;
					}

					_parentCharacter.MyAI.BlackBoard.TargetEnemy = (Character)selected.Target;
					_parentCharacter.MyAI.BlackBoard.TargetEnemyThreat = selected.ThreatLevel;
					_parentCharacter.MyAI.BlackBoard.InvisibleEnemy = null;



					//_parentCharacter.MyAI.BlackBoard.IsTargetEnemyHittable = true;
					selected.LastKnownPos = _parentCharacter.MyAI.BlackBoard.TargetEnemy.transform.position;
					_parentCharacter.MyAI.BlackBoard.LastKnownEnemyPosition = _parentCharacter.MyAI.BlackBoard.TargetEnemy.transform.position;
					//trigger when a new target is selected
					CsDebug.Inst.CharLog(_parentCharacter, "AITargeting found a new target!");

					if(willCallForHelp)
					{
						_parentCharacter.MyAI.CallForHelp(_parentCharacter.MyAI.BlackBoard.TargetEnemy);
					}

					_parentCharacter.MyAI.OnImportantEvent(0.8f);
				}
				else
				{
					_parentCharacter.MyAI.BlackBoard.TargetEnemy = (Character)selected.Target;
					_parentCharacter.MyAI.BlackBoard.TargetEnemyThreat = selected.ThreatLevel;
					_parentCharacter.MyAI.BlackBoard.InvisibleEnemy = null;
					_parentCharacter.MyAI.BlackBoard.LastKnownEnemyPosition = _parentCharacter.MyAI.BlackBoard.TargetEnemy.transform.position;
				}
			}

		}
		else
		{
			_parentCharacter.MyAI.BlackBoard.TargetEnemy = null;
			_parentCharacter.MyAI.BlackBoard.InvisibleEnemy = null;
			//_parentCharacter.MyAI.BlackBoard.IsTargetEnemyHittable = false;

		}

		//if(_parentCharacter.name != "HumanCharacter")
		//	Debug.Log("AITargeting Update half second COMPLETED " + _parentCharacter.name);
	}


	public void UpdatePerSecond()
	{
		//if(_parentCharacter.name != "HumanCharacter")
		//	Debug.Log("AITargeting Update full second " + _parentCharacter.name);

		_parentCharacter.SendCommand(CharacterCommands.SetAlert);

		if(UnityEngine.Random.value > 0.6f)
		{
			ResetLookAroundAngle();
		}

		//check current target. If it's still in sight (confidence 1) increase threat level
		//if no longer in sight, move it to invisible enemy
		//if no longer in memory then remove
		if(_parentCharacter.MyAI.BlackBoard.TargetEnemy != null)
		{
			WorkingMemoryFact currentTargetFact = _parentCharacter.MyAI.WorkingMemory.FindExistingFact(FactType.KnownEnemy, _parentCharacter.MyAI.BlackBoard.TargetEnemy);


			if(currentTargetFact != null && currentTargetFact.Confidence >= 1)
			{
				currentTargetFact.ThreatLevel += 0.05f;
				if(currentTargetFact.ThreatLevel > 1)
				{
					currentTargetFact.ThreatLevel = 1;
				}
				_parentCharacter.MyAI.BlackBoard.TargetEnemyThreat = currentTargetFact.ThreatLevel;
				_parentCharacter.MyAI.BlackBoard.LastKnownEnemyPosition = _parentCharacter.MyAI.BlackBoard.TargetEnemy.transform.position;
				if(currentTargetFact.ThreatLevel > 0.1f)
				{
					_parentCharacter.MyAI.BlackBoard.GuardLevel = 3;
				}
				//_parentCharacter.MyAI.BlackBoard.IsTargetEnemyHittable = currentTargetFact.IsHittable;

				return;
			}
			else if(currentTargetFact != null)
			{
				//when confidence is low, move enemy to invisible (I know there's this guy but I don't see him)
				//Debug.Log("AI Targeting current target fact confidence is " + currentTargetFact.Confidence + " " + _parentCharacter.name);
				_parentCharacter.MyAI.BlackBoard.InvisibleEnemy = _parentCharacter.MyAI.BlackBoard.TargetEnemy;
				currentTargetFact.LastKnownPos = _parentCharacter.MyAI.BlackBoard.LastKnownEnemyPosition;
				_parentCharacter.MyAI.BlackBoard.TargetEnemy = null;
				//_parentCharacter.MyAI.BlackBoard.IsTargetEnemyHittable = currentTargetFact.IsHittable;

				currentTargetFact.ThreatLevel -= currentTargetFact.ThreatDropRate;
				if(currentTargetFact.ThreatLevel < 0)
				{
					currentTargetFact.ThreatLevel = 0;
				}
				if(currentTargetFact.ThreatLevel > 0.1f)
				{
					_parentCharacter.MyAI.BlackBoard.GuardLevel = 2;
				}
				_parentCharacter.MyAI.BlackBoard.TargetEnemyThreat = currentTargetFact.ThreatLevel;
			}
			else
			{
				_parentCharacter.MyAI.BlackBoard.TargetEnemy = null;
				_parentCharacter.MyAI.BlackBoard.TargetEnemyThreat = 0;
				_parentCharacter.MyAI.BlackBoard.InvisibleEnemy = null;
				//_parentCharacter.MyAI.BlackBoard.IsTargetEnemyHittable = false;
			}
		}


		//if(_parentCharacter.name != "HumanCharacter")
		//	Debug.Log("AITargeting Update full second FINISH " + _parentCharacter.name);

	}

	public Vector3 GetAimPointOnTarget(Character target)
	{
		Vector3 relativeVelocity = target.GetCharacterVelocity() - _parentCharacter.GetCharacterVelocity();

		CapsuleCollider collider = target.GetComponent<CapsuleCollider>();

		float colliderHeight = collider.height;
		int colliderDir = collider.direction;
		Vector3 colliderOffset = collider.center;
		if(colliderDir == 2)
		{
			return target.transform.position + colliderOffset * 0.8f + relativeVelocity / 4;
		}
		else
		{
			return target.transform.position + Vector3.up * colliderHeight * 0.8f + relativeVelocity / 4;
		}
	}

	public void SetTargetingMode(AITargetingModes mode, Vector3 direction)
	{

		Mode = mode;
		if(_lookAroundDir != direction)
		{
			_lookAroundDir = direction;

			ResetLookAroundAngle();
		}
	}





	private void ResetLookAroundAngle()
	{
		if(Mode == AITargetingModes.LookAheadAround)
		{
			_lookAroundAngle = UnityEngine.Random.Range(-60, 60);
		}
		else if(Mode == AITargetingModes.LookAround)
		{
			_lookAroundAngle = UnityEngine.Random.Range(-30, 30);
		}
	}
}

public enum AITargetingModes
{
	LookAhead,
	LookAheadAround,
	LookAround,
}
