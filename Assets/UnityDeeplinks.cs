using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class UnityDeeplinks : MonoBehaviour {

	public Dropdown sceneDropdown;

	#if UNITY_IOS
	[DllImport("__Internal")]
	private static extern void UnityDeeplinks_init(string gameObject = null, string deeplinkMethod = null);
	#endif

	// Use this for initialization
	void Start () {
		#if UNITY_IOS
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			UnityDeeplinks_init(gameObject.name);
		}
		#endif

		// TEST for decoding...
		/*string fpb64 = "eyJhdGxhc01vZGUiOiJ0cnVlIiwiaW1hZ2VBbHBoYSI6InRydWUiLCJwYXRoVG9GUEJpb2ltYWdlIjoiaHR0cDovL2ZwYi5jZWIuY2FtLmFjLnVrL2RldjMiLCJwYXRoVG9JbWFnZXMiOiJodHRwOi8vZnBiLmNlYi5jYW0uYWMudWsvZGVtby9leGFtcGxlcy9jdGhlYWQtaW1hZ2VzIiwidW5pcXVlTmFtZSI6ImN0aGVhZCIsImltYWdlUHJlZml4IjoiY3RoZWFkX3oiLCJudW1iZXJpbmdGb3JtYXQiOiIwMDAwIiwiZmlsZVR5cGUiOiJwbmciLCJ2b3hlbFNpemUiOnsieCI6MSwieSI6MSwieiI6Mn0sIm51bWJlck9mSW1hZ2VzIjo5OSwic2xpY2VXaWR0aCI6MjU2LCJzbGljZUhlaWdodCI6MjU2LCJvcGFjaXR5IjozLjMsImludGVuc2l0eSI6MS40LCJ0aHJlc2hvbGQiOjAuMjUsInByb2plY3Rpb24iOjJ9"; // copy in from javascript

		// Assume we have a base64 encoded string coming in from the link
		byte[] data = System.Convert.FromBase64String(fpb64);
		string fpbString = System.Text.Encoding.UTF8.GetString (data);
		GameObject go = GameObject.Find ("Title");
		go.GetComponent<Text> ().text = fpbString;*/
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void onDeeplink(string deeplink) {
		Debug.Log("onDeeplink " + deeplink);

		// Find part of URL starting 'f='
		int index = deeplink.IndexOf("f="); 
		if (index > -1){
			string fpb64 = deeplink.Substring (index+2);

			// Assume we have a base64 encoded string coming in from the link
			byte[] data = System.Convert.FromBase64String(fpb64);
			string fpbString = System.Text.Encoding.UTF8.GetString (data);
			FpbJSON fpb = JsonUtility.FromJson<FpbJSON> (fpbString);
			if (fpb.uniqueName != null) {
				variables.fpbFromURL = fpb;
				//variables.fpbJSON = fpb;
				sceneDropdown.AddOptions (new List<string>{ fpb.uniqueName });
				sceneDropdown.value = sceneDropdown.options.Count - 1;
			}
		}
	}
}
