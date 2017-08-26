using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestManager 
{
	public QuestBase CurrentQuest;

	public Dictionary<string, StoryCondition> StoryConditions;
	public Dictionary<string, StoryEventScript> Scripts;



	public void Initialize()
	{
		Scripts = GameManager.Inst.DBManager.DBHandlerStoryEvent.LoadScripts();
		if(StoryConditions == null)
		{
			StoryConditions = GameManager.Inst.DBManager.DBHandlerStoryEvent.LoadStoryConditions();
		}

		//CurrentQuest = new WaveDefenseQuest();
		//CurrentQuest.StartQuest();

		//populate story conditions manually for now
		/*
		StoryConditionItem cond1 = new StoryConditionItem();
		cond1.ID = "hastomatoseeds";
		cond1.ItemID = "mutantheart";
		StoryConditions.Add(cond1.ID, cond1);

		StoryConditionTrigger cond2 = new StoryConditionTrigger();
		cond2.ID = "zsk_village_gate_open";
		cond2.SetValue(0);
		StoryConditions.Add(cond2.ID, cond2);

		StoryConditionTrigger cond3 = new StoryConditionTrigger();
		cond3.ID = "zsk_sid_quest";
		cond3.SetValue(0);
		StoryConditions.Add(cond3.ID, cond3);

		StoryConditionTrigger cond4 = new StoryConditionTrigger();
		cond4.ID = "zsk_barn_cleared";
		cond4.SetValue(0);
		StoryConditions.Add(cond4.ID, cond4);

		StoryConditionTrigger cond5 = new StoryConditionTrigger();
		cond5.ID = "zsk_barn_water_on";
		cond5.SetValue(0);
		StoryConditions.Add(cond5.ID, cond5);
		*/


		/*
		StoryEventScript script1 = new StoryEventScript();
		script1.Script.Add("door/Level1RoadBlockGate/toggle");
		Scripts.Add("zsk_roadblockgate_toggle", script1);

		StoryEventScript script2 = new StoryEventScript();
		script2.Script.Add("object/FarmIrrigatorHandle/on");
		script2.Script.Add("condition/zsk_barn_water_on/true");
		Scripts.Add("zsk_irrigator_on", script2);

		StoryEventScript script3 = new StoryEventScript();
		script3.Script.Add("door/ZernaskayaSheetFenceDoor/unlock");
		script3.Script.Add("condition/zsk_village_gate_open/true");
		script3.Script.Add("condition/zsk_sid_quest/1");
		script3.Script.Add("hook/zsk_barn_clear_done/OnSquadDeath");
		Scripts.Add("zsk_village_exit_unlock", script3);

		StoryEventScript script4 = new StoryEventScript();
		script4.Script.Add("if/param/0/is/zsk_barn_legionnaires");
		script4.Script.Add("condition/zsk_barn_cleared/true");
		script4.Script.Add("message/Barn has been cleared!");
		Scripts.Add("zsk_barn_clear_done", script4);

		StoryEventScript script5 = new StoryEventScript();
		script5.Script.Add("condition/zsk_sid_quest/2");
		Scripts.Add("zsk_barn_quest_done1", script5);

		StoryEventScript script6 = new StoryEventScript();
		script6.Script.Add("condition/zsk_sid_quest/2");
		script6.Script.Add("item/receive/huntingshotgun/1");
		script6.Script.Add("item/receive/ammo12shot/10");
		Scripts.Add("zsk_barn_quest_done2", script6);
		*/



		//run initial setup script
		//ONLY to be run when creating a new game
		if(Scripts.ContainsKey("new_game_setup"))
		{
			Scripts["new_game_setup"].Trigger(new object[]{});
		}

		//run initial testing scsripts
		//SetSidQuestStage(5);
	}

	public void PerSecondUpdate()
	{
		//CurrentQuest.PerSecondUpdate();
	}

	private void SetSidQuestStage(int stage)
	{
		if(stage > 0)
		{
			SetSidQuestStage(stage - 1);
		}

		if(stage == 1)
		{
			Scripts["zsk_village_exit_unlock"].Trigger(new object[]{});
		}
		else if(stage == 2)
		{
			Scripts["zsk_barn_quest_done1"].Trigger(new object[]{});
		}
		else if(stage == 3)
		{
			Scripts["zsk_sid_church_begin"].Trigger(new object[]{});
		}
		else if(stage == 4)
		{
			Scripts["zsk_sid_church_quest_done"].Trigger(new object[]{});
		}
		else if(stage == 5)
		{
			Scripts["zsk_sid_gas_station_begin"].Trigger(new object[]{});
		}


	}

}

