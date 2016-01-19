using UnityEngine;
using System.Collections;

public class VideoPS_scripts : MonoBehaviour {
	
	public static int status = 0;
	
	private void Update(){
		switch (status) {
		case 1:
			transform.position = new Vector3 (0, 0, 20);
			transform.rotation = new Quaternion(0,0,0,0);
			break;
		case 2:
			transform.position = new Vector3 (10500, 0, 20);
			transform.rotation = new Quaternion(0,0,0,0);
			break;
		}
	}
	
}
