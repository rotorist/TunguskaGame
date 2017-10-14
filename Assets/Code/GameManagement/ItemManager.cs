using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ItemManager
{


	public void Initialize()
	{

		/*
		backpack.AddGridItem(item1, 0, 0, GridItemOrient.Landscape);
		backpack.AddGridItem(item2, 2, 3, GridItemOrient.Portrait);
		backpack.AddGridItem(item3, 7, 5, GridItemOrient.Landscape);
		backpack.AddGridItem(item4, 4, 3, GridItemOrient.Landscape);
		backpack.AddGridItem(item5, 0, 3, GridItemOrient.Landscape);
		backpack.AddGridItem(item5, 1, 3, GridItemOrient.Landscape);
		*/

		GameObject [] objects = GameObject.FindGameObjectsWithTag("Chest");

		foreach(GameObject o in objects)
		{
			Chest chest = o.GetComponent<Chest>();
			chest.GenerateContent();

		}



		objects = GameObject.FindGameObjectsWithTag("PickupItem");

		foreach(GameObject o in objects)
		{
			PickupItem pickup = o.GetComponent<PickupItem>();
			pickup.Item = LoadItem(pickup.ItemID);
			pickup.Item.Durability = pickup.Item.MaxDurability * pickup.Durability;
		}


	}


	public List<GridItemData> GenerateRandomChestInventory(List<ItemType> Types, int ColSize, int RowSize)
	{
		List<GridItemData> items = new List<GridItemData>();

		/*
		Item item3 = LoadItem("flakjacket");

		Item item7 = LoadItem("pasgthelmet");


		Item item8 = LoadItem("ammo762_39");


		items.Add(new GridItemData(item3, 0, 0, GridItemOrient.Portrait, 1));
		items.Add(new GridItemData(item7, 0, 3, GridItemOrient.Landscape, 1));
		items.Add(new GridItemData(item8, 4, 0, GridItemOrient.Portrait, 50));
		*/

		return items;
	}

	public List<GridItemData> GeneratePresetChest(string [] itemIDs, int [] itemQuantity, int ColSize, int RowSize)
	{
		if(itemIDs.Length != itemQuantity.Length)
		{
			return null;
		}

		List<GridItemData> gridItems = new List<GridItemData>();

		for(int i=0; i < itemIDs.Length; i ++)
		{
			Item item = LoadItem(itemIDs[i]);
			gridItems.Add(new GridItemData(item, 0, 0, GridItemOrient.Landscape, itemQuantity[i]));
		}

		return gridItems;
	}


	public void LoadPartyInventory()
	{





		CharacterInventory inventory1 = GameManager.Inst.PlayerControl.Party.Members[0].Inventory;
		//CharacterInventory inventory2 = GameManager.Inst.PlayerControl.Party.Members[1].Inventory;



		//inventory1.Backpack.Add(new GridItemData(LoadItem("makarov"), 2, 3, GridItemOrient.Landscape, 1));
		//inventory1.Backpack.Add(new GridItemData(LoadItem("rgd5grenade"), 0, 7, GridItemOrient.Landscape, 1));
		//inventory1.Backpack.Add(new GridItemData(LoadItem("kevlarhelmet"), 0, 6, GridItemOrient.Landscape, 1));
		//inventory1.Backpack.Add(new GridItemData(LoadItem("ammo762_39"), 0, 9, GridItemOrient.Portrait, 80));
		//inventory1.Backpack.Add(new GridItemData(LoadItem("ammo9_18"), 0, 6, GridItemOrient.Portrait, 40));
		inventory1.Backpack.Add(new GridItemData(LoadItem("ammo12shot"), 4, 9, GridItemOrient.Landscape, 20));
		//inventory1.Backpack.Add(new GridItemData(LoadItem("ammo44magnum"), 1, 8, GridItemOrient.Landscape, 32));

		//inventory1.Backpack.Add(new GridItemData(LoadItem("svd"), 0, 3, GridItemOrient.Landscape, 1));
		inventory1.Backpack.Add(new GridItemData(LoadItem("sleepingbag"), 0, 8, GridItemOrient.Landscape,1));
		inventory1.Backpack.Add(new GridItemData(LoadItem("bandages"), 6, 9, GridItemOrient.Landscape, 5));
		inventory1.Backpack.Add(new GridItemData(LoadItem("rubles"), 8, 9, GridItemOrient.Landscape, 200));
		inventory1.Backpack.Add(new GridItemData(LoadItem("bread1"), 8, 7, GridItemOrient.Landscape, 1));
		inventory1.Backpack.Add(new GridItemData(LoadItem("sausage"), 9, 8, GridItemOrient.Landscape, 1));
		inventory1.Backpack.Add(new GridItemData(LoadItem("borisphoto"), 4, 8, GridItemOrient.Landscape, 1));
		//inventory1.Backpack.Add(new GridItemData(LoadItem("gasoline"), 5, 6, GridItemOrient.Landscape, 5));
		//inventory1.Backpack.Add(new GridItemData(LoadItem("mutantblood"), 0, 0, GridItemOrient.Landscape, 10));
		//inventory1.Backpack.Add(new GridItemData(LoadItem("mutantheart"), 1, 0, GridItemOrient.Landscape, 6));
		//inventory1.Backpack.Add(new GridItemData(LoadItem("alcohol"), 2, 0, GridItemOrient.Landscape, 15));
		//inventory1.Backpack.Add(new GridItemData(LoadItem("recipe_hr1"), 3, 0, GridItemOrient.Landscape, 1));

		if(GameManager.Inst.GodMode)
		{
			inventory1.Backpack.Add(new GridItemData(LoadItem("skorpion"), 0, 3, GridItemOrient.Landscape, 1));
			inventory1.Backpack.Add(new GridItemData(LoadItem("huntingshotgun"), 0, 0, GridItemOrient.Landscape, 1));
			inventory1.Backpack.Add(new GridItemData(LoadItem("ammo9_18"), 5, 8, GridItemOrient.Portrait, 40));
			inventory1.Backpack.Add(new GridItemData(LoadItem("ammo762_39"), 7, 8, GridItemOrient.Portrait, 60));
			inventory1.Backpack.Add(new GridItemData(LoadItem("f1grenade"), 0, 5, GridItemOrient.Landscape, 1));
			inventory1.Backpack.Add(new GridItemData(LoadItem("f1grenade"), 1, 5, GridItemOrient.Landscape, 1));
			inventory1.Backpack.Add(new GridItemData(LoadItem("f1grenade"), 2, 5, GridItemOrient.Landscape, 1));
			inventory1.Backpack.Add(new GridItemData(LoadItem("serum_hr1"), 3, 5, GridItemOrient.Landscape, 1));
			inventory1.Backpack.Add(new GridItemData(LoadItem("serum_hr1"), 4, 5, GridItemOrient.Landscape, 1));
		}

		if(GameManager.Inst.GodMode)
		{
			inventory1.RifleSlot = LoadItem("ak47");
			inventory1.ThrowSlot = LoadItem("f1grenade");
			inventory1.ArmorSlot = LoadItem("lightarmor");
			inventory1.SideArmSlot = LoadItem("makarov");
			inventory1.HeadSlot = LoadItem("kevlarhelmet");
		}
		else
		{
			inventory1.RifleSlot = LoadItem("doublebarrelshotgun");
			inventory1.ToolSlot = LoadItem("geigercounter");
		}


	}
		

	public void LoadNPCInventory(CharacterInventory inventory, Faction faction, int tier)
	{
		if(tier <= 0)
		{
			return;
		}

		if(tier == 1)
		{
			//sometimes has primary weapon only, sometimes secondary weapon only
			Item weapon = GetRandomGunByTier(tier, false, ItemType.PrimaryWeapon);
			if(weapon.Type == ItemType.PrimaryWeapon)
			{
				inventory.RifleSlot = weapon;
			}
			else if(weapon.Type == ItemType.SideArm)
			{
				inventory.SideArmSlot = weapon;
			}

			//sometimes has grenade
			if(UnityEngine.Random.value > 0.75f)
			{
				Item grenade = GetRandomGrenadeByTier(tier);
			}
			//sometimes has helmet or armor
			if(UnityEngine.Random.value > 0.8f)
			{
				Item armor = GetRandomArmorByTier(tier, false, ItemType.Armor);
				if(armor.Type == ItemType.Helmet)
				{
					inventory.HeadSlot = armor;
				}
				else if(armor.Type == ItemType.Armor)
				{
					inventory.ArmorSlot = armor;
				}
			}

		}

		if(tier == 2)
		{
			//always has primary weapon, sometimes has secondary weapon
			Item weapon = GetRandomGunByTier(tier, true, ItemType.PrimaryWeapon);
			inventory.RifleSlot = weapon;

			if(UnityEngine.Random.value > 0.6f)
			{
				Item sideArm = GetRandomGunByTier(tier, true, ItemType.SideArm);
				inventory.SideArmSlot = sideArm;
			}

			//sometimes has grenade
			if(UnityEngine.Random.value > 0.5f)
			{
				Item grenade = GetRandomGrenadeByTier(tier);
			}

			//sometimes has armor, sometimes also has helmet
			if(UnityEngine.Random.value > 0.65f)
			{
				Item armor = GetRandomArmorByTier(tier, true, ItemType.Armor);
			}

			if(UnityEngine.Random.value > 0.5f)
			{
				Item helmet = GetRandomArmorByTier(tier, true, ItemType.Helmet);
			}
		}

		/*
		if(faction == Faction.Civilian)
		{

		}
		else if(faction == Faction.Bootleggers || faction == Faction.Legionnaires)
		{
			float rand1 = UnityEngine.Random.value;
			float rand2 = UnityEngine.Random.value;

			if(rand1 > 0.5f && rand1 < 0.95f)
			{
				inventory.RifleSlot = LoadItem("huntingshotgun");
				if(rand2 > 0.5f)
				{
					inventory.SideArmSlot = LoadItem("44magnum");
				}
			}
			else if(rand1 > 0.95f)
			{
				inventory.RifleSlot = LoadItem("ak47");
				if(rand2 > 0.5f)
				{
					inventory.SideArmSlot = LoadItem("44magnum");
				}
			}
			else 
			{
				inventory.SideArmSlot = LoadItem("44magnum");
			}

			float rand3 = UnityEngine.Random.value;

			if(rand3 > 0.8f)
			{
				inventory.ArmorSlot = LoadItem("flakjacket");
			}

			if(rand3 > 0.6f)
			{
				inventory.HeadSlot = LoadItem("pasgthelmet");
			}

			float rand4 = UnityEngine.Random.value;

			if(rand4 > 0f)
			{
				inventory.ThrowSlot = LoadItem("rgd5grenade");
			}
		}
		else if(faction == Faction.Military)
		{
			float rand1 = UnityEngine.Random.value;
			float rand2 = UnityEngine.Random.value;

			if(rand1 > 0.6f && rand1 < 0.8f)
			{
				inventory.RifleSlot = LoadItem("huntingshotgun");

			}
			else if(rand1 > 0.8f)
			{
				inventory.RifleSlot = LoadItem("svd");
				inventory.SideArmSlot = LoadItem("44magnum");
			}
			else 
			{
				inventory.RifleSlot = LoadItem("ak47");
				if(rand2 > 0.5f)
				{
					inventory.SideArmSlot = LoadItem("44magnum");
				}
			}

			float rand3 = UnityEngine.Random.value;

			if(rand3 > 0.4f)
			{
				inventory.ArmorSlot = LoadItem("lightarmor");
				inventory.HeadSlot = LoadItem("kevlarhelmet");
			}

			float rand4 = UnityEngine.Random.value;

			if(rand4 > 0.4f)
			{
				inventory.ThrowSlot = LoadItem("rgd5grenade");
			}
		}
		else if(faction == Faction.Mutants)
		{
			
		}
		*/
	
	}

	public void LoadNPCInventory(CharacterInventory inventory, PresetInventory presetInventory)
	{
		if(inventory == null || presetInventory == null)
		{
			return;
		}

		int count = 1;

		if(presetInventory.HeadSlotData.Length > 0)
		{
			inventory.HeadSlot = ParsePresetItem(presetInventory.HeadSlotData, out count);
		}

		if(presetInventory.ArmorSlotData.Length > 0)
		{
			inventory.ArmorSlot = ParsePresetItem(presetInventory.ArmorSlotData, out count);
		}

		if(presetInventory.RifleSlotData.Length > 0)
		{
			inventory.RifleSlot = ParsePresetItem(presetInventory.RifleSlotData, out count);
		}

		if(presetInventory.SideSlotData.Length > 0)
		{
			inventory.SideArmSlot = ParsePresetItem(presetInventory.SideSlotData, out count);
		}

		foreach(string data in presetInventory.BackpackData)
		{
			Item item = ParsePresetItem(data, out count);
			inventory.Backpack.Add(new GridItemData(item, 0, 0, GridItemOrient.Landscape, count));
		}
	}

	public List<GridItemData> GetNPCLoot(Character character)
	{
		List<GridItemData> items = new List<GridItemData>();
		CharacterInventory inventory = character.Inventory;

		//first add whatever is already in the backpack
		foreach(GridItemData item in inventory.Backpack)
		{
			items.Add(item);
		}

		if(character.CharacterType == CharacterType.Human)
		{
			//add primary and secondary weapons
			if(inventory.RifleSlot != null)
			{
				int magSize = (int)inventory.RifleSlot.GetAttributeByName("Magazine Size").Value;
				inventory.RifleSlot.SetAttribute("_LoadedAmmos", UnityEngine.Random.Range(0, magSize + 1));
				inventory.RifleSlot.Durability = UnityEngine.Random.Range(inventory.RifleSlot.MaxDurability * 0.6f, inventory.RifleSlot.MaxDurability * 0.9f);
				items.Add(new GridItemData(inventory.RifleSlot, 0, 0, GridItemOrient.Landscape, 1));
				Item ammo = LoadItem(inventory.RifleSlot.GetAttributeByName("_LoadedAmmoID").Value.ToString());
				if(UnityEngine.Random.value > 0.75f)
				{
					items.Add(new GridItemData(ammo, 0, 0, GridItemOrient.Landscape, UnityEngine.Random.Range(1, 30)));
				}
			}

			if(inventory.SideArmSlot != null)
			{
				bool isRanged = (bool)inventory.SideArmSlot.GetAttributeByName("_IsRanged").Value;
				if(isRanged)
				{
					int magSize = (int)inventory.SideArmSlot.GetAttributeByName("Magazine Size").Value;
					inventory.SideArmSlot.SetAttribute("_LoadedAmmos", UnityEngine.Random.Range(0, magSize + 1));
				}
				inventory.SideArmSlot.Durability = UnityEngine.Random.Range(inventory.SideArmSlot.MaxDurability * 0.6f, inventory.SideArmSlot.MaxDurability * 0.9f);
				items.Add(new GridItemData(inventory.SideArmSlot, 0, 0, GridItemOrient.Landscape, 1));
				Item ammo = LoadItem(inventory.SideArmSlot.GetAttributeByName("_LoadedAmmoID").Value.ToString());
				if(UnityEngine.Random.value > 0.75f)
				{
					items.Add(new GridItemData(ammo, 0, 0, GridItemOrient.Landscape, UnityEngine.Random.Range(1, 30)));
				}
			}

			//if there are grenades, there's some chance of having them in loot
			if(inventory.ThrowSlot != null)
			{
				if(UnityEngine.Random.value > 0.8f)
				{
					items.Add(new GridItemData(inventory.ThrowSlot, 0, 0, GridItemOrient.Landscape, 1));
				}
			}

			//add some medicine, food etc.
			if(UnityEngine.Random.value > 0.7f)
			{
				items.Add(new GridItemData(LoadItem("bandages"), 0, 0, GridItemOrient.Landscape, UnityEngine.Random.Range(1, 8)));
			}
		}
		else
		{
			if(character.Faction == Faction.Mutants)
			{
				//add mutant part depending on cause of death
				float dropRate = 0.5f;
				if(character.DeathReason == DamageType.Melee)
				{
					dropRate = 0.85f;
				}

				string [] dropItems = new string[]{"mutantheart", "mutantbrain", "mutantblood"};

				if(UnityEngine.Random.value < dropRate)
				{
					Item dropItem = LoadItem(dropItems[UnityEngine.Random.Range(0, dropItems.Length)]);
					float rarity = (float)dropItem.GetAttributeByName("_Rarity").Value;
					int maxDrop = Mathf.CeilToInt(dropItem.MaxStackSize * GameManager.Inst.Constants.RarityDropRate.Evaluate(rarity));
					int dropQuantity = UnityEngine.Random.Range(1, maxDrop + 1);
					items.Add(new GridItemData(dropItem, 0, 0, GridItemOrient.Landscape, dropQuantity));
				}
			}
			else if(character.Faction == Faction.Scythes)
			{
				float dropRate = 0.4f;
				if(character.DeathReason == DamageType.Melee)
				{
					dropRate = 0.85f;
				}

				string [] dropItems = new string[]{"scytheblood"};

				if(UnityEngine.Random.value < dropRate)
				{
					Item dropItem = LoadItem(dropItems[UnityEngine.Random.Range(0, dropItems.Length)]);
					float rarity = (float)dropItem.GetAttributeByName("_Rarity").Value;
					int maxDrop = Mathf.CeilToInt(dropItem.MaxStackSize * GameManager.Inst.Constants.RarityDropRate.Evaluate(rarity));
					int dropQuantity = UnityEngine.Random.Range(1, maxDrop + 1);
					items.Add(new GridItemData(dropItem, 0, 0, GridItemOrient.Landscape, dropQuantity));
				}
			}
			else if(character.Faction == Faction.Animals)
			{
				float dropRate = 0.3f;
				if(character.DeathReason == DamageType.Melee)
				{
					dropRate = 0.55f;
				}

				string [] dropItems = new string[]{"wolfgallstones"};

				if(UnityEngine.Random.value < dropRate)
				{
					Item dropItem = LoadItem(dropItems[UnityEngine.Random.Range(0, dropItems.Length)]);
					float rarity = (float)dropItem.GetAttributeByName("_Rarity").Value;
					int maxDrop = Mathf.CeilToInt(dropItem.MaxStackSize * GameManager.Inst.Constants.RarityDropRate.Evaluate(rarity));
					int dropQuantity = UnityEngine.Random.Range(1, maxDrop + 1);
					items.Add(new GridItemData(dropItem, 0, 0, GridItemOrient.Landscape, dropQuantity));
				}
			}
		}



		return items;
	}

	public List<GridItemData> GetTraderInventory(ItemType [] types, int tier)
	{
		List<GridItemData> allItems = new List<GridItemData>();
		foreach(ItemType type in types)
		{
			if(type == ItemType.PrimaryWeapon || type == ItemType.SideArm || type == ItemType.Thrown)
			{
				/*
				Item weapon1 = LoadItem("ak47");
				Item weapon2 = LoadItem("44magnum");
				weapon1.SetAttribute("_LoadedAmmos", 0);
				weapon2.SetAttribute("_LoadedAmmos", 0);
				allItems.Add(new GridItemData(weapon1, 0, 0, GridItemOrient.Landscape, 1));
				allItems.Add(new GridItemData(weapon2, 0, 0, GridItemOrient.Landscape, 1));
				*/
				int count = UnityEngine.Random.Range(3, 6);
				for(int i=0; i<count; i++)
				{
					Item item = GetRandomWeaponByTier(tier, false, false, ItemType.PrimaryWeapon);
					if(item.Type == ItemType.PrimaryWeapon || item.Type == ItemType.SideArm)
					{
						item.SetAttribute("_LoadedAmmos", 0);
						item.Durability = item.MaxDurability * 0.95f;
						string ammoID = item.GetAttributeByName("_LoadedAmmoID").Value.ToString();
						if(!String.IsNullOrEmpty(ammoID))
						{
							//load ammo
							int ammoCount = UnityEngine.Random.Range(1, 5);
							for(int j=0; j<ammoCount; j++)
							{
								Item ammoItem = LoadItem(ammoID);
								allItems.Add(new GridItemData(ammoItem, 0, 0, GridItemOrient.Landscape, 1));
							}
						}

					}

					allItems.Add(new GridItemData(item, 0, 0, GridItemOrient.Landscape, 1));
				}
			}
			else if(type == ItemType.Armor)
			{
				int count = UnityEngine.Random.Range(1, 3);
				for(int i=0; i<count; i++)
				{
					Item item = GetRandomArmorByTier(tier, false, ItemType.Armor);
					allItems.Add(new GridItemData(item, 0, 0, GridItemOrient.Landscape, 1));
				}

			}
			else if(type == ItemType.Food)
			{
				int count = UnityEngine.Random.Range(2, 5);
				for(int i=0; i<count; i++)
				{
					Item item = GetRandomItemByType(ItemType.Food);
					allItems.Add(new GridItemData(item, 0, 0, GridItemOrient.Landscape, 1));
				}
			}
			else if(type == ItemType.Medicine)
			{
				int count = UnityEngine.Random.Range(2, 5);
				for(int i=0; i<count; i++)
				{
					Item item = GetRandomItemByType(ItemType.Medicine);
					allItems.Add(new GridItemData(item, 0, 0, GridItemOrient.Landscape, 1));
				}
			}
			else if(type == ItemType.Solvent)
			{
				int count = UnityEngine.Random.Range(1, 4);
				for(int i=0; i<count; i++)
				{
					Item item = GetRandomItemByType(ItemType.Solvent);
					allItems.Add(new GridItemData(item, 0, 0, GridItemOrient.Landscape, 1));
				}
			}
		}


		return allItems;
	}

	public Transform FindPickupItemParent(Transform item)
	{
		RaycastHit buildingHit;
		if(Physics.Raycast(item.position, Vector3.down, out buildingHit, 200, (1 << 9 | 1 << 8 | 1 << 10)))
		{
			BuildingComponent component = buildingHit.collider.GetComponent<BuildingComponent>();
			if(component != null && component.Level > 0)
			{
				return component.transform;
			}
		}

		return null;
	}





	public string GetItemNameFromID(string itemID)
	{
		Item item = LoadItem(itemID);

		return item.Name;
	}

	public Item GetRandomWeaponByTier(int tier, bool rangedOnly, bool byType, ItemType type)
	{

		List<Item> allTierItems = GameManager.Inst.DBManager.DBHandlerItem.GetAllWeaponsByTier(tier);
		if(rangedOnly)
		{
			List<Item> allTierItemsCopy = new List<Item>(allTierItems);
			foreach(Item item in allTierItemsCopy)
			{
				if((bool)item.GetAttributeByName("_IsRanged").Value == false)
				{
					allTierItems.Remove(item);
				}
				if(byType && item.Type != type)
				{
					allTierItems.Remove(item);
				}
			}
			return allTierItems[UnityEngine.Random.Range(0, allTierItems.Count)];
		}
		else
		{
			return allTierItems[UnityEngine.Random.Range(0, allTierItems.Count)];
		}
	}

	public Item GetRandomGunByTier(int tier, bool byType, ItemType type)
	{
		List<Item> allTierItems = GameManager.Inst.DBManager.DBHandlerItem.GetAllGunsByTier(tier);
		List<Item> allTierItemsCopy = new List<Item>(allTierItems);
		foreach(Item item in allTierItemsCopy)
		{
			if((bool)item.GetAttributeByName("_IsRanged").Value == false)
			{
				allTierItems.Remove(item);
			}
			if(byType && item.Type != type)
			{
				allTierItems.Remove(item);
			}
		}
		return allTierItems[UnityEngine.Random.Range(0, allTierItems.Count)];
	}

	public Item GetRandomGrenadeByTier(int tier)
	{
		List<Item> allTierItems = GameManager.Inst.DBManager.DBHandlerItem.GetAllGrenadesByTier(tier);
		return allTierItems[UnityEngine.Random.Range(0, allTierItems.Count)];
	}

	public Item GetRandomArmorByTier(int tier, bool byType, ItemType type)
	{

		List<Item> allTierItems = GameManager.Inst.DBManager.DBHandlerItem.GetAllArmorsByTier(tier);
		List<Item> allTierItemsCopy = new List<Item>(allTierItems);
		if(byType)
		{
			foreach(Item item in allTierItemsCopy)
			{
				if(item.Type != type)
				{
					allTierItems.Remove(item);
				}
			}
		}
		return allTierItems[UnityEngine.Random.Range(0, allTierItems.Count)];

	}

	public Item GetRandomItemByType(ItemType type)
	{
		List<Item> allTypeItems = GameManager.Inst.DBManager.DBHandlerItem.GetAllItemsByType(type);
		return allTypeItems[UnityEngine.Random.Range(0, allTypeItems.Count)];
	}

	public Item LoadItem(string itemID)
	{

		Item item = GameManager.Inst.DBManager.DBHandlerItem.LoadItemByID(itemID);
		if(item == null)
		{
			return null;
		}


		return item;



	}
	

	private Item ParsePresetItem(string data, out int count)
	{
		string [] tokens = data.Split('/'); //format: itemID/durability0-1/count
		count = 1;
		if(data.Length <= 0 || tokens.Length <= 0)
		{
			return null;
		}

		Item item = LoadItem(tokens[0]);

		if(item == null)
		{
			return null;
		}

		if(tokens.Length == 2)
		{
			item.Durability = item.MaxDurability * Convert.ToSingle(tokens[1]);
		}

		if(tokens.Length == 3)
		{
			count = Convert.ToInt32(tokens[2]);
		}

		return item;

	}
}
