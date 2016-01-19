using UnityEngine;
using System.Collections;
using NyARUnityUtils;

public class GUIButtonVideo_scripts : MonoBehaviour
{
	
	// Public static variables
	public static string clientType = "";
	
	// Public variable
	public Texture2D texture = null;
	public Texture2D textureVideo = null;
	public Texture2D textureStopVideo = null;
	
	// Private variables
	private bool showInventory = false;

	private void OnGUI ()
	{
		// Log in as worker (type)
		if (!showInventory) {
			UDPVoiceRecognition_scripts.startVoiceRecognition = true;
			texture = textureStopVideo;
		} else {
			UDPVoiceRecognition_scripts.startVoiceRecognition =  false;
			texture = textureVideo;
		}
		if (Network.isClient && cameraBehaviour_scripts.webSelected) {

			if (GUI.Button (new Rect (Screen.width - 3 * texture.width - 20, 5, texture.width, texture.height), texture)) {
				// Change the video mode: enale/disable the webcam
				showInventory = !showInventory;
				if(!showInventory){
					VideoPS_scripts.status = 1;
				}else{
					VideoPS_scripts.status = 2;
				}
			}
			
		}
	}
}