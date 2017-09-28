using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.SqliteClient;
using System.Data;
using System.Text;
using System.Xml;
using System.IO;
using System;

public class DBHandlerDialogue 
{
	public XmlDocument CurrentDialogXML;
	public List<string> FirstNames;
	public List<string> LastNames;

	public List<Topic> GetPlayerTopics()
	{
		List<Topic> topics = new List<Topic>();

		//topics.Add(new Topic("1", "This place", TopicType.Info));
		//topics.Add(new Topic("2", "Life in the Zone", TopicType.Info));

		return topics;
	}

	public List<Topic> GetNPCTopics(Character npc, Character initiator)
	{
		List<Topic> topics = new List<Topic>();

		if(npc.GetComponent<Trader>() != null)
		{
			topics.Add(new Topic("3", "Trade", TopicType.Trade));
		}

		topics.Add(new Topic("5", "Change Subject", TopicType.Return));
		topics.Add(new Topic("4", "Goodbye", TopicType.Exit));

		//topics.Add(new Topic("1", "This place", TopicType.Info));
		//topics.Add(new Topic("2", "Life in the Zone", TopicType.Info));

		XmlNodeList topicList = CurrentDialogXML.GetElementsByTagName("topic");

		int tempIndex = 0;

		if(topicList.Count > 0)
		{
			foreach(XmlNode topic in topicList)
			{
				string id = topic.Attributes["id"].Value;
				string response = "";
				string nextNode = "";
				string title = "";

				XmlNodeList nodeContent = topic.ChildNodes;
				foreach(XmlNode nodeItem in nodeContent)
				{
					XmlAttributeCollection attributes = nodeItem.Attributes;

						
					if(nodeItem.Name == "response")
					{
						DialogueResponse resp = GetDialogueResponse(nodeItem);
						response = resp.Text;
					}

					if(nodeItem.Name == "next_node")
					{
						nextNode = attributes["id"].Value;
					}

					if(nodeItem.Name == "title")
					{
						title = nodeItem.InnerText;
					}
						
				}

				if(id == "")
				{
					id = "temp" + tempIndex;
				}
				if(title == "")
				{
					title = GetTopicTitle(id);
				}

				Topic newTopic = new Topic(id, title, TopicType.Info);

				newTopic.Response = response;
				newTopic.NextNode = nextNode;


				topics.Add(newTopic);


				tempIndex ++;
			}
		}

		return topics;
	}

	public string GetTopicTitle(string id)
	{

		if(id == "1")
		{
			return "This place";
		}
		else if(id == "2")
		{
			return "How is life";
		}
		else 
		{
			return "Placeholder";
		}
	}


	public DialogueHandle LoadNPCDialogue(HumanCharacter npc)
	{
		if(npc == null)
		{
			return null;
		}

		XmlDocument xmlDoc = new XmlDocument();
		string path = Application.dataPath + "/GameData/Dialogue/";
		string dialogueID = npc.CharacterID;
		if(!File.Exists(path + dialogueID + ".xml"))
		{
			dialogueID = npc.SquadID;
			if(!File.Exists(path + dialogueID + ".xml"))
			{
				dialogueID = npc.Faction.ToString();
				if(!File.Exists(path + dialogueID + ".xml"))
				{
					dialogueID = "Default";
				}
			}
		}



		string file = File.ReadAllText(path + dialogueID + ".xml");
		try
		{
			xmlDoc.LoadXml(file);
		}
		catch (XmlException)
		{
			return null;
		}

		CurrentDialogXML = xmlDoc;

		string introText = "";
		string nextNode = "";

		DialogueHandle handle = new DialogueHandle();

		if(GetDialogueIntro(out introText, out nextNode))
		{
			handle.NextNode = nextNode;
			handle.IntroText = introText;

			return handle;
		}

		return null;
	}

	public DialogueNode GetDialogueNode(string id)
	{
		XmlElement node = CurrentDialogXML.GetElementById(id);
		if(node == null)
		{
			return null;
		}

		XmlNodeList nodeContent = node.ChildNodes;
		DialogueNode dialogueNode = new DialogueNode();

		foreach(XmlNode nodeItem in nodeContent)
		{
			if(nodeItem.Name == "response")
			{
				DialogueResponse response = GetDialogueResponse(nodeItem);
				dialogueNode.Responses.Add(response);
			}
			else if(nodeItem.Name == "option")
			{
				Topic option = GetDialogueOption(nodeItem);
				dialogueNode.Options.Add(option);
			}
		}

		return dialogueNode;
	}

	public Topic GetDialogueOption(XmlNode node)
	{
		XmlNodeList nodeContent = node.ChildNodes;
		Topic topic = new Topic();

		XmlAttributeCollection nodeAttributes = node.Attributes;
		if(nodeAttributes["id"] != null)
		{
			topic.ID = nodeAttributes["id"].Value;
		}

		foreach(XmlNode nodeItem in nodeContent)
		{
			XmlAttributeCollection attributes = nodeItem.Attributes;

			if(nodeItem.Name == "condition" || nodeItem.Name == "logic")
			{
				topic.Conditions = ParseConditionList(nodeItem);
			}
			else if(nodeItem.Name == "title")
			{
				topic.Title = nodeItem.InnerText;
			}
			else if(nodeItem.Name == "text")
			{
				topic.Request = nodeItem.InnerText;
			}
			else if(nodeItem.Name == "next_node")
			{
				topic.NextNode = attributes["id"].Value;
			}

		}

		return topic;
	}

