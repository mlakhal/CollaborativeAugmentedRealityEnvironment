using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using System;
using System.IO;

public class GUIButton_scripts : MonoBehaviour
{

	public class ChatEntry
	{
		public string name = "";
		public string text = "";
	}

	// Public variables
	public Texture2D texture = null;
	public int i = 0;
	public bool showInventory = false;
	public static string clientType = "";
	private string regUsername = "";
	private string regPassword = "";
	private string regRePassword = "";
	private string regType = "";
	private string regWorkerName = "";
	
	private void OnGUI ()
	{

		// This method will show on the screen all the button when we'll login at, as
		if (Network.isServer) {
			registerGUI ();
			switch (i) {
			case 6:
				if (GUI.Button (new Rect (Screen.width - texture.width - 10, 5, texture.width, texture.height), texture)) {
					// Here's go the code for regestring new user
					showInventory = !showInventory;
				}
				break;
			}
		}
		// Log in as Expert (type)
		if (Network.isClient && ( cameraBehaviour_scripts.webSelected  ) ) {
			if ( (clientType == "worker" || clientType == "expert") && i < 5) {
				switch (i) {
				case 1:
					if (GUI.Button (new Rect (Screen.width - texture.width - 10, 5, texture.width, texture.height), texture)) {
						sendMailBox_scripts.showInventory = !sendMailBox_scripts.showInventory;
					}
					break;
				case 2:
					if (GUI.Button (new Rect (Screen.width - 2 * texture.width - 15, 5, texture.width, texture.height), texture)) {
						screenShot_scripts.saveFile ();
					}
					break;
				case 4:
					if (GUI.Button (new Rect (Screen.width - 4 * texture.width - 25, 5, texture.width, texture.height), texture)) {
						SaveFileName ofn = new SaveFileName ();
						
						ofn.structSize = Marshal.SizeOf (ofn);
						
						ofn.filter = "All Files\0*.*\0\0";
						
						ofn.file = new string (new char[256]);
						
						ofn.maxFile = ofn.file.Length;
						
						ofn.fileTitle = new string (new char[64]);
						
						ofn.maxFileTitle = ofn.fileTitle.Length;
						
						ofn.initialDir = UnityEngine.Application.dataPath;
						
						ofn.title = "Save Report Log";
						
						ofn.defExt = "txt";
						ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;//OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR
						
						if (DllSave.GetSaveFileName (ofn)) {
							
							StartCoroutine (WaitLoad (ofn.file));
							
						}
					}
					break;
				}
			}
			// Log in as worker (type)
			if (clientType == "worker") {
				switch (i) {
				case 7:
					if (GUI.Button (new Rect (Screen.width - 6 * texture.width - 35, 5, texture.width, texture.height), texture)) {
						// Change the voice mode: enale/disable voice recongnition
						int indexCombo = ARComboBox_scripts.selectedItem_;
						switch(indexCombo){
						case 0:
							GameObject.Find("Bugatti-Veyron").GetComponent<NetworkView>().RPC ("reloadObject", RPCMode.Server, true);
							break;
						case 1:
							GameObject.Find("lamborghini").GetComponent<NetworkView>().RPC ("reloadObject", RPCMode.Server, true);
							break;
						case 2:
							GameObject.Find("Car").GetComponent<NetworkView>().RPC ("reloadObject", RPCMode.Server, true);	
							break;
						}
					}
					break;

				}
			}
		}
	}

	IEnumerator WaitLoad (string fileName)
	{
		if (File.Exists (fileName)) {
			Debug.Log (fileName + " already exists.");
			return false;
		}
		
		
		printMessage (fileName);
		
		
		WWW wwwTexture = new WWW ("file://" + fileName);
		
		yield return wwwTexture;

	}
	
	private void printMessage (string fileName)
	{
		string theTime = System.DateTime.Now.ToString ("hh:mm:ss");
		string theDate = System.DateTime.Now.ToString ("dd/MM/yyyy"); 
		var sr = File.CreateText (fileName);
		
		sr.WriteLine ("--------------------------------");
		sr.WriteLine ("File Report Log. Date : " + theDate + "  Time : " + theTime);
		sr.WriteLine ("--------------------------------");
		sr.WriteLine ("============================================================================================================");
		sr.WriteLine ("\t\t        -------------------------------------------------");
		sr.WriteLine ("\t\t\t    - Documentation of users manipulations -");
		sr.WriteLine ("\t\t\t-------------------------------------------------");
		sr.WriteLine ("============================================================================================================");
		sr.WriteLine ("  _____________________________________________       ____________________________________________________");
		sr.WriteLine (" /        \t\t\t\t       \\     /      \t\t\t\t                  \\");
		sr.WriteLine ("<-} Ecole nationale supérieure d'informatique {->   <-} Centre de développement des technologies avancées{->");
		sr.WriteLine (" \\_____________________________________________/     \\____________________________________________________/");
		sr.WriteLine ("");
		sr.WriteLine ("============================================================================================================");
		sr.WriteLine ("");	
		sr.WriteLine ("\t\t         	-----------------------");
		sr.WriteLine ("\t\t		-+ Table of Contents +-");
		sr.WriteLine ("\t\t		-----------------------");
		sr.WriteLine ("");		
		sr.WriteLine ("");		
		sr.WriteLine ("		I.  Basic Transformation");
		sr.WriteLine ("		------------------------------");
		sr.WriteLine ("		A).  Type of transformation");
		sr.WriteLine ("	        	1). Rotation");
		sr.WriteLine ("	        	2). Scale");
		sr.WriteLine ("	        	3). Translation");
		sr.WriteLine ("	        	4). Coloration");
		sr.WriteLine ("	        	Note : for every game object on the système, we can do one of the transformation above.");
		sr.WriteLine ("		B).  Message structure");
		sr.WriteLine ("	        	Note : the message displayed on the log-box has the following structure.");
		sr.WriteLine ("	        	       message{");
		sr.WriteLine ("	        	          string: name; // user name (i.e: it can be either Expert or Worker)    ");
		sr.WriteLine ("	        	          int: type; // type of transformation (i.e: it can be either Expert or Worker)    ");
		sr.WriteLine ("	        	                     //  0. Rotation, 1. Scale, 2. Translation, 3. Coloration");
		sr.WriteLine ("	        	          Vector3: vector; // the new vector value");
		sr.WriteLine ("	        	          float: time; // current date time");
		sr.WriteLine ("	        	          }");
		sr.WriteLine ("		B).  Data");
		sr.WriteLine ("");		
		sr.WriteLine ("");

		// Write down the Transform-Log
		ArrayList logMessage = GameObject.Find ("logTransform").GetComponent<GUILogTransform_scripts> ().logMessages;

		foreach (var i in logMessage) {
			sr.WriteLine (i);
		}
		sr.Close ();
	}

	//This method will login the accounts.
	private void registerGUI ()
	{
		if (showInventory) {
			GUI.Box (new Rect (350, 160, 300, 280), "Register new user");
			
			// From (email)
			GUI.Label (new Rect (390, 180, 220, 23), "Username :");
			regUsername = GUI.TextField (new Rect (390, 200, 220, 23), regUsername);
			// From (password)
			GUI.Label (new Rect (390, 220, 220, 23), "Paswword :");
			regPassword = GUI.PasswordField (new Rect (390, 240, 220, 23), regPassword, "*" [0]);
			// From (password)
			GUI.Label (new Rect (390, 260, 220, 23), "Confirm paswword :");
			regRePassword = GUI.PasswordField (new Rect (390, 280, 220, 23), regRePassword, "*" [0]);
			// Subject
			GUI.Label (new Rect (390, 300, 220, 23), "Type :");
			regType = GUI.TextField (new Rect (390, 320, 220, 23), regType);
			// Message
			GUI.Label (new Rect (390, 340, 220, 23), "Worker name :");
			regWorkerName = GUI.TextField (new Rect (390, 360, 220, 23), regWorkerName);

			if (GUI.Button (new Rect (390, 400, 120, 25), "Register")) {
				if (regPassword == regRePassword) {
					string loginURL = "http://" + ConnectionHandler_scripts.ip + ":80/register.php?username=" + regUsername + "&password=" + regPassword + "&type="
						+ regType + "&workerName=" + regWorkerName;
					WWW w = new WWW (loginURL);
					StartCoroutine (register (w));
				}
			}
		}
		
	}//End Login GUI

	IEnumerator notificationSuccessCoroutine ()
	{
		float timeToWait = 3;
		yield return new WaitForSeconds (timeToWait);
	}

	IEnumerator register (WWW w)
	{		
		yield return w;
		// Hide the sendail GUI
		showInventory = false;
		// Clear all the fields
		regUsername = "";
		regPassword = "";
		regRePassword = "";
		regType = "";
		regWorkerName = "";
		 
	}
}
