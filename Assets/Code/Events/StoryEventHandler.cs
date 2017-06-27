using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate bool StoryEventDelegate(StoryEvent storyEvent);

public class StoryEventListener
{
	public StoryEventListenerType Type;
	public string ScriptName;

	public StoryEventDelegate OnStoryEvent;

}

public class StoryEvent
{
	public StoryEventType Type;
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

	public LinkedList<StoryEventListener> CharacterDeathListeners;
	public LinkedList<StoryEventListener> SquadDeathListeners;
	public LinkedList<StoryEventListener> ApproachLocListeners;
	public LinkedList<StoryEventListener> PlayerTakeItemListeners;
	public LinkedList<StoryEventListener> PlayerDropItemListeners;
	public LinkedList<StoryEventListener> PlayerSellItemListeners;



	public Queue<StoryEvent> StoryEventQueue;



	private StoryEvent _currentStoryEvent;
	private LinkedListNode<StoryEventListener> _currentListenerNode;


	private bool _isCurrentEventDone;
	private LinkedList<StoryEventListener> [] _allListenerLists;



	public StoryEventHandler()
	{
		CharacterDeathListeners = new LinkedList<StoryEventListener>();
		SquadDeathListeners = new LinkedList<StoryEventListener>();
		ApproachLocListeners = new LinkedList<StoryEventListener>();
		PlayerTakeItemListeners = new LinkedList<StoryEventListener>();
		PlayerDropItemListeners = new LinkedList<StoryEventListener>();
		PlayerSellItemListeners = new LinkedList<StoryEventListener>();

		_allListenerLists = new LinkedList<StoryEventListener>[]{
			CharacterDeathListeners,
			SquadDeathListeners,
			ApproachLocListeners,
			PlayerTakeItemListeners,
			PlayerDropItemListeners,
			PlayerSellItemListeners,
		};

		StoryEventQueue = new Queue<StoryEvent>();


	}

	public void PerFrameUpdate()
	{
		if(StoryEventQueue.Count <= 0 && _currentStoryEvent == null)
		{
			return;
		}
		//see if there's event
		if(_currentStoryEvent == null || _isCurrentEventDone)
		{
			try
			{
				_currentStoryEvent = StoryEventQueue.Dequeue();
			}
			catch
			{
				_currentStoryEvent = null;
				return;
			}
				
			_currentListenerNode = null;
			_isCurrentEventDone = false;

		}

		if(_currentStoryEvent == null)
		{
			return;
		}



		//for each frame, execute one listener
		if(_currentListenerNode != null)
		{
			bool isDone = false;

			if(_currentListenerNode.Value.Type == StoryEventListenerType.Delegate)
			{
				isDone = _currentListenerNode.Value.OnStoryEvent(_currentStoryEvent);

			}
			else if(_currentListenerNode.Value.Type == StoryEventListenerType.Script)
			{
				Debug.Log("checking script " + _currentListenerNode.Value.ScriptName);
				if(GameManager.Inst.QuestManager.Scripts.ContainsKey(_currentListenerNode.Value.ScriptName))
				{
					isDone = GameManager.Inst.QuestManager.Scripts[_currentListenerNode.Value.ScriptName].Trigger(_currentStoryEvent.Parameters);
				}
			}


			LinkedListNode<StoryEventListener> currentNode = _currentListenerNode;
			LinkedListNode<StoryEventListener> nextNode = _currentListenerNode.Next;
			if(nextNode == null)
			{
				//we are done here
				_currentListenerNode = null;
				_isCurrentEventDone = true;
			}
			else
			{
				_currentListenerNode = nextNode;
			}

			if(isDone)
			{
				currentNode.List.Remove(currentNode);
				_isCurrentEventDone = true;
			}
		}
		else
		{
			//check if there's any listener for this type of event
			LinkedList<StoryEventListener> listenerList = GetListnerListFromEventType(_currentStoryEvent.Type);
			if(listenerList != null && listenerList.First != null)
			{
				_currentListenerNode = listenerList.First;
				Debug.Log("Found listener");
			}
			else
			{
				//done with this event
				_currentListenerNode = null;
				_isCurrentEventDone = true;

			}

		}
	}

	public void EnqueueStoryEvent(StoryEventType type, object caller, object [] parameters)
	{
		StoryEvent storyEvent = new StoryEvent();
		storyEvent.Type = type;
		storyEvent.Parameters = parameters;
		StoryEventQueue.Enqueue(storyEvent);
		Debug.Log("enqueued story event " + type);
	}

	public void AddScriptListener(string scriptName, StoryEventType eventType)
	{
		StoryEventListener listener = new StoryEventListener();
		listener.Type = StoryEventListenerType.Script;
		listener.ScriptName = scriptName;
		AddListenerToList(listener, eventType);

		Debug.Log("Added script listener " + scriptName);
	}

	public void AddDelegateListener(StoryEventDelegate callBack, StoryEventType eventType)
	{
		StoryEventListener listener = new StoryEventListener();
		listener.Type = StoryEventListenerType.Delegate;
		listener.OnStoryEvent = callBack;
		AddListenerToList(listener, eventType);
	}




	private void AddListenerToList(StoryEventListener listener, StoryEventType eventType)
	{
		switch(eventType)
		{
		case StoryEventType.OnApproachingLoc:
			ApproachLocListeners.AddLast(listener);
			break;
		case StoryEventType.OnCharacterDeath:
			CharacterDeathListeners.AddLast(listener);
			break;
		case StoryEventType.OnSquadDeath:
			SquadDeathListeners.AddLast(listener);
			break;
		case StoryEventType.OnPlayerDropItem:
			PlayerDropItemListeners.AddLast(listener);
			break;
		case StoryEventType.OnPlayerSellItem:
			PlayerSellItemListeners.AddLast(listener);
			break;
		case StoryEventType.OnPlayerTakeItem:
			PlayerTakeItemListeners.AddLast(listener);
			break;
			
		}

	}

	private LinkedList<StoryEventListener> GetListnerListFromEventType(StoryEventType eventType)
	{
		switch(eventType)
		{
		case StoryEventType.OnApproachingLoc:
			return ApproachLocListeners;
			break;
		case StoryEventType.OnCharacterDeath:
			return CharacterDeathListeners;
			break;
		case StoryEventType.OnSquadDeath:
			return SquadDeathListeners;
			break;
		case StoryEventType.OnPlayerDropItem:
			return PlayerDropItemListeners;
			break;
		case StoryEventType.OnPlayerSellItem:
			return PlayerSellItemListeners;
			break;
		case StoryEventType.OnPlayerTakeItem:
			return PlayerTakeItemListeners;
			break;

		}

		return null;
	}


}


public enum StoryEventType
{
	OnCharacterDeath,
	OnSquadDeath,
	OnApproachingLoc,


	OnPlayerTakeItem,
	OnPlayerDropItem,
	OnPlayerSellItem,

}

public enum StoryEventListenerType 
{
	Script,
	Delegate,
}