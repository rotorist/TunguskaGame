using UnityEngine;
using System.Collections;

public class BloodDamage : MonoBehaviour {

	public enum damageDirection {
		up, down, left, right
	}
	public enum Gender {
		male, female
	}

	public static BloodDamage instance;
	public float fadeOutDelay=0.3f;
	public float fadeOutSpeed=1f;
	public bool useAudioDamage=true;
	public Gender sex;
	public bool startDamage = false;

	private AudioClip[] soundLevelMale = new AudioClip[5];
	private AudioClip[] soundLevelFemale = new AudioClip[5];
	private Texture2D[] dirLevel = new Texture2D[4];
	private Texture2D[] level = new Texture2D[5];
	private float alpha=0f;
	private Color guiColor;
	private float delayTime=0f;

	private Texture2D damageTexture;
	private bool fixedDamage = false;
	private Texture2D fixedDamageTexture;
	private AudioClip damageSound;

	public void doDamage(int dlevel) {
		if(dlevel<0) dlevel=0;
		if(dlevel>4) dlevel=4;
		damageTexture = level [dlevel];
		startDamage = true;
		delayTime = Time.time;
		alpha=0.0f;
		if(sex==Gender.male)
			damageSound = soundLevelMale [dlevel];
		else
			damageSound = soundLevelFemale [dlevel];
		//GetComponent<AudioSource>().PlayOneShot (damageSound);
	}

	public void doDirectionalDamage(damageDirection dir) {
		damageTexture = dirLevel [dir.GetHashCode()];
		startDamage = true;
		delayTime = Time.time;
		alpha=0.0f;
		if(sex==Gender.male)
			damageSound = soundLevelMale [0];
		else
			damageSound = soundLevelFemale [0];
		//GetComponent<AudioSource>().PlayOneShot (damageSound);
	}

	public void SetFixedDamage(int dlevel) {
		if(dlevel<0) dlevel=0;
		if(dlevel>4) dlevel=4;
		fixedDamageTexture = level [dlevel];
		fixedDamage = true;
	}
	
	public void ClearFixedDamage() {
		fixedDamage = false;
	}

	public void PlayDamagSound(int dlevel) {
		if(dlevel<0) dlevel=0;
		if(dlevel>4) dlevel=4;
		if(sex==Gender.male)
			damageSound = soundLevelMale [dlevel];
		else
			damageSound = soundLevelFemale [dlevel];
		//GetComponent<AudioSource>().PlayOneShot (damageSound);
	}

	public void SetSoundGender(Gender gender) {
		sex = gender;
	}

	public Gender GetSoundGender() {
		return sex;
	}

	void Awake() {
		dirLevel[0]=Resources.Load<Texture2D> ("dir-up");
		dirLevel[1]=Resources.Load<Texture2D> ("dir-down");
		dirLevel[2]=Resources.Load<Texture2D> ("dir-left");
		dirLevel[3]=Resources.Load<Texture2D> ("dir-right");
		level[0] = Resources.Load<Texture2D> ("level-0");
		level[1] = Resources.Load<Texture2D> ("level-1");
		level[2] = Resources.Load<Texture2D> ("level-2");
		level[3] = Resources.Load<Texture2D> ("level-3");
		level[4] = Resources.Load<Texture2D> ("level-4");
		soundLevelMale[0] = Resources.Load<AudioClip> ("male_damage_0");
		soundLevelMale[1] = Resources.Load<AudioClip> ("male_damage_1");
		soundLevelMale[2] = Resources.Load<AudioClip> ("male_damage_2");
		soundLevelMale[3] = Resources.Load<AudioClip> ("male_damage_3");
		soundLevelMale[4] = Resources.Load<AudioClip> ("male_damage_4");
		soundLevelFemale[0] = Resources.Load<AudioClip> ("female_damage_0");
		soundLevelFemale[1] = Resources.Load<AudioClip> ("female_damage_1");
		soundLevelFemale[2] = Resources.Load<AudioClip> ("female_damage_2");
		soundLevelFemale[3] = Resources.Load<AudioClip> ("female_damage_3");
		soundLevelFemale[4] = Resources.Load<AudioClip> ("female_damage_4");
		instance = this;
	}

	// Use this for initialization
	void Start () {
		guiColor = Color.white;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		if (fixedDamage) {
			GUI.DrawTexture (new Rect(0,0, Screen.width, Screen.height), fixedDamageTexture);
		}
		Color alphaAnt = GUI.color;
		if(startDamage) {
			if(Time.time-delayTime > fadeOutDelay) {
				alpha = Mathf.Lerp(alpha, 1f, fadeOutSpeed * Time.deltaTime);
			}
			guiColor.a = 1f-alpha;
			GUI.color = guiColor;
			GUI.DrawTexture (new Rect(0,0, Screen.width, Screen.height), damageTexture);
			if(alpha>0.99)
				startDamage=false;
		}
		GUI.color = alphaAnt;
	}
}
