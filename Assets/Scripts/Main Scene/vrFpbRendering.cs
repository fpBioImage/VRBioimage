using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vrFpbRendering : MonoBehaviour {

	public GameObject volumetricCube;

	// Keyboard input
	private float opacitySpeed = 0.8f;
	private float thresholdSpeed = 0.07f;
	private float intensitySpeed = 0.2f;

	// Values
	private float opacity = 4.0f;
	private float threshold = 0.2f;
	private float intensity = 1.0f;
	private int renderingMode = 1;

	// Shader IDs
	private int _opacityID;
	private int _thresholdID;
	private int _intensityID;
	private int _renderID;

	private Material _rayMarchMaterial;

	// Use this for initialization
	void Start () {
		//cardboardMain.GetComponent<Cardboard> ().VRModeEnabled = true;
		_rayMarchMaterial = volumetricCube.GetComponent<Renderer> ().material;
		StartCoroutine(switchToVR());

		// Get Property IDs
		_opacityID = Shader.PropertyToID("_Opacity");
		_intensityID = Shader.PropertyToID ("_Intensity");
		_thresholdID = Shader.PropertyToID ("_DataMin");
		_renderID = Shader.PropertyToID ("_RenderMode");

		opacity = (variables.fpbJSON.opacity != -1.0f) ? variables.fpbJSON.opacity : 5.0f;
		intensity = (variables.fpbJSON.intensity != -1.0f) ? variables.fpbJSON.intensity : 1.0f;
		threshold = (variables.fpbJSON.threshold != -1.0f) ? variables.fpbJSON.threshold : 0.2f;
		renderingMode = (variables.fpbJSON.projection != -1) ? variables.fpbJSON.projection : 1;
	}

	// Update is called once per frame
	void Update () {
		opacity += Input.GetAxis("OpacityAxis") * opacitySpeed * Time.deltaTime * opacity;
		threshold += Input.GetAxis ("ThresholdAxis") * thresholdSpeed * Time.deltaTime;
		intensity += Input.GetAxis ("IntensityAxis") * intensitySpeed * Time.deltaTime * intensity;

		opacity = clamp (opacity, 0.0f, 1.6f);
		threshold = clamp (threshold, 0.0f, 1.0f);
		intensity = clamp (intensity, 0.0f, 8.0f);
	}

	void LateUpdate(){
		_rayMarchMaterial.SetFloat (_opacityID, opacity*5.0f); // Blending strength 
		_rayMarchMaterial.SetFloat (_thresholdID, threshold); // alpha cutoff value
		_rayMarchMaterial.SetFloat (_intensityID, intensity*intensity); // blends image a bit better
		_rayMarchMaterial.SetFloat (_renderID, (float)renderingMode);
	}

	IEnumerator switchToVR() {
		UnityEngine.VR.VRSettings.LoadDeviceByName("cardboard");
		// Must wait one frame after calling `XRSettings.LoadDeviceByName()`.
		yield return null;
		// Now it's ok to enable VR mode.
		UnityEngine.VR.VRSettings.enabled = true;
	}

	public float clamp(float input, float limLow, float limHigh){
		if (input > limHigh)
			input = limHigh;
		if (input < limLow)
			input = limLow;
		return input;
	}
}
