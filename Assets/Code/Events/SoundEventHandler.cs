using UnityEngine;
using System.Collections;

public class SoundEventHandler
{

	#region Singleton 
	private static SoundEventHandler _instance;
	public static SoundEventHandler Instance	
	{
		get 
		{
			if (_instance == null)
				_instance = new SoundEventHandler();
			
			return _instance;
		}
	}
	#endregion
	
	#region Constructor
	public SoundEventHandler()
	{
		Initialize();
	}
	
	#endregion

	#region Public Events
	public delegate void NoiseSourceEventDelegate(Noise noise);

	public static event NoiseSourceEventDelegate OnNoiseMade;
	#endregion

	#region Public Methods

	public void TriggerNoiseEvent(Noise noise)
	{
		if(OnNoiseMade != null)
		{
			OnNoiseMade(noise);
		}
	}

	#endregion
	
	#region Private Methods
	
	private void Initialize()
	{
		
	}
	
	#endregion
}
