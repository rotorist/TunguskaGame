using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.SqliteClient;
using System.Data;
using System.Text;
using System.Xml;
using System.IO;
using System;

public class DBHandlerEnvironment
{
	public void LoadPrimaryEnvironmentSounds(string location, string time, string weather, out List<string> soundSet, out List<int> choices)
	{
		IDataReader soundsReader = GameManager.Inst.DBManager.RunQuery(
			"SELECT primary_set from environment_sounds WHERE location='" + location + "'" );

		List<string> resultSounds = new List<string>();
		List<int> resultChoices = new List<int>();

		while(soundsReader.Read())
		{
			string primarySetRaw = soundsReader.GetString(0);
			string [] lines = primarySetRaw.Split('\n');
			foreach(string line in lines)
			{
				string [] tokens = line.Split('=');
				if(tokens.Length > 1)
				{
					resultSounds.Add(tokens[0]);
					resultChoices.Add(Convert.ToInt32(tokens[1]));
				}
			}
		}

		soundSet = resultSounds;
		choices = resultChoices;
	}

	public void LoadSecondaryEnvironmentSounds(string location, string time, string weather, out List<string> soundSet, out List<int> choices)
	{
		IDataReader soundsReader = GameManager.Inst.DBManager.RunQuery(
			"SELECT secondary_set from environment_sounds WHERE location='" + location + "'" );

		List<string> resultSounds = new List<string>();
		List<int> resultChoices = new List<int>();

		while(soundsReader.Read())
		{
			string primarySetRaw = soundsReader.GetString(0);
			string [] lines = primarySetRaw.Split('\n');
			foreach(string line in lines)
			{
				string [] tokens = line.Split('=');
				if(tokens.Length > 1)
				{
					resultSounds.Add(tokens[0]);
					resultChoices.Add(Convert.ToInt32(tokens[1]));
				}
			}
		}

		soundSet = resultSounds;
		choices = resultChoices;

	}

}
