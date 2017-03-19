using UnityEngine;
using System.Collections;

public class ArmorSystem 
{
	public GameObject ArmorTop;
	public GameObject ArmorBottom;
	public GameObject Helmet;

	private Character _parentCharacter;

	public ArmorSystem(Character parent)
	{
		_parentCharacter = parent;
	}

	public void SwitchToHelmet(Item helmet)
	{
		//first clear what's already there
		if(Helmet != null)
		{
			GameObject.Destroy(Helmet);
			Helmet = null;
		}

		GameObject hat = _parentCharacter.Model.transform.Find("Hats").gameObject;


		if(helmet == null)
		{
			//show hats

			if(hat != null)
			{
				hat.GetComponent<SkinnedMeshRenderer>().enabled = true;
			}
		}
		else
		{
			bool hideHats = (bool)helmet.GetAttributeByName("_hideHats").Value;

			//hide hats and load helmet
			if(hat != null)
			{
				if(hideHats)
				{
					hat.GetComponent<SkinnedMeshRenderer>().enabled = false;
				}
				else
				{
					hat.GetComponent<SkinnedMeshRenderer>().enabled = true;
				}
			}

			Helmet = GameObject.Instantiate(Resources.Load(helmet.PrefabName)) as GameObject;
			Helmet.transform.parent = _parentCharacter.MyReference.HelmetMount.transform;
			Helmet.transform.localPosition = Vector3.zero;
			Helmet.transform.localEulerAngles = Vector3.zero;
		}
	}

	public void SwitchToArmor(Item armor)
	{
		if(armor != null)
		{
			//load new character model according to armor's name
			string newModelName = _parentCharacter.CharacterID + armor.GetAttributeByName("_ModelSuffix").Value.ToString();
			if(_parentCharacter.Model.name != newModelName)
			{
				_parentCharacter.SendCommand(CharacterCommands.Idle);
				_parentCharacter.LoadCharacterModel(newModelName);

			}

			bool isFull = (bool)armor.GetAttributeByName("_IsFull").Value;

			Material m = (Material)Resources.Load(armor.GetAttributeByName("_TextureName").Value.ToString());
			ArmorTop = _parentCharacter.Model.transform.Find("Tops").gameObject;
			ArmorTop.GetComponent<SkinnedMeshRenderer>().material = m;

			if(isFull)
			{
				//load bottoms as well
				Material m2 = (Material)Resources.Load(armor.GetAttributeByName("_TextureName2").Value.ToString());
				ArmorBottom = _parentCharacter.Model.transform.Find("Bottoms").gameObject;
				ArmorBottom.GetComponent<SkinnedMeshRenderer>().material = m2;

			}
		}
		else
		{
			//load base character model
			string newModelName = _parentCharacter.CharacterID;
			if(_parentCharacter.Model.name != newModelName)
			{
				_parentCharacter.SendCommand(CharacterCommands.Idle);
				_parentCharacter.LoadCharacterModel(newModelName);

			}
		}


	}

}
