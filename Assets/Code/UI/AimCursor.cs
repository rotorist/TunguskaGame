using UnityEngine;
using System.Collections;

public class AimCursor : MonoBehaviour 
{
	public UISprite AimCursorTop;
	public UISprite AimCursorBottom;
	public UISprite AimCursorLeft;
	public UISprite AimCursorRight;
	public UISprite AimCursorCenter;

	public void SetExpansion(float amount)
	{
		Vector3 pos = AimCursorCenter.transform.localPosition;	
		AimCursorTop.transform.localPosition = new Vector3(pos.x, pos.y + amount, 0);
		AimCursorBottom.transform.localPosition = new Vector3(pos.x, pos.y - amount, 0);
		AimCursorLeft.transform.localPosition = new Vector3(pos.x - amount, pos.y, 0);
		AimCursorRight.transform.localPosition = new Vector3(pos.x + amount, pos.y, 0);

	}

	public void SetCenterCursorAlpha(float alpha)
	{
		AimCursorCenter.alpha = alpha;
	}
}
