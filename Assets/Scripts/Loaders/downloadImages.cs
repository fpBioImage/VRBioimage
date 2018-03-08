using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

// OFFLINE VERSION OF THIS FILE FOR ANDROID

public class downloadImages : MonoBehaviour {

	public bool changingQuality = false;

	public GameObject infoBox;
	public GameObject infoTextObject;
	public GameObject qualityButton;
	public GameObject fullScreenQuad;

	public bool offlineMode = false;
	public bool atlasMode = true;
	public string pathToImages = "";
	public string imagePrefix = "";
	public string numberingFormat = "0000";

	public int imageWidth = 1;
	public int imageHeight = 1;
	public int imageDepth = 1;
	public Vector3 voxelSize = new Vector3(1,1,1);

	private int sizeLimit = 500;
	private float numAtlases = 8.0f;

	public GameObject cube;
	private Material rayMarchMaterial;

	private bool vr;

	void Awake() {
		//offlineMode = true;
		setVariables ();
		vr = UnityEngine.SceneManagement.SceneManager.GetActiveScene ().buildIndex == 2;
	}

	void Start () {
		qualityButton.SetActive (false);
		rayMarchMaterial = cube.GetComponent<Renderer> ().material;

		if (!atlasMode) {
			print ("Loading by image slices");
			if (vr)	infoTextObject.GetComponent<TextMesh> ().text = ("Downloading image\nslices..."); 
			else infoTextObject.GetComponent<Text>().text = "Downloading image\nslices...";
			StartCoroutine (loadBySlices ());
		} else {
			print ("Loading atlases directly");
			if (vr)	infoTextObject.GetComponent<TextMesh> ().text = "Downloading\natlases..."; 
			else infoTextObject.GetComponent<Text>().text = "Downloading\natlases...";
			for (int i = 0; i < numAtlases; i++) {
				StartCoroutine (loadByAtlas (i));
			}
		}
	}


