using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Connect_scripts: MonoBehaviour {
	
	/*  This file is part of the "Ultimate Unity networking project" by M2H (http://www.M2H.nl)
     *  This project is available on the Unity Store. You are only allowed to use these
     *  resources if you've bought them from the Unity Assets Store.
     */

		// Public variables
	public static List<string> playerListIP;
	public static List<int> playerListID;
	public static bool selected = false;


	public string name_ = "";
	public string type_ = "";

	// Private Static variables
	private static int cpt = 0;
	private int connect = -1;

	//Obviously the GUI is for both client&servers (mixed!)
	void OnGUI()
	{
		
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			//We are currently disconnected: Neither client nor host
			//GUILayout.Label("Connection status: Disconnected");			
		}
		else
		{
			//We've got one or more connection(s)!		
			
			if (Network.peerType == NetworkPeerType.Connecting)
			{
				
					GUILayout.Label("Connection status: Connecting");
				
			}
			else if (Network.peerType == NetworkPeerType.Client)
			{
				GUILayout.Label("Welcome, "+name_+"!");
				GUILayout.Label("Status: "+type_);
				
			}
			else if (Network.peerType == NetworkPeerType.Server)
			{
				GUILayout.Label("Welcome, "+name_+"!");
				GUILayout.Label("Your working status: "+type_);
				GUILayout.Label("Connection status: Server!");
				GUILayout.Label("Connections: " + Network.connections.Length);
				if (Network.connections.Length >= 1)
				{
					GUILayout.Label("Ping to first player: " + Network.GetAveragePing(Network.connections[0]));
				}
			}
			
			if (GUILayout.Button("Disconnect"))
			{
				Network.Disconnect();
				gameObject.GetComponent<Connect_scripts>().enabled = true;
				Application.LoadLevel("login_scene");
			}
		}
		
		
	}
	
	// NONE of the functions below is of any use in this demo, the code below is only used for demonstration.
	// First ensure you understand the code in the OnGUI() function above.
	
	//Client functions called by Unity
	void OnConnectedToServer()
	{
		Debug.Log("This CLIENT has connected to a server");
	}
	
	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		Debug.Log("This CLIENT has disconnected from a server OR this SERVER was just shut down");
		gameObject.GetComponent<Connect_scripts>().enabled = true;
		Application.LoadLevel("login_scene");
	}
	
	void OnFailedToConnect(NetworkConnectionError error)
	{
		Debug.Log("Could not connect to server: " + error);
	}
	//Server functions called by Unity
	void OnPlayerConnected(NetworkPlayer player) {
		connect ++;
		if (connect == 0) {
			playerListID = new List<int> ();
			playerListIP = new List<string> ();
		}
		playerListIP.Add (player.ipAddress);
		playerListID.Add (cpt);
		cpt++;
	}



	void OnServerInitialized()
	{
		Debug.Log("Server initialized and ready");
	}
	
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		Debug.Log("Player disconnected from: " + player.ipAddress + ":" + player.port);
		int index = playerListIP.IndexOf (player.ipAddress);
		playerListIP.Remove (player.ipAddress);
		playerListID.RemoveAt (index);
	}
	// OTHERS:
	// To have a full overview of all network functions called by unity
	// the next four have been added here too, but they can be ignored for now
	
	void OnFailedToConnectToMasterServer(NetworkConnectionError info)
	{
		Debug.Log("Could not connect to master server: " + info);
	}
	
	void OnNetworkInstantiate(NetworkMessageInfo info)
	{
		Debug.Log("New object instantiated by " + info.sender);
	}
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		//Custom code here (your code!)
	}

}