using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class sendMailBox_scripts : MonoBehaviour
{
	
	// Public static variables
	public static bool showInventory = false;
	public static string sereverIP = "http://";
	
	public GUISkin myskin;
	// Private variables
	private string fromEmail = "";
	private string fromName = "";
	private string fromPassword = "";
	private string fromSubject = "";
	private string fromBody = "";
	private string fromAttachement = "";
	private string toEmail = "";
	private Vector2 scrollPosition = Vector2.zero;
	private Rect window;
	
	private void OnGUI ()
	{
		LoginGUI ();
	}
	
	//This method will login the accounts.
	private void LoginGUI ()
	{
		if (showInventory) {
			GUI.Box (new Rect (550, 160, 300, 400), "Send email");
			
			// From (email)
			GUI.Label (new Rect (590, 180, 220, 23), "From (email):");
			fromEmail = GUI.TextField (new Rect (590, 200, 220, 23), fromEmail);
			// From (name)
			GUI.Label (new Rect (590, 220, 220, 23), "From (name):");
			fromName = GUI.TextField (new Rect (590, 240, 220, 23), fromName);
			// From (password)
			GUI.Label (new Rect (590, 260, 220, 23), "Paswword (sender):");
			fromPassword = GUI.PasswordField (new Rect (590, 280, 220, 23), fromPassword, "*" [0]);
			// Subject
			GUI.Label (new Rect (590, 300, 220, 23), "Subject :");
			fromSubject = GUI.TextField (new Rect (590, 320, 220, 23), fromSubject);
			// Message
			GUI.Label (new Rect (590, 340, 220, 23), "Message :");
			scrollPosition = GUI.BeginScrollView (new Rect (590, 360, 220, 200), scrollPosition, new Rect (0, 0, 200, 200));
			fromBody = GUI.TextArea (new Rect (0, 00, 220, 70), fromBody);
			GUI.EndScrollView ();


			if(GUIButton_scripts.clientType == "worker"){
			
				// Attachement
				GUI.Label (new Rect (590, 430, 220, 23), "Attache file :");
				fromAttachement = GUI.TextField (new Rect (640, 450, 170, 23), fromAttachement);

				if (GUI.Button (new Rect (590, 450, 45, 23), "Open")) {
					OpenFileName ofn = new OpenFileName ();
					
					ofn.structSize = Marshal.SizeOf (ofn);
					
					ofn.filter = "All Files\0*.*\0\0";
					
					ofn.file = new string (new char[256]);
					
					ofn.maxFile = ofn.file.Length;
					
					ofn.fileTitle = new string (new char[64]);
					
					ofn.maxFileTitle = ofn.fileTitle.Length;
					
					ofn.initialDir = UnityEngine.Application.dataPath;
					
					ofn.title = "Attache file to your email";
					
					ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
					//OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR
					
					if (DllOpen.GetOpenFileName (ofn)) {
						fromAttachement = ofn.file;
					}
				}
			}

			// To (email)
			GUI.Label (new Rect (590, 470, 220, 23), "To (email):");
			toEmail = GUI.TextField (new Rect (590, 490, 220, 23), toEmail);
			
			if (GUI.Button (new Rect (590, 520, 120, 25), "Send")) {
				sereverIP += ConnectionHandler_scripts.ip+":80";

				if(GUIButton_scripts.clientType == "worker"){
					WebClient client = new WebClient();
					client.UploadFile(sereverIP+"/sendMail/upload.php", "POST", fromAttachement);
				}
				
				string filename = Path.GetFileName(fromAttachement);
				
				string loginURL = sereverIP+"/sendMail/mail.php?from="+fromName+"&fromName="+fromEmail+"&password="+fromPassword+"&to="
					+toEmail+"&subject="+fromSubject+"&body="+fromBody+"&file="+filename;
				Debug.Log(loginURL);
				WWW w = new WWW(loginURL);
				StartCoroutine(login(w));
			}
		}
		
	}//End Login GUI
	
	
	IEnumerator notificationCoroutine()
	{
		float timeToWait = 5;
		yield return new WaitForSeconds(timeToWait);
	}
	
	IEnumerator login(WWW w)
	{		
		yield return w;
		// Hide the sendail GUI
		showInventory = false;
		// Clear all the fields
		fromEmail = "";
		fromName = "";
		fromPassword = "";
		fromSubject = "";
		fromBody = "";
		fromAttachement = "";
		toEmail = "";
		
		// check for errors
		if (w.error == null)
		{
			gameObject.GetComponent<sendMailNotification_scripts>().i = 1;
			gameObject.GetComponent<sendMailNotification_scripts>().show = true;
		} else {
			gameObject.GetComponent<sendMailNotification_scripts>().i = 2;
			gameObject.GetComponent<sendMailNotification_scripts>().show = true;
		} 
	}
}
