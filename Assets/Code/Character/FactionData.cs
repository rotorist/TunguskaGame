using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class FactionData
{
	public Faction FactionID;
	public string Name;
	public Dictionary<Faction, float> Relationships;

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
			Relationships.Add(relationship.Key, relationship.Value);
		}
	}
}
