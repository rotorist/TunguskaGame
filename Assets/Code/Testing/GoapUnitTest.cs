using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GoapUnitTest 
{
	private System.Diagnostics.Stopwatch _stopWatch;

	public GoapUnitTest()
	{
		_stopWatch = new System.Diagnostics.Stopwatch();
	}

	public void RunUnitTest()
	{

		/*
		//start GOAP unit test
		GoapPlanner planner = new GoapPlanner();

		ActionGoToLocation action1 = new ActionGoToLocation("go to ");
		action1.AddPrecondition(new GoapWorldState(2, "IsRangedWeaponLoaded", WorldStateOperator.Equals, true));
		action1.AddPrecondition(new GoapWorldState(3, "IsTargetInRange", WorldStateOperator.Equals, true));
		action1.AddEffect(new GoapWorldState(4, "IsTargetDead", WorldStateOperator.Equals, true));
		action1.Cost = 3;

		Debug.Log(action1.Preconditions[0].Name);

		//the "attack target" action

		GoapAction action1 = new GoapAction("Ranged Attack Target");
		action1.AddPrecondition(new GoapWorldState("IsRangedWeaponLoaded", WorldStateOperator.Equals, true));
		action1.AddPrecondition(new GoapWorldState("IsTargetInRange", WorldStateOperator.Equals, true));
		action1.AddEffect(new GoapWorldState("IsTargetDead", WorldStateOperator.Equals, true));
		action1.Cost = 3;

		GoapAction action2 = new GoapAction("Load Ranged weapon");
		action2.AddPrecondition(new GoapWorldState("HasRangedWeapon", WorldStateOperator.Equals, true));
		action2.AddEffect(new GoapWorldState("IsRangedWeaponLoaded", WorldStateOperator.Equals, true));
		action2.Cost = 2;

		GoapAction action3 = new GoapAction("Pick up Ranged weapon");
		action3.AddPrecondition(new GoapWorldState("NearWeapon", WorldStateOperator.Equals, true));
		action3.AddPrecondition(new GoapWorldState("HasMeleeWeapon", WorldStateOperator.Equals, false));
		action3.AddEffect(new GoapWorldState("HasRangedWeapon", WorldStateOperator.Equals, true));
		action3.AddEffect(new GoapWorldState("IsTargetInRange", WorldStateOperator.Equals, true));
		action3.Cost = 1;

		GoapAction action4 = new GoapAction("Melee Attack Target");
		action4.AddPrecondition(new GoapWorldState("HasMeleeWeapon", WorldStateOperator.Equals, true));
		action4.AddPrecondition(new GoapWorldState("IsTargetNear", WorldStateOperator.Equals, true));
		action4.AddEffect(new GoapWorldState("IsTargetDead", WorldStateOperator.Equals, true));
		action4.Cost = 2;

		GoapAction action5 = new GoapAction("Pick up Melee weapon");
		action5.AddPrecondition(new GoapWorldState("NearWeapon", WorldStateOperator.Equals, true));
		action5.AddEffect(new GoapWorldState("HasMeleeWeapon", WorldStateOperator.Equals, true));
		action5.Cost = 1;

		GoapAction action6 = new GoapAction("Switch to Ranged Weapon");
		action6.AddPrecondition(new GoapWorldState("NearWeapon", WorldStateOperator.Equals, true));
		action6.AddPrecondition(new GoapWorldState("HasMeleeWeapon", WorldStateOperator.Equals, true));
		action6.AddEffect(new GoapWorldState("IsTargetInRange", WorldStateOperator.Equals, true));
		action6.AddEffect(new GoapWorldState("HasRangedWeapon", WorldStateOperator.Equals, true));
		action6.Cost = 1;

		GoapAction action7 = new GoapAction("Run up to target");
		action7.AddPrecondition(new GoapWorldState("IsTargetNear", WorldStateOperator.Equals, false));
		action7.AddEffect(new GoapWorldState("IsTargetNear", WorldStateOperator.Equals, true));
		action7.AddEffect(new GoapWorldState("NearWeapon", WorldStateOperator.Equals, false));
		action7.Cost = 2;

		GoapAction action8 = new GoapAction("Dummy Action");
		action8.AddPrecondition(new GoapWorldState("foo", WorldStateOperator.Equals, false));
		action8.AddEffect(new GoapWorldState("bar", WorldStateOperator.Equals, true));
		action8.Cost = 2;

		GoapAction action9 = new GoapAction("Dummy Action");
		action9.AddPrecondition(new GoapWorldState("foo", WorldStateOperator.Equals, false));
		action9.AddEffect(new GoapWorldState("bar", WorldStateOperator.Equals, true));
		action9.Cost = 2;

		GoapAction action10 = new GoapAction("Dummy Action");
		action10.AddPrecondition(new GoapWorldState("foo", WorldStateOperator.Equals, false));
		action10.AddEffect(new GoapWorldState("bar", WorldStateOperator.Equals, true));
		action10.Cost = 2;

		GoapAction action11 = new GoapAction("Dummy Action");
		action11.AddPrecondition(new GoapWorldState("foo", WorldStateOperator.Equals, false));
		action11.AddEffect(new GoapWorldState("bar", WorldStateOperator.Equals, true));
		action11.Cost = 2;

		GoapAction action12 = new GoapAction("Dummy Action");
		action12.AddPrecondition(new GoapWorldState("foo", WorldStateOperator.Equals, false));
		action12.AddEffect(new GoapWorldState("bar", WorldStateOperator.Equals, true));
		action12.Cost = 2;
		

		List<GoapWorldState> goalStates = new List<GoapWorldState>
		{
			new GoapWorldState("IsTargetDead", WorldStateOperator.Equals, true)	
		};

		GoapGoal goal = new GoapGoal();
		goal.GoalStates = goalStates;

		List<GoapWorldState> current = new List<GoapWorldState>
		{
			new GoapWorldState("NearWeapon", WorldStateOperator.Equals, true),
			new GoapWorldState("HasRangedWeapon", WorldStateOperator.Equals, false),
			new GoapWorldState("IsRangedWeaponLoaded", WorldStateOperator.Equals, false),
			new GoapWorldState("HasMeleeWeapon", WorldStateOperator.Equals, false),
			new GoapWorldState("IsTargetInRange", WorldStateOperator.Equals, false),
			new GoapWorldState("IsTargetNear", WorldStateOperator.Equals, false),
		};

		planner.InjectActions(new List<GoapAction>{action1, action2, action3, action4, action5, action6, action7, action8, action9, action10, action11});

		//start time
		_stopWatch.Start();
		Queue<GoapAction> actions = null;
		for(int i=0; i<1; i++)
		{
			actions = planner.GetActionQueue(goal, current);
		}


		//stop time
		_stopWatch.Stop();



		if(actions != null)
		{
			Debug.Log("GoapUnitTest: found best path! Dequeuing actions");
			while(actions.Count() > 0)
			{
				GoapAction action = actions.Dequeue();
				Debug.Log("Dequeued action " + action.Name);
			}
		}

		Debug.Log("GoapUnitTest: elapsed time: " + _stopWatch.ElapsedMilliseconds);
		_stopWatch.Reset();
		*/
	}
}
