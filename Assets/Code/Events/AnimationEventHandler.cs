using UnityEngine;
using System.Collections;

public class AnimationEventHandler : MonoBehaviour 
{
	
	public delegate void AnimationEventDelegate();
	public event AnimationEventDelegate OnThrowLeaveHand;
	public event AnimationEventDelegate OnThrowFinish;
	public event AnimationEventDelegate OnPistolPullOutFinish;
	public event AnimationEventDelegate OnPistolPutAwayFinish;
	public event AnimationEventDelegate OnLongGunPullOutFinish;
	public event AnimationEventDelegate OnLongGunPutAwayFinish;
	public event AnimationEventDelegate OnGrenadePullOutFinish;
	public event AnimationEventDelegate OnMeleePullOutFinish;
	public event AnimationEventDelegate OnMeleePutAwayFinish;
	public event AnimationEventDelegate OnReloadFinish;
	public event AnimationEventDelegate OnStartStrangle;
	public event AnimationEventDelegate OnEndStrangle;
	public event AnimationEventDelegate OnDeath;
	public event AnimationEventDelegate OnStrangledDeath;
	public event AnimationEventDelegate OnHitReover;
	public event AnimationEventDelegate OnSwitchWeapon;
	public event AnimationEventDelegate OnFinishTakeObject;
	public event AnimationEventDelegate OnFinishInteract;
	public event AnimationEventDelegate OnMeleeStrikeHalfWay;
	public event AnimationEventDelegate OnMeleeComboStageOne;
	public event AnimationEventDelegate OnMeleeComboStageTwo;
	public event AnimationEventDelegate OnMeleeStrikeLeftFinish;
	public event AnimationEventDelegate OnMeleeStrikeRightFinish;
	public event AnimationEventDelegate OnMeleeBlockFinish;
	public event AnimationEventDelegate OnMeleeBlocked;
	public event AnimationEventDelegate OnAnimationActionEnd;
	public event AnimationEventDelegate OnFootStep;

	//Animation events
	public void TriggerOnThrowLeaveHand()
	{
		if(OnThrowLeaveHand != null)
		{
			OnThrowLeaveHand();
		}
	}

	public void TriggerOnThrowFinish()
	{
		if(OnThrowFinish != null)
		{
			OnThrowFinish();
		}
	}

	public void TriggerOnLongGunPullOutFinish()
	{
		if(OnLongGunPullOutFinish != null)
		{
			OnLongGunPullOutFinish();
		}
	}

	public void TriggerOnLongGunPutAwayFinish()
	{
		if(OnLongGunPutAwayFinish != null)
		{
			OnLongGunPutAwayFinish();
		}
	}

	public void TriggerOnPistolPullOutFinish()
	{
		if(OnPistolPullOutFinish != null)
		{
			OnPistolPullOutFinish();
		}
	}

	public void TriggerOnPistolPutAwayFinish()
	{
		if(OnPistolPutAwayFinish != null)
		{
			OnPistolPutAwayFinish();
		}
	}

	public void TriggerOnGrenadePullOutFinish()
	{
		if(OnGrenadePullOutFinish != null)
		{
			OnGrenadePullOutFinish();
		}
	}

	public void TriggerOnMeleePullOutFinish()
	{
		if(OnMeleePullOutFinish != null)
		{
			OnMeleePullOutFinish();
		}
	}

	public void TriggerOnMeleePutAwayFinish()
	{
		if(OnMeleePutAwayFinish != null)
		{
			OnMeleePutAwayFinish();
		}
	}

	public void TriggerOnReloadFinish()
	{
		if(OnReloadFinish != null)
		{
			OnReloadFinish();
		}
	}

	public void TriggerOnStartStrangle()
	{
		if(OnStartStrangle != null)
		{
			OnStartStrangle();
		}
	}

	public void TriggerOnEndStrangle()
	{
		if(OnEndStrangle != null)
		{
			OnEndStrangle();
		}
	}



	public void TriggerOnDeath()
	{
		if(OnDeath != null)
		{
			OnDeath();
		}
	}

	public void TriggerOnStrangledDeath()
	{
		if(OnStrangledDeath != null)
		{
			OnStrangledDeath();
		}
	}

	public void TriggerOnHitRecover()
	{
		if(OnHitReover != null)
		{
			OnHitReover();
		}
	}

	public void TriggerOnSwitchWeapon()
	{
		if(OnSwitchWeapon != null)
		{
			OnSwitchWeapon();
		}
	}

	public void TriggerOnFinishTakeObject()
	{
		if(OnFinishTakeObject != null)
		{
			OnFinishTakeObject();
		}
	}

	public void TriggerOnFinishInteract()
	{
		if(OnFinishInteract != null)
		{
			OnFinishInteract();
		}
	}

	public void TriggerOnStrikeHalfWay()
	{
		if(OnMeleeStrikeHalfWay != null)
		{
			OnMeleeStrikeHalfWay();
		}
	}

	public void TriggerOnComboStageOne()
	{
		if(OnMeleeComboStageOne != null)
		{
			OnMeleeComboStageOne();
		}
	}

	public void TriggerOnComboStageTwo()
	{
		if(OnMeleeComboStageTwo != null)
		{
			OnMeleeComboStageTwo();
		}
	}



	public void TriggerOnStrikeLeftFinish()
	{
		if(OnMeleeStrikeLeftFinish != null)
		{
			OnMeleeStrikeLeftFinish();
		}
	}

	public void TriggerOnStrikeRightFinish()
	{
		if(OnMeleeStrikeRightFinish != null)
		{
			OnMeleeStrikeRightFinish();
		}
	}

	public void TriggerOnBlockFinish()
	{
		if(OnMeleeBlockFinish != null)
		{
			OnMeleeBlockFinish();
		}
	}

	public void TriggerOnBlocked()
	{
		if(OnMeleeBlocked != null)
		{
			OnMeleeBlocked();
		}
	}

	public void TriggerOnAnimationEnd()
	{
		if(OnAnimationActionEnd != null)
		{
			OnAnimationActionEnd();
		}
	}

	public void TriggerOnFootStep()
	{
		if(OnFootStep != null)
		{
			OnFootStep();
		}
	}
}
