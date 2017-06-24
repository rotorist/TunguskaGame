using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
			}
		}

		return true;


	}



	private bool CheckPrerequisite(string [] tokens, object [] parameters)
	{
		if(tokens[1] == "params")
		{
			int paramNumber = Convert.ToInt32(tokens[2]);
			string operation = tokens[3];
			string compValue = tokens[4];
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
				condition.SetValue(Convert.ToInt32(1));
			}
			else if(tokens[2] == "false")
			{
				condition.SetValue(Convert.ToInt32(0));
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
}
