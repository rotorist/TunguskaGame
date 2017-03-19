using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestManager 
{
	public QuestBase CurrentQuest;

	public Dictionary<string, StoryCondition> StoryConditions;

	public void Initialize()
	{
		StoryConditions = new Dictionary<string, StoryCondition>();

		//CurrentQuest = new WaveDefenseQuest();
		//CurrentQuest.StartQuest();

		//populate story conditions manually for now
		 
		StoryConditionItem cond1 = new StoryConditionItem();
		cond1.ID = "hastomatoseeds";
		cond1.ItemID = "mutantheart";
		StoryConditions.Add(cond1.ID, cond1);
	}

	public void PerSecondUpdate()
	{
		//CurrentQuest.PerSecondUpdate();
	}
}