	public DialogueResponse GetDialogueResponse(XmlNode node)
	{
		XmlNodeList nodeContent = node.ChildNodes;

		DialogueResponse response = new DialogueResponse();

		foreach(XmlNode nodeItem in nodeContent)
		{
			XmlAttributeCollection attributes = nodeItem.Attributes;

			if(nodeItem.Name == "condition" || nodeItem.Name == "logic")
			{
				response.Conditions = ParseConditionList(nodeItem);
			}
			else if(nodeItem.Name == "text")
			{
				response.Text = nodeItem.InnerText;
			}
			else if(nodeItem.Name == "event")
			{
				response.Events.Add(attributes["name"].Value);
			}
		}

		return response;
	}

	public string GetGlobalResponse(string id, string characterName)
	{
		IDataReader reader = GameManager.Inst.DBManager.RunQuery(
			"SELECT response FROM global_dialogue_response WHERE id = '" + id + "'");

		List<string> output = new List<string>();

		while(reader.Read())
		{
			output.Add(reader.GetString(0));
		}

		int hash = GetNameHash(characterName);



		return output[hash % output.Count];
	}


	public bool GetDialogueIntro(out string text, out string nextNode)
	{
		text = "";
		nextNode = "";

		XmlNodeList intros = CurrentDialogXML.GetElementsByTagName("intro");

		if(intros.Count <= 0)
		{
			
			return false;
		}

		foreach(XmlNode intro in intros)
		{
			XmlNodeList nodeContent = intro.ChildNodes;
			foreach(XmlNode nodeItem in nodeContent)
			{
				if(nodeItem.Name == "text")
				{
					text = nodeItem.InnerText;
				}

				if(nodeItem.Name == "next_node")
				{
					nextNode = nodeItem.Attributes["id"].Value;
				}
			}
		}

		return true;
	}

	public string GetRandomName()
	{
		if(FirstNames == null || FirstNames.Count <= 0)
		{
			LoadNames();
		}

		int rand1 = UnityEngine.Random.Range(0, FirstNames.Count);
		int rand2 = UnityEngine.Random.Range(0, LastNames.Count);

		return FirstNames[rand1] + " " + LastNames[rand2];
	}




	private Stack<ConditionToken> ParseConditionList(XmlNode nodeItem)
	{
		Stack<ConditionToken> tokensReverse = new Stack<ConditionToken>();

		GetTokens(nodeItem, tokensReverse);
		Debug.Log("toeknsreverse count " + tokensReverse.Count);

		return tokensReverse;
	}

	private void GetTokens(XmlNode nodeItem, Stack<ConditionToken> theStack)
	{
		if(nodeItem.Name == "condition")
		{
			
			XmlAttributeCollection attributes = nodeItem.Attributes;
			Debug.Log("found condition " + attributes["story"].Value);
			DialogueCondition condition = new DialogueCondition();

			condition.ID = attributes["name"].Value;
			if(condition.ID.Length > 0)
			{


				if(attributes["story"] != null)
				{
					condition.StoryTriggerID = attributes["story"].Value;
				}

				if(attributes["compare"] != null)
				{
					condition.StoryTriggerCompare = Convert.ToInt32(attributes["compare"].Value);
				}

				if(attributes["op"] != null)
				{
					condition.StoryTriggerCompareOp = Convert.ToInt32(attributes["op"].Value);
				}

				theStack.Push(condition);
			}

		}
		else if(nodeItem.Name == "logic")
		{
			DialogueConditionOperator op = new DialogueConditionOperator();
			XmlAttributeCollection attributes = nodeItem.Attributes;
			op.Op = (LogicOperator)Enum.Parse(typeof(LogicOperator), attributes["type"].Value);
			theStack.Push(op);

			XmlNodeList nodeContent = nodeItem.ChildNodes;
			if(nodeContent.Count > 0)
			{
				foreach(XmlNode child in nodeContent)
				{
					Debug.Log("Found logic child");
					GetTokens(child, theStack);
				}
			}
		}
	}

	private int GetNameHash(string characterName)
	{
		//create a hash from characterName
		int hash = 0;
		foreach(char c in characterName)
		{
			int i = (int)c % 32;
			hash += i;
		}


		return hash;
	}

	private void LoadNames()
	{
		string [] rawFileFirstNames;

		try
		{
			rawFileFirstNames = File.ReadAllLines(Application.dataPath + "/GameData/Names/FirstNames.txt");
		}
		catch(Exception e)
		{
			UnityEngine.Debug.LogError(e.Message);
			return;
		}

		FirstNames = new List<string>();

		foreach(string line in rawFileFirstNames)
		{
			FirstNames.Add(line);
		}

		string [] rawFileLastNames;

		try
		{
			rawFileLastNames = File.ReadAllLines(Application.dataPath + "/GameData/Names/LastNames.txt");
		}
		catch(Exception e)
		{
			UnityEngine.Debug.LogError(e.Message);
			return;
		}

		LastNames = new List<string>();

		foreach(string line in rawFileLastNames)
		{
			LastNames.Add(line);
		}



	}
}