	IEnumerator loadBySlices(){
		bool pngMode = true;

		// First, calculate image size from first texture
		Texture2D texture0 = new Texture2D (4, 4);
		int ii = 0;
		string loadImage0 = pathToImages + imagePrefix + ii.ToString (numberingFormat);
		if (loadImage0.Substring (0, 4) == "http") {
			loadImage0 += ".png";
			WWW ww0 = new WWW (loadImage0);
			yield return ww0;

			if (!string.IsNullOrEmpty(ww0.error)){
				pngMode = false;
				loadImage0 = pathToImages + imagePrefix + ii.ToString (numberingFormat) + ".jpg";
				ww0 = new WWW (loadImage0);
				yield return ww0;
			}

			ww0.LoadImageIntoTexture (texture0);

		} else {
			offlineMode = true;
			texture0 = Resources.Load (loadImage0) as Texture2D;
		}

		int texWidth = texture0.width;
		int texHeight = texture0.height;

		// Make sure we're within size limits
		imageWidth = texWidth > sizeLimit ? sizeLimit : texWidth;
		imageHeight = texHeight > sizeLimit ? sizeLimit : texHeight;

		// Calcualte atlas size
		float paddedImageWidth = (float)ceil2 ((uint)imageWidth);
		float paddedImageHeight = (float)ceil2 ((uint)imageHeight);
		float slicesPerAtlas = Mathf.Ceil (imageDepth / numAtlases);

		int atlasWidth = (int) ceil2 ((uint)paddedImageWidth);
		int atlasHeight = (int) ceil2 ((uint)(paddedImageHeight * slicesPerAtlas));

		while ((atlasHeight > 2*atlasWidth) && (atlasHeight > paddedImageHeight)){
			atlasHeight /= 2;
			atlasWidth *= 2;
		}

		// Create array of atlas textures
		Color32 black = new Color32 (0, 0, 0, 255);

		Texture2D[] atlasArray = new Texture2D[(int)numAtlases];
		for (int i = 0; i < (int)numAtlases; i++) {
			atlasArray[i] = new Texture2D (atlasWidth, atlasHeight, TextureFormat.ARGB32, false);
		}

		// Set all pixels in the atlases to be clear
		Color32[] bigClearArray = atlasArray[0].GetPixels32 ();
		for (int i = 0; i < bigClearArray.Length; i++)
			bigClearArray [i] = black;

		for (int i = 0; i < (int)numAtlases; i++) {
			atlasArray[i].SetPixels32(bigClearArray);
		}


		// Set up some variables for atlas filling
		int xOffset = Mathf.FloorToInt(((float)paddedImageWidth-(float)texWidth)/2.0f);
		int yOffset = Mathf.FloorToInt(((float)paddedImageHeight-(float)texHeight)/2.0f);

		float slicesPerRow = Mathf.Floor((float)atlasWidth / (float)paddedImageWidth);

		// This loop does the actual downloading and filling of the atlases
		for (int i = 0; i < imageDepth; i++) {

			int atlasNumber = (int) (((float)i) % numAtlases);
			float locationIndex = Mathf.Floor(((float)i)/numAtlases);

			Texture2D downloadedImage = new Texture2D (imageWidth, imageHeight);
			if(vr) infoTextObject.GetComponent<TextMesh>().text = "Loading slice " + (i+1).ToString () + " of " + imageDepth + ".";
			else infoTextObject.GetComponent<Text>().text = "Loading slice " + (i+1).ToString () + " of " + imageDepth + ".";

			string imageToLoad = pathToImages + imagePrefix + i.ToString (numberingFormat);
			if (!offlineMode) {
				if (pngMode)
					imageToLoad += ".png";
				else
					imageToLoad += ".jpg";
				WWW www = new WWW (imageToLoad);
				yield return www;
				www.LoadImageIntoTexture (downloadedImage);
				//yield return new WaitForSeconds (0.025f);
			} else {
				//byte[] fileData = File.ReadAllBytes (imageToLoad);
				//downloadedImage.LoadImage (fileData);
				downloadedImage = Resources.Load(imageToLoad) as Texture2D;
				yield return null;
			}

			if (texWidth > sizeLimit || texHeight > sizeLimit) {
				TextureScaler.scale (downloadedImage, imageWidth, imageHeight);
			}

			float xCoord = (locationIndex % slicesPerRow) * paddedImageWidth;
			xCoord += xOffset;
			float yCoord = Mathf.Floor(locationIndex / slicesPerRow) * paddedImageHeight;
			yCoord += yOffset;

			atlasArray[atlasNumber].SetPixels ((int)xCoord, (int)yCoord, texWidth, texHeight, downloadedImage.GetPixels (0, 0, texWidth, texHeight));
		}

		// Apply pixels and send to GPU
		if(vr) infoTextObject.GetComponent<TextMesh>().text = "Preparing volumetric renderer...";
		else infoTextObject.GetComponent<Text>().text = "Preparing volumetric renderer...";
		for (int i = 0; i < (int)numAtlases; i++) {
			atlasArray [i].Apply ();
			rayMarchMaterial.SetTexture ("_Atlas" + i, atlasArray [i]);
		}

		// Now set the material and rendering properties
		rayMarchMaterial.SetFloat("_atlasWidth", atlasWidth);
		rayMarchMaterial.SetFloat ("_atlasHeight", atlasHeight);
		rayMarchMaterial.SetFloat ("_imageDepth", imageDepth);
		rayMarchMaterial.SetFloat ("_imageWidth", paddedImageWidth);
		rayMarchMaterial.SetFloat ("_imageHeight", paddedImageHeight);
		rayMarchMaterial.SetFloat ("_slicesPerAtlas", slicesPerAtlas);
		rayMarchMaterial.SetFloat ("_slicesPerRow", slicesPerRow);

		Vector3 cubeSize = new Vector3 (imageWidth * voxelSize.x, imageHeight * voxelSize.y, imageDepth * voxelSize.z).normalized;
		cubeSize *= 3.5f * Mathf.Min (1.0f / cubeSize.x, Mathf.Min (1.0f / cubeSize.y, 1.0f / cubeSize.z));
		cube.transform.localScale = cubeSize;

		// Load the scene
		if(vr) infoTextObject.GetComponent<TextMesh>().text = "Click to start";
		else infoTextObject.GetComponent<Text>().text = "Click to start";
		variables.freezeAll = false;
		cube.SetActive (true);
		qualityButton.SetActive (true);
		variables.triggerRender = true;
		variables.volumeReadyState = 1;
	}

