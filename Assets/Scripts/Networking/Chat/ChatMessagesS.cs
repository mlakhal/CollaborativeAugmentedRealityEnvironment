using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChatMessagesS : MonoBehaviour {
	
	public List<string> messages;
	public string username ="";
	
	[RPC]
	public void PostMessage(string message)
	{
		string mess = username + message;
		messages.Add (mess);
	}
	
}