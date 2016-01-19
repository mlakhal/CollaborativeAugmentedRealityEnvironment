using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnWheel : MonoBehaviour
{
	/*  This file is part of the "Ultimate Unity networking project" by M2H (http://www.M2H.nl)
     *  This project is available on the Unity Store. You are only allowed to use these
     *  resources if you've bought them from the Unity Assets Store.
     */
	
	public Transform playerPrefab;
	public Transform volantPrefab;
	public Transform rouePrefab;
	// List of actions -- Translation, Scale, Rotation, Color, ...
	public List<Translation_scripts> playerScripts = new List<Translation_scripts> ();
	public List<Scale_scripts> scaleScripts = new List<Scale_scripts> ();
	public List<Rotate_scripts> rotateScripts = new List<Rotate_scripts> ();
	public List<ColorPicker_scripts> colorScripts = new List<ColorPicker_scripts> ();

	public Texture2D texture = null;
	public Texture2D volant = null;
	public Texture2D roue = null;
	public int i = 0;
	public string prefabName = "";
	public string clientName = "";

	// Private variables
	private int spawnID;
	private string clientType = "";

	private void OnConnectedToServer ()
	{
		spawnID = 0;
		clientType = GameObject.Find ("connect").gameObject.GetComponent<Connect_scripts> ().type_;
		GameObject.Find ("comboBox").gameObject.GetComponent<ComboBox_scripts> ().clientType = clientType;
		GUIButton_scripts.clientType = clientType;
		switch (clientName) {
		case "Worker-01":
			playerPrefab = rouePrefab;
			texture = roue;
			break;
		case "Worker-02":
			playerPrefab = volantPrefab;
			texture = volant;
			break;
		}
	}

	private void OnGUI ()
	{
		if (cameraBehaviour_scripts.webSelected) {
			// Display the GUI only if we are a client!
			if (Network.isClient && clientType == "worker") {
				switch (i) {
				case 1:
					if (GUI.Button (new Rect (Screen.width - texture.width - 10, Screen.height - 70, texture.width, texture.height), texture)) {
						Spawnplayer (Network.player);
					}
					break;
				}
			}
		}
	}

		
	void Spawnplayer (NetworkPlayer player)
	{
		//Called on the server only
		
		//Instantiate a new object for this player, remember; the server is therefore the owner.
		Transform myNewTrans = Network.Instantiate (playerPrefab, new Vector3 (0, 0, 150), Quaternion.identity, 0) as Transform;
		
		//Get the networkview of this new transform
		NetworkView newObjectsNetworkview = myNewTrans.GetComponent<NetworkView> ();
		Scale_scripts.spawn = true;
			
		//Keep track of this new player so we can properly destroy it when required.
		playerScripts.Add (myNewTrans.GetComponent<Translation_scripts> () as Translation_scripts);
		scaleScripts.Add (myNewTrans.GetComponent<Scale_scripts> () as Scale_scripts);
		rotateScripts.Add (myNewTrans.GetComponent<Rotate_scripts> () as Rotate_scripts);
		colorScripts.Add (myNewTrans.GetComponent<ColorPicker_scripts> () as ColorPicker_scripts);


		string msg = "has Spawn a new object. At : " + System.DateTime.Now;
		GameObject.Find ("logTransform").gameObject.GetComponent<NetworkView> ().RPC ("ApplyGlobalLogText", RPCMode.AllBuffered, GUILogTransform_scripts.playerName, msg);
		//Call an RPC on this new networkview, set the NetworkPlayer who controls this new player

		newObjectsNetworkview.RPC ("SetPlayer", RPCMode.AllBuffered, player);//Set it on the owner
		newObjectsNetworkview.RPC ("SetScale", RPCMode.AllBuffered, player);//Set it on the owner
		newObjectsNetworkview.RPC ("SetRotation", RPCMode.AllBuffered, player);//Set it on the owner
		newObjectsNetworkview.RPC ("SetColor", RPCMode.AllBuffered, player);//Set it on the owner

		// Update the Combo Box
		
		// Save the Network Id
		string id = newObjectsNetworkview.viewID.ToString ();
		int networkID_ = -1; 
		int.TryParse (id.Replace ("AllocatedID: ", ""), out networkID_);		
		ComboBox_scripts.newNetID = networkID_;
		// Save the other variables
		spawnID++;
		ComboBox_scripts.newSpawnObject = prefabName + spawnID;
		ComboBox_scripts.spawn = true;
	}
	
	void OnPlayerDisconnected (NetworkPlayer player)
	{
		removeObject (player);
	}

	public void removeObject (NetworkPlayer player)
	{
		
		// Translation
		foreach (var scripta in playerScripts) {
			Translation_scripts script = scripta as Translation_scripts;
			if (player == script.owner) {//We found the players object
				Network.RemoveRPCs (script.gameObject.GetComponent<NetworkView> ().viewID);//remove the bufferd SetPlayer call
				Network.Destroy (script.gameObject);//Destroying the GO will destroy everything
				playerScripts.Remove (script);//Remove this player from the list
				break;
			}
		}
		// Scale
		foreach (var scripta in scaleScripts) {
			Scale_scripts script = scripta as Scale_scripts;
			if (player == script.owner) {//We found the players object
				Network.RemoveRPCs (script.gameObject.GetComponent<NetworkView> ().viewID);//remove the bufferd SetPlayer call
				Network.Destroy (script.gameObject);//Destroying the GO will destroy everything
				scaleScripts.Remove (script);//Remove this player from the list
				break;
			}
		}
		
		// Rotation
		foreach (var scripta in rotateScripts) {
			Rotate_scripts script = scripta as Rotate_scripts;
			if (player == script.owner) {//We found the players object
				Network.RemoveRPCs (script.gameObject.GetComponent<NetworkView> ().viewID);//remove the bufferd SetPlayer call
				Network.Destroy (script.gameObject);//Destroying the GO will destroy everything
				rotateScripts.Remove (script);//Remove this player from the list
				break;
			}
		}
		
		//Remove the buffered RPC call for this player (SetPlayer, line 37)
		int playerNumber = int.Parse (player + "");
		Network.RemoveRPCs (Network.player, playerNumber);
		
		// The next destroys will not destroy anything since the players never
		// instantiated anything nor buffered RPCs
		Network.RemoveRPCs (player);
		Network.DestroyPlayerObjects (player);
	}

	void OnDisconnectedFromServer (NetworkDisconnection info)
	{
		Debug.Log ("Resetting the scene the easy way.");
		Application.LoadLevel (Application.loadedLevel);
	}
	
}