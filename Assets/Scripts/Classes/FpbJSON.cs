using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FpbJSON {
		public string uniqueName;
		public int numberOfImages;
		public int sliceWidth;
		public int sliceHeight;
		public string imagePrefix;
		public string numberingFormat;
		public string pathToImages;
		public Vector3 voxelSize;
		public float opacity = -1.0f;
		public float intensity = -1.0f;
		public float threshold = -1.0f;
		public int projection = -1;

		public string atlasMode; // note that bools are stored as strings...
		public string imageAlpha;

	public bool getAtlasMode(){
		return (atlasMode == "true" || atlasMode == "1") ? true : false;
	}

	public bool getImageAlpha(){
		return (imageAlpha == "true" || imageAlpha == "1") ? true : false;
	}

	public FpbJSON(){
	}

	public FpbJSON(bool offlineMode){
		if (offlineMode) {
			uniqueName = "mouse";
			numberOfImages = 255;
			sliceWidth = 336;
			sliceHeight = 255;
			imagePrefix = "mouse_z";
			numberingFormat = "0000";
			pathToImages = "C:\\Users\\carcu\\Documents\\Unity Projects\\FP Bioimage\\Builds\\mouse\\";
			voxelSize = new Vector3 (1.0f, 1.0f, 1.0f);
			opacity = 5.0f;
			intensity = 1.0f;
			threshold = 0.2f;
			projection = 1;
			atlasMode = "true";
			imageAlpha = "true";
		}
	}

	public FpbJSON(int preset){
		switch (preset) {
		case 0:
			uniqueName = "mouse";
			numberOfImages = 255;
			sliceWidth = 336/2;
			sliceHeight = 255/2;
			imagePrefix = "mouse_z";
			numberingFormat = "0000";
			pathToImages = "mouse/";
			voxelSize = new Vector3 (1.0f, 1.0f, 1.0f/2);
			opacity = 3.6f;
			intensity = 1.3f;
			threshold = 0.2f;
			projection = 2;
			atlasMode = "true";
			imageAlpha = "true";
			return;
		case 1:
			atlasMode = "true";
			imageAlpha = "true";
			pathToImages = "mrbrain/";
			imagePrefix= "mrbrain_z";
			numberingFormat= "0000";
			voxelSize= new Vector3(1.0f,1.0f,2.0f);
			numberOfImages= 99;
			sliceWidth= 256;
			sliceHeight= 256;
			opacity= 5.0f;
			intensity= 1.4f;
			threshold= 0.15f;
			projection= 2;
			return;

		case 2:
			atlasMode = "true";
			imageAlpha = "true";
			pathToImages = "flyembryo/";
			imagePrefix= "keller_z";
			numberingFormat= "0000";
			voxelSize= new Vector3(0.66f,1.32f,0.67f);
			numberOfImages= 240;
			sliceWidth= 232;
			sliceHeight= 317;
			opacity= 3.0f;
			intensity= 1.5f;
			threshold= 0.12f;
			projection= 2;
			return;

		case 3:
			atlasMode = variables.fpbFromURL.atlasMode;
			imageAlpha = variables.fpbFromURL.imageAlpha;
			pathToImages = variables.fpbFromURL.pathToImages;
			imagePrefix= variables.fpbFromURL.imagePrefix;
			numberingFormat= variables.fpbFromURL.numberingFormat;
			voxelSize= variables.fpbFromURL.voxelSize;
			numberOfImages= variables.fpbFromURL.numberOfImages;
			sliceWidth= variables.fpbFromURL.sliceWidth;
			sliceHeight= variables.fpbFromURL.sliceHeight;
			opacity= variables.fpbFromURL.opacity;
			intensity= variables.fpbFromURL.intensity;
			threshold= variables.fpbFromURL.threshold;
			projection= variables.fpbFromURL.projection;
			return;

		default:
			// Do nothing.
			return;
		}
	}
}
