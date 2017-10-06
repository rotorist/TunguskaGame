using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;


public class JournalPanel : PanelBase
{
	public UILabel TestLabel;
	public int JournalEntrySpacing;
	public int JournalPageHeight;
	public GameObject LeftPageAnchor;
	public GameObject RightPageAnchor;
	public UIButton ArrowLeft;
	public UIButton ArrowRight;
	public UILabel PageNumberLeft;
	public UILabel PageNumberRight;
	public List<List<string>> ProcessedJournal;

	private int _currentLeftPage;

	public override void Initialize ()
	{
		LoadJournal();
		if(ProcessedJournal.Count % 2 == 0)
		{
			_currentLeftPage = ProcessedJournal.Count - 2;
		}
		else
		{
			_currentLeftPage = ProcessedJournal.Count - 1;
		}
		Hide();
	}

	public override void PerFrameUpdate ()
	{
		

	}

	public override void Show ()
	{
		NGUITools.SetActive(this.gameObject, true);
		this.IsActive = true;



		InputEventHandler.Instance.State = UserInputState.WindowsOpen;



		Time.timeScale = 0;



		LoadJournal();
		if(_currentLeftPage < ProcessedJournal.Count)
		{
			DisplayJournalPage(_currentLeftPage);

		}

		if(_currentLeftPage + 1 < ProcessedJournal.Count)
		{
			DisplayJournalPage(_currentLeftPage + 1);
		}

	}

	public override void Hide ()
	{
		NGUITools.SetActive(this.gameObject, false);
		this.IsActive = false;

		UIEventHandler.Instance.TriggerCloseWindow();

		Time.timeScale = 1;

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


	public void OnLeftArrowClick()
	{
		if(_currentLeftPage >= 2)
		{
			DisplayJournalPage(_currentLeftPage - 2);
			DisplayJournalPage(_currentLeftPage + 1);

			PlayFlipPageSound();
		}


	}

	public void OnRightArrowClick()
	{
		if(_currentLeftPage < ProcessedJournal.Count - 1)
		{
			DisplayJournalPage(_currentLeftPage + 2);
			DisplayJournalPage(_currentLeftPage + 1);

			PlayFlipPageSound();
		}
	}

	public void OnCloseButtonClick()
	{
		Hide();
	}

	public void OnTasksButtonClick()
	{
		GameManager.Inst.UIManager.WindowPanel.TaskListPanel.Show();

	}


	private void LoadJournal()
	{
		//first figure out how many pages will current day's entries take
		//build a list of pages based on text size
		ProcessedJournal = new List<List<string>>();
		int pageNumber = 0;
		int yPos = 0;
		int dayNumber = 1;
		int entryIndex = 0;
		foreach(List<string> dayJournal in GameManager.Inst.PlayerProgress.JournalEntries)
		{
			List<string> page = new List<string>();
			ProcessedJournal.Add(page);
			page.Add("Day " + dayNumber);
			yPos = TestLabel.height + JournalEntrySpacing;


			//loop through each day's entry and if its longer than one page, overflow to next page
			foreach(string entry in dayJournal)
			{
				TestLabel.text = entry;
				if(yPos + TestLabel.height > JournalPageHeight)
				{
					//start a new page
					page = new List<string>();
					ProcessedJournal.Add(page);
					page.Add(entry);
					yPos = TestLabel.height + JournalEntrySpacing;
				}
				else
				{
					page.Add(entry);
					yPos += TestLabel.height + JournalEntrySpacing;
				}
			}


			dayNumber ++;
		}

		TestLabel.text = "";

	}

	private void DisplayJournalPage(int pageNumber)
	{
		GameObject anchor;
		bool isLeft = true;
		if(pageNumber % 2 == 0)
		{
			//use left page
			anchor = LeftPageAnchor;
		}
		else
		{
			//use right page
			anchor = RightPageAnchor;
			isLeft = false;
		}

		//first remove all current entry labels
		foreach(Transform child in anchor.transform)
		{
			GameObject.Destroy(child.gameObject);
		}

		if(ProcessedJournal.Count <= pageNumber || pageNumber < 0)
		{
			return;
		}

		int yPos = 0;

		foreach(string entry in ProcessedJournal[pageNumber])
		{
			GameObject o = GameObject.Instantiate(Resources.Load("JournalEntry")) as GameObject;
			UILabel entryLabel = o.GetComponent<UILabel>();

			entryLabel.transform.parent = anchor.transform;
			entryLabel.transform.localPosition = new Vector3(0, yPos, 0);
			entryLabel.MakePixelPerfect();

			entryLabel.text = entry;
			yPos -= entryLabel.height + JournalEntrySpacing;
		}



		int rightPageNumber;

		if(isLeft)
		{
			_currentLeftPage = pageNumber;
			rightPageNumber = _currentLeftPage + 1;
		}
		else
		{
			_currentLeftPage = pageNumber - 1;
			rightPageNumber = pageNumber;
		}

		PageNumberLeft.text = (_currentLeftPage + 1).ToString();
		PageNumberRight.text = (rightPageNumber + 1).ToString();

		NGUITools.SetActive(ArrowRight.gameObject, true);
		NGUITools.SetActive(ArrowLeft.gameObject, true);

		if(rightPageNumber >= ProcessedJournal.Count - 1)
		{
			NGUITools.SetActive(ArrowRight.gameObject, false);
		}

		if(_currentLeftPage <= 0)
		{
			NGUITools.SetActive(ArrowLeft.gameObject, false);
		}

			

	}

	private void PlayFlipPageSound()
	{
		int choice = UnityEngine.Random.Range(1, 4);
		GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("PageFlip" + choice.ToString()), 0.2f);
	}
}
