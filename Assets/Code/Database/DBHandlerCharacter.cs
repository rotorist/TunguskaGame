using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.SqliteClient;
using System.Data;
using System.Text;
using System.Xml;
using System.IO;
using System;

public class DBHandlerCharacter
{

	public Dictionary<Faction, FactionData> LoadFactionData()
	{
		Dictionary<Faction, FactionData> factions = new Dictionary<Faction, FactionData>();
		IDataReader factionReader = GameManager.Inst.DBManager.RunQuery(
			"SELECT * from factions");

		while(factionReader.Read())
		{
			FactionData faction = new FactionData();
			int factionID = factionReader.GetInt32(0);
			faction.FactionID = (Faction)Enum.Parse(typeof(Faction), factionID.ToString());
			faction.Name = factionReader.GetString(1);
			faction.CharacterType = (CharacterType)Enum.Parse(typeof(CharacterType), factionReader.GetInt32(3).ToString());
			string rawModelList = factionReader.GetString(2);
			faction.MemberModelIDs = rawModelList.Split('/');

			//now get relationship
			IDataReader relReader = GameManager.Inst.DBManager.RunQuery(
				"SELECT * from faction_relationships WHERE faction_id = '" + factionID + "'");
			
			while(relReader.Read())
			{
				int targetID = relReader.GetInt32(1);
				Faction target = (Faction)Enum.Parse(typeof(Faction), targetID.ToString());
				float relationship = relReader.GetFloat(2);
				faction.AddRelationshipEntry(target, relationship);
			}

			factions.Add(faction.FactionID, faction);
		}

		return factions;
	}

	public int GetNumberOfFactions()
	{
		IDataReader countReader = GameManager.Inst.DBManager.RunQuery(
			"SELECT COUNT(*) from factions");
		while(countReader.Read())
		{
			return countReader.GetInt32(0);
		}

		return 1;
	}

}
