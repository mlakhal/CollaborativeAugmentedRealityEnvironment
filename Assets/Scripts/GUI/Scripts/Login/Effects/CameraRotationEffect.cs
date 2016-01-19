using UnityEngine;
using System.Collections;

public class CameraRotationEffect : MonoBehaviour {

	public GameObject mainCamera;
	
	// This function will be called when scene loaded
	void Start () {
		// Fixed update will be performed 20 time per second:
		Time.fixedDeltaTime = 0.05f;
		// Getting MainCamera gameobject:
		mainCamera =  GameObject.Find("Main Camera");
	}
	
	// This function will be called every time.fixedDeltaTime
	// seconds:
	void FixedUpdate () {
		// Camera rotation with step 2 * time.deltaTime:
		mainCamera.transform.Rotate(0, 2 * Time.deltaTime, 0);
	}
}
