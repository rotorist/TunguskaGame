using UnityEngine;
using System.Collections;

public class DummyAnimation : MonoBehaviour {

	public string aimAnimation = "AimMP40StandingFront";
	public Transform spine;

	void Start() {
		GetComponent<Animation>()[aimAnimation].layer = 1;
		GetComponent<Animation>()[aimAnimation].AddMixingTransform(spine, true);
		GetComponent<Animation>().Play(aimAnimation);
	}
}
