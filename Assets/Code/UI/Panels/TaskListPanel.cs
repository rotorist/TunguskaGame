using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;



public class TaskListPanel : PanelBase
{
	public GameObject TaskListAnchor;

	private List<TaskListEntry> _taskList;

	public override void Initialize ()
	{
		_taskList = new List<TaskListEntry>();
		Hide();
	}

	public override void PerFrameUpdate ()
	{

	}

	public override void Show ()
	{
		NGUITools.SetActive(this.gameObject, true);
		this.IsActive = true;

		BuildTaskPage();

		InputEventHandler.Instance.State = UserInputState.PopupOpen;

		GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("OpenSplitMenu"), 0.5f);


	}

	public override void Hide ()
	{
		NGUITools.SetActive(this.gameObject, false);
		this.IsActive = false;

		InputEventHandler.Instance.State = UserInputState.WindowsOpen;


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



	public void OnCloseButtonPress()
	{

		Hide();
	}

	private void BuildTaskPage()
	{
		//first remove all existing task entries
		foreach(TaskListEntry entry in _taskList)
		{
			GameObject.Destroy(entry.Checkmark.gameObject);
			GameObject.Destroy(entry.Circle.gameObject);
			GameObject.Destroy(entry.TaskText.gameObject);

		}

		_taskList.Clear();
		float y = 0;
		for(int i = 0; i < GameManager.Inst.PlayerProgress.IncompleteTasks.Count; i++)
		{
			int id = GameManager.Inst.PlayerProgress.IncompleteTasks[i];

			GameObject o = GameObject.Instantiate(Resources.Load("TaskListEntry")) as GameObject;
			TaskListEntry entry = o.GetComponent<TaskListEntry>();
			entry.Checkmark.alpha = 0;
			entry.Circle.alpha = 1;
			entry.transform.parent = TaskListAnchor.transform;

			entry.TaskText.MakePixelPerfect();
			//entry.Checkmark.MakePixelPerfect();
			//entry.Circle.MakePixelPerfect();
			entry.TaskText.text = GameManager.Inst.DBManager.DBHandlerStoryEvent.LoadTask(id);
			entry.transform.localPosition = new Vector3(0, y, 0);

			_taskList.Add(entry);

			y -= entry.TaskText.height + 15;

		}

		y -= 10;

		for(int i = GameManager.Inst.PlayerProgress.CompletedTasks.Count - 1; i >= 0 ; i--)
		{
			int id = GameManager.Inst.PlayerProgress.CompletedTasks[i];

			GameObject o = GameObject.Instantiate(Resources.Load("TaskListEntry")) as GameObject;
			TaskListEntry entry = o.GetComponent<TaskListEntry>();
			entry.Checkmark.alpha = 1;
			entry.Circle.alpha = 0;
			entry.transform.parent = TaskListAnchor.transform;

			entry.TaskText.MakePixelPerfect();
			entry.TaskText.color = new Color(0.5f, 0.5f, 0.5f);
			//entry.Checkmark.MakePixelPerfect();
			//entry.Circle.MakePixelPerfect();
			entry.TaskText.text = GameManager.Inst.DBManager.DBHandlerStoryEvent.LoadTask(id);
			entry.transform.localPosition = new Vector3(0, y, 0);
			_taskList.Add(entry);

			y -= entry.TaskText.height + 15;

			if(y < -675)
			{
				break;
			}
		}
	}


}
