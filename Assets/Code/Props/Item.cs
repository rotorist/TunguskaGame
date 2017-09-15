using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Item 
{
	public string ID;
	public string Name;
	public string Description;
	public float Weight;
	public string SpriteName;
	public string PrefabName;
	public ItemType Type;
	public int GridCols;
	public int GridRows;
	public int MaxStackSize;
	public int UseLimit;
	public bool IsUsable;
	public int Tier; //1, 2, 3; 3 is best; 0 is any tier
	public float BasePrice;
	public float MaxDurability;
	public float Durability;

	public List<ItemAttribute> Attributes;

	public Dictionary<string, int> AttributeIndex;

	public Item()
	{
		Attributes = new List<ItemAttribute>();
		AttributeIndex = new Dictionary<string, int>();
	}

	public Item(Item item)
	{
		//clone an existing item
		ID = item.ID;
		Name = item.Name;
		Description = item.Description;
		Weight = item.Weight;
		SpriteName = item.SpriteName;
		PrefabName = item.PrefabName;
		Type = item.Type;
		GridCols = item.GridCols;
		GridRows = item.GridRows;
		MaxStackSize = item.MaxStackSize;
		BasePrice = item.BasePrice;
		Tier = item.Tier;
		IsUsable = item.IsUsable;
		UseLimit = item.UseLimit;
		Attributes = new List<ItemAttribute>();
		foreach(ItemAttribute attribute in item.Attributes)
		{
			ItemAttribute newAttribute = new ItemAttribute(attribute.Name, attribute.Value);
			Attributes.Add(newAttribute);
		}

		AttributeIndex = new Dictionary<string, int>();
		BuildIndex();
	}

	public ItemAttribute GetAttributeByName(string name)
	{
		if(AttributeIndex.Count > 0)
		{
			if(AttributeIndex.ContainsKey(name))
			{
				
				return Attributes[AttributeIndex[name]];
			}
			else
			{
				return null;
			}
		}
		else
		{
			return null;
		}
			
	}

	public void SetAttribute(string name, object value)
	{
		if(AttributeIndex.ContainsKey(name))
		{
			Attributes[AttributeIndex[name]].Value = value;
		}
	}

	public void BuildIndex()
	{
		AttributeIndex.Clear();
		for(int i=0; i<Attributes.Count; i++)
		{
			if(Attributes[i] != null)
			{
				AttributeIndex.Add(Attributes[i].Name, i);
			}
		}
	}

	public void PostLoad()
	{
		BuildIndex();
	}
}


public enum ItemType
{
	PrimaryWeapon,
	SideArm,
	Ammo,
	Thrown,
	Armor,
	Tool,
	Helmet,
	Food,
	Medicine,
	Money,
	SleepingBag,
	Fuel,
	Note,
	Key,
	Misc,
	SupplyPack,
	Ingredient,
	Solvent,
	Serum,
}