using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.SqliteClient;
using System.Data;
using System.Text;
using System.Xml;
using System.IO;
using System;

public class StoryEventScript
{
	public List<string> Script;


	public StoryEventScript()
	{
		Script = new List<string>();
	}

	//returns true if script is over and won't be called again
	//otherwise returns false
	public bool Trigger(object [] parameters)
	{
		foreach(string line in Script)
		{
			string [] tokens = line.Split(new char[]{'/'}, System.StringSplitOptions.None);

			switch(tokens[0])
			{
			case "object":
				ExecuteObjectScript(tokens);
				break;
			case "door":
				ExecuteDoorScript(tokens);
				break;
			case "condition":
				ExecuteConditionScript(tokens);
				break;
			case "if":
				if(!CheckPrerequisite(tokens, parameters))
				{
					//script won't run based on parameters
					return false;
				}
				break;
			case "hook":
				ExecuteHookEventScript(tokens);
				break;
			case "message":
				ExecuteMessageScript(tokens);
				break;
			case "item":
				ExecuteItemScript(tokens);
				break;
			case "journal":
				ExecuteJournalScript(tokens);
				break;
			case "topic":
				ExecuteTopicScript(tokens);
				break;
			}
		}

		return true;


	}



	private bool CheckPrerequisite(string [] tokens, object [] parameters)
	{
		if(tokens[1] == "param")
		{
			
			int paramNumber = Convert.ToInt32(tokens[2]);
			string operation = tokens[3];
			string compValue = tokens[4];

			Debug.Log("Check prerequisite, compValue " + tokens[4] + " param " + parameters[paramNumber]);
				
			if(operation == "is")
			{
				//string comparison
				if(compValue == (string)parameters[paramNumber])
				{
					return true;
				}
			}
		}
		else
		{
			
		}

		return false;
	}

	private void ExecuteObjectScript(string [] tokens)
	{
		GameObject o = GameObject.Find(tokens[1]);
		ToggleObject tO = o.GetComponent<ToggleObject>();
		if(tokens[2] == "on")
		{
			tO.Toggle(true);
		}
		else if(tokens[2] == "off")
		{
			tO.Toggle(false);
		}
		else if(tokens[2] == "toggle")
		{
			tO.Toggle();
		}
	}

	private void ExecuteDoorScript(string [] tokens)
	{
		GameObject o = GameObject.Find(tokens[1]);
		Door door = o.GetComponent<Door>();
		if(tokens[2] == "open")
		{
			if(!door.IsOpen)
			{
				door.Open(GameManager.Inst.PlayerControl.SelectedPC.transform);
			}
		}
		else if(tokens[2] == "close")
		{
			if(door.IsOpen)
			{
				door.Close();
			}
		}
		else if(tokens[2] == "toggle")
		{
			if(door.IsOpen)
			{
				door.Close();
			}
			else
			{
				door.Open(GameManager.Inst.PlayerControl.SelectedPC.transform);
			}
		}
		else if(tokens[2] == "lock")
		{
			door.IsLocked = true;
		}
		else if(tokens[2] == "unlock")
		{
			door.IsLocked = false;
		}
	}

	private void ExecuteConditionScript(string [] tokens)
	{
		if(GameManager.Inst.QuestManager.StoryConditions.ContainsKey(tokens[1]))
		{
			StoryCondition condition = GameManager.Inst.QuestManager.StoryConditions[tokens[1]];
			if(tokens[2] == "true")
			{
				condition.SetValue(1);
			}
			else if(tokens[2] == "false")
			{
				condition.SetValue(0);
			}
			else if(tokens[2] == "toggle")
			{
				if(condition.GetValue() == 1)
				{
					condition.SetValue(0);
				}
				else
				{
					condition.SetValue(1);
				}
			}
			else
			{
				condition.SetValue(Convert.ToInt32(tokens[2]));
			}
		}
	}

	private void ExecuteHookEventScript(string [] tokens)
	{
		if(GameManager.Inst.QuestManager.Scripts.ContainsKey(tokens[1]))
		{
			StoryEventType eventType = (StoryEventType)Enum.Parse(typeof(StoryEventType), tokens[2]);
			StoryEventHandler.Instance.AddScriptListener(tokens[1], eventType);
		}
	}

	private void ExecuteMessageScript(string [] tokens)
	{
		
		GameManager.Inst.UIManager.SetConsoleText(tokens[1]);
	}

	private void ExecuteItemScript(string [] tokens)
	{
		if(tokens[1] == "receive")
		{
			string itemID = tokens[2];
			int quantity = Convert.ToInt32(tokens[3]);

			int colPos;
			int rowPos;
			GridItemOrient orientation;
			Item item = GameManager.Inst.ItemManager.LoadItem(itemID);
			HumanCharacter player = GameManager.Inst.PlayerControl.SelectedPC;
			if(player.Inventory.FitItemInBackpack(item, out colPos, out rowPos, out orientation))
			{
				Debug.Log("Found backpack fit " + colPos + ", " + rowPos + " orientation " + orientation);

				GridItemData itemData = new GridItemData(item, colPos, rowPos, orientation, quantity);
				player.Inventory.Backpack.Add(itemData);

				GameManager.Inst.PlayerControl.Party.RefreshAllMemberWeight();

			}
			else
			{
				var resource = Resources.Load(item.PrefabName + "Pickup");
				if(resource != null)
				{
					GameObject pickup = GameObject.Instantiate(resource) as GameObject;
					pickup.transform.position = player.transform.position + new Vector3(UnityEngine.Random.Range(-0.2f, 0.2f), 1f, UnityEngine.Random.Range(-0.2f, 0.2f));
					Transform parent = GameManager.Inst.ItemManager.FindPickupItemParent(pickup.transform);
					if(parent != null)
					{
						pickup.transform.parent = parent;
					}
					pickup.GetComponent<PickupItem>().Item = item;
					pickup.GetComponent<PickupItem>().Quantity = quantity;
				}

			}

			GameManager.Inst.UIManager.SetConsoleText("Received item: " + item.Name + " x " + quantity);

		}
		else if(tokens[1] == "lose")
		{

		}
	}

	private void ExecuteJournalScript(string [] tokens)
	{
		string entry = tokens[1];

		//check if journal entry is text or tag
		if(entry[0] == '{')
		{
			string journalID = entry.Split('{','}')[1];
			string text = GameManager.Inst.DBManager.DBHandlerStoryEvent.LoadJournalEntry(Convert.ToInt32(journalID));
			GameManager.Inst.PlayerProgress.AddJournalEntry(text);
		}
		else
		{
			GameManager.Inst.PlayerProgress.AddJournalEntry(entry);
		}
	}

	private void ExecuteTopicScript(string [] tokens)
	{
		if(tokens[1] == "discover")
		{
			GameManager.Inst.PlayerProgress.AddDiscoveredTopic(tokens[2]);
		}
		else if(tokens[1] == "forget")
		{
			GameManager.Inst.PlayerProgress.RemoveDiscoveredTopics(tokens[2]);
		}
	}
}
