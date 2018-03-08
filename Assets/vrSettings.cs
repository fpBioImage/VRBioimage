using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VR;

public class vrSettings : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (SceneManager.GetActiveScene ().buildIndex == 0 || !variables.vr) {
			StartCoroutine(SwitchTo2D ());
		} else {
			StartCoroutine(SwitchToVR ());
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp (KeyCode.Escape)) {
			SceneManager.LoadScene (0);
		}
	}

	IEnumerator SwitchToVR() {
		print ("Switching to VR");
		// Device names are lowercase, as returned by `XRSettings.supportedDevices`.
		string desiredDevice = "cardboard"; // Or "daydream".
		VRSettings.LoadDeviceByName(desiredDevice);

		// Must wait one frame after calling `XRSettings.LoadDeviceByName()`.
		yield return null;

		// Now it's ok to enable VR mode.
		VRSettings.enabled = true;
	}

	// Call via `StartCoroutine(SwitchTo2D())` from your code. Or, use
	// `yield SwitchTo2D()` if calling from inside another coroutine.
	IEnumerator SwitchTo2D() {
		print ("Switching to 2D");
		// Empty string loads the "None" device.
		VRSettings.LoadDeviceByName("");

		// Must wait one frame after calling `XRSettings.LoadDeviceByName()`.
		yield return null;

		// Not needed, since loading the None (`""`) device takes care of this.
		// XRSettings.enabled = false;

		// Restore 2D camera settings.
		//ResetCameras();
	}

	// Resets camera transform and settings on all enabled eye cameras.
	void ResetCameras() {
		// Camera looping logic copied from GvrEditorEmulator.cs
		for (int i = 0; i < Camera.allCameras.Length; i++) {
			Camera cam = Camera.allCameras[i];
			if (cam.enabled && cam.stereoTargetEye != StereoTargetEyeMask.None) {

				// Reset local position.
				// Only required if you change the camera's local position while in 2D mode.
				cam.transform.localPosition = Vector3.zero;

				// Reset local rotation.
				// Only required if you change the camera's local rotation while in 2D mode.
				cam.transform.localRotation = Quaternion.identity;

			}
		}
	}

}
