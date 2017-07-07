using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.SqliteClient;
using System.Data;
using System;

public class DBHandlerAI 
{
	public List<GoapGoal> GetCharacterGoalSet(int characterID)
	{
		IDataReader goalReader = GameManager.Inst.DBManager.RunAIQuery(
			"SELECT goap_goals.*, character_goals.priority " +
			"FROM goap_goals INNER JOIN character_goals " +
			"ON goap_goals.id = character_goals.goal_id AND character_goals.character_id = '" + characterID + "'");

		List<GoapGoal> goals = new List<GoapGoal>();
		while(goalReader.Read())
		{
			int goalID = goalReader.GetInt32(0);
			string goalName = goalReader.GetString(1);
			int priority = goalReader.GetInt32(2);
			GoapGoal goal = new GoapGoal();
			goal.Name = goalName;
			goal.Priority = priority;

			//query for conditions for each goal
			IDataReader condReader = GameManager.Inst.DBManager.RunAIQuery(
				"SELECT world_states.*, goal_conditions.* " +
				"FROM goal_conditions INNER JOIN world_states " +
				"ON goal_conditions.world_state_id = world_states.id AND goal_conditions.goal_id = '" + goalID + "'");

			List<GoapWorldState> conditions = new List<GoapWorldState>();
			while(condReader.Read())
			{
				GoapWorldState state = ParseWorldState(condReader);
				conditions.Add(state);
			}
			condReader.Close();
			condReader = null;

			goal.GoalStates = conditions;
			goals.Add(goal);
		}

		goalReader.Close();
		goalReader = null;
		return goals;
	}

	public GoapGoal GetGoalByID(int id)
	{
		IDataReader goalReader = GameManager.Inst.DBManager.RunAIQuery(
			"SELECT goap_goals.* " +
			"FROM goap_goals " +
			"WHERE goap_goals.id = '" + id + "'");

		while(goalReader.Read())
		{
			string goalName = goalReader.GetString(1);
			GoapGoal goal = new GoapGoal();
			goal.Name = goalName;

			//query for conditions for each goal
			IDataReader condReader = GameManager.Inst.DBManager.RunAIQuery(
				"SELECT world_states.*, goal_conditions.* " +
				"FROM goal_conditions INNER JOIN world_states " +
				"ON goal_conditions.world_state_id = world_states.id AND goal_conditions.goal_id = '" + id + "'");

			List<GoapWorldState> conditions = new List<GoapWorldState>();
			while(condReader.Read())
			{
				GoapWorldState state = ParseWorldState(condReader);
				conditions.Add(state);
			}
			condReader.Close();
			condReader = null;

			goal.GoalStates = conditions;

			return goal;
		}

		return null;
	}

	public List<GoapAction> GetCharacterActionSet(int characterID)
	{
		IDataReader actionReader = GameManager.Inst.DBManager.RunAIQuery(
			"SELECT goap_actions.* " +
			"FROM goap_actions INNER JOIN character_actions " +
			"ON goap_actions.id = character_actions.action_id AND character_actions.character_id = '" + characterID + "'");
		List<GoapAction> actions = new List<GoapAction>();
		while(actionReader.Read()) 
		{
			int id = actionReader.GetInt32(0);
			string className = actionReader.GetString(1);
			string description = actionReader.GetString(2);
			//Debug.Log("get action " + className + description);
			float cost = actionReader.GetFloat(3);

			GoapAction action = (GoapAction)System.Activator.CreateInstance(System.Type.GetType(className), className, description, cost);

			///query for preconditions and context preconditions
			IDataReader condReader = GameManager.Inst.DBManager.RunAIQuery(
				"SELECT world_states.*, action_preconditions.* " +
				"FROM action_preconditions INNER JOIN world_states " +
				"ON action_preconditions.world_state_id = world_states.id AND action_preconditions.action_id = '" + id + "'");

			List<GoapWorldState> preconditions = new List<GoapWorldState>();
			List<GoapWorldState> context = new List<GoapWorldState>();
			while(condReader.Read())
			{
				GoapWorldState state = ParseWorldState(condReader);
				bool isContext = (condReader.GetInt32(7) == 1);
				if(isContext)
				{
					context.Add(state);
				}
				else
				{
					preconditions.Add(state);
				}
			}

			condReader.Close();
			condReader = null;

			//query for effects
			IDataReader effReader = GameManager.Inst.DBManager.RunAIQuery(
				"SELECT world_states.*, action_effects.* " +
				"FROM action_effects INNER JOIN world_states " +
				"ON action_effects.world_state_id = world_states.id AND action_effects.action_id = '" + id + "'");

			List<GoapWorldState> effects = new List<GoapWorldState>();
			while(effReader.Read())
			{
				GoapWorldState state = ParseWorldState(effReader);
				effects.Add(state);
			}

			effReader.Close();
			effReader = null;

			action.AddPrecondition(preconditions);
			action.AddEffect(effects);

			actions.Add(action);
		}

		actionReader.Close();
		actionReader = null;
		return actions;
	}

	public GoapWorldState ParseWorldState(IDataReader reader)
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
