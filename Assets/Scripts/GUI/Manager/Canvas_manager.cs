using UnityEngine;
using System.Collections;

public class Canvas_manager : MonoBehaviour {

	public static bool show_= false;


	// Update is called once per frame
	void Update () {
		if (!show_) {
			gameObject.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
		} else {
			gameObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
		}
	}
}
