using UnityEngine;
using System.Collections;

[System.Serializable]
public class PickupItemData
{
	public string ItemID;
	public int Quantity;
	public float Durability;
	public SerVector3 Pos;
	public SerVector3 EulerAngles;

}

public class PickupItem : MonoBehaviour 
{
	public Item Item;
	public string ItemID;
	public int Quantity;
	public float Durability = 1;


	private ItemSparkle _sparkle;
	private float _timer;
	private bool _isShining;

	private float _checkLoSTimer;
	private float _timeToWait;


	void Update()
	{


		if(_sparkle != null && _isShining)
		{
			_sparkle.transform.position = transform.position + _sparkle.transform.forward * 0.1f;

			float scale = _sparkle.ShiningCurve.Evaluate(_timer);
			_sparkle.transform.localScale = scale * new Vector3(0.5f, 0.5f, 0.5f);
			_sparkle.transform.RotateAround(_sparkle.transform.position, _sparkle.transform.forward, 300 * Time.deltaTime);


			_timer += Time.deltaTime * 2;
			if(_timer >= 1)
			{
				_isShining = false;
			}

		}

		if(!_isShining)
		{
			if(_checkLoSTimer >= _timeToWait)
			{
				//check each player party member to see if his flashlight is shining on me
				foreach(HumanCharacter character in GameManager.Inst.PlayerControl.Party.Members)
				{
					Vector3 distance = Vector3.zero;
					if(character.MyReference.Flashlight != null)
					{
						distance = character.MyReference.Flashlight.transform.position - transform.position;
					}
					else
					{
						distance = character.MyReference.Eyes.transform.position - transform.position;
					}

					if(character.MyReference.Flashlight != null && character.MyReference.Flashlight.IsOn && 
						distance.magnitude < character.MyReference.Flashlight.Light.range &&
						Vector3.Angle(distance, character.MyReference.Flashlight.transform.forward * -1) < character.MyReference.Flashlight.Light.spotAngle/2f)
					{
						//do a raycast check. Only shines if raycast from flashlight hits the item
						RaycastHit hit;
						bool isHit = Physics.Raycast(character.MyReference.Flashlight.transform.position, distance * -1, out hit, distance.magnitude * 2);
						//Debug.Log(isHit + " " + hit.collider.name);
						if(hit.collider.gameObject == this.gameObject)
						{
							
			
							ShineSparkle();
							break;

						}


					}
				}

				_checkLoSTimer = 0;
				_timeToWait = UnityEngine.Random.Range(0.75f, 1.5f);
			}

			_checkLoSTimer += Time.deltaTime;
		}

	}

	public void ShineSparkle()
	{
		if(_sparkle == null)
		{
			GameObject o = GameObject.Instantiate(Resources.Load("ItemSparkle")) as GameObject;
			_sparkle = o.GetComponent<ItemSparkle>();

		}

		_sparkle.transform.LookAt(Camera.main.transform);

		_isShining = true;
		_timer = 0;
	}

	public GameObject GetSparkleObject()
	{
		if(_sparkle == null)
		{
			return null;
		}

		return _sparkle.gameObject;
	}


}
