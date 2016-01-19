using UnityEngine;
using System.Collections;

public class loginHandler_scripts : MonoBehaviour
{
	public static bool login_ = false;

	public string ip = "";
	public string port = "";
	public string login = "";
	public string password = "";
	public string message = "";
	public string loginURL = "";

	public void loginSystem ()
	{
		if (login == "" || password == "")
			message += "Please enter all the fields \n";
		else {
			this.loginURL = "http://" + ip + ":80/login.php";
			string loginURL = this.loginURL + "?username=" + login + "&password=" + password;
			WWW w = new WWW (loginURL);
			StartCoroutine (loginHandler (w));
		} 
	}

	IEnumerator loginHandler (WWW w)
	{
		yield return w;
		if (w.error == null) {
			login_ = true;
			gameObject.GetComponent<StartMenuController>().enabled = false;
			DontDestroyOnLoad(this);
			Application.LoadLevel("arScene_scene");
			// We recieve the result as a string, i.e: "connectionStatus#status(users table)#workerName"
			// we split the string by '#' !
			
			string[] results = w.text.Split ('#');
			
			// Set the player name to display on the chat GUI
			GUIChatMessagesS.playerName = results [2];
			GUILogTransform_scripts.playerName = results [2];
			
			// Set the name and the type of the client, and display it on the welcome screen!!
			ConnectionHandler_scripts.clientNameString = results [2];
			ConnectionHandler_scripts.clientTypeString = results [1];
			ConnectionHandler_scripts.resultString = results[0];
			ConnectionHandler_scripts.loginBool = login_;
			ConnectionHandler_scripts.ip = ip;
			int port_ = 0;
			int.TryParse( port, out port_);
			ConnectionHandler_scripts.port = port_;
			GUIButtonVoice_scripts.clientType = results [1];

			// Update the new Scene

		} else {
			message += "ERROR: " + w.error + "\n";
		}
	}
}
