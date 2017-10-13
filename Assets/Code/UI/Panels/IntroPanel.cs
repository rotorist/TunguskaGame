using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;

public class IntroPanel : PanelBase
{
	public float SlideTimeout = 5;
	public UISprite Photo0;
	public UISprite Photo1;
	public UISprite Photo2;
	public UISprite Photo3;
	public UISprite Photo4;
	public UISprite Photo5;
	public UISprite Photo6;
	public UILabel Caption;

	private int _currentSlide;
	private float _slideTimer;
	private UISprite _currentPhoto;
	private float _originalMusicVolume;

	public override void Initialize ()
	{
		_currentSlide = 0;
		_slideTimer = 0;
		Hide();
	}

	public override void PerFrameUpdate ()
	{
		if(_slideTimer < SlideTimeout)
		{
			if(_slideTimer < 2)
			{
				_currentPhoto.alpha += Time.deltaTime * 3;
				Caption.alpha += Time.deltaTime * 3;

			}
			else if(_slideTimer > SlideTimeout - 2)
			{
				_currentPhoto.alpha -= Time.deltaTime * 3;
				Caption.alpha -= Time.deltaTime * 3;
			}
			else
			{
				_currentPhoto.alpha = 1;
				Caption.alpha = 1;
			}

			if(_currentSlide < 6)
			{
				_currentPhoto.transform.Translate(new Vector3(-1, -1, 0) * Time.deltaTime * 0.005f);
			}
			else
			{
				if(_slideTimer > 2)
				{
					GameManager.Inst.SoundManager.Music.volume -= Time.deltaTime * 0.06f;
				}
			}

			_slideTimer += Time.deltaTime;
		}
		else
		{
			if(_currentSlide >= 6)
			{
				EndSlideShow();
			}
			else
			{
				_currentSlide ++;
				AssembleSlide(_currentSlide);
				_slideTimer = 0;
			}
		}

		if(Input.GetKeyDown(KeyCode.Escape))
		{
			EndSlideShow();
		}

		if((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0)) && _currentSlide < 6)
		{
			
			_currentSlide ++;
			AssembleSlide(_currentSlide);
			_slideTimer = 0;
		}
	}

	public override void Show ()
	{
		NGUITools.SetActive(this.gameObject, true);
		this.IsActive = true;

		_slideTimer = 0;
		_currentSlide = 0;
		AssembleSlide(_currentSlide);
		_originalMusicVolume = GameManager.Inst.SoundManager.Music.volume;
		AudioClip clip = Resources.Load("times") as AudioClip;
		GameManager.Inst.SoundManager.Music.PlayOneShot(clip);

		InputEventHandler.Instance.State = UserInputState.Intro;




	}

	public override void Hide ()
	{
		NGUITools.SetActive(this.gameObject, false);
		this.IsActive = false;



		InputEventHandler.Instance.State = UserInputState.Normal;


	}

	public override bool HasBodySlots (out List<BodySlot> bodySlots)
	{
		bodySlots = null;
		return false;
	}

	public override bool HasTempSlots (out List<TempSlot> tempSlots)
	{
		tempSlots = null;

		return false;
	}

	private void EndSlideShow()
	{
		GameManager.Inst.SoundManager.Music.Stop();
		GameManager.Inst.SoundManager.Music.volume = _originalMusicVolume;

		UIEventHandler.Instance.TriggerEndIntro();

		UIEventHandler.Instance.TriggerJournal();
		NGUITools.SetActive(GameManager.Inst.UIManager.HUDPanel.HelpText.gameObject, true);
	}


	private void AssembleSlide(int slideNumber)
	{
		if(_currentPhoto != null)
		{
			_currentPhoto.alpha = 0;
		}

		switch(slideNumber)
		{
		case 0:
			_currentPhoto = Photo0;
			Caption.text = "One night in 1908, when my grand mother was just a young girl living in Sibera, she witnessed a devastating explosion in the sky. ";
			break;
		case 1:
			_currentPhoto = Photo1;
			Caption.text = "This explosion lit up her home hundreds of kilometers away like a winter sun, and flattened an entire forest to the ground. It was known as the Tunguska Event, and scientists quickly dismissed it as just a stray asteroid.\n\nI wish it was really that simple.";
			break;
		case 2:
			_currentPhoto = Photo2;
			Caption.text = "After the Great Wars, an abundance of plutonium ore was found near the event site, and it quickly brought prosperity to Tunguska. But the region suddenly fell into the hands of Death within a decade of its modernization.";
			break;
		case 3:
			_currentPhoto = Photo3;
			Caption.text = "Residents mysteriously mutated into devilish beasts and it spread across Tunguska within weeks. The authorities immediately quarantined the area, and named it officially as the Tunguska Exclusion Zone.Tunguska was left to rot in isolation.";
			break;
		case 4:
			_currentPhoto = Photo4;
			Caption.text = "While the citizens evacuated, some researchers stayed behind to study the mutation and find a cure. No cure was found, but an accidental discovery triggered a gold rush in Tunguska.";
			break;
		case 5:
			_currentPhoto = Photo5;
			Caption.text = "Despite the government's effort to cover up and the Zone's immense danger, prospectors flooded into the Exclusion Zone, hunted for mutants and used their organs to concoct highly priced \"serums\". Some found gold, most perished. But it never stopped those who would rather risk death in the Zone, than to loiter their lives away in the motionless world outside.";
			break;
		case 6:
			_currentPhoto = Photo6;
			Caption.text = "My brother, Boris Kravshenko, was one of them.";
			break;
		}

		if(_currentSlide < 6)
		{
			SlideTimeout = 5 + (1f * Caption.height / Caption.fontSize - 1) * 6f;
		}
		else
		{
			SlideTimeout = 6;
		}

		_currentPhoto.alpha = 0;
		Caption.alpha = 0;
	}





}
