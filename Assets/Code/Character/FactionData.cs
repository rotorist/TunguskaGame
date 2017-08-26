using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class FactionData
{
	public Faction FactionID;
	public string Name;
	public Dictionary<Faction, float> Relationships;
	public string [] MemberModelIDs;
	public CharacterType CharacterType;
	[SerializeField] private List<KeyValuePair<Faction, float>> _serRelationships;

	public FactionData()
	{
		Relationships = new Dictionary<Faction, float>();
	}

	public void AddRelationshipEntry(Faction id, float rep)
	{
		if(Relationships.ContainsKey(id))
		{
			Relationships[id] = rep;
		}
		else
		{
			Relationships.Add(id, rep);
		}
	}

	public float GetRelationshipByID(Faction id)
	{
		//Debug.Log("get relationship of " + id + " " + Relationships.Count);
		if(Relationships.ContainsKey(id))
		{
			return Relationships[id];
		}
		else
		{
			return 0;
		}
	}

	public void ReduceRelationshipByID(Faction id, float value)
	{
		if(Relationships.ContainsKey(id))
		{
			Relationships[id] -= value;
			if(Relationships[id] < 0)
			{
				Relationships[id] = 0;
			}
		}
	}

	public void IncreaseRelationshipByID(Faction id, float value)
	{
		if(Relationships.ContainsKey(id))
		{
			Relationships[id] += value;
			if(Relationships[id] > 1)
			{
				Relationships[id] = 1;
			}
		}
	}

	public void PrepareSave()
	{
		_serRelationships = new List<KeyValuePair<Faction, float>>();
		foreach(KeyValuePair<Faction, float> relationship in Relationships)
		{
			_serRelationships.Add(relationship);
		}

	
	}

	public void PostLoad()
	{
		Relationships.Clear();
		foreach(KeyValuePair<Faction, float> relationship in _serRelationships)
		{
			//Debug.Log("Setting relationship for " + FactionID + " with " + relationship.Key + " to " + relationship.Value);
			Relationships.Add(relationship.Key, relationship.Value);
		}
	}
}
