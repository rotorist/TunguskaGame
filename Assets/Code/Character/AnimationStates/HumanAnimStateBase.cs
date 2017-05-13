using UnityEngine;
using System.Collections;

public abstract class HumanAnimStateBase
{
	public HumanCharacter ParentCharacter;

	public abstract void SendCommand(CharacterCommands command);
	public abstract void Update();
	public abstract void FixedUpdate();
	public abstract bool IsRotatingBody();


}
