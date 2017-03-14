using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

public class AimIKAxisController : MonoBehaviour {

	[Tooltip("Reference to the AimIK component.")]
	public AimIK aim;
	
	[Tooltip("The root Transform of the character.")]
	public Transform characterRoot;
	
	[Space(10)]
	
	[Tooltip("Direction (in character space) that the gun is aimed at in animation. If this is Vector3.forward, make sure the character is aiming exactly forward in it's animated aiming pose. If for instance the character is aiming left in the current animation clip, set it to (-1, 0, 0) instead.")] 
	public Vector3 animatedAimingDirection = Vector3.forward;
	
	[Tooltip("The weight of adjusting the axis of AimIK to maintain the animation of the gun.")]
	[Range(0f, 1f)] public float maintainAnimationWeight;
	
	private Vector3 originalAxis;
	
	void Start() {
		originalAxis = aim.solver.axis;
	}
	
	void LateUpdate() {
		Vector3 x = aim.solver.transform.InverseTransformDirection(characterRoot.rotation * animatedAimingDirection);
	
		aim.solver.axis = Vector3.Lerp(originalAxis, x, maintainAnimationWeight);
	}
}
