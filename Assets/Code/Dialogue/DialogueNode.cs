using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogueNode
{
	public List<DialogueResponse> Responses;
	public List<Topic> Options;

	public DialogueNode()
	{
		Responses = new List<DialogueResponse>();
		Options = new List<Topic>();
	}
}

public class ConditionToken
{
	public bool IsOperator;
}

public class DialogueCondition : ConditionToken
{
	public string ID;
	public bool IsAND; //true = AND
	public string StoryTriggerID;
	public int StoryTriggerCompare;
	public int StoryTriggerCompareOp;
	public bool Value;

	public DialogueCondition()
	{
		IsOperator = false;
	}
}

public class DialogueConditionOperator : ConditionToken
{
	public LogicOperator Op;

	public DialogueConditionOperator()
	{
		IsOperator = true;
	}
}

public class DialogueResponse
{
	public Stack<ConditionToken> Conditions;
	public List<string> Events;
	public string Text;

	public DialogueResponse()
	{
		
		Conditions = new Stack<ConditionToken>();
		Events = new List<string>();
	}
}



public class DialogueHandle
{
	public string IntroText;
	public string NextNode;
}