using UnityEngine;
using System.Collections;

public class GUILogTransform_scripts : MonoBehaviour
{
	/*  This file is part of the "Ultimate Unity networking project" by M2H (http://www.M2H.nl)
     *  This project is available on the Unity Store. You are only allowed to use these
     *  resources if you've bought them from the Unity Assets Store.
     */
	public ArrayList logMessages = new ArrayList();
	
	
	public bool usingChat = false;	//Can be used to determine if we need to stop player movement since we're chatting
	public GUISkin skin;						//Skin
	public GUILogTransform_scripts LT;
	public static string playerName;
	public bool showChat = false;			//Show/Hide the chat

	private Vector2 scrollPosition;
	private int width = 500;
	private int height = 180;
	private Rect window;
	
	
	//Server-only playerlist
	private ArrayList playerList = new ArrayList();
	public class PlayerNode
	{
		public string playerName;
		public NetworkPlayer networkPlayer;
	}
	
	private ArrayList chatEntries = new ArrayList();
	public class ChatEntry
	{
		public string name = "";
		public string text = "";
	}
	
	void Awake()
	{
		window = new Rect(10, 80, width, height);
		LT = this;
		
		//We get the name from the masterserver example, if you entered your name there ;).
		playerName = PlayerPrefs.GetString("playerName", "");
		if (playerName == null || playerName == "")
		{
			playerName = "RandomName" + Random.Range(1, 999);
		}
		
	}
	
	
	//Client function
	void OnConnectedToServer()
	{
		ShowLogWindow();
		GetComponent<NetworkView>().RPC("TellServerOurName", RPCMode.Server, playerName);
	}
	
	
	//A handy wrapper function to get the PlayerNode by networkplayer
	PlayerNode GetPlayerNode(NetworkPlayer networkPlayer)
	{
		foreach (var entry in playerList)
		{
			if ((entry as PlayerNode).networkPlayer == networkPlayer)
			{
				return (entry as PlayerNode);
			}
		}
		Debug.LogError("GetPlayerNode: Requested a playernode of non-existing player!");
		return null;
	}
	
	
	//Server function
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		//Remove player from the server list
		playerList.Remove(GetPlayerNode(player));
	}
	
	void OnDisconnectedFromServer()
	{
		CloseLogWindow();
	}

	[RPC]
	//Sent by newly connected clients, recieved by server
	void TellServerOurName(string name, NetworkMessageInfo info)
	{
		PlayerNode newEntry = new PlayerNode();
		newEntry.playerName = name;
		newEntry.networkPlayer = info.sender;
		playerList.Add(newEntry);

	}
	
	
	public void CloseLogWindow()
	{
		showChat = false;
		chatEntries = new ArrayList();
	}
	
	public void ShowLogWindow()
	{
		showChat = true;
		chatEntries = new ArrayList();
	}
	
	void OnGUI()
	{
		if (!showChat)
		{
			return;
		}
		if ( !cameraBehaviour_scripts.webSelected) {
			return;
		}

		GUI.skin = skin;
		
		window = GUI.Window(25, window, GlobalLogWindow, "Log **-- Transformation --**. Coordination");
	}
	
	
	void GlobalLogWindow(int id)
	{
		
		GUILayout.BeginVertical();
		GUILayout.Space(10);
		GUILayout.EndVertical();
		
		// Begin a scroll view. All rects are calculated automatically - 
		// it will use up any available screen space and make sure contents flow correctly.
		// This is kept small with the last two parameters to force scrollbars to appear.
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);
		
		foreach (var entry in chatEntries)
		{
			GUILayout.BeginHorizontal();
			if ((entry as ChatEntry).name == "")
			{//Game message
				GUILayout.Label((entry as ChatEntry).text);
			}
			else
			{
				GUILayout.Label((entry as ChatEntry).name + ": " + (entry as ChatEntry).text);
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(3);
			
		}
		// End the scrollview we began above.
		GUILayout.EndScrollView();
		
		GUI.DragWindow (new Rect (0, 0, Screen.width, Screen.height));
	}
	
	[RPC]
	public void ApplyGlobalLogText(string name, string msg)
	{
		ChatEntry entry = new ChatEntry();
		entry.name = name;
		entry.text = msg;
		
		chatEntries.Add(entry);
		logMessages.Add (name+ ": " +msg);
		
		//Remove old entries
		if (chatEntries.Count > 4)
		{
			height += 15;
		}
		scrollPosition.y +=10;
	}
	
	//Add game messages etc
	public void addGameLogMessage(string str)
	{
		ApplyGlobalLogText("", str);
		if (Network.connections.Length > 0)
		{
			GetComponent<NetworkView>().RPC("ApplyGlobalLogText", RPCMode.Others, "", str);
		}
	}
}