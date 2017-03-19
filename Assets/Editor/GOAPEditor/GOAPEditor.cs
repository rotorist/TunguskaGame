using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using Mono.Data.SqliteClient;
using System.Data;

public class GOAPEditor : EditorWindow
{
	private int _selectedTab;
	private IDbConnection _aiDBConn;

	private Dictionary<int, WorldStatesData> _worldStates;

	private int _tab0WorldState;
	private string _tab0Description = "";
	private int _tab0IsBool = 1;

	private int _tab1Goal;
	private string _tab1Description = "";

	private int _tab2Action;
	private string _tab2ClassName = "";
	private string _tab2Description = "";
	private string _tab2CostString = "";

	private int _tab3Char;

	private struct WorldStatesData
	{
		public int ID;
		public string Description;
		public int IsBool;
	}

	private struct GoalData
	{
		public int ID;
		public string Description;
	}

	private struct ActionData
	{
		public int ID;
		public string ClassName;
		public string Description;
		public float DefaultCost;
	}

	private struct CharacterData
	{
		public int ID;
		public string Title;
		public string FirstName;
		public string LastName;
	}

	public GOAPEditor()
	{
		


	}

	void OnEnable()
	{
		_selectedTab = 3;

		//open main db
		string conn = "URI=file:" + Application.dataPath + "/GameData/Database/AI.s3db"; //Path to database.

		_aiDBConn = (IDbConnection)new SqliteConnection(conn);
		_aiDBConn.Open(); //Open connection to the database.
	}

	[MenuItem("Window/GOAPEditor")]

	public static void ShowWindow()
	{

		EditorWindow.GetWindow(typeof(GOAPEditor));

	}

	void OnGUI()
	{
		_selectedTab = GUI.Toolbar(new Rect(10, 10, 350, 20), _selectedTab, new string[]{"World States", "Goals", "Actions", "Characters"});

		if(_selectedTab == 0)
		{
			RefreshWorldStatesTab();
		}
		else if(_selectedTab == 1)
		{
			RefreshGoalsTab();
		}
		else if(_selectedTab == 2)
		{
			RefreshActionsTab();
		}
		else if(_selectedTab == 3)
		{
			RefreshCharactersTab();
		}

	}

	private void RefreshWorldStatesTab()
	{
		IDataReader reader = RunQuery("SELECT world_states.* FROM world_states");

		_worldStates = new Dictionary<int, WorldStatesData>();
		int i = 1;
		while(reader.Read())
		{
			int c_id = reader.GetInt32(0);
			string c_desc = reader.GetString(1);
			int isBool = reader.GetInt32(2);

			WorldStatesData data = new WorldStatesData();
			data.ID = c_id;
			data.Description = c_desc;
			data.IsBool = isBool;

			_worldStates.Add(i, data);
			i++;
		}

		string [] dropDownOptions = new string[_worldStates.Count + 1];
		dropDownOptions[0] = "Add New..";

		if(_worldStates.Count > 0)
		{
			for(int index=1; index <= _worldStates.Count; index++)
			{
				dropDownOptions[index] = _worldStates[index].Description;
			}
		}

		GUI.Label(new Rect(10, 40, 200, 20), "Available World States:");
		_tab0WorldState = EditorGUI.Popup(new Rect(10, 60, 150, 20), _tab0WorldState, dropDownOptions);

		GUI.Label(new Rect(10, 90, 250, 20), "Description: " + dropDownOptions[_tab0WorldState]);
		_tab0Description = GUI.TextField(new Rect(10, 110, 150, 15), _tab0Description, 100);

		string [] isBoolOptions = new string[]{"False", "True"};
		int isBoolValue = 1;
		if(_tab0WorldState > 0)
		{
			isBoolValue = _worldStates[_tab0WorldState].IsBool;
		}
		GUI.Label(new Rect(10, 140, 150, 20), "Is Boolean: " + (isBoolValue == 1));
		_tab0IsBool = EditorGUI.Popup(new Rect(10, 160, 50, 20), _tab0IsBool, isBoolOptions);

		if(GUI.Button(new Rect(10, 200, 60, 20), "Save"))
		{
			//check if the description already exists in table; if so don't save and give error
			IDataReader checkExistReader = RunQuery("SELECT world_states.* FROM world_states WHERE world_states.description = '" + _tab0Description + "'");
			bool entryExists = false;
			if(checkExistReader.Read())
			{
				entryExists = true;
			}

			if(_tab0WorldState == 0)
			{
				if(entryExists)
				{
					Debug.LogError("Cannot create new world state; description already exists in table.");
				}
				else
				{
					RunQuery("INSERT INTO world_states (id, description, is_bool) VALUES (null, '" + _tab0Description + "', " + _tab0IsBool +")");
				}
			}
			else
			{
				//update table with new info
				if(!entryExists)
				{
					Debug.LogError("Cannot update world state; description doesn't exist in table.");
				}
				else
				{
					RunQuery("UPDATE world_states SET is_bool = " + _tab0IsBool + " WHERE description = '" + _tab0Description + "'");
				}
			}
		}

		if(GUI.Button(new Rect(90, 200, 60, 20), "Delete"))
		{
			if(_tab0WorldState > 0)
			{
				RunQuery("DELETE FROM world_states WHERE id = " + _worldStates[_tab0WorldState].ID);
				RunQuery("DELETE FROM goal_conditions WHERE world_state_id = " + _worldStates[_tab0WorldState].ID);
				RunQuery("DELETE FROM action_preconditions WHERE world_state_id = " + _worldStates[_tab0WorldState].ID);
				RunQuery("DELETE FROM action_effects WHERE world_state_id = " + _worldStates[_tab0WorldState].ID);
				_tab0WorldState = 0;
			}
			else
			{
				Debug.LogError("Nothing to delete here!");
			}
		}
	}

	private void RefreshGoalsTab()
	{
		IDataReader reader = RunQuery("SELECT goap_goals.* FROM goap_goals");

		Dictionary<int, GoalData> goals = new Dictionary<int, GoalData>();
		int i = 1;
		while(reader.Read())
		{
			int c_id = reader.GetInt32(0);
			string c_desc = reader.GetString(1);

			GoalData data = new GoalData();
			data.ID = c_id;
			data.Description = c_desc;

			goals.Add(i, data);
			i++;
		}

		string [] dropDownOptions = new string[goals.Count + 1];
		dropDownOptions[0] = "Add New..";

		if(goals.Count > 0)
		{

			for(int index=1; index <= goals.Count; index++)
			{
				dropDownOptions[index] = goals[index].Description;
			}


		}

		GUI.Label(new Rect(10, 40, 200, 20), "Available Goals:");
		_tab1Goal = EditorGUI.Popup(new Rect(10, 60, 150, 20), _tab1Goal, dropDownOptions);


		GUI.Label(new Rect(10, 90, 250, 20), "Description: " + dropDownOptions[_tab1Goal]);
		_tab1Description = GUI.TextField(new Rect(10, 110, 150, 15), _tab1Description, 100);

		if(GUI.Button(new Rect(170, 110, 60, 20), "Save"))
		{
			if(_tab1Goal > 0)
			{
				RunQuery("UPDATE goap_goals SET description = '" + _tab1Description + "' WHERE id = " + goals[_tab1Goal].ID);
			}
			else if(_tab1Goal == 0)
			{
				RunQuery("INSERT INTO goap_goals (id, description) VALUES (null, '" + _tab1Description + "')");
			}
		}

		if(_tab1Goal > 0 && GUI.Button(new Rect(240, 110, 60, 20), "Delete"))
		{
			RunQuery("DELETE FROM goap_goals WHERE id = " + goals[_tab1Goal].ID);
			RunQuery("DELETE FROM goal_conditions WHERE goal_id = " + goals[_tab1Goal].ID);
			RunQuery("DELETE FROM character_goals WHERE goal_id = " + goals[_tab1Goal].ID);
			_tab1Goal = 0;
		}

		if(_tab1Goal > 0)
		{
			GUI.Label(new Rect(10, 140, 200, 20), "Conditions:");
			IDataReader conditionReader = RunQuery(
				"SELECT world_states.*, goal_conditions.* " +
				"FROM goal_conditions INNER JOIN world_states " +
				"ON goal_conditions.world_state_id = world_states.id AND goal_conditions.goal_id = '" + goals[_tab1Goal].ID + "'");

			List<GoapWorldState> conditions = new List<GoapWorldState>();
			while(conditionReader.Read())
			{
				GoapWorldState state = ParseWorldStateJoint(conditionReader);
				conditions.Add(state);
			}

			//get all the world states in order to form a drop down list
			IDataReader worldStatesReader = RunQuery("SELECT world_states.* FROM world_states");

			_worldStates = new Dictionary<int, WorldStatesData>();
			int stateIndex = 0;
			while(worldStatesReader.Read())
			{
				int c_id = worldStatesReader.GetInt32(0);
				string c_desc = worldStatesReader.GetString(1);
				int isBool = worldStatesReader.GetInt32(2);

				WorldStatesData data = new WorldStatesData();
				data.ID = c_id;
				data.Description = c_desc;
				data.IsBool = isBool;

				_worldStates.Add(stateIndex, data);
				stateIndex++;
			}

			string [] conditionOptions = new string[_worldStates.Count];

			int rectHeight = 160;
			int heightStep = 30;
			int condIndex = 0;

			//remove existing goal conditions in db
			RunQuery("DELETE FROM goal_conditions WHERE goal_id = " + goals[_tab1Goal].ID);

			foreach(GoapWorldState state in conditions)
			{
				int selectedName = 0;

				if(_worldStates.Count > 0)
				{
					for(int index=0; index < _worldStates.Count; index++)
					{
						conditionOptions[index] = _worldStates[index].Description;
						if(state.Name == _worldStates[index].Description)
						{
							selectedName = index;
						}
					}

					int height = rectHeight + heightStep * condIndex;
					selectedName = EditorGUI.Popup(new Rect(10, height, 150, 20), selectedName, conditionOptions);
					GUI.Label(new Rect(170, height, 40, 20), "Equals");

					int selectedValue = EditorGUI.Popup(new Rect(220, height, 60, 20), ((bool)state.Value) == true ? 1 : 0, new string[]{"False", "True"});


					bool toBeDeleted = false;
					if(GUI.Button(new Rect(290, height, 15, 15), "-"))
					{
						toBeDeleted = true;
					}
					//add new entries to db
					if(!toBeDeleted)
					{
						RunQuery("INSERT INTO goal_conditions (goal_id, world_state_id, operator_id, value) VALUES (" 
							+ goals[_tab1Goal].ID + ", " + _worldStates[selectedName].ID + ", 0, " + selectedValue + ")");
					}
				}

				condIndex ++;
			}

			//a button to add new condition
			if(GUI.Button(new Rect(290, rectHeight + heightStep * conditions.Count, 20, 20), "+"))
			{
				RunQuery("INSERT INTO goal_conditions (goal_id, world_state_id, operator_id, value) VALUES (" 
					+ goals[_tab1Goal].ID + ", " + _worldStates[0].ID + ", 0, 1)");
			}
		}



	}

