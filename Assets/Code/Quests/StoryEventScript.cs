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

	public void Trigger()
	{
		foreach(string line in Script)
		{
			string [] tokens = line.Split(new char[]{' '}, System.StringSplitOptions.None);

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
			}
		}




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
}
