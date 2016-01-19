using UnityEngine;
using System.Collections;

public class GUIButtonVoice_scripts : MonoBehaviour
{

	// Public static variables
	public static string clientType = "";

	// Public variable
	public Texture2D texture = null;
	public Texture2D textureVoice = null;
	public Texture2D textureStopVoice = null;

	// Private variables
	private bool showInventory = false;

	private void OnGUI ()
	{
		// Log in as worker (type)
		if (!showInventory) {
			UDPVoiceRecognition_scripts.startVoiceRecognition = false;
			texture = textureVoice;
		} else {
			UDPVoiceRecognition_scripts.startVoiceRecognition = true;
			texture = textureStopVoice;
		}
		if (Network.isClient && clientType == "worker" && cameraBehaviour_scripts.webSelected) {
			if (GUI.Button (new Rect (Screen.width - 5 * texture.width - 30, 5, texture.width, texture.height), texture)) {
				// Change the voice mode: enale/disable voice recongnition
				showInventory = !showInventory;
			}
			
		}
	}

}