	private void RefreshActionsTab()
	{
		IDataReader reader = RunQuery("SELECT goap_actions.* FROM goap_actions");

		Dictionary<int, ActionData> actions = new Dictionary<int, ActionData>();
		int i = 1;
		while(reader.Read())
		{
			int c_id = reader.GetInt32(0);
			string c_name = reader.GetString(1);
			string c_desc = reader.GetString(2);
			float c_cost = reader.GetFloat(3);

			ActionData data = new ActionData();
			data.ID = c_id;
			data.ClassName = c_name;
			data.Description = c_desc;
			data.DefaultCost = c_cost;

			actions.Add(i, data);
			i++;
		}

		string [] dropDownOptions = new string[actions.Count + 1];
		dropDownOptions[0] = "Add New..";

		if(actions.Count > 0)
		{

			for(int index=1; index <= actions.Count; index++)
			{
				dropDownOptions[index] = actions[index].Description;
			}


		}

		GUI.Label(new Rect(10, 40, 200, 20), "Available Actions:");
		_tab2Action = EditorGUI.Popup(new Rect(10, 60, 150, 20), _tab2Action, dropDownOptions);


		GUI.Label(new Rect(10, 90, 250, 20), "Description: " + dropDownOptions[_tab2Action]);
		_tab2Description = GUI.TextField(new Rect(10, 110, 150, 15), _tab2Description, 100);

		string className = "";
		if(_tab2Action > 0)
		{
			className = actions[_tab2Action].ClassName;
		}
		GUI.Label(new Rect(10, 130, 250, 20), "Class Name: " + className);
		_tab2ClassName = GUI.TextField(new Rect(10, 150, 150, 15), _tab2ClassName, 100);

		string cost = "";
		if(_tab2Action > 0)
		{
			cost = actions[_tab2Action].DefaultCost.ToString();
		}
		GUI.Label(new Rect(10, 170, 250, 20), "Default Cost: " + cost);
		_tab2CostString = GUI.TextField(new Rect(10, 190, 150, 15), _tab2CostString, 100);

		if(GUI.Button(new Rect(10, 210, 60, 20), "Save"))
		{
			if(_tab2Action > 0)
			{
				RunQuery("UPDATE goap_actions SET description = '" + _tab2Description + "', class_name = '" + _tab2ClassName + "', default_cost = '" + actions[_tab2Action].DefaultCost + "' WHERE id = " + actions[_tab2Action].ID);
			}
			else if(_tab2Action == 0)
			{
				float costFloat = Convert.ToSingle(_tab2CostString);
				RunQuery("INSERT INTO goap_actions (id, description, class_name, default_cost) VALUES (null, '" + _tab2Description + "', '" + _tab2ClassName + "', " + costFloat + ")");
			}

			_tab2ClassName = "";
			_tab2Description = "";
			_tab2CostString = "";
		}

		if(_tab2Action > 0 && GUI.Button(new Rect(80, 210, 60, 20), "Delete"))
		{
			RunQuery("DELETE FROM goap_actions WHERE id = " + actions[_tab2Action].ID);
			RunQuery("DELETE FROM action_effects WHERE action_id = " + actions[_tab2Action].ID);
			RunQuery("DELETE FROM action_preconditions WHERE action_id = " + actions[_tab2Action].ID);
			RunQuery("DELETE FROM character_actions WHERE action_id = " + actions[_tab2Action].ID);
			_tab2Action = 0;
			_tab2ClassName = "";
			_tab2Description = "";
			_tab2CostString = "";
		}

		if(_tab2Action > 0)
		{
			GUI.Label(new Rect(10, 240, 200, 20), "Conditions:");
			IDataReader conditionReader = RunQuery(
				"SELECT world_states.*, action_preconditions.* " +
				"FROM action_preconditions INNER JOIN world_states " +
				"ON action_preconditions.world_state_id = world_states.id AND action_preconditions.action_id = '" + actions[_tab2Action].ID + "'");

			List<GoapWorldState> conditions = new List<GoapWorldState>();
			while(conditionReader.Read())
			{
				GoapWorldState state = ParseWorldStateJoint(conditionReader);
				conditions.Add(state);
			}

			//get all the world states in order to form a drop down list
			IDataReader worldStatesReader = RunQuery("SELECT world_states.* FROM world_states");

			_worldStates = new Dictionary<int, WorldStatesData>();
			int stateIndex = 0;
			while(worldStatesReader.Read())
			{
				int c_id = worldStatesReader.GetInt32(0);
				string c_desc = worldStatesReader.GetString(1);
				int isBool = worldStatesReader.GetInt32(2);

				WorldStatesData data = new WorldStatesData();
				data.ID = c_id;
				data.Description = c_desc;
				data.IsBool = isBool;

				_worldStates.Add(stateIndex, data);
				stateIndex++;
			}

			string [] conditionOptions = new string[_worldStates.Count];

			int rectHeight = 260;
			int heightStep = 30;
			int condIndex = 0;

			foreach(GoapWorldState state in conditions)
			{
				int selectedName = 0;

				if(_worldStates.Count > 0)
				{
					for(int index=0; index < _worldStates.Count; index++)
					{
						conditionOptions[index] = _worldStates[index].Description;
						if(state.Name == _worldStates[index].Description)
						{
							selectedName = index;
						}
					}

					int height = rectHeight + heightStep * condIndex;
					int newSelectedName = EditorGUI.Popup(new Rect(10, height, 150, 20), selectedName, conditionOptions);
					GUI.Label(new Rect(170, height, 40, 20), "Equals");

					int selectedValue = EditorGUI.Popup(new Rect(220, height, 60, 20), ((bool)state.Value) == true ? 1 : 0, new string[]{"False", "True"});

					bool toBeUpdated = false;
					if(newSelectedName != selectedName || selectedValue != (((bool)state.Value) == true ? 1 : 0))
					{
						toBeUpdated = true;
					}

					bool toBeDeleted = false;
					if(GUI.Button(new Rect(290, height, 15, 15), "-"))
					{
						toBeDeleted = true;
					}

					if(toBeUpdated)
					{
						RunQuery("UPDATE action_preconditions SET world_state_id = " + _worldStates[newSelectedName].ID + ", value = " + selectedValue
							+ " WHERE action_id = " + actions[_tab2Action].ID + " AND world_state_id = " + _worldStates[selectedName].ID);
					}
					if(toBeDeleted)
					{
						RunQuery("DELETE FROM action_preconditions WHERE world_state_id = " + _worldStates[newSelectedName].ID + " AND action_id = " + actions[_tab2Action].ID);
					}

				}

				condIndex ++;
			}

			rectHeight = rectHeight + heightStep * conditions.Count;
			//a button to add new condition
			if(GUI.Button(new Rect(290, rectHeight, 20, 20), "+"))
			{
				RunQuery("INSERT INTO action_preconditions (action_id, world_state_id, operator_id, value, is_context) VALUES (" 
					+ actions[_tab2Action].ID + ", " + _worldStates[0].ID + ", 0, 1, 0)");
			}





			//now list effects
			GUI.Label(new Rect(10, rectHeight + 30, 200, 20), "Effects:");
			IDataReader effectsReader = RunQuery(
				"SELECT world_states.*, action_effects.* " +
				"FROM action_effects INNER JOIN world_states " +
				"ON action_effects.world_state_id = world_states.id AND action_effects.action_id = '" + actions[_tab2Action].ID + "'");

			List<GoapWorldState> effects = new List<GoapWorldState>();
			while(effectsReader.Read())
			{
				GoapWorldState state = ParseWorldStateJoint(effectsReader);
				effects.Add(state);
			}

			string [] effectOptions = new string[_worldStates.Count];
			int effectIndex = 0;
			rectHeight += 50;

			foreach(GoapWorldState state in effects)
			{
				int selectedName = 0;

				if(_worldStates.Count > 0)
				{
					for(int index=0; index < _worldStates.Count; index++)
					{
						effectOptions[index] = _worldStates[index].Description;
						if(state.Name == _worldStates[index].Description)
						{
							selectedName = index;
						}
					}

					int height = rectHeight + heightStep * effectIndex;
					int newSelectedName = EditorGUI.Popup(new Rect(10, height, 150, 20), selectedName, effectOptions);
					GUI.Label(new Rect(170, height, 40, 20), "Equals");

					int selectedValue = EditorGUI.Popup(new Rect(220, height, 60, 20), ((bool)state.Value) == true ? 1 : 0, new string[]{"False", "True"});

					bool toBeUpdated = false;
					if(newSelectedName != selectedName || selectedValue != (((bool)state.Value) == true ? 1 : 0))
					{
						toBeUpdated = true;
					}

					bool toBeDeleted = false;
					if(GUI.Button(new Rect(290, height, 15, 15), "-"))
					{
						toBeDeleted = true;
					}

					if(toBeUpdated)
					{
						RunQuery("UPDATE action_effects SET world_state_id = " + _worldStates[newSelectedName].ID + ", value = " + selectedValue
							+ " WHERE action_id = " + actions[_tab2Action].ID + " AND world_state_id = " + _worldStates[selectedName].ID);
					}
					if(toBeDeleted)
					{
						RunQuery("DELETE FROM action_effects WHERE world_state_id = " + _worldStates[newSelectedName].ID + " AND action_id = " + actions[_tab2Action].ID);
					}

				}

				effectIndex ++;
			}

			rectHeight = rectHeight + heightStep * effects.Count;
			//a button to add new condition
			if(GUI.Button(new Rect(290, rectHeight, 20, 20), "+"))
			{
				RunQuery("INSERT INTO action_effects (action_id, world_state_id, operator_id, value) VALUES (" 
					+ actions[_tab2Action].ID + ", " + _worldStates[0].ID + ", 0, 1)");
			}


		}
	}

