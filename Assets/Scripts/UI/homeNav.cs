using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class homeNav : MonoBehaviour {

	// Use this for initialization
	void Start () {
		variables.vr = false;
		changeModelDropdown (0);
		changeQualityDropdown (1); // Maybe remember these...! 
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//public void onDeepLink

	public void loadMainScene(int sceneNumber){
		if (sceneNumber == 2) {
			variables.vr = true;
			sceneNumber = 1;
		}
		UnityEngine.SceneManagement.SceneManager.LoadScene (sceneNumber);
	}

	public void changeModelDropdown(int model){
		if (model == 2) {
			variables.rainbow = 1;
		} else {
			variables.rainbow = 0;
			variables.fpbJSON = new FpbJSON (model);
		}
	}

	public void changeQualityDropdown(int quality){
		variables.fpbQuality = quality;
	}

}
