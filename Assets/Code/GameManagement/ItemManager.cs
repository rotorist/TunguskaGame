using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
		}


	}


	public List<GridItemData> GenerateRandomChestInventory(List<ItemType> Types, int ColSize, int RowSize)
	{
		List<GridItemData> items = new List<GridItemData>();


		Item item3 = LoadItem("flakjacket");

		Item item7 = LoadItem("pasgthelmet");


		Item item8 = LoadItem("ammo762_39");


		items.Add(new GridItemData(item3, 0, 0, GridItemOrient.Portrait, 1));
		items.Add(new GridItemData(item7, 0, 3, GridItemOrient.Landscape, 1));
		items.Add(new GridItemData(item8, 4, 0, GridItemOrient.Portrait, 50));


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



		inventory1.Backpack.Add(new GridItemData(LoadItem("44magnum"), 2, 3, GridItemOrient.Portrait, 1));
		inventory1.Backpack.Add(new GridItemData(LoadItem("pipegrenade"), 0, 7, GridItemOrient.Landscape, 1));
		//inventory1.Backpack.Add(new GridItemData(LoadItem("kevlarhelmet"), 0, 6, GridItemOrient.Landscape, 1));
		inventory1.Backpack.Add(new GridItemData(LoadItem("ammo762_39"), 0, 9, GridItemOrient.Landscape, 80));
		inventory1.Backpack.Add(new GridItemData(LoadItem("ammo762_54r"), 2, 9, GridItemOrient.Landscape, 40));
		inventory1.Backpack.Add(new GridItemData(LoadItem("ammo12shot"), 4, 9, GridItemOrient.Landscape, 20));
		inventory1.Backpack.Add(new GridItemData(LoadItem("ammo44magnum"), 1, 8, GridItemOrient.Landscape, 32));
		inventory1.Backpack.Add(new GridItemData(LoadItem("huntingshotgun"), 0, 0, GridItemOrient.Landscape, 1));
		//inventory1.Backpack.Add(new GridItemData(LoadItem("svd"), 0, 3, GridItemOrient.Landscape, 1));
		inventory1.Backpack.Add(new GridItemData(LoadItem("sleepingbag"), 1, 6, GridItemOrient.Landscape,1));
		//inventory1.Backpack.Add(new GridItemData(LoadItem("mutantheart"), 8, 8, GridItemOrient.Landscape, 1));
		inventory1.Backpack.Add(new GridItemData(LoadItem("bandages"), 6, 9, GridItemOrient.Landscape, 5));
		inventory1.Backpack.Add(new GridItemData(LoadItem("rubles"), 8, 9, GridItemOrient.Landscape, 110));
		inventory1.Backpack.Add(new GridItemData(LoadItem("bread1"), 8, 7, GridItemOrient.Landscape, 1));
		inventory1.Backpack.Add(new GridItemData(LoadItem("sausage"), 9, 8, GridItemOrient.Landscape, 1));
		inventory1.Backpack.Add(new GridItemData(LoadItem("gasoline"), 5, 6, GridItemOrient.Landscape, 5));

		inventory1.RifleSlot = LoadItem("ak47");
		inventory1.ThrowSlot = LoadItem("pipegrenade");
		inventory1.ArmorSlot = LoadItem("flakjacket");
		inventory1.SideArmSlot = LoadItem("machete");
		inventory1.HeadSlot = LoadItem("kevlarhelmet");
		inventory1.ToolSlot = LoadItem("geigercounter");

	}

	public void LoadPresetNPCInventory(Character character)
	{

	}

	public void LoadNPCInventory(CharacterInventory inventory, Faction faction)
	{
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
				inventory.ThrowSlot = LoadItem("pipegrenade");
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
				inventory.ThrowSlot = LoadItem("pipegrenade");
			}
		}
		else if(faction == Faction.Mutants)
		{
			
		}
	
	}

	public List<GridItemData> GetNPCLoot(Character character)
	{
		List<GridItemData> items = new List<GridItemData>();
		CharacterInventory inventory = character.Inventory;

		if(character.IsHuman)
		{
			//add primary and secondary weapons
			if(inventory.RifleSlot != null)
			{
				items.Add(new GridItemData(inventory.RifleSlot, 0, 0, GridItemOrient.Landscape, 1));
				Item ammo = LoadItem(inventory.RifleSlot.GetAttributeByName("_LoadedAmmoID").Value.ToString());
				if(UnityEngine.Random.value > 0.75f)
				{
					items.Add(new GridItemData(ammo, 0, 0, GridItemOrient.Landscape, UnityEngine.Random.Range(1, 30)));
				}
			}

			if(inventory.SideArmSlot != null)
			{
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
					items.Add(new GridItemData(inventory.ThrowSlot, 0, 0, GridItemOrient.Landscape, UnityEngine.Random.Range(1, 3)));
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
				//add mutant heart depending on cause of death
				float dropRate = 0.2f;
				if(character.DeathReason == DamageType.Melee)
				{
					dropRate = 0.6f;
				}
				Debug.Log("mutant drop rate" + dropRate + " death reason " + character.DeathReason);
				if(UnityEngine.Random.value < dropRate)
				{
					items.Add(new GridItemData(LoadItem("mutantheart"), 0, 0, GridItemOrient.Landscape, 1));
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
			if(type == ItemType.PrimaryWeapon || type == ItemType.SideArm)
			{
				Item weapon1 = LoadItem("ak47");
				Item weapon2 = LoadItem("44magnum");
				weapon1.SetAttribute("_LoadedAmmos", 0);
				weapon2.SetAttribute("_LoadedAmmos", 0);
				allItems.Add(new GridItemData(weapon1, 0, 0, GridItemOrient.Landscape, 1));
				allItems.Add(new GridItemData(weapon2, 0, 0, GridItemOrient.Landscape, 1));
			}
			else if(type == ItemType.Ammo)
			{
				Item ammo1 = LoadItem("ammo762_39");
				Item ammo2 = LoadItem("ammo44magnum");
				allItems.Add(new GridItemData(ammo1, 0, 0, GridItemOrient.Landscape, ammo1.MaxStackSize));
				allItems.Add(new GridItemData(ammo2, 0, 0, GridItemOrient.Landscape, ammo2.MaxStackSize));
			}
			else if(type == ItemType.Armor)
			{
				allItems.Add(new GridItemData(LoadItem("flakjacket"), 0, 0, GridItemOrient.Landscape, 1));
			}
			else if(type == ItemType.Thrown)
			{
				allItems.Add(new GridItemData(LoadItem("pipegrenade"), 0, 0, GridItemOrient.Landscape, 1));
			}
			else if(type == ItemType.Medicine)
			{
				allItems.Add(new GridItemData(LoadItem("bandages"), 0, 0, GridItemOrient.Landscape, 10));
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
			if(component != null)
			{
				return component.transform;
			}
		}

		return null;
	}





	public string GetItemNameFromID(string itemID)
	{
		if(itemID == "ammo762_39")
		{
			return "7.62x39mm SD";
		}

		if(itemID == "ammo44magnum")
		{
			return ".44 Magnum";
		}

		return "";
	}

	public Item LoadItem(string itemID)
	{
		Item item1 = new Item();
		item1.Name = "AK 47";
		item1.Description = "Standard service rifle of the army, high damage and low accuracy.";
		item1.PrefabName = "AK47";
		item1.SpriteName = "ak47";
		item1.Weight = 6;
		item1.ID = "ak47";
		item1.Type = ItemType.PrimaryWeapon;
		item1.GridCols = 8;
		item1.GridRows = 3;
		item1.MaxStackSize = 1;
		item1.Tier = 2;
		item1.BasePrice = 200;
		item1.MaxDurability = 120;
		item1.Durability = 120;
		item1.Attributes.Add(new ItemAttribute("_Muzzle Velocity", 110f));
		item1.Attributes.Add(new ItemAttribute("Impact", 10f));
		item1.Attributes.Add(new ItemAttribute("Accuracy", 0.7f));
		item1.Attributes.Add(new ItemAttribute("Range", 20f));
		item1.Attributes.Add(new ItemAttribute("Magazine Size", 30));
		item1.Attributes.Add(new ItemAttribute("Recoil", 0.35f));
		item1.Attributes.Add(new ItemAttribute("Rate of Fire", 10f));
		item1.Attributes.Add(new ItemAttribute("Handling", 0.6f));
		item1.Attributes.Add(new ItemAttribute("_Encumbrance", 0.13f));
		item1.Attributes.Add(new ItemAttribute("_Caliber", "7.62x39mm"));
		item1.Attributes.Add(new ItemAttribute("_LoadedAmmoID", "ammo762_39"));
		item1.Attributes.Add(new ItemAttribute("_LoadedAmmos", 30));
		item1.Attributes.Add(new ItemAttribute("_IsRanged", true));
		item1.Attributes.Add(new ItemAttribute("_ReloadToUnjam", true));
		item1.BuildIndex();

		Item item2 = new Item();
		item2.Name = "44 Magnum Revolver";
		item2.Description = "One of the most famous modern revolvers of all times. High damage with nice range, but packs a punch to the wielder.";
		item2.PrefabName = "44MagnumRevolver";
		item2.SpriteName = "44magnum";
		item2.Weight = 1.6f;
		item2.ID = "44magnum";
		item2.Type = ItemType.SideArm;
		item2.GridCols = 3;
		item2.GridRows = 2;
		item2.MaxStackSize = 1;
		item2.Tier = 1;
		item2.BasePrice = 125;
		item2.MaxDurability = 80;
		item2.Durability = 80;
		item2.Attributes.Add(new ItemAttribute("_Muzzle Velocity", 100f));
		item2.Attributes.Add(new ItemAttribute("Impact", 5f));
		item2.Attributes.Add(new ItemAttribute("Accuracy", 0.5f));
		item2.Attributes.Add(new ItemAttribute("Range", 15f));
		item2.Attributes.Add(new ItemAttribute("Magazine Size", 6));
		item2.Attributes.Add(new ItemAttribute("Recoil", 3.5f));
		item2.Attributes.Add(new ItemAttribute("Rate of Fire", 2f));
		item2.Attributes.Add(new ItemAttribute("Handling", 0.8f));
		item2.Attributes.Add(new ItemAttribute("_Encumbrance", 0.1f));
		item2.Attributes.Add(new ItemAttribute("_Caliber", ".44"));
		item2.Attributes.Add(new ItemAttribute("_LoadedAmmoID", "ammo44magnum"));
		item2.Attributes.Add(new ItemAttribute("_LoadedAmmos", 6));
		item2.Attributes.Add(new ItemAttribute("_IsRanged", true));
		item2.Attributes.Add(new ItemAttribute("_ReloadToUnjam", false));
		item2.BuildIndex();

		Item item3 = new Item();
		item3.Name = "Flak Jacket";
		item3.Description = "Light weight, kevlar based body armor.";
		item3.PrefabName = "FlakJacket";
		item3.SpriteName = "flakjacket";
		item3.Weight = 3.5f;
		item3.ID = "flakjacket";
		item3.Type = ItemType.Armor;
		item3.GridCols = 3;
		item3.GridRows = 4;
		item3.MaxStackSize = 1;
		item3.Tier = 1;
		item3.BasePrice = 350;
		item3.MaxDurability = 70;
		item3.Durability = 30;
		item3.Attributes.Add(new ItemAttribute("Armor", 30f)); 
		item3.Attributes.Add(new ItemAttribute("Padding", 10f));
		item3.Attributes.Add(new ItemAttribute("Coverage", 0.6f));
		item3.Attributes.Add(new ItemAttribute("_ModelSuffix", "HalfArmor"));
		item3.Attributes.Add(new ItemAttribute("_IsFull", true));
		item3.Attributes.Add(new ItemAttribute("_TextureName", "ArmorTopWoodland"));
		item3.Attributes.Add(new ItemAttribute("_TextureName2", "ArmorBottomWoodland"));
		item3.BuildIndex();

		Item item4 = new Item();
		item4.Name = "Kevlar Helmet";
		item4.Description = "Polymer-based, lined with Kevlar. Offers basic protection against shrapnel and handgun bullets.";
		item4.PrefabName = "KevlarHelmet";
		item4.SpriteName = "kevlarhelmet";
		item4.Weight = 1.0f;
		item4.ID = "kevlarhelmet";
		item4.Type = ItemType.Helmet;
		item4.GridCols = 3;
		item4.GridRows = 3;
		item4.MaxStackSize = 1;
		item4.Tier = 2;
		item4.BasePrice = 250;
		item4.MaxDurability = 50;
		item4.Durability = 40;
		item4.Attributes.Add(new ItemAttribute("Armor", 50f));
		item4.Attributes.Add(new ItemAttribute("Coverage", 0.5f));
		item4.Attributes.Add(new ItemAttribute("_hideHats", true));
		item4.BuildIndex();

		Item item5 = new Item();
		item5.Name = "Pipe Grenade";
		item5.Description = "Makeshift grenade made with plumbing material and gun powder.";
		item5.PrefabName = "PipeGrenade";
		item5.SpriteName = "pipegrenade";
		item5.Weight = 0.9f;
		item5.ID = "pipegrenade";
		item5.Type = ItemType.Thrown;
		item5.GridCols = 1;
		item5.GridRows = 2;
		item5.MaxStackSize = 1;
		item5.Tier = 1;
		item5.BasePrice = 45;
		item5.Attributes.Add(new ItemAttribute("Damage", 100f));
		item5.Attributes.Add(new ItemAttribute("Effective Radius", 5f));
		item5.BuildIndex();

		Item item6 = new Item();
		item6.Name = "Light Body Armor";
		item6.Description = "Light weight ceramic plate body armor.";
		item6.PrefabName = "LightArmor";
		item6.SpriteName = "lightarmor";
		item6.Weight = 4.3f;
		item6.ID = "lightarmor";
		item6.Type = ItemType.Armor;
		item6.GridCols = 3;
		item6.GridRows = 3;
		item6.MaxStackSize = 1;
		item6.Tier = 2;
		item6.BasePrice = 560;
		item6.MaxDurability = 50;
		item6.Durability = 40;
		item6.Attributes.Add(new ItemAttribute("Armor", 50f)); 
		item6.Attributes.Add(new ItemAttribute("Padding", 20f));
		item6.Attributes.Add(new ItemAttribute("Coverage", 0.4f));
		item6.Attributes.Add(new ItemAttribute("_ModelSuffix", "HalfArmor"));
		item6.Attributes.Add(new ItemAttribute("_IsFull", false));
		item6.Attributes.Add(new ItemAttribute("_TextureName", "ArmorTopBlue"));
		item6.BuildIndex();

		Item item7 = new Item();
		item7.Name = "PASGT Ballistic Helmet";
		item7.Description = "Layered with aramid and polyethylene. Offers basic protection against shrapnel and handgun bullets.";
		item7.PrefabName = "PASGTHelmet";
		item7.SpriteName = "pasgthelmet";
		item7.Weight = 1.2f;
		item7.ID = "pasgthelmet";
		item7.Type = ItemType.Helmet;
		item7.GridCols = 3;
		item7.GridRows = 3;
		item7.MaxStackSize = 1;
		item7.Tier = 3;
		item7.BasePrice = 500;
		item7.MaxDurability = 50;
		item7.Durability = 40;
		item7.Attributes.Add(new ItemAttribute("Armor", 40f));
		item7.Attributes.Add(new ItemAttribute("Coverage", 0.5f));
		item7.Attributes.Add(new ItemAttribute("_hideHats", true));
		item7.BuildIndex();

		Item item8 = new Item();
		item8.Name = "7.62x39mm FMJ";
		item8.Description = "Standard ammo for AK47. Fully jacketed bullets.";
		item8.PrefabName = "Ammo762_39";
		item8.SpriteName = "ammo762_39";
		item8.Weight = 0.05f;
		item8.ID = "ammo762_39";
		item8.Type = ItemType.Ammo;
		item8.GridCols = 2;
		item8.GridRows = 1;
		item8.MaxStackSize = 100;
		item8.Tier = 0;
		item8.BasePrice = 4;
		item8.Attributes.Add(new ItemAttribute("_Caliber", "7.62x39mm"));
		item8.Attributes.Add(new ItemAttribute("_numberOfProjectiles", 1));
		item8.Attributes.Add(new ItemAttribute("Damage", 10f));
		item8.Attributes.Add(new ItemAttribute("Penetration", 20f));
		item8.Attributes.Add(new ItemAttribute("_Bleeding", 0.3f));
		item8.BuildIndex();

		Item item9 = new Item();
		item9.Name = "12 Gauge Buckshot";
		item9.Description = "Ammo for shotguns.";
		item9.PrefabName = "Ammo12Shot";
		item9.SpriteName = "ammo12shot";
		item9.Weight = 0.05f;
		item9.ID = "ammo12shot";
		item9.Type = ItemType.Ammo;
		item9.GridCols = 2;
		item9.GridRows = 1;
		item9.MaxStackSize = 20;
		item9.Tier = 0;
		item9.BasePrice = 2;
		item9.Attributes.Add(new ItemAttribute("_Caliber", "12g"));
		item9.Attributes.Add(new ItemAttribute("_numberOfProjectiles", 6));
		item9.Attributes.Add(new ItemAttribute("Damage", 10f));
		item9.Attributes.Add(new ItemAttribute("Penetration", 10f));
		item9.Attributes.Add(new ItemAttribute("_Bleeding", 0.02f));
		item9.BuildIndex();

		Item item10 = new Item();
		item10.Name = "Pump Action Hunting Shotgun";
		item10.Description = "Shotgun used for hunting doves.";
		item10.PrefabName = "HuntingShotgun";
		item10.SpriteName = "huntingshotgun";
		item10.Weight = 5.6f;
		item10.ID = "huntingshotgun";
		item10.Type = ItemType.PrimaryWeapon;
		item10.GridCols = 9;
		item10.GridRows = 3;
		item10.MaxStackSize = 1;
		item10.Tier = 1;
		item10.BasePrice = 175;
		item10.MaxDurability = 150;
		item10.Durability = 150;
		item10.Attributes.Add(new ItemAttribute("_Muzzle Velocity", 100f));
		item10.Attributes.Add(new ItemAttribute("Impact", 5f));
		item10.Attributes.Add(new ItemAttribute("Accuracy", 0.4f));
		item10.Attributes.Add(new ItemAttribute("Range", 12f));
		item10.Attributes.Add(new ItemAttribute("Magazine Size", 5));
		item10.Attributes.Add(new ItemAttribute("Recoil", 4f));
		item10.Attributes.Add(new ItemAttribute("Rate of Fire", 1f));
		item10.Attributes.Add(new ItemAttribute("Handling", 0.5f));
		item10.Attributes.Add(new ItemAttribute("_Encumbrance", 0.13f));
		item10.Attributes.Add(new ItemAttribute("_Caliber", "12g"));
		item10.Attributes.Add(new ItemAttribute("_LoadedAmmoID", "ammo12shot"));
		item10.Attributes.Add(new ItemAttribute("_LoadedAmmos", 5));
		item10.Attributes.Add(new ItemAttribute("_IsRanged", true));
		item10.Attributes.Add(new ItemAttribute("_ReloadToUnjam", false));
		item10.BuildIndex();

		Item item11 = new Item();
		item11.Name = "Dragunov Sniper Rifle";
		item11.Description = "Semi-automatic marksman rifle.";
		item11.PrefabName = "SVD";
		item11.SpriteName = "svd";
		item11.Weight = 6f;
		item11.ID = "svd";
		item11.Type = ItemType.PrimaryWeapon;
		item11.GridCols = 10;
		item11.GridRows = 3;
		item11.MaxStackSize = 1;
		item11.Tier = 2;
		item11.BasePrice = 720;
		item11.MaxDurability = 60;
		item11.Durability = 30;
		item11.Attributes.Add(new ItemAttribute("_Muzzle Velocity", 150f));
		item11.Attributes.Add(new ItemAttribute("Impact", 15f));
		item11.Attributes.Add(new ItemAttribute("Accuracy", 1f));
		item11.Attributes.Add(new ItemAttribute("Range", 40f));
		item11.Attributes.Add(new ItemAttribute("Magazine Size", 10));
		item11.Attributes.Add(new ItemAttribute("Recoil", 5.5f));
		item11.Attributes.Add(new ItemAttribute("Rate of Fire", 1.2f));
		item11.Attributes.Add(new ItemAttribute("Handling", 0.5f));
		item11.Attributes.Add(new ItemAttribute("_Encumbrance", 0.13f));
		item11.Attributes.Add(new ItemAttribute("_Caliber", "7.62x54mmr"));
		item11.Attributes.Add(new ItemAttribute("_LoadedAmmoID", "ammo762_54r"));
		item11.Attributes.Add(new ItemAttribute("_LoadedAmmos", 10));
		item11.Attributes.Add(new ItemAttribute("_IsRanged", true));
		item11.Attributes.Add(new ItemAttribute("_ReloadToUnjam", true));
		item11.BuildIndex();

		Item item12 = new Item();
		item12.Name = "7.62x54mmr";
		item12.Description = "Rimmed round for SVD";
		item12.PrefabName = "Ammo762_54r";
		item12.SpriteName = "ammo762_54r";
		item12.Weight = 0.05f;
		item12.ID = "ammo762_54r";
		item12.Type = ItemType.Ammo;
		item12.GridCols = 2;
		item12.GridRows = 1;
		item12.MaxStackSize = 40;
		item12.Tier = 2;
		item12.BasePrice = 10;
		item12.Attributes.Add(new ItemAttribute("_Caliber", "7.62x54mmr"));
		item12.Attributes.Add(new ItemAttribute("_numberOfProjectiles", 1));
		item12.Attributes.Add(new ItemAttribute("Damage", 50f));
		item12.Attributes.Add(new ItemAttribute("Penetration", 40f));
		item12.Attributes.Add(new ItemAttribute("_Bleeding", 0.4f));
		item12.BuildIndex();

		Item item13 = new Item();
		item13.Name = "Machete";
		item13.Description = "A broad blade for clearing paths and melee combat. Best for unarmored targets.";
		item13.PrefabName = "Machete";
		item13.SpriteName = "machete";
		item13.Weight = 1f;
		item13.ID = "machete";
		item13.Type = ItemType.SideArm;
		item13.GridCols = 6;
		item13.GridRows = 2;
		item13.MaxStackSize = 1;
		item13.Tier = 1;
		item13.BasePrice = 80;
		item13.MaxDurability = 80;
		item13.Durability = 60;
		item13.Attributes.Add(new ItemAttribute("Sharp Damage", 30f));
		item13.Attributes.Add(new ItemAttribute("Blunt Damage", 5f));
		item13.Attributes.Add(new ItemAttribute("_Bleeding", 0.4f));
		item13.Attributes.Add(new ItemAttribute("_IsRanged", false));
		item13.BuildIndex();

		Item item14 = new Item();
		item14.Name = "Geiger Counter";
		item14.Description = "Invented by Hans Geiger. An instrument used to detect radiation.";
		item14.PrefabName = "GeigerCounter";
		item14.SpriteName = "geigercounter";
		item14.Weight = 1.5f;
		item14.ID = "geigercounter";
		item14.Type = ItemType.Tool;
		item14.GridCols = 2;
		item14.GridRows = 2;
		item14.MaxStackSize = 1;
		item14.Tier = 0;
		item14.BasePrice = 260;
		item14.Attributes.Add(new ItemAttribute("Measurement", "Radiation"));
		item14.BuildIndex();

		Item item15 = new Item();
		item15.Name = "Bandages";
		item15.Description = "Millitary Issue bandages. Use as much as you need to stop bleeding.";
		item15.PrefabName = "Bandages";
		item15.SpriteName = "bandages";
		item15.Weight = 0.05f;
		item15.ID = "bandages";
		item15.Type = ItemType.Medicine;
		item15.GridCols = 2;
		item15.GridRows = 1;
		item15.MaxStackSize = 50;
		item15.IsUsable = true;
		item15.UseLimit = 10;
		item15.Tier = 0;
		item15.BasePrice = 25;
		item15.Attributes.Add(new ItemAttribute("_Function", "ReduceBleeding"));
		item15.Attributes.Add(new ItemAttribute("_FunctionValue", 1f));
		item15.BuildIndex();

		Item item16 = new Item();
		item16.Name = "44 Magnum HP";
		item16.Description = "Hollow point 44 Magnum ammo.";
		item16.PrefabName = "Ammo44Magnum";
		item16.SpriteName = "ammo44magnum";
		item16.Weight = 0.05f;
		item16.ID = "ammo44magnum";
		item16.Type = ItemType.Ammo;
		item16.GridCols = 2;
		item16.GridRows = 1;
		item16.MaxStackSize = 50;
		item16.Tier = 0;
		item16.BasePrice = 6;
		item16.Attributes.Add(new ItemAttribute("_Caliber", ".44"));
		item16.Attributes.Add(new ItemAttribute("_numberOfProjectiles", 1));
		item16.Attributes.Add(new ItemAttribute("Damage", 30f));
		item16.Attributes.Add(new ItemAttribute("Penetration", 5f));
		item16.Attributes.Add(new ItemAttribute("_Bleeding", 0.6f));
		item16.Attributes.Add(new ItemAttribute("+Heavy Bleeding", 1f));
		item16.BuildIndex();

		Item item17 = new Item();
		item17.Name = "Mutant Heart";
		item17.Description = "The heart extracted from deseased mutants. Recovers health when consumed. More likely to harvest when mutant is killed with melee weapons.";
		item17.PrefabName = "MutantHeart";
		item17.SpriteName = "mutantheart";
		item17.Weight = 10.05f;
		item17.ID = "mutantheart";
		item17.Type = ItemType.Medicine;
		item17.GridCols = 1;
		item17.GridRows = 1;
		item17.MaxStackSize = 1;
		item17.IsUsable = true;
		item17.UseLimit = 3;
		item17.Tier = 0;
		item17.BasePrice = 500;
		item17.Attributes.Add(new ItemAttribute("_Function", "RestoreHealth"));
		item17.Attributes.Add(new ItemAttribute("_FunctionValue", 50f));
		item17.BuildIndex();

		Item item18 = new Item();
		item18.Name = "Rock";
		item18.Description = "For detecting things";
		item18.PrefabName = "Rock";
		item18.SpriteName = "rock";
		item18.Weight = 0.05f;
		item18.ID = "rock";
		item18.Type = ItemType.Thrown;
		item18.GridCols = 1;
		item18.GridRows = 1;
		item18.MaxStackSize = 1;
		item18.Tier = 0;
		item18.BasePrice = 0;
		item18.BuildIndex();

		Item item19 = new Item();
		item19.Name = "Russian Rubles";
		item19.Description = "Banknotes issued in 1961.";
		item19.PrefabName = "Rubles";
		item19.SpriteName = "rubles";
		item19.Weight = 0.0f;
		item19.ID = "rubles";
		item19.Type = ItemType.Money;
		item19.GridCols = 1;
		item19.GridRows = 1;
		item19.MaxStackSize = 100000;
		item19.Tier = 0;
		item19.BasePrice = 1;
		item19.BuildIndex();

		Item item20 = new Item();
		item20.Name = "Bread";
		item20.Description = "Cold, hard wheat bread. Can stay fresh for a long time.";
		item20.PrefabName = "Bread1";
		item20.SpriteName = "bread1";
		item20.Weight = 0.3f;
		item20.ID = "bread1";
		item20.Type = ItemType.Food;
		item20.GridCols = 1;
		item20.GridRows = 2;
		item20.MaxStackSize = 1;
		item20.IsUsable = true;
		item20.UseLimit = 1;
		item20.Tier = 0;
		item20.BasePrice = 50;
		item20.Attributes.Add(new ItemAttribute("Calories", 350f));
		item20.BuildIndex();

		Item item21 = new Item();
		item21.Name = "Sausages";
		item21.Description = "Smoked Russian sausages, flavored with garlic.";
		item21.PrefabName = "Sausage";
		item21.SpriteName = "sausage";
		item21.Weight = 0.2f;
		item21.ID = "sausage";
		item21.Type = ItemType.Food;
		item21.GridCols = 1;
		item21.GridRows = 1;
		item21.MaxStackSize = 1;
		item21.IsUsable = true;
		item21.UseLimit = 1;
		item21.Tier = 0;
		item21.BasePrice = 100;
		item21.Attributes.Add(new ItemAttribute("Calories", 500f));
		item21.BuildIndex();

		Item item22 = new Item();
		item22.Name = "Sleeping Bag";
		item22.Description = "Old army-issue sleeping bag. Protects against the harsh Russian nights.";
		item22.PrefabName = "SleepingBag";
		item22.SpriteName = "sleepingbag";
		item22.Weight = 4.2f;
		item22.ID = "sleepingbag";
		item22.Type = ItemType.SleepingBag;
		item22.GridCols = 4;
		item22.GridRows = 2;
		item22.MaxStackSize = 1;
		item22.Tier = 0;
		item22.BasePrice = 600;
		item22.BuildIndex();

		Item item23 = new Item();
		item23.Name = "Gasoline";
		item23.Description = "A precious commodity in the zone. Used in gasoline engines and power generators.";
		item23.PrefabName = "Gasoline";
		item23.SpriteName = "gasoline";
		item23.Weight = 0.4f;
		item23.ID = "gasoline";
		item23.Type = ItemType.Fuel;
		item23.GridCols = 2;
		item23.GridRows = 2;
		item23.MaxStackSize = 20;
		item23.Tier = 0;
		item23.BasePrice = 200;
		item23.BuildIndex();

		switch(itemID)
		{
		case "ak47":
			return item1;
			break;
		case "44magnum":
			return item2;
			break;
		case "flakjacket":
			return item3;
			break;
		case "kevlarhelmet":
			return item4;
			break;
		case "pipegrenade":
			return item5;
			break;
		case "lightarmor":
			return item6;
			break;
		case "pasgthelmet":
			return item7;
			break;
		case "ammo762_39":
			return item8;
			break;
		case "ammo12shot":
			return item9;
			break;
		case "huntingshotgun":
			return item10;
			break;
		case "svd":
			return item11;
			break;
		case "ammo762_54r":
			return item12;
			break;
		case "machete":
			return item13;
			break;
		case "geigercounter":
			return item14;
			break;
		case "bandages":
			return item15;
			break;
		case "ammo44magnum":
			return item16;
			break;
		case "mutantheart":
			return item17;
			break;
		case "rock":
			return item18;
			break;
		case "rubles":
			return item19;
			break;
		case "bread1":
			return item20;
			break;
		case "sausage":
			return item21;
			break;
		case "sleepingbag":
			return item22;
			break;
		case "gasoline":
			return item23;
			break;
		}

		return null;
	}
}
