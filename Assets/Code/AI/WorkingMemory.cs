using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorkingMemory 
{
	private Character _parentCharacter;
	public HashSet<WorkingMemoryFact> Facts;

	public WorkingMemory()
	{
		

	}

	public void Initialize(Character parent)
	{
		_parentCharacter = parent;
		Facts = new HashSet<WorkingMemoryFact>();

		_parentCharacter.MyEventHandler.OnOneSecondTimer -= UpdateFact;
		_parentCharacter.MyEventHandler.OnOneSecondTimer += UpdateFact;
	}



	public void OnDeath()
	{
		_parentCharacter.MyEventHandler.OnOneSecondTimer -= UpdateFact;
	}


	public void UpdateFact()
	{
		
		//every second we will reduce the confidence value of each fact by reduce rate until
		//zero, and then remove (forget) it

		_parentCharacter.MyAI.BlackBoard.HighestPersonalThreat = 0;
		Vector3 threatDir = Vector3.zero;
		int knownEnemyCount = 0;

		HashSet<WorkingMemoryFact> copy = new HashSet<WorkingMemoryFact>(Facts);
		foreach(WorkingMemoryFact fact in copy)
		{
			if(fact.FactType == FactType.KnownEnemy || fact.FactType == FactType.KnownNeutral || fact.FactType == FactType.KnownBeast)
			{
				//Debug.Log("threat level " + fact.ThreatLevel + " I am " + _parentCharacter.name);
				if(((Character)fact.Target).MyStatus.Health <= 0)
				{
					//Debug.Log("enemy in memory is dead, removing fact");
					RemoveFact(fact);
					continue;
				}
				else
				{
					knownEnemyCount ++;
				}
			}

			//also update personal threat on the blackboard with highest number
			//then reduce the personal threat level by drop rate
			if(fact.FactType == FactType.PersonalThreat)
			{
				if(fact.ThreatLevel >= _parentCharacter.MyAI.BlackBoard.HighestPersonalThreat)
				{
					_parentCharacter.MyAI.BlackBoard.HighestPersonalThreat = fact.ThreatLevel;
				}
				threatDir = (threatDir + (fact.LastKnownPos - _parentCharacter.transform.position).normalized).normalized;

			}



			fact.ThreatLevel -= fact.ThreatDropRate;
			if(fact.ThreatLevel < 0)
			{
				fact.ThreatLevel = 0;
			}

			//update corpse fact
			if(fact.FactType == FactType.NearbyCorpse)
			{
				if(_parentCharacter.MyAI.BlackBoard.TargetCorpse == null && fact.ThreatLevel > 0)
				{
					_parentCharacter.MyAI.BlackBoard.TargetCorpse = fact;
				}
			}


			fact.Confidence -= fact.ConfidenceDropRate;
			//Debug.Log("Working memory updated confidence for " + fact.FactType + " to " +fact.Confidence);
			if(fact.Confidence <= 0)
			{
				//Debug.Log("Working memory removing fact of " + fact.FactType + " confidence " + fact.Confidence);
				RemoveFact(fact.FactType, fact.Target);
			}
		}

		//update personal threat dir
		_parentCharacter.MyAI.BlackBoard.AvgPersonalThreatDir = threatDir;

		_parentCharacter.MyAI.BlackBoard.NumberOfKnownEnemies = knownEnemyCount;


	}

	public WorkingMemoryFact AddFact(FactType type, object target, Vector3 lastKnownPos, float confidence, float dropRate)
	{
		WorkingMemoryFact fact = new WorkingMemoryFact();
		fact.LastKnownPos = lastKnownPos;
		fact.Confidence = confidence;
		fact.ConfidenceDropRate = dropRate;
		fact.FactType = type;
		fact.Target = target;
		Facts.Add(fact);

		return fact;
	}

	public void AddFact(WorkingMemoryFact fact)
	{
		if(!Facts.Contains(fact))
		{
			Facts.Add(fact);
		}
	}
		
	public void RemoveFact(FactType type, object target)
	{
		WorkingMemoryFact fact = null;
		foreach(WorkingMemoryFact f in Facts)
		{
			if(f.FactType == type && Object.Equals(f.Target, target))
			{
				fact = f;
			}
		}
		if(fact != null)
		{
			Facts.Remove(fact);
		}
	}

	public void RemoveFact(WorkingMemoryFact fact)
	{
		if(fact != null)
		{
			Facts.Remove(fact);
		}
	}

	public void RemoveFact(FactType type)
	{
		HashSet<WorkingMemoryFact> copy = new HashSet<WorkingMemoryFact>(Facts);
		foreach(WorkingMemoryFact f in copy)
		{
			if(f.FactType == type)
			{
				Facts.Remove(f);
			}
		}
	}

	public bool CompareFact(WorkingMemoryFact fact1, WorkingMemoryFact fact2)
	{
		//returns true if the facts are the same 
		// check if types and handle are same
		if(fact1.FactType == fact2.FactType && Object.Equals(fact1.Target, fact2.Target))
		{
			return true;
		}

		return false;
	}

	public WorkingMemoryFact FindExistingFact(FactType type, object target)
	{
		foreach(WorkingMemoryFact f in Facts)
		{
			if(type == f.FactType && Object.Equals(f.Target, target))
			{
				return f;
			}
		}

		return null;
	}

	public WorkingMemoryFact FindExistingFact(object target)
	{
		foreach(WorkingMemoryFact f in Facts)
		{
			if(Object.Equals(f.Target, target))
			{
				return f;
			}
		}

		return null;
	}

	public List<WorkingMemoryFact> FindExistingFactOfType(FactType type)
	{
		List<WorkingMemoryFact> facts = new List<WorkingMemoryFact>();
		foreach(WorkingMemoryFact f in Facts)
		{
			if(type.Equals(f.FactType))
			{
				facts.Add(f);
			}
		}

		//Debug.Log("found existing facts of type " + type + " " + facts.Count);

		return facts;
	}

	public WorkingMemoryFact FindFailedActionFact(GoapAction action, object target)
	{
		foreach(WorkingMemoryFact f in Facts)
		{
			if(f.FactType == FactType.FailedAction && Object.Equals(f.Target, target) && f.PastAction == action.Name)
			{
				return f;
			}
		}

		return null;
	}

}

public class WorkingMemoryFact
{
	public FactType FactType;
	public object Target;
	public Vector3 LastKnownPos;//use this when confidence is low enough
	public string PastAction;//only for fact type failedAction
	public float Confidence;
	public float ConfidenceDropRate;
	public float ThreatLevel;
	public float ThreatDropRate;
}

public enum FactType
{
	KnownEnemy,
	KnownFriend,
	KnownNeutral,
	KnownBeast,
	NearbyExplosive,
	NearbyContainer,
	NearbyCorpse,
	NearbyPickupObject,
	NearbySmartObject,
	PersonalThreat,
	Disturbance,
	Event,
	FailedAction,
}