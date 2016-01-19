using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LogTransform_scripts : MonoBehaviour {
	
	public List<string> messages;
	public string username ="";
	
	[RPC]
	public void PostMessage(string message)
	{
		string mess = username + message;
		messages.Add (mess);
	}
	
}