using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.SqliteClient;
using System.Data;
using System.Text;
using System.Xml;
using System.IO;
using System;

public class DBHandlerItem
{
	public Item LoadItemByID(string id)
	{
		IDataReader itemReader = GameManager.Inst.DBManager.RunQuery(
			"SELECT * from base_items WHERE id='" + id +"'");

		while(itemReader.Read())
		{
			Item item = new Item();
			item.ID = itemReader.GetString(0);
			item.Name = itemReader.GetString(1);
			item.Description = itemReader.GetString(2);
			item.PrefabName = itemReader.GetString(3);
			item.SpriteName = itemReader.GetString(4);
			item.Weight = itemReader.GetFloat(5);
			item.Type = (ItemType)Enum.Parse(typeof(ItemType), itemReader.GetString(6));
			item.GridCols = itemReader.GetInt32(7);
			item.GridRows = itemReader.GetInt32(8);
			item.MaxStackSize = itemReader.GetInt32(9);
			item.Tier = itemReader.GetInt32(10);
			item.BasePrice = itemReader.GetFloat(11);
			item.MaxDurability = itemReader.GetFloat(12);
			item.Durability = itemReader.GetFloat(13);
			item.UseLimit = itemReader.GetInt32(14);
			item.IsUsable = itemReader.GetBoolean(15);
			if(!itemReader.IsDBNull(16))
			{
				item.Attributes = ParseAttributes(itemReader.GetString(16));
			}

			item.BuildIndex();

			return item;
		}

		return null;
	}



	private List<ItemAttribute> ParseAttributes(string blob)
	{
		string [] lines = blob.Split('\n');
		List<ItemAttribute> attributes = new List<ItemAttribute>();
		foreach(string line in lines)
		{
			if(line.Length <= 3)
			{
				continue;
			}

			string [] tokens = line.Split('=');
			if(tokens.Length < 2)
			{
				continue;
			}

			object value = null;
			ItemAttribute attribute = null;
			if(tokens[1][0] == '"')
			{
				//we have a string
				attribute = new ItemAttribute(tokens[0], tokens[1].Substring(1, tokens[1].Length - 2));
			}
			else if(tokens[1][0] == '#')
			{
				//bool
				string boolValue = tokens[1].Substring(1);
				attribute = new ItemAttribute(tokens[0], Convert.ToBoolean(boolValue));
			}
			else if(tokens[1][0] == 'd')
			{
				//int
				string intValue = tokens[1].Substring(1);
				attribute = new ItemAttribute(tokens[0], Convert.ToInt32(intValue));
			}
			else
			{
				//float
				attribute = new ItemAttribute(tokens[0], Convert.ToSingle(tokens[1]));
			}

			if(attribute != null)
			{
				attributes.Add(attribute);
			}
		}

		return attributes;
	}

}
