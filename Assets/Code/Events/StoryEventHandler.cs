using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate bool StoryEventDelegate(StoryEvent storyEvent);

[System.Serializable]
public class StoryEventListener
{
	public StoryEventListenerType Type;
	public string ScriptName;

	public StoryEventDelegate OnStoryEvent;

}

[System.Serializable]
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
	public LinkedList<StoryEventListener> PlayerReadNoteListeners;


	public Queue<StoryEvent> StoryEventQueue;

	public LinkedList<StoryEventListener> [] AllListenerLists
	{
		get { return _allListenerLists; }
	}

	public StoryEvent CurrentStoryEvent
	{
		get { return _currentStoryEvent; }
		set { _currentStoryEvent = value; }
	}

	public bool IsCurrentEventDone
	{
		get { return _isCurrentEventDone; }
		set { _isCurrentEventDone = value; }
	}



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
		PlayerReadNoteListeners = new LinkedList<StoryEventListener>();

		_allListenerLists = new LinkedList<StoryEventListener>[]{
			CharacterDeathListeners,
			SquadDeathListeners,
			ApproachLocListeners,
			PlayerTakeItemListeners,
			PlayerDropItemListeners,
			PlayerSellItemListeners,
			PlayerReadNoteListeners,
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

	public List<StoryEventListener> ConvertLinkedListenerToList(LinkedList<StoryEventListener> linkedList)
	{
		List<StoryEventListener> result = new List<StoryEventListener>();
		if(linkedList != null && linkedList.Count > 0)
		{
			LinkedListNode<StoryEventListener> currentNode = linkedList.First;
			while(currentNode != null)
			{
				result.Add(currentNode.Value);
				currentNode = currentNode.Next;
			}

		}
		return result;
	}

	public LinkedList<StoryEventListener> ConvertListListenerToLinked(List<StoryEventListener> list)
	{
		LinkedList<StoryEventListener> result = new LinkedList<StoryEventListener>();
		for(int i=0; i<list.Count; i++)
		{
			result.AddLast(list[i]);
		}

		return result;
	}

	public List<StoryEvent> ConvertQueueStoryEventToList(Queue<StoryEvent> queue)
	{
		List<StoryEvent> result = new List<StoryEvent>();
		if(queue != null && queue.Count > 0)
		{
			Queue<StoryEvent> queueCopy = new Queue<StoryEvent>(queue);
			while(queueCopy.Count > 0)
			{
				result.Add(queueCopy.Dequeue());
			}
		}

		return result;
	}

	public Queue<StoryEvent> ConvertListStoryEventToQueue(List<StoryEvent> list)
	{
		Queue<StoryEvent> result = new Queue<StoryEvent>();
		for(int i=0; i<list.Count; i++)
		{
			result.Enqueue(list[i]);
		}

		return result;
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
		case StoryEventType.OnPlayerReadNote:
			PlayerReadNoteListeners.AddLast(listener);
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
		case StoryEventType.OnPlayerReadNote:
			return PlayerReadNoteListeners;
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
	OnPlayerReadNote,
}

public enum StoryEventListenerType 
{
	Script,
	Delegate,
}