	// LOAD BY ATLAS
	private int atlasesLoaded = 0;
	private float[] downloadProgress = new float[8];
	IEnumerator loadByAtlas(int atlasNumber){
		float atlasWidth = 1; 

		//infoText.text = "Downloading texture map " + (atlasNumber+1) + " of " + (int)numAtlases + ".";

		string atlasToLoad = pathToImages + imagePrefix + atlasNumber.ToString (numberingFormat);
		Texture2D atlasSlice = new Texture2D (4, 4, TextureFormat.ARGB32, false);

		if (atlasToLoad.Substring (0, 4) == "http") {
			atlasToLoad += ".png";
			WWW www = new WWW (atlasToLoad);

			while (!www.isDone) {
				downloadProgress [atlasNumber] = www.progress;
				if (vr) infoTextObject.GetComponent<TextMesh> ().text = "Downloaded " + (sum (downloadProgress) / numAtlases).ToString ("P1");
				else infoTextObject.GetComponent<Text> ().text = "Downloaded " + (sum (downloadProgress) / numAtlases).ToString ("P1"); 
				yield return null;
			}
			yield return www;

			// Load image into atlas
			www.LoadImageIntoTexture (atlasSlice);
		} else {
			//byte[] fileData = File.ReadAllBytes (atlasToLoad);
			//atlasSlice.LoadImage (fileData);
			atlasSlice = Resources.Load(atlasToLoad) as Texture2D;
		}

		// Set atlas to material
		rayMarchMaterial.SetTexture("_Atlas"+atlasNumber, atlasSlice);


		atlasesLoaded++;
		// Calculate a few more useful variables
		if (atlasesLoaded == (int)numAtlases) {
			atlasWidth = atlasSlice.width;
			rayMarchMaterial.SetFloat("_atlasWidth", atlasWidth);
			rayMarchMaterial.SetFloat ("_atlasHeight", atlasSlice.height);

			float paddedImageWidth = (float)ceil2 ((uint)imageWidth);
			float paddedImageHeight = (float)ceil2 ((uint)imageHeight);

			float slicesPerRow = Mathf.Floor (atlasWidth / paddedImageWidth);
			float slicesPerAtlas = Mathf.Ceil (imageDepth / numAtlases);

			// Set other material properties
			rayMarchMaterial.SetFloat ("_imageDepth", imageDepth);
			rayMarchMaterial.SetFloat ("_imageWidth", paddedImageWidth);
			rayMarchMaterial.SetFloat ("_imageHeight", paddedImageHeight);
			rayMarchMaterial.SetFloat ("_slicesPerAtlas", slicesPerAtlas);
			rayMarchMaterial.SetFloat ("_slicesPerRow", slicesPerRow);


			Vector3 cubeSize = new Vector3 (imageWidth * voxelSize.x, imageHeight * voxelSize.y, imageDepth * voxelSize.z).normalized;
			cubeSize *= 3.5f * Mathf.Min (1.0f / cubeSize.x, Mathf.Min (1.0f / cubeSize.y, 1.0f / cubeSize.z));

			cube.transform.localScale = cubeSize;

			// Load the scene
			if (vr)	infoTextObject.GetComponent<TextMesh> ().text = "Click to start";
			else infoTextObject.GetComponent<Text> ().text = "Click to start";
			variables.freezeAll = false;
			cube.SetActive (true);
			qualityButton.SetActive (true);
			variables.triggerRender = true;
			variables.volumeReadyState = 1;
		}
	}

	void Update(){
		if (variables.volumeReadyState == 1){
			if (Input.anyKeyDown) {
				if (changingQuality && Input.GetKey (KeyCode.Mouse0)) {
					infoBox.SetActive (false);
					// Then open the quality changer
					GameObject.Find("Arrow Left").GetComponent<leftArrowControl>().arrowClicked();
				} else {
					infoBox.SetActive (false);
				}
				// Off you go! 
				variables.volumeReadyState = 2;
			}
		}
	}

	public void setChangingQuality(bool mouseIn){
		changingQuality = mouseIn;
	}

	// Set variables for online mode
	private void setVariables(){
		//Application.ExternalEval ("fpcanvas.SendMessage('Main Camera', 'parseFpbJSON', JSON.stringify(fpb));");

		atlasMode = variables.fpbJSON.getAtlasMode();
		pathToImages = variables.fpbJSON.pathToImages;
		if (pathToImages.Substring (pathToImages.Length - 1) != "/") {
			pathToImages = pathToImages + "/";
		}


		imagePrefix = variables.fpbJSON.imagePrefix;
		numberingFormat = variables.fpbJSON.numberingFormat;

		imageWidth = variables.fpbJSON.sliceWidth;
		imageHeight = variables.fpbJSON.sliceHeight;
		imageDepth = variables.fpbJSON.numberOfImages;
		voxelSize = variables.fpbJSON.voxelSize; // this one might not work so well... 
	}

	public void parseFpbJSON(string jsonString){
		variables.fpbJSON = JsonUtility.FromJson<FpbJSON> (jsonString);
	}

	//  Check if there is a bookmark in the URL
	public void setLoadFromBookmark(string boolean){
		variables.loadBookmarkFromURL = (boolean=="true") ? true : false;
	}

	// Some efficient power-of-two rounders:
	private uint ceil2(uint x) {
		x--;
		x |= (x >> 1);
		x |= (x >> 2);
		x |= (x >> 4);
		x |= (x >> 8);
		x |= (x >> 16);
		x |= (x >> 32);
		x |= (x >> 64);
		x |= (x >> 128);
		x |= (x >> 256);
		x |= (x >> 512);
		x |= (x >> 1024);
		x |= (x >> 2048);
		x |= (x >> 4096);
		x |= (x >> 8192);
		x |= (x >> 16384);
		x |= (x >> 32768);
		x |= (x >> 65536);
		x |= (x >> 131072);
		x |= (x >> 262144);
		return (x+1);
	}

	// Just a useful sum helper-function 
	private float sum(float[] arrayToSum){
		float output = 0.0f;
		for (int i = 0; i < arrayToSum.Length; i++) {
			output += arrayToSum [i];
		}
		return output;
	}

}
