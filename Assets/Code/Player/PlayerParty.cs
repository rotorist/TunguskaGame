using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerParty 
{
	public List<HumanCharacter> Members;
	public HumanCharacter SelectedMember;
	public GoapGoal MemberGuardGoal;
	public GoapGoal MemberFollowGoal;

	public PartyTasks SelectedMemberTask;




	private PlayerControl _playerControl;
	private HumanCharacter _lastAssignedMember;


	public void PerFrameUpdate()
	{
		//update task marker
		/*
		SelectedMember.Markers.TaskMarker.transform.eulerAngles = new Vector3(-90, 0, 0);
		Vector3 direction = SelectedMember.MyAI.BlackBoard.GuardDirection;
		if(SelectedMember.MyAI.BlackBoard.GuardConfigStage == 1)
		{
			direction = SelectedMember.AimPoint - SelectedMember.Markers.TaskMarker.transform.position;
			SelectedMember.Markers.TaskMarker.transform.position = _playerControl.GetTempGuardDest() + new Vector3(0, 0.1f, 0);
		}
		else
		{
			SelectedMember.Markers.TaskMarker.transform.position = SelectedMember.MyAI.BlackBoard.PatrolLoc + new Vector3(0, 0.1f, 0);
		}

		direction = new Vector3(direction.x, 0, direction.z);

		SelectedMember.Markers.TaskMarker.transform.rotation = Quaternion.FromToRotation(SelectedMember.Markers.TaskMarker.transform.up, direction) 
																* SelectedMember.Markers.TaskMarker.transform.rotation;

		//update enemy target circle
		Character target = SelectedMember.MyAI.BlackBoard.TargetEnemy;
		if(target == null)
		{
			target = SelectedMember.MyAI.BlackBoard.InvisibleEnemy;
		}
		if(target != null)
		{
			SelectedMember.Markers.CircleEnemyTarget.transform.position = target.transform.position + new Vector3(0, 0.01f, 0);
		}
		else
		{
			SelectedMember.Markers.CircleEnemyTarget.transform.position = new Vector3(0, -100, 0);
		}

		//update follow team arrow
		Character followTarget = SelectedMember.MyAI.BlackBoard.FollowTarget;
		if(followTarget != null)
		{
			direction = followTarget.transform.position - SelectedMember.transform.position;
			direction = new Vector3(direction.x, 0, direction.z);

			SelectedMember.Markers.TeamFollowArrow.transform.position = followTarget.transform.position + new Vector3(0, 0.01f, 0);
			SelectedMember.Markers.TeamFollowArrow.transform.rotation = Quaternion.FromToRotation(SelectedMember.Markers.TeamFollowArrow.transform.up, direction) 
				* SelectedMember.Markers.TeamFollowArrow.transform.rotation;
		}
		else
		{
			SelectedMember.Markers.TeamFollowArrow.transform.position = new Vector3(0, -100, 0);
		}
		*/

	}


	public PlayerParty(PlayerControl control)
	{
		Members = new List<HumanCharacter>();
		_playerControl = control;


	}

	public void Initialize()
	{
		GameObject playerObj = GameManager.Instantiate(Resources.Load("PlayerCharacter")) as GameObject;
		SelectedMember = playerObj.GetComponent<HumanCharacter>();
		//SelectedMember = GameObject.Find("PlayerCharacter").GetComponent<HumanCharacter>();

		SelectedMember.Initialize();
		//SelectedMember.MyNavAgent.enabled = true;
		RefreshMarkerForMember(SelectedMember);

		/*
		HumanCharacter member2 = GameObject.Find("HumanCharacter3").GetComponent<HumanCharacter>();
		member2.Initialize();
		RefreshMarkerForMember(member2);


		//for testing
		member2.MyAI.BlackBoard.FollowTarget = SelectedMember;
		*/

		Members.Add(SelectedMember);
		//Members.Add(member2);



		MemberGuardGoal = GameManager.Inst.DBManager.DBHandlerAI.GetGoalByID(6);
		MemberGuardGoal.Priority = 0;
		MemberFollowGoal = GameManager.Inst.DBManager.DBHandlerAI.GetGoalByID(8);
		MemberFollowGoal.Priority = 0;

		//initialize inventory and then load weapons to prefab
		GameManager.Inst.ItemManager.LoadPartyInventory();
		RefreshAllMemberWeight();

		RefreshAllMemberArmors();
		RefreshAllMemberWeapons();
	}

	public void Pause()
	{
		GameManager.Inst.CameraController.SetCameraMode(CameraModeEnum.Party);
		_lastAssignedMember = null;
		GameManager.Inst.CursorManager.SetCursorState(CursorState.Default);

		InputEventHandler.OnIssueTaskRMB -= GameManager.Inst.PlayerControl.OnIssueTaskMouseDown;
		InputEventHandler.OnIssueTaskRMB += GameManager.Inst.PlayerControl.OnIssueTaskMouseDown;
		InputEventHandler.OnIssueTaskLMB -= GameManager.Inst.PlayerControl.OnIssueTaskMouseDown;


		SelectedMemberTask = PartyTasks.Default;

		InputEventHandler.Instance.TriggerOnGamePause();
	}

	public void Unpause()
	{
		if(SelectedMember.MyAI.ControlType == AIControlType.Player && _lastAssignedMember != SelectedMember)
		{
			GameManager.Inst.CameraController.SetCameraMode(CameraModeEnum.Leader);
			ClearAIForMember(SelectedMember);
		}
		else
		{
			SelectedMember.MyAI.ControlType = AIControlType.PlayerTeam;
			GameManager.Inst.CameraController.SetCameraMode(CameraModeEnum.Party);
		}

		//throw grenade
		foreach(HumanCharacter member in Members)
		{
			if(member.MyAI.BlackBoard.IsGrenadePending)
			{
				member.AimPoint = member.MyAI.BlackBoard.PendingGrenadeTarget;
				member.SendCommand(CharacterCommands.StopAim);
				member.SendCommand(CharacterCommands.ThrowGrenade);
				member.MyAI.BlackBoard.IsGrenadePending = false;
			}
		}

		//clear up loose ends
		SelectedMember.MyAI.BlackBoard.GuardConfigStage = 0;
		SelectedMemberTask = PartyTasks.Default;


		InputEventHandler.Instance.TriggerOnGameUnpause();
	}

	public void SetActiveMember(int index)
	{
		SelectedMember.MyAI.ControlType = AIControlType.PlayerTeam;
		RefreshMarkerForMember(SelectedMember);
		HumanCharacter prev = SelectedMember;

		SelectedMember = Members[index];
		SelectedMember.MyAI.ControlType = AIControlType.Player;
		RefreshMarkerForMember(SelectedMember);
		if(SelectedMember.CurrentStance != HumanStances.Crouch)
		{
			SelectedMember.CurrentStance = HumanStances.Run;
		}

		if(!_playerControl.IsGamePaused)
		{
			GameManager.Inst.CameraController.SetCameraMode(CameraModeEnum.Leader);
			ClearAIForMember(SelectedMember);
		}

		InputEventHandler.Instance.TriggerOnSelectActiveMember(prev);
	}

	public void SetActiveMember(HumanCharacter character)
	{
		SelectedMember.MyAI.ControlType = AIControlType.PlayerTeam;
		RefreshMarkerForMember(SelectedMember);
		HumanCharacter prev = SelectedMember;

		SelectedMember = character;
		SelectedMember.MyAI.ControlType = AIControlType.Player;
		RefreshMarkerForMember(SelectedMember);
		if(SelectedMember.CurrentStance != HumanStances.Crouch)
		{
			SelectedMember.CurrentStance = HumanStances.Run;
		}

		if(!_playerControl.IsGamePaused)
		{
			GameManager.Inst.CameraController.SetCameraMode(CameraModeEnum.Leader);
			ClearAIForMember(SelectedMember);
		}

		InputEventHandler.Instance.TriggerOnSelectActiveMember(prev);
	}

	public void ClearActiveMember()
	{
		SelectedMember.MyAI.ControlType = AIControlType.PlayerTeam;
		RefreshMarkerForMember(SelectedMember);

		InputEventHandler.Instance.TriggerOnSelectActiveMember(SelectedMember);
	}

	public void RefreshAllMemberWeapons()
	{
		foreach(HumanCharacter character in Members)
		{
			character.MyAI.WeaponSystem.LoadWeaponsFromInventory(true);
		}
	}

	public void RefreshAllMemberArmors()
	{
		foreach(HumanCharacter character in Members)
		{
			character.ArmorSystem.SwitchToArmor(character.Inventory.ArmorSlot);
			character.ArmorSystem.SwitchToHelmet(character.Inventory.HeadSlot);
		}
	}

	public void RefreshAllMemberWeight()
	{
		foreach(HumanCharacter character in Members)
		{
			float weight = 0;

			if(character.Inventory.RifleSlot != null)
			{
				weight += character.Inventory.RifleSlot.Weight;
			}

			if(character.Inventory.SideArmSlot != null)
			{
				weight += character.Inventory.SideArmSlot.Weight;
			}

			if(character.Inventory.HeadSlot != null)
			{
				weight += character.Inventory.HeadSlot.Weight;
			}

			if(character.Inventory.ArmorSlot != null)
			{
				weight += character.Inventory.ArmorSlot.Weight;
			}

			if(character.Inventory.ToolSlot != null)
			{
				weight += character.Inventory.ToolSlot.Weight;
			}

			if(character.Inventory.ThrowSlot != null)
			{
				weight += character.Inventory.ThrowSlot.Weight;
			}

			foreach(GridItemData item in character.Inventory.Backpack)
			{
				weight += item.Item.Weight * item.Quantity;
			}

			character.MyStatus.CarryWeight = weight;
		}
	}

	public void SetGuardTaskForSelectedMember(int guardLevel, Vector3 guardDirection, Vector3 guardPos, float range)
	{
		SelectedMember.MyAI.BlackBoard.GuardLevel = guardLevel;
		SelectedMember.MyAI.BlackBoard.GuardDirection = guardDirection;
		SelectedMember.MyAI.BlackBoard.PatrolLoc = guardPos;
		SelectedMember.MyAI.BlackBoard.PatrolRange = new Vector3(range, 5, range);
		SelectedMember.MyAI.BlackBoard.HasPatrolInfo = true;
		SelectedMember.MyAI.SetDynamicyGoal(MemberGuardGoal, 0);
		_lastAssignedMember = SelectedMember;
	}

	public void SetFollowTaskForSelectedMember(HumanCharacter target)
	{
		SelectedMember.MyAI.BlackBoard.FollowTarget = target;
		SelectedMember.MyAI.SetDynamicyGoal(MemberFollowGoal, 0);
		RefreshMarkerForMember(SelectedMember);
		_lastAssignedMember = SelectedMember;
	}

	public void SetAttackTaskForSelectedMember(Character target)
	{
		SelectedMember.MyAI.BlackBoard.TargetEnemy = target;
		SelectedMember.MyAI.BlackBoard.IsTargetLocked = true;
		WorkingMemoryFact fact = SelectedMember.MyAI.WorkingMemory.FindExistingFact(FactType.KnownEnemy, target);
		if(fact == null)
		{
			//didn't find it in working memory, create a new fact
			fact = SelectedMember.MyAI.WorkingMemory.AddFact(FactType.KnownEnemy, target, target.transform.position, 0.99f, 0.01f);
			fact.ThreatLevel = 1f;
			fact.ThreatDropRate = 0.03f;
		}
		else
		{
			//found it in working memory, refresh confidence level
			fact.Confidence = 0.99f;
			fact.LastKnownPos = target.transform.position;
		}

		//SelectedMember.MyAI.ClearDynamicGoal(0);
		_lastAssignedMember = SelectedMember;
	}

	public void ClearTaskForSelectedMember()
	{
		SelectedMember.MyAI.ClearDynamicGoal(0);
		RefreshMarkerForMember(SelectedMember);
		SelectedMember.MyAI.BlackBoard.IsTargetLocked = false;
		SelectedMember.MyAI.BlackBoard.IsGrenadePending = false;
	}

	public void RefreshMarkerForAll()
	{
		foreach(HumanCharacter member in Members)
		{
			RefreshMarkerForMember(member);
		}
	}

	public void RefreshMarkerForMember(HumanCharacter member)
	{
		/*
		if(member.Markers.CircleFriendly == null)
		{
			GameObject circle = GameObject.Instantiate(Resources.Load("CharCircleFriendly")) as GameObject;
			member.Markers.CircleFriendly = circle.GetComponent<SpriteRenderer>();
			circle.transform.parent = member.transform;
			circle.transform.localPosition = new Vector3(0, 0.01f, 0);
			circle.transform.localEulerAngles = new Vector3(-90, 0, 0);
			member.Markers.CircleFriendly.enabled = false;
		}

		if(member.Markers.CircleSelected == null)
		{
			GameObject circle = GameObject.Instantiate(Resources.Load("CharCircleSelected")) as GameObject;
			member.Markers.CircleSelected = circle.GetComponent<SpriteRenderer>();
			circle.transform.parent = member.transform;
			circle.transform.localPosition = new Vector3(0, 0.01f, 0);
			circle.transform.localEulerAngles = new Vector3(-90, 0, 0);
			member.Markers.CircleSelected.enabled = false;
		}

		if(member.Markers.TaskMarker == null)
		{
			GameObject marker = GameObject.Instantiate(Resources.Load("GuardTaskMarker")) as GameObject;
			member.Markers.TaskMarker = marker.GetComponent<SpriteRenderer>();
			marker.transform.position = new Vector3(0, 0.01f, 0);
			marker.transform.eulerAngles = new Vector3(-90, 0, 0);
			member.Markers.TaskMarker.enabled = false;
		}

		if(member.Markers.CircleEnemyTarget == null)
		{
			GameObject marker = GameObject.Instantiate(Resources.Load("CharCircleEnemy")) as GameObject;
			member.Markers.CircleEnemyTarget = marker.GetComponent<SpriteRenderer>();
			marker.transform.position = new Vector3(0, 0.01f, 0);
			marker.transform.eulerAngles = new Vector3(-90, 0, 0);
			member.Markers.TaskMarker.enabled = false;
		}

		if(member.Markers.TeamFollowArrow == null)
		{
			GameObject marker = GameObject.Instantiate(Resources.Load("TeamFollowArrow")) as GameObject;
			member.Markers.TeamFollowArrow = marker.GetComponent<SpriteRenderer>();
			marker.transform.position = new Vector3(0, 0.01f, 0);
			marker.transform.eulerAngles = new Vector3(90, 0, 0);
			member.Markers.TeamFollowArrow.enabled = false;
		}

		if(member.MyAI.ControlType == AIControlType.Player)
		{
			member.Markers.CircleFriendly.enabled = false;
			member.Markers.CircleSelected.enabled = true;
			member.Markers.CircleEnemyTarget.enabled = true;

			if(_playerControl.IsGamePaused)
			{
				GoapGoal topGoal = member.MyAI.GetDynamicGoal(0);
				if(topGoal == null)
				{
					member.Markers.TaskMarker.enabled = false;
					member.Markers.TeamFollowArrow.enabled = false;
				}
				else
				{
					if(topGoal.Name == "Defend Position")
					{
						if(member.MyAI.BlackBoard.GuardConfigStage >= 1)
						{
							member.Markers.TaskMarker.enabled = true;
						}
						else
						{
							member.Markers.TaskMarker.enabled = false;
						}
						member.Markers.TeamFollowArrow.enabled = false;
					}
					else if(topGoal.Name == "Follow target")
					{
						member.Markers.TaskMarker.enabled = false;
						member.Markers.TeamFollowArrow.enabled = true;
					}
					else
					{
						member.Markers.TaskMarker.enabled = false;
						member.Markers.TeamFollowArrow.enabled = false;
					}
				}
			}
			else
			{
				member.Markers.TaskMarker.enabled = false;
				member.Markers.TeamFollowArrow.enabled = false;
			}
		}
		else if(member.MyAI.ControlType == AIControlType.PlayerTeam)
		{
			member.Markers.CircleFriendly.enabled = true;
			member.Markers.CircleSelected.enabled = false;
			member.Markers.TaskMarker.enabled = false;
			member.Markers.CircleEnemyTarget.enabled = false;
			member.Markers.TeamFollowArrow.enabled = false;
		}
		*/

	}






	private void ClearAIForMember(HumanCharacter member)
	{
		member.MyAI.ForceStopCurrentAction();
		member.MyAI.ClearDynamicGoal(0);
		member.SendCommand(CharacterCommands.ReleaseTrigger);

		if(member.UpperBodyState == HumanUpperBodyStates.Aim && Input.GetKey(KeyCode.Mouse1) 
			&& member == SelectedMember && member.MyAI.ControlType == AIControlType.Player)
		{
			member.SendCommand(CharacterCommands.Aim);
		}
		else
		{
			member.SendCommand(CharacterCommands.StopAim);
		}
		member.SendCommand(CharacterCommands.Idle);
	}
}
