using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerProgress
{
	public string PlayerFirstName;
	public string PlayerLastName;
	public List<string> DiscoveredTopics;//contains topic ID, not topic title
	public List<List<string>> JournalEntries; //journal entries apprended directly to it. Each day at the end of the day, a new day entry is added
	public List<int> IncompleteTasks;
	public List<int> CompletedTasks;

	public PlayerProgress()
	{
		PlayerFirstName = "Leov";
		PlayerLastName = "Kravshenko";
		DiscoveredTopics = new List<string>();
		DiscoveredTopics.Add("your_story");
		DiscoveredTopics.Add("zsk_locker");

		JournalEntries = new List<List<string>>();
		AddJournalEntry(GameManager.Inst.DBManager.DBHandlerStoryEvent.LoadJournalEntry(1));

		IncompleteTasks = new List<int>();
		IncompleteTasks.Add(0);
		IncompleteTasks.Add(1);

		CompletedTasks = new List<int>();
		CompletedTasks.Add(4);


	}

	public void AddJournalEntry(string entry)
	{
		//need to search through all days and make sure there is no duplicates
		foreach(List<string> day in JournalEntries)
		{
			foreach(string e in day)
			{
				if(string.Compare(e, entry) == 0)
				{
					return;
				}
			}
		}

		if(JournalEntries.Count <= 0)
		{
			List<string> newDay = new List<string>();
			JournalEntries.Add(newDay);
		}
		JournalEntries[JournalEntries.Count - 1].Add(entry);
		if(GameManager.Inst.UIManager != null)
		{
			GameManager.Inst.UIManager.SetConsoleText("Journal entry added.");
			GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("PencilWrite"), 0.2f);
		}
	}

	public bool IsTopicDiscovered(string topicID)
	{
		return DiscoveredTopics.Contains(topicID);
	}

	public void AddDiscoveredTopic(string topicID)
	{
		if(!DiscoveredTopics.Contains(topicID))
		{
			DiscoveredTopics.Add(topicID);
		}
	}

	public void RemoveDiscoveredTopics(string topicID)
	{
		if(DiscoveredTopics.Contains(topicID))
		{
			DiscoveredTopics.Remove(topicID);
		}
	}

	public void AddNewTask(int id)
	{
		if(IncompleteTasks.Contains(id) || CompletedTasks.Contains(id))
		{
			return;
		}

		IncompleteTasks.Add(id);

		GameManager.Inst.UIManager.SetConsoleText("New task added to the list.");
		GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("Checkmark"), 0.2f);
	}

	public void ResolveTask(int id)
	{
		if(!IncompleteTasks.Contains(id) || CompletedTasks.Contains(id))
		{
			return;
		}

		IncompleteTasks.Remove(id);
		CompletedTasks.Add(id);

		GameManager.Inst.UIManager.SetConsoleText("Task completed!");
		GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("PencilWrite"), 0.2f);
	}
}
