using UnityEngine;
using System.Collections;

public class BuildingComponent : MonoBehaviour 
{

	public EnterableBuilding Building;
	public float YMin;//when between Ymin and Ymax, the component should be revealed
	public float YMax;
	public float YThreshold;
	public float YThresholdNPC;
	public bool IsHidden;
	public int Level;

}
