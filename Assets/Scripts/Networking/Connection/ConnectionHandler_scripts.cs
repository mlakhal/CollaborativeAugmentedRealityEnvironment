using UnityEngine;
using System.Collections;

public class ConnectionHandler_scripts : MonoBehaviour
{
	
	public static string clientTypeString = "";
	public static string clientNameString = "";
	public static string resultString = "";
	public static string ip = "";
	public static int port = 0;
	public static bool loginBool = false;
	
	// Update is called once per frame
	void Update ()
	{
		if (loginBool) {
			// Set the name and the type of the client, and display it on the welcome screen!!
			gameObject.GetComponent<Connect_scripts> ().name_ = clientNameString;
			gameObject.GetComponent<Connect_scripts> ().type_ = clientTypeString;
			GUIChatMessagesS.playerName = clientNameString;
			GUILogTransform_scripts.playerName = clientNameString;
			GUIButtonVoice_scripts.clientType = clientTypeString;
			if (resultString == "login-SUCCESS") {
				switch (clientTypeString) {
				case "root":
					Network.InitializeServer (32, port, false);
					// Setting up the camera rotation script
					GameObject cameraServer= GameObject.Find("CameraServer");
					cameraServer.GetComponent<CameraRotationEffect>().mainCamera = cameraServer; 
					//GameObject.Find("cameraClient").GetComponent<cameraBehaviour_scripts>().enabled = true;
					Destroy(GameObject.Find("cameraClient"));
					break;
				case "expert": 
					Network.Connect (ip, port);
					Destroy(GameObject.Find("CameraServer"));
					//GUIButton_scripts.login = true;
					GameObject.Find("cameraClient").GetComponent<cameraBehaviour_scripts>().enabled = true;
					GUIButtonVideo_scripts.clientType = clientTypeString;
					VideoPS_scripts.status = 1;
					break;
				case "worker":
					Network.Connect (ip, port);
					Destroy(GameObject.Find("CameraServer"));
					GameObject.Find("cameraClient").GetComponent<cameraBehaviour_scripts>().enabled = true;
					GUIButtonVideo_scripts.clientType = clientTypeString;
					UDPVoiceRecognition_scripts.runTh = true;
					VideoPS_scripts.status = 1;
					// Get client name
					GameObject.Find("guiVertical").GetComponent<Spawn_scripts>().clientName = clientNameString;
					SpawnControl.spawnC = true;
					// Set client type to car combo-box
					GameObject.Find("comboCar").GetComponent<ARComboBox_scripts>().clientType = clientTypeString;
					break;	
				}
				Destroy(GameObject.Find("StartMenu"));
			}
			loginBool = false;
		}
	}
}
