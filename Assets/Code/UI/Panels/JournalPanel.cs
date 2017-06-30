using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;


public class JournalPanel : PanelBase
{
	public UILabel TestLabel;
	public int JournalFontSize;
	public int JournalEntrySpacing;
	public int JournalPageHeight;
	public GameObject LeftPageAnchor;
	public GameObject RightPageAnchor;


	public List<List<string>> ProcessedJournal;

	public override void Initialize ()
	{

		Hide();
	}

	public override void PerFrameUpdate ()
	{
		

	}

	public override void Show ()
	{
		NGUITools.SetActive(this.gameObject, true);
		this.IsActive = true;



		InputEventHandler.Instance.State = UserInputState.Dialogue;



		Time.timeScale = 0;



		LoadJournal();
		DisplayJournalPage(2);
		DisplayJournalPage(3);

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
			yPos = JournalFontSize + JournalEntrySpacing;


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

		if(pageNumber % 2 == 0)
		{
			//use left page
			anchor = LeftPageAnchor;
		}
		else
		{
			//use right page
			anchor = RightPageAnchor;
		}

		//first remove all current entry labels
		foreach(Transform child in anchor.transform)
		{
			GameObject.Destroy(child.gameObject);
		}

		if(ProcessedJournal.Count <= pageNumber)
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
	}
}
