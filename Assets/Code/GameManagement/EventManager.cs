using UnityEngine;
using System.Collections;

public class EventManager
{
	#region Public fields
	
	
	#endregion
	
	#region Private fields
	
	#endregion
	
	#region Public methods
	public void Initialize()
	{
		
	}
	
	public void ManagerPerFrameUpdate()
	{

		InputEventHandler.Instance.PerFrameUpdate();
		StoryEventHandler.Instance.PerFrameUpdate();
	}

	public void ManagerFixedUpdate()
	{
		InputEventHandler.Instance.FixedUpdate();
	}
	#endregion
	
	
	#region Private methods
	
	
	
	#endregion



}
