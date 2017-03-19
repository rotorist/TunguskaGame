using UnityEngine;
using System.Collections;

public abstract class QuestBase
{
	public int Stage;

	public abstract void StartQuest();
	public abstract void StopQuest();
	public abstract void PerSecondUpdate();
}
