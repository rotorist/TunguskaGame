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
	public List<Item> GetAllWeaponsByTier(int tier)
	{
		IDataReader itemReader = GameManager.Inst.DBManager.RunQuery(
			"SELECT * from base_items WHERE tier='" + tier +"' AND (type='PrimaryWeapon' OR type='SideArm' OR type='Thrown')" );

		List<Item> result = new List<Item>();
		while(itemReader.Read())
		{
			result.Add(ParseItem(itemReader));
		}

		return result;
	}

	public List<Item> GetAllGunsByTier(int tier)
	{
		IDataReader itemReader = GameManager.Inst.DBManager.RunQuery(
			"SELECT * from base_items WHERE tier='" + tier +"' AND (type='PrimaryWeapon' OR type='SideArm')" );

		List<Item> result = new List<Item>();
		while(itemReader.Read())
		{
			result.Add(ParseItem(itemReader));
		}

		return result;
	}

	public List<Item> GetAllGrenadesByTier(int tier)
	{
		IDataReader itemReader = GameManager.Inst.DBManager.RunQuery(
			"SELECT * from base_items WHERE tier='" + tier +"' AND (type='Thrown')" );

		List<Item> result = new List<Item>();
		while(itemReader.Read())
		{
			result.Add(ParseItem(itemReader));
		}

		return result;
	}

	public List<Item> GetAllArmorsByTier(int tier)
	{
		IDataReader itemReader = GameManager.Inst.DBManager.RunQuery(
			"SELECT * from base_items WHERE tier='" + tier +"' AND (type='Armor' OR type='Helmet')" );

		List<Item> result = new List<Item>();
		while(itemReader.Read())
		{
			result.Add(ParseItem(itemReader));
		}

		return result;
	}

	public List<Item> GetAllItemsByType(ItemType type)
	{
		IDataReader itemReader = GameManager.Inst.DBManager.RunQuery(
			"SELECT * from base_items WHERE type='" + type.ToString() + "'" );

		List<Item> result = new List<Item>();
		while(itemReader.Read())
		{
			result.Add(ParseItem(itemReader));
		}

		return result;
	}

	//check if input ingredients and temperature matches any recipe
	public Item CheckSerumRecipe(List<GridItem> ingredients, float temperature, out string hint)
	{
		//consolidate the list 
		Dictionary<string,int> ingredientList = new Dictionary<string, int>();
		foreach(GridItem item in ingredients)
		{
			if(!ingredientList.ContainsKey(item.Item.ID))
			{
				ingredientList.Add(item.Item.ID, item.GetQuantity());
			}
			else
			{
				ingredientList[item.Item.ID] += item.GetQuantity();
			}
		}

		bool recipeFound = false;
		string candidateSerumID = "";

		foreach(KeyValuePair<string,int> ingredient in ingredientList)
		{
			
			string query = "SELECT serum_id FROM recipes WHERE ingredients = '" + ingredient.Key + "' AND ingredients_count = '" 
				+ ingredient.Value + "'";

			IDataReader ingReader = GameManager.Inst.DBManager.RunQuery(query);
			int numberRows = 0;
			while(ingReader.Read())
			{
				string serumID = ingReader.GetString(0);
				numberRows ++;
				if(candidateSerumID == "")
				{
					candidateSerumID = serumID;
				}
				else
				{
					if(serumID != candidateSerumID)
					{
						hint = "The ingredients combined nicely, but only some unknown substance came out of disdiller.";
						return null;
					}
				}
			}

			if(numberRows <= 0)
			{
				Item ing = LoadItemByID(ingredient.Key);
				hint = "Judging from the reaction process, the amount of " + ing.Name + " doesn't seem right";
				return null;
			}
		}

		if(candidateSerumID == "")
		{
			hint = "The ingredients turned into a useless dark red paste.";
			return null;
		}
		else
		{
			//check if the number of ingredients for this serum matches number of input ingredients
			IDataReader countReader = GameManager.Inst.DBManager.RunQuery(
				"SELECT COUNT(*) FROM recipes WHERE serum_id = '" + candidateSerumID + "'");
			while(countReader.Read())
			{
				if(countReader.GetInt32(0) == ingredientList.Count)
				{
					recipeFound = true;
				}
			}

			if(!recipeFound)
			{
				hint = "The concoction seems very close to a useful serum but still missing something.";
				return null;
			}

			//check if temperature matches
			IDataReader tempReader = GameManager.Inst.DBManager.RunQuery(
				"SELECT temperature FROM recipes WHERE serum_id = '" + candidateSerumID + "'");

			tempReader.Read();
			float targetTemp = tempReader.GetFloat(0);
			if(targetTemp == temperature)
			{
				//all good, return serum
				Item serum = LoadItemByID(candidateSerumID);
				hint = "You have successfully created " + serum.Name;
				return serum;
				 
			}
			else if(targetTemp > temperature)
			{
				hint = "Too little product came out of the distiller. Perhaps more heat is needed.";
				return null;
			}
			else
			{
				hint = "The ingredients seem burned. Nothing came out of the distiller.";
				return null;
			}

		}
	}

	public Item LoadItemByID(string id)
	{
		IDataReader itemReader = GameManager.Inst.DBManager.RunQuery(
			"SELECT * from base_items WHERE id='" + id +"'");

		while(itemReader.Read())
		{
			

			return ParseItem(itemReader);
		}

		return null;
	}

	private Item ParseItem(IDataReader itemReader)
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
