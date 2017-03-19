using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Topic 
{
	public string ID;
	public string Title;

	public TopicType Type;
	public string Request;
	public string Response;

	public string NextNode;

	public Stack<ConditionToken> Conditions;

	public Topic(string id, string title, TopicType type)
	{
		ID = id;
		Title = title;
		Type = type;

		Conditions = new Stack<ConditionToken>();
	}

	public Topic()
	{
		Conditions = new Stack<ConditionToken>();
	}
}

public enum TopicType
{
	Info,
	Trade,
	Return,
	Exit,
}