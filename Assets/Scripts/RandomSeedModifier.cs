//Sets random seed to 0 for purpose of testing
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomSeedModifier : MonoBehaviour {

	public void SetRandomSeedToZero() {
		Random.InitState (0);
	}

}
