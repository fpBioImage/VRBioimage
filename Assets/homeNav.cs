using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class homeNav : MonoBehaviour {

	// Use this for initialization
	void Start () {
		changeModelDropdown (0);
		changeQualityDropdown (0);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//public void onDeepLink

	public void loadMainScene(int sceneNumber){
		UnityEngine.SceneManagement.SceneManager.LoadScene (sceneNumber);
	}

	public void changeModelDropdown(int model){
		variables.fpbJSON = new FpbJSON (model);
	}

	public void changeQualityDropdown(int quality){
		variables.fpbQuality = quality;
	}

}