	private void RefreshCharactersTab()
	{
		IDataReader reader = RunQuery("SELECT characters.* FROM characters");

		Dictionary<int, CharacterData> characters = new Dictionary<int, CharacterData>();
		int i = 1;
		while(reader.Read())
		{
			int c_id = reader.GetInt32(0);
			string c_title = reader.GetString(1);
			string c_first = reader.GetString(2);
			string c_last = reader.GetString(3);

			CharacterData data = new CharacterData();
			data.ID = c_id;
			data.Title = c_title;
			data.FirstName = c_first;
			data.LastName = c_last;

			characters.Add(i, data);
			i++;
		}

		string [] dropDownOptions = new string[characters.Count + 1];
		dropDownOptions[0] = "None";

		if(characters.Count > 0)
		{

			for(int index=1; index <= characters.Count; index++)
			{
				dropDownOptions[index] = characters[index].Title + ", " + characters[index].FirstName + " " + characters[index].LastName;
			}


		}

		GUI.Label(new Rect(10, 40, 200, 20), "Characters:");
		_tab3Char = EditorGUI.Popup(new Rect(10, 60, 250, 20), _tab3Char, dropDownOptions);

		if(_tab3Char > 0)
		{

			//now we have the character ID, we make a list of character goals
			IDataReader goalReader = RunQuery(
				"SELECT goap_goals.*, character_goals.priority " +
				"FROM goap_goals INNER JOIN character_goals " +
				"ON goap_goals.id = character_goals.goal_id AND character_goals.character_id = '" + characters[_tab3Char].ID + "'");

			List<GoapGoal> goals = new List<GoapGoal>();
			while(goalReader.Read())
			{
				int goalID = goalReader.GetInt32(0);
				string goalName = goalReader.GetString(1);
				int priority = goalReader.GetInt32(2);
				GoapGoal goal = new GoapGoal();
				goal.Name = goalName;
				goal.Priority = priority;
				goals.Add(goal);
			}


			IDataReader allGoalsReader = RunQuery("SELECT goap_goals.* FROM goap_goals");

			Dictionary<int, GoalData> allGoals = new Dictionary<int, GoalData>();
			int goalIndex = 0;
			while(allGoalsReader.Read())
			{
				int c_id = allGoalsReader.GetInt32(0);
				string c_desc = allGoalsReader.GetString(1);

				GoalData data = new GoalData();
				data.ID = c_id;
				data.Description = c_desc;

				allGoals.Add(goalIndex, data);
				goalIndex++;
			}

			GUI.Label(new Rect(10, 90, 150, 20), "Goals:");
			int rectHeight = 110;
			int heightStep = 20;
			int gIndex = 0;

			foreach(GoapGoal goal in goals)
			{
				int selectedName = 0;
				int selectedPriority = 0;

				string [] options = new string[allGoals.Count];

				for(int index=0; index < allGoals.Count; index++)
				{
					options[index] = allGoals[index].Description;
					if(goal.Name == allGoals[index].Description)
					{
						selectedName = index;
						selectedPriority = goal.Priority;
					}
				}
				int height = rectHeight + gIndex * heightStep;
				int newSelectedName = EditorGUI.Popup(new Rect(10, height, 150, 20), selectedName, options);
				GUI.Label(new Rect(170, height, 40, 20), "Priority");

				int newSelectedPriority = EditorGUI.Popup(new Rect(220, height, 60, 20), selectedPriority, new string[]{"0", "1", "2", "3", "4", "5"});

				bool toBeUpdated = false;
				if(newSelectedName != selectedName || newSelectedPriority != selectedPriority)
				{
					toBeUpdated = true;
				}

				bool toBeDeleted = false;
				if(GUI.Button(new Rect(290, height, 15, 15), "-"))
				{
					toBeDeleted = true;
				}

				if(toBeUpdated)
				{
					RunQuery("UPDATE character_goals SET goal_id = " + allGoals[newSelectedName].ID + ", priority = " + newSelectedPriority 
						+ " WHERE character_id = " + characters[_tab3Char].ID + " AND goal_id = " + allGoals[selectedName].ID);
				}
				if(toBeDeleted)
				{
					RunQuery("DELETE FROM character_goals WHERE goal_id = " + allGoals[newSelectedName].ID + " AND character_id = " + characters[_tab3Char].ID);
				}

				gIndex ++;
			}

			rectHeight = rectHeight + heightStep * goals.Count;
			//a button to add new condition
			if(GUI.Button(new Rect(290, rectHeight, 20, 20), "+"))
			{
				RunQuery("INSERT INTO character_goals (character_id, goal_id, priority) VALUES (" 
					+ characters[_tab3Char].ID + ", " + allGoals[0].ID + ", 0)");
			}





			//now we have the character ID, we make a list of character actions

			IDataReader actionReader = RunQuery(
				"SELECT goap_actions.* " +
				"FROM goap_actions INNER JOIN character_actions " +
				"ON goap_actions.id = character_actions.action_id AND character_actions.character_id = '" + characters[_tab3Char].ID + "'");

			List<ActionData> actions = new List<ActionData>();
			while(actionReader.Read())
			{
				int id = actionReader.GetInt32(0);
				string className = actionReader.GetString(1);
				string description = actionReader.GetString(2);
				float cost = actionReader.GetFloat(3);

				ActionData action = new ActionData();
				action.ID = id;
				action.ClassName = className;
				action.Description = description;
				action.DefaultCost = cost;

				actions.Add(action);
			}


			IDataReader allActionsReader = RunQuery("SELECT goap_actions.* FROM goap_actions");

			Dictionary<int, ActionData> allActions = new Dictionary<int, ActionData>();
			int actionIndex = 0;
			while(allActionsReader.Read())
			{
				int c_id = allActionsReader.GetInt32(0);
				string c_desc = allActionsReader.GetString(2);

				ActionData data = new ActionData();
				data.ID = c_id;
				data.Description = c_desc;

				allActions.Add(actionIndex, data);
				actionIndex++;
			}

			rectHeight = rectHeight + 30;

			GUI.Label(new Rect(10, rectHeight, 150, 20), "Actions:");
			rectHeight += 20;
			int aIndex = 0;

			foreach(ActionData action in actions)
			{
				int selectedName = 0;

				string [] options = new string[allActions.Count];

				for(int index=0; index < allActions.Count; index++)
				{
					options[index] = allActions[index].Description;
					if(action.Description == allActions[index].Description)
					{
						selectedName = index;
					}
				}
				int height = rectHeight + aIndex * heightStep;
				int newSelectedName = EditorGUI.Popup(new Rect(10, height, 150, 20), selectedName, options);


				bool toBeUpdated = false;
				if(newSelectedName != selectedName)
				{
					toBeUpdated = true;
				}

				bool toBeDeleted = false;
				if(GUI.Button(new Rect(290, height, 15, 15), "-"))
				{
					toBeDeleted = true;
				}

				if(toBeUpdated)
				{
					RunQuery("UPDATE character_actions SET action_id = " + allActions[newSelectedName].ID 
						+ " WHERE character_id = " + characters[_tab3Char].ID + " AND action_id = " + allActions[selectedName].ID);
				}
				if(toBeDeleted)
				{
					RunQuery("DELETE FROM character_actions WHERE action_id = " + allActions[newSelectedName].ID + " AND character_id = " + characters[_tab3Char].ID);
				}

				aIndex ++;
			}

			rectHeight = rectHeight + heightStep * actions.Count;
			//a button to add new condition
			if(GUI.Button(new Rect(290, rectHeight, 20, 20), "+"))
			{
				RunQuery("INSERT INTO character_actions (character_id, action_id) VALUES (" 
					+ characters[_tab3Char].ID + ", " + allActions[0].ID + ")");
			}

		}

	}


	private IDataReader RunQuery(string query)
	{
		IDbCommand dbcmd = _aiDBConn.CreateCommand();
		dbcmd.CommandText = query;
		IDataReader reader = dbcmd.ExecuteReader();
		dbcmd.Dispose();
		dbcmd = null;

		return reader;
	}


	public GoapWorldState ParseWorldStateJoint(IDataReader reader)
	{
		int c_id = reader.GetInt32(0);
		string c_desc = reader.GetString(1);
		bool isBool = (reader.GetInt32(2) == 1);
		WorldStateOperator op = (WorldStateOperator)Enum.Parse(typeof(WorldStateOperator), reader.GetString(5));

		bool valBool = (reader.GetInt32(6) == 1);
		float valFloat = reader.GetFloat(6);
		object value;
		if(isBool)
		{
			value = valBool;
		}
		else
		{
			value = valFloat;
		}


		GoapWorldState state = new GoapWorldState(c_id, c_desc, op, value);

		return state;
	}
}
