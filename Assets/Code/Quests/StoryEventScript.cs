using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
