using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager
{
	public AudioSource Detector;
	public AudioSource Music;
	public AudioSource UI;
	public AudioSource PlayerAudio;
	public AudioSource Ambient1;
	public AudioSource Ambient2;

	public Transform Listener;

	private Dictionary<string, AudioClip> _audioClipPool;
	private float _earRingTimer;
	private float _earRingDuration;
	private float _earRingYPos;
	private float _normalYPos;
	private AudioClip _nextMusicClip;

	public void PerFrameUpdate()
	{
		if(Listener != null)
		{

			if(_earRingDuration > _earRingTimer)
			{
				_normalYPos = _earRingYPos;
				_earRingTimer += Time.deltaTime;
			}
			else
			{
				_normalYPos = Mathf.Lerp(_normalYPos, 3f, Time.deltaTime * 6);
			}

			Listener.localPosition = GameManager.Inst.PlayerControl.SelectedPC.transform.position + new Vector3(0, _normalYPos, 0);
			Listener.rotation = GameManager.Inst.CameraController.transform.rotation;

			//Listener.localPosition = GameManager.Inst.CameraController.transform.position;
			//Listener.LookAt(GameManager.Inst.PlayerControl.SelectedPC.transform);

		}

		if(!Music.isPlaying)
		{
			if(_nextMusicClip != null)
			{
				Music.clip = _nextMusicClip;
				Music.Play();
			}

		}
	}



	public AudioClip GetClip(string id)
	{
		if(_audioClipPool.ContainsKey(id))
		{
			return _audioClipPool[id];
		}
		else
		{
			//try to load the clip
			AudioClip clip = Resources.Load(id) as AudioClip;
			if(clip == null)
			{
				return null;
			}
			else
			{
				_audioClipPool.Add(id, clip);
				return clip;
			}
		}
	}

	public void PlayDetectorSound(string id)
	{
		Detector.clip = GetClip(id);
		Detector.Play();
	}

	public string GetDetectorCurrentClip()
	{
		if(Detector.clip != null)
		{
			return Detector.clip.name;
		}
		else
		{
			return "";
		}
	}

	public void StartPlayerEarRingEffect(float intensity)
	{
		_earRingYPos = 40 * intensity;
		_earRingDuration = 4 * intensity;
		_earRingTimer = 0;
		UI.PlayOneShot(GetClip("ear_ring"), 0.1f * intensity);
	}

	public void SetMusic(string envName, bool isDayTime)
	{
		if(envName == "Wilderness")
		{
			if(isDayTime)
			{
				AudioClip clip = Resources.Load("DayBreak") as AudioClip;

				_nextMusicClip = clip;
			}
			else
			{
				AudioClip clip = Resources.Load("TheHorrorNight") as AudioClip;
				_nextMusicClip = clip;
			}
		}
		else
		{
			Music.Stop();
			_nextMusicClip = null;
		}
	}

	public void Initialize()
	{
		GameObject listenerObj = GameObject.Instantiate(Resources.Load("AudioListener")) as GameObject;


		_audioClipPool = new Dictionary<string, AudioClip>();
		Listener = listenerObj.transform;
		Detector = GameObject.Find("AudioSourceDetector").GetComponent<AudioSource>();
		UI = GameObject.Find("AudioSourceUI").GetComponent<AudioSource>();
		Ambient1 = GameObject.Find("AudioSourceAmbient1").GetComponent<AudioSource>();
		Ambient2 = GameObject.Find("AudioSourceAmbient2").GetComponent<AudioSource>();
		Music = Listener.GetComponent<AudioSource>();
		_normalYPos = 3;
	}
}
