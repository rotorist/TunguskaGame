using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.SqliteClient;
using System.Data;
using System.Text;
using System.Xml;
using System.IO;
using System;

public class DBHandlerStoryEvent 
{

	public Dictionary<string, StoryCondition> LoadStoryConditions()
	{
		IDataReader condItemReader = GameManager.Inst.DBManager.RunQuery(
			"SELECT * from story_conditions_item");

		IDataReader condTriggerReader = GameManager.Inst.DBManager.RunQuery(
			"SELECT * from story_conditions_trigger");

		Dictionary<string, StoryCondition> conditions = new Dictionary<string, StoryCondition>();

		while(condItemReader.Read())
		{
			string condID = condItemReader.GetString(0);
			string condItemID = condItemReader.GetString(1);
			StoryConditionItem condItem = new StoryConditionItem();
			condItem.ID = condID;
			condItem.ItemID = condItemID;
			condItem.IsActive = condItemReader.GetBoolean(2);
			condItem.IsForDurability = condItemReader.GetBoolean(3);
			condItem.Type = StoryConditionType.Item;
			conditions.Add(condID, condItem);
		}

		while(condTriggerReader.Read())
		{
			string condID = condTriggerReader.GetString(0);
			int initValue = condTriggerReader.GetInt32(1);
			StoryConditionTrigger condTrigger = new StoryConditionTrigger();
			condTrigger.ID = condID;
			condTrigger.SetValue(initValue);
			condTrigger.IsActive = condTriggerReader.GetBoolean(2);
			condTrigger.Type = StoryConditionType.Trigger;
			conditions.Add(condID, condTrigger);
		}

		return conditions;
	}

	public Dictionary<string, StoryEventScript> LoadScripts()
	{
		string [] rawFile;
		string levelName = GameManager.Inst.WorldManager.CurrentLevelName;

		try
		{
			rawFile = File.ReadAllLines(Application.dataPath + "/GameData/Scripts/" + levelName + ".txt");
		}
		catch(Exception e)
		{
			UnityEngine.Debug.LogError(e.Message);
			return null;
		}

		Dictionary<string, StoryEventScript> scripts = new Dictionary<string, StoryEventScript>();
		StoryEventScript currentScript = new StoryEventScript();

		foreach(string line in rawFile)
		{
			if(line.Length <= 5)
			{
				continue;
			}


			if(line[0] == '.')
			{
				//get script name and create a new currentScript
				string [] splitString = line.Split('/');
				currentScript = new StoryEventScript();
				scripts.Add(splitString[1], currentScript);

			}
			else
			{
				if(currentScript != null)
				{
					currentScript.Script.Add(line);
				}
			}
		}

		return scripts;
	}

	public List<List<string>> LoadJournal()
	{
		string [] rawFile;

		try
		{
			rawFile = File.ReadAllLines(Application.dataPath + "/GameData/Journal/Journal.txt");
		}
		catch(Exception e)
		{
			UnityEngine.Debug.LogError(e.Message);
			return null;
		}

		List<List<string>> journal = new List<List<string>>();
		List<string> dayEntry = new List<string>();
		int currentDay = 0;
		foreach(string line in rawFile)
		{
			if(line.Length < 3)
			{
				continue;
			}
			if(line[0] == '.')
			{
				string [] splitString = line.Split('/');
				int entryDate = Convert.ToInt32(splitString[1]);

				if(entryDate > currentDay)
				{
					dayEntry = new List<string>();
					journal.Add(dayEntry);
					currentDay = entryDate;
				}
			}
			else
			{
				if(dayEntry != null)
				{
					dayEntry.Add(line);
				}
			}
		}


		return journal;
	}

	public string LoadJournalEntry(int id)
	{
		IDataReader journalReader = GameManager.Inst.DBManager.RunQuery(
			"SELECT entry from journal_entries where id = '" + id + "'");

		while(journalReader.Read())
		{
			return journalReader.GetString(0);
		}

		return "";
	}

	public string LoadNotePaper(string id)
	{
		IDataReader noteReader = GameManager.Inst.DBManager.RunQuery(
			"SELECT text from note_papers where id = '" + id + "'");

		while(noteReader.Read())
		{
			return noteReader.GetString(0);
		}

		return "";
	}



	public string LoadTask(int id)
	{

		IDataReader taskDataReader = GameManager.Inst.DBManager.RunQuery(
			"SELECT text FROM task_data WHERE id = " + id);

		while(taskDataReader.Read())
		{
			return taskDataReader.GetString(0);
		}

		return "";
	}
}
