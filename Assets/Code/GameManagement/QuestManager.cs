using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestManager 
{
	public QuestBase CurrentQuest;

	public Dictionary<string, StoryCondition> StoryConditions;
	public Dictionary<string, StoryEvent> StoryEvents;

	public void Initialize()
	{
		StoryConditions = new Dictionary<string, StoryCondition>();
		StoryEvents = new Dictionary<string, StoryEvent>();
		//CurrentQuest = new WaveDefenseQuest();
		//CurrentQuest.StartQuest();

		//populate story conditions manually for now
		 
		StoryConditionItem cond1 = new StoryConditionItem();
		cond1.ID = "hastomatoseeds";
		cond1.ItemID = "mutantheart";
		StoryConditions.Add(cond1.ID, cond1);

		StoryEventDoor event1 = new StoryEventDoor();
		event1.ID = "Level1RoadBlockGateOpen";
		event1.IsOpen = true;
		event1.TargetDoorName = "Level1RoadBlockGate";
		StoryEvents.Add(event1.ID, event1);

		StoryEventDoor event2 = new StoryEventDoor();
		event2.ID = "Level1RoadBlockGateClose";
		event2.IsOpen = false;
		event2.TargetDoorName = "Level1RoadBlockGate";
		StoryEvents.Add(event2.ID, event2);

		StoryEventToggleComponent event3 = new StoryEventToggleComponent();
		event3.ID = "Level1TurnOnIrrigator";
		event3.IsOn = false;
		event3.TargetName = "FarmIrrigator";
		event3.Initialize();
		StoryEvents.Add(event3.ID, event3);

	}

	public void PerSecondUpdate()
	{
		//CurrentQuest.PerSecondUpdate();
	}
}
