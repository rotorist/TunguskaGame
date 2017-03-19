using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BarkPanel : PanelBase
{
	private Dictionary<Character, BarkLabel> _barks;
	private Character _currentCharacter;
	private HealthFloater _currentFloater;

	public override void Initialize()
	{
		_barks = new Dictionary<Character, BarkLabel>();

	}

	// Update is called once per frame
	public override void PerFrameUpdate ()
	{
		Dictionary<Character, BarkLabel> barksCopy = new Dictionary<Character, BarkLabel>(_barks);

		foreach(KeyValuePair<Character, BarkLabel> bark in barksCopy)
		{
			Character target = bark.Key;
			Vector3 screenPos = GetBarkScreenPos(target);
			bark.Value.BarkText.transform.localPosition = screenPos;

			//check if bark is outside screen and is inactive. if outside screen and inactive, delete the bark

			if(!bark.Value.IsActive())
			{
				if(Mathf.Abs(screenPos.x) > Screen.width/2 || Mathf.Abs(screenPos.y) > Screen.height/2)
				{
					GameObject.Destroy(bark.Value.gameObject);
					_barks.Remove(bark.Key);

				}
			}
		}

		/*
		GameObject aimedObject = GameManager.Inst.PlayerControl.GetAimedObject();
		if(aimedObject != null && aimedObject.GetComponent<Character>() != null && aimedObject.GetComponent<Character>().Faction != GameManager.Inst.PlayerControl.SelectedPC.Faction)
		{
			Character aimedCharacter = aimedObject.GetComponent<Character>();
			if(aimedCharacter != _currentCharacter)
			{
				AddFloater(aimedCharacter);
			}
			Vector3 screenPosFloater = GetFloaterScreenPos(_currentCharacter);
			_currentFloater.transform.localPosition = screenPosFloater;

			_currentFloater.Fill.width = Mathf.CeilToInt(100 * aimedCharacter.MyStatus.Health / aimedCharacter.MyStatus.MaxHealth);
		}
		else
		{
			_currentCharacter = null;
			if(_currentFloater != null)
			{
				GameObject.Destroy(_currentFloater.gameObject);
			}
		}
		*/
	}

	public void AddBark(Character target, string text)
	{
		if(!_barks.ContainsKey(target))
		{
			GameObject o = GameObject.Instantiate(Resources.Load("BarkLabel")) as GameObject;
			BarkLabel label = o.GetComponent<BarkLabel>();
			
			label.transform.parent = this.transform;
			label.transform.localScale = new Vector3(1, 1, 1);
			_barks.Add(target, label);
		}

		_barks[target].SetBarkText(text, 0.2f * text.Length);

		Vector3 screenPos = GetBarkScreenPos(target);
		_barks[target].transform.localPosition = screenPos;
	}

	public void AddFloater(Character target)
	{
		if(_currentFloater == null)
		{
			GameObject o = GameObject.Instantiate(Resources.Load("EnemyHealth")) as GameObject;
			_currentFloater = o.GetComponent<HealthFloater>();

			_currentFloater.transform.parent = this.transform;
			_currentFloater.transform.localScale = new Vector3(1, 1, 1);
		}

		_currentFloater.Fill.MakePixelPerfect();
		_currentFloater.Fill.height = 10;
		_currentCharacter = target;

	}



	private Vector3 GetBarkScreenPos(Character target)
	{
		if(target == null)
		{
			return Vector3.zero;
		}
		Vector3 worldPos = target.transform.position + new Vector3(0, target.GetComponent<CapsuleCollider>().height + 0.4f, 0);
		Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

		screenPos -= new Vector3(Screen.width/2, Screen.height/2);

		return screenPos;
	}

	private Vector3 GetFloaterScreenPos(Character target)
	{
		Vector3 worldPos = target.transform.position;
		Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

		screenPos -= new Vector3(Screen.width/2, Screen.height/2);
		return screenPos;
	}
}
