using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StoryEvent
{
	public StoryEventType Type;
	public object Caller;
	public object [] Parameters;
}


public class StoryEventHandler
{
	private static StoryEventHandler _instance;
	public static StoryEventHandler Instance	
	{
		get 
		{
			if (_instance == null)
				_instance = new StoryEventHandler();

			return _instance;
		}
	}

	public Queue<StoryEvent> StoryEventQueue;


	public StoryEventHandler()
	{
		StoryEventQueue = new Queue<StoryEvent>();
	}

	public void PerFrameUpdate()
	{
		//for each frame, dequeue one or more events depending on queue size

	}

	public void EnqueueEvent(StoryEventType type, object caller, object [] parameters)
	{

	}

}


public enum StoryEventType
{
	OnCharacterDeath,

	OnApproachingLoc,

	OnPlayerTakeDamage,

	OnPlayerTakeItem,
	OnPlayerDropItem,

}