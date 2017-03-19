using UnityEngine;
using System.Collections;

public class BarkLabel : MonoBehaviour 
{
	public UILabel BarkText;


	private float _duration;
	private float _fadeTimer;

	void Update()
	{
		if(IsActive())
		{
			BarkText.alpha = Mathf.Clamp(1 - (_fadeTimer / _duration), 0, 1);

			_fadeTimer += Time.deltaTime;
		}
	}

	public void SetBarkText(string text, float duration)
	{
		BarkText.text = text;
		_duration = duration;
		_fadeTimer = 0;
	}

	public bool IsActive()
	{
		return _fadeTimer < _duration;
	}
}
