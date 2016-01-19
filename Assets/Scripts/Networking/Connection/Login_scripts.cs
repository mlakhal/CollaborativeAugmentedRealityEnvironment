using UnityEngine;
using System.Collections;

public class Login_scripts : MonoBehaviour
{
	public static string user = "", name_ = "";
	private string connectToIP = "127.0.0.1";
	private int connectPort = 25001;
	private string password = "", message = "";
	private string loginURL = "";

	private void Start ()
	{
		loginURL = "http://" + connectToIP + "/login.php";
	}
	
	private void OnGUI ()
	{
		if (message != "")
			GUILayout.Box (message);
		
		LoginGUI ();
	}
	
	//This method will login the accounts.
	void LoginGUI ()
	{
		GUI.Box (new Rect (350, 160, 300, 260), "Login");
		
		if (GUI.Button (new Rect (380, 360, 120, 25), "Login")) {
			
			if (user == "" || password == "")
				message += "Please enter all the fields \n";
			else {
				this.loginURL = "http://" + connectToIP + ":80/login.php";
				string loginURL = this.loginURL + "?username=" + user + "&password=" + password;
				WWW w = new WWW (loginURL);
				StartCoroutine (login (w, true));
			} 
			/////////
		}
		// IP Adress
		GUI.Label (new Rect (390, 180, 220, 23), "IP adress:");
		connectToIP = GUI.TextField (new Rect (390, 200, 220, 23), connectToIP);
		// Port Number
		GUI.Label (new Rect (390, 220, 220, 23), "Port number:");
		connectPort = int.Parse (GUI.TextField (new Rect (390, 240, 220, 23), connectPort.ToString ()));
		// Username
		GUI.Label (new Rect (390, 260, 220, 23), "Username:");
		user = GUI.TextField (new Rect (390, 280, 220, 23), user);
		// Password
		GUI.Label (new Rect (390, 300, 220, 23), "Password:");
		password = GUI.PasswordField (new Rect (390, 320, 220, 23), password, "*" [0]);
	}//End Login GUI
	
	
	IEnumerator login (WWW w, bool server)
	{
		yield return w;
		if (w.error == null) {

			// We recieve the result as a string, i.e: "connectionStatus#status(users table)#workerName"
			// we split the string by '#' !

			string[] results = w.text.Split ('#');

			// Set the player name to display on the chat GUI
			GUIChatMessagesS.playerName = results [2];
			GUILogTransform_scripts.playerName = results[2];

			// Set the name and the type of the client, and display it on the welcome screen!!
			gameObject.GetComponent<Connect_scripts>().name_ = results[2];
			gameObject.GetComponent<Connect_scripts>().type_ = results[1];
			GUIButtonVoice_scripts.clientType = results[1];

			if (results [0] == "login-SUCCESS") {
				switch (results [1]) {
				case "admin":
					Network.InitializeServer (32, connectPort, false);
					gameObject.GetComponent<Login_scripts> ().enabled = false;
					user = "";
					password = "";
					break;
				case "expert":
					Network.Connect (connectToIP, connectPort);
					gameObject.GetComponent<Login_scripts> ().enabled = false;
					user = "";
					password = "";
					break;
				case "worker":
					Network.Connect (connectToIP, connectPort);
					gameObject.GetComponent<Login_scripts> ().enabled = false;
					user = "";
					password = "";
					break;
					
				}
				
			} else
				message += w.text;
		} else {
			message += "ERROR: " + w.error + "\n";
		}
	}
	
	IEnumerator registerFunc (WWW w)
	{
		yield return w;
		if (w.error == null) {
			message += w.text;
		} else {
			message += "ERROR: " + w.error + "\n";
		}
	}
}
