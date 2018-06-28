using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boxTexture : MonoBehaviour {

	public Texture grid;
	public Texture black;
	public Renderer[] rend;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		// Touch controls: double-tap hides binding box
		bool doubledTapped = false;
		if (Input.touchCount == 1) { 
			Touch touch0 = Input.GetTouch (0);
			if (touch0.tapCount == 2) {
				doubledTapped = true;
			}
		}

		if ((Input.GetKeyUp (KeyCode.H) || doubledTapped) && !variables.freezeAll) {
			variables.showBindingBox = !variables.showBindingBox;

			for (int r = 0; r < rend.Length; r++) {
				rend[r].material.mainTexture = (variables.showBindingBox) ? grid : black;
			}
		}

	}

}
