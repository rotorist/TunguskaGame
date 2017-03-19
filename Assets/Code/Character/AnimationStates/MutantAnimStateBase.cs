using UnityEngine;
using System.Collections;

public abstract class MutantAnimStateBase
{
	public MutantCharacter ParentCharacter;

	public abstract void SendCommand(CharacterCommands command);
	public abstract void Update();
	public abstract bool IsRotatingBody();
}
