using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QuestDebugPanel : PanelBase 
{
	public Transform LocEntryAnchor;
	public UILabel ConditionsList;
	public UILabel TopicList;
	public UILabel Output;
	public UIInput Command;

	public List<GameObject> LocEntries;

	public LinkedList<string> CommandHistory;
	public LinkedListNode<string> CurrentNode;

	public override void Initialize ()
	{
		CommandHistory = new LinkedList<string>();
		Hide();
	}

	public override void PerFrameUpdate ()
	{
		if(IsActive && Command.isSelected)
		{
			if(Input.GetKeyDown(KeyCode.UpArrow))
			{
				string command = CurrentNode.Value;
				Command.value = command;
				if(CurrentNode.Next != null)
				{
					CurrentNode = CurrentNode.Next;
				}
			}
			if(Input.GetKeyDown(KeyCode.DownArrow))
			{
				if(CurrentNode.Previous != null)
				{
					CurrentNode = CurrentNode.Previous;
					string command = CurrentNode.Value;
					Command.value = command;
				}
				else
				{
					Command.value = null;
				}
			}

		}


		
	}

	public override void Show ()
	{
		Time.timeScale = 0;

		NGUITools.SetActive(this.gameObject, true);
		this.IsActive = true;

		Populate();

		InputEventHandler.Instance.State = UserInputState.WindowsOpen;

	}

	public override void Hide ()
	{
		UIEventHandler.Instance.TriggerCloseWindow();
		Time.timeScale = 1;

		NGUITools.SetActive(this.gameObject, false);
		this.IsActive = false;


		InputEventHandler.Instance.State = UserInputState.Normal;
	}

	public override bool HasBodySlots (out List<BodySlot> bodySlots)
	{
		bodySlots = null;
		return false;
	}

	public override bool HasInventoryGrids (out List<InventoryGrid> grids)
	{
		grids = null;
		return false;
	}

	public override bool HasTempSlots (out List<TempSlot> tempSlots)
	{
		tempSlots = null;
		return false;
	}

	public void OnSubmitCommand()
	{
		string input = UIInput.current.value;
		UIInput.current.value = null;

		CommandHistory.AddFirst(input);
		CurrentNode = CommandHistory.First;

		string [] tokens = input.Split(new char[]{' '}, System.StringSplitOptions.RemoveEmptyEntries);
		if(tokens[0] == "god")
		{
			if(tokens[1] == "off")
			{
				GameManager.Inst.GodMode = false;
			}
			else
			{
				GameManager.Inst.GodMode = true;
			}

			Output.text = "God mode: " + GameManager.Inst.GodMode;

			return;
		}
		else if(tokens[0] == "set")
		{
			string condName = tokens[1];
			string condValue = tokens[2];
			int value;
			StoryCondition condition;

			try
			{
				condition = GameManager.Inst.QuestManager.StoryConditions[condName];
			}
			catch
			{
				Output.text = "Condition doesn't exist!";
				return;
			}

			if(condValue == "active")
			{
				condition.IsActive = true;
				Output.text = "Activated " + condName;
			}
			else if(condValue == "inactive")
			{
				condition.IsActive = false;
				Output.text = "Deactivated " + condName;
			}
			else
			{
				try
				{
					value = Convert.ToInt32(tokens[2]);
				}
				catch
				{
					Output.text = "Cannot parse value, must be integer!";
					return;
				}

				try
				{
					condition.SetValue(value);
				}
				catch
				{
					Output.text = "Unable to set value!";
					return;
				}

				Output.text = "Set condition " + condName + " to " + value;
			}


			Populate();
			return;
		}



	}



	private void Populate()
	{
		string list = "";
		foreach(KeyValuePair<string, StoryCondition> condition in GameManager.Inst.QuestManager.StoryConditions)
		{
			list = list + condition.Value.ID + " (" + condition.Value.IsActive + ") = " + condition.Value.GetValue().ToString() + "\n";

		}

		ConditionsList.text = list;

		list = "";
		foreach(string topic in GameManager.Inst.PlayerProgress.DiscoveredTopics)
		{
			list = list + topic + "\n";
		}

		TopicList.text = list;

		int y = 0;
		GameObject [] markers = GameObject.FindGameObjectsWithTag("LocMarker");
		//remove existing loc entries
		foreach(GameObject locEntry in LocEntries)
		{
			GameObject.Destroy(locEntry);
		}

		LocEntries.Clear();

		foreach(GameObject o in markers)
		{
			LocationMarker marker = o.GetComponent<LocationMarker>();
			GameObject entry = GameObject.Instantiate(Resources.Load("LocationEntry")) as GameObject;
			entry.transform.parent = LocEntryAnchor;
			entry.transform.localPosition = Vector3.zero - new Vector3(0, y, 0);
			entry.transform.localScale = new Vector3(1, 1, 1);
			LocEntries.Add(entry);

			LocEntry loc = entry.GetComponent<LocEntry>();
			loc.LocName.text = marker.Name;
			loc.Location = marker.transform.position;



			y += 45;
		}

	}



}
