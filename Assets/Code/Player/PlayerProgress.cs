using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerProgress
{
	public List<string> DiscoveredTopics;//contains topic ID, not topic title
	public List<List<string>> JournalEntries; //journal entries apprended directly to it. Each day at the end of the day, a new day entry is added

	public PlayerProgress()
	{
		DiscoveredTopics = new List<string>();
		DiscoveredTopics.Add("your_story");

		JournalEntries = new List<List<string>>();
		AddJournalEntry(GameManager.Inst.DBManager.DBHandlerStoryEvent.LoadJournalEntry(1));


		/*
		//manually populate entries for journal for testing
		List<string> day1 = new List<string>(){
			"PlayerUnknown's Battlegrounds, Dead by Daylight and Friday the 13th were all made with Unreal Engine 4. Every comment I see about these games is about how janky they are when you play online, with issues such as rubber banding or de-sync.",
			"I thought it was purely a P2P problem but PlayerUnknown's Battlegrounds has dedicated servers and still has some of these issues. I'm guessing they all use Unreal's built in network support. Is it just bad?",
			"I'm looking for an online demo of game mechanics, one that has parameters that can easily be tweaked by someone with no prior knowledge e.g. a student. Is there such a thing? I've googled around and found simple demos here but I'm after something more complete.",
			"I'm sorry!",
			"This is terrible performance compared to NG where the two games have 1459 plays (admittedly they've been up longer but the traffic dies after a couple of days anyways) and a lot of good feedback about how to improve the games. It was actually fun to create those NG games, players started following me and stuff.",
			"ps: You should also read ducks post carefully. GetComponentsInChildren will also include the Transform of the parent object as well, also keep in mind that you iterate over all Transform components which are a child or a child of a child. It iterates over the whole hierarchy downwards.",
			"Bonus info: Color Fill was a crazy attempt to make an online realtime pvp game. I knew it would fail because I can't get enough players to play simultaneously. Still it got quite a few sessions. I'm yet to confirm this from analytics but I believe people failed to find an opponent and came back a lot of times. Means that players are interested in such games! I might try these more, just have to figure out how to get enough players.",
			"This C# class will extend all GameObjects to allow easy walking of an entire transform tree. It's inclusive, so it hits the root first, and then all the children, as deep as they go. If this looks strange, or too good to be true, google C# extension methods.",
			"PlayerUnknown's Battlegrounds, Dead by Daylight and Friday the 13th were all made with Unreal Engine 4. Every comment I see about these games is about how janky they are when you play online, with issues such as rubber banding or de-sync.",
			"I thought it was purely a P2P problem but PlayerUnknown's Battlegrounds has dedicated servers and still has some of these issues. I'm guessing they all use Unreal's built in network support. Is it just bad?",
			"I'm looking for an online demo of game mechanics, one that has parameters that can easily be tweaked by someone with no prior knowledge e.g. a student. Is there such a thing? I've googled around and found simple demos here but I'm after something more complete.",
			"I'm sorry!",
			"This is terrible performance compared to NG where the two games have 1459 plays (admittedly they've been up longer but the traffic dies after a couple of days anyways) and a lot of good feedback about how to improve the games. It was actually fun to create those NG games, players started following me and stuff.",
			"ps: You should also read ducks post carefully. GetComponentsInChildren will also include the Transform of the parent object as well, also keep in mind that you iterate over all Transform components which are a child or a child of a child. It iterates over the whole hierarchy downwards.",

		};
		List<string> day2 = new List<string>(){
			"I've checked the wikis, but I can't seem to find anything specific on \"How to write properly structured code that is conducive to gamedev, and scalable for games to expand as development continues and the game is updated.\"",
			"Note however, that results from GetComponentsInChildren will also include that component from the object itself. So the name is slightly misleading - it should be thought of as \"Get Components from Self And Children\"!",
			"You probably want to set child.localPosition. child.position sets a world position which gets translated into the local position behind the scene.",
			"Usually the first solution in ducks post is the preferred one. GetComponentsInChildren is only used in special cases.",
			"I submitted two of my first NG games to Kongregate and the reception was really bad. Ratings are around 1.5-1.9 and I have 44 plays at the moment of which most quit the game in a matter of seconds. Also the games run on lower fps for some reason, probably all the ads eating up the juice.",

		};

		JournalEntries.Add(day1);
		JournalEntries.Add(day2);
		*/
	}

	public void AddJournalEntry(string entry)
	{
		if(JournalEntries.Count <= 0)
		{
			List<string> newDay = new List<string>();
			JournalEntries.Add(newDay);
		}
		JournalEntries[JournalEntries.Count - 1].Add(entry);
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
}
