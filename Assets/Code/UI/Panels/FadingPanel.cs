using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class FadingPanel : PanelBase
{
	public UISprite Background;

	private float _timer;
	private float _fadeOutDuration;
	private float _fadeInDuration;
	private float _interDuration;
	private int _direction; //-1 = fade out, 0 = stay, 1 = fade in

	public delegate void FadeCallBack();

	private FadeCallBack _onFadeOutDone;
	private bool _callBackRequested;

	public override void Initialize ()
	{

		Hide();
	}

	public override void PerFrameUpdate ()
	{
		if(_timer < 0)
		{
			return;
		}

		float deltaTime = Time.deltaTime;
		if(Time.timeScale == 0)
		{
			deltaTime = Time.unscaledDeltaTime;
		}

		if(_direction == -1)
		{
			
			Background.alpha = Background.alpha + deltaTime * (1 / _fadeOutDuration);
			if(Background.alpha > 1)
			{
				Background.alpha = 1;
				if(_interDuration > 0)
				{
					_fadeOutDuration = 0;
					_timer = 0;
					_direction = 0;
					if(_callBackRequested && _onFadeOutDone != null)
					{
						_onFadeOutDone();
						_callBackRequested = false;
					}
				}
				else
				{
					_timer = -1;
				}
			}
		}
		else if(_direction == 1)
		{
			Background.alpha = Background.alpha - deltaTime * (1 / _fadeInDuration);
			if(Background.alpha < 0)
			{
				Background.alpha = 0;
				if(_interDuration > 0)
				{
					_fadeInDuration = 0;
					_timer = 0;
					_direction = 0;
				}
				else
				{
					_timer = -1;
				}
			}
		}
		else if(_direction == 0)
		{
			if(_timer < _interDuration)
			{
				_timer += deltaTime * (1 / _interDuration);
			}
			else
			{
				if(_fadeInDuration > 0)
				{
					_interDuration = 0;
					_timer = 0;
					_direction = 1;
				}
				else if(_fadeOutDuration > 0)
				{
					_interDuration = 0;
					_timer = 0;
					_direction = -1;
				}
				else
				{
					_interDuration = 0;
					_timer = -1;
				}
			}
		}

	}

	public override void Show ()
	{
		Background.MakePixelPerfect();
		Background.alpha = 1;
		Background.height = GameManager.Inst.UIManager.GetScreenHeight() + 500;
		Background.width = GameManager.Inst.UIManager.GetScreenWidth() + 500;

		NGUITools.SetActive(this.gameObject, true);
		this.IsActive = true;

	}

	public override void Hide ()
	{

		NGUITools.SetActive(this.gameObject, false);
		this.IsActive = false;



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

	public void FadeOutAndIn(float fadeInDuration, float interDuration, float fadeOutDuration)
	{
		_fadeOutDuration = fadeOutDuration;
		_fadeInDuration = fadeInDuration;
		_interDuration = interDuration;
		_timer = 0;
		_direction = -1;
		Background.alpha = 0;
	}

	public void FadeOutAndInCallBack(float fadeInDuration, float interDuration, float fadeOutDuration, FadeCallBack callBack)
	{
		_fadeOutDuration = fadeOutDuration;
		_fadeInDuration = fadeInDuration;
		_interDuration = interDuration;
		_timer = 0;
		_direction = -1;
		Background.alpha = 0;

		_onFadeOutDone = callBack;
		_callBackRequested = true;
	}

	public void FadeIn(float fadeInDuration)
	{
		_fadeOutDuration = 0;
		_fadeInDuration = fadeInDuration;
		_interDuration = 0;
		_timer = 0;
		_direction = 1;

		Background.alpha = 1;
	}

}
