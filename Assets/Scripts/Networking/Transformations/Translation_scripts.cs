using UnityEngine;
using System.Collections;

public class Translation_scripts : MonoBehaviour
{
	/*  This file is part of the "Ultimate Unity networking project" by M2H (http://www.M2H.nl)
     *  This project is available on the Unity Store. You are only allowed to use these
     *  resources if you've bought them from the Unity Assets Store.
     */
	public NetworkPlayer owner;
	public float speed = 5;
	public float serverCurrentHInput = 0;
	public float serverCurrentVInput = 0;
	//Last input value, we're saving this to be able to save network messages/bandwidth.
	private float lastClientHInput = 0;
	private float lastClientVInput = 0;
	
	//The input values the server will execute on this object
	private float updateLogGUIHInput = 0;
	private float updateLogGUIVInput = 0;
	private float variation = 10;
	
	void Awake ()
	{
		if (Network.isClient) {
			// We are probably not the owner of this object: disable this script.
			// RPC's and OnSerializeNetworkView will STILL get trough!
			// The server ALWAYS run this script though
			enabled = false;	 // disable this script (this disables Update());	
		}
	}
	
	[RPC]
	void SetPlayer (NetworkPlayer player)
	{
		owner = player;
		if (player == Network.player) {
			//Hey thats us! We can control this player: enable this script (this enables Update());
			enabled = true;
		}
	}

	void Update ()
	{
		//Client code
		if (Network.player == owner && Scale_scripts.spawn && ComboBox_scripts.selectedItem_ != -1 && ComboBox_scripts.selectedItem_ != ComboBox_scripts.lenght - 1) {
			string id = GetComponent<NetworkView>().viewID.ToString ();
			int networkID_ = -1; 
			int.TryParse (id.Replace ("AllocatedID: ", ""), out networkID_);		
			bool enable_ = ComboBox_scripts.newNetIDStatic [ComboBox_scripts.selectedItem_].Equals (networkID_);
			// Select only the gameObject from the combo box
			if (enable_) {
				//Only the client that owns this object executes this code
				float HInput = Input.GetAxis ("Horizontal");
				float VInput = Input.GetAxis ("Vertical");
				
				//Is our input different? Do we need to update the server?
				if (lastClientHInput != HInput || lastClientVInput != VInput) {
					lastClientHInput = HInput;
					lastClientVInput = VInput;
					
					if (Network.isServer) {
						//Too bad a server can't send an rpc to itself using "RPCMode.Server"!
						//This is a Unity "feature", see `Tips`
						SendMovementInput (HInput, VInput);
					} else if (Network.isClient) {
						SendMovementInput (HInput, VInput); //Use this (and line 64) for simple "prediction"
						GetComponent<NetworkView>().RPC ("SendMovementInput", RPCMode.Server, HInput, VInput);

						// Verify if we can update the GUI Log view!
						if (Mathf.Abs (updateLogGUIHInput - lastClientHInput) > variation
							&& Mathf.Abs (updateLogGUIVInput - lastClientVInput) > variation) {
							string msg = "has done a Translation on object. New vector : " + transform.position.ToString () + ".\nAt : " + System.DateTime.Now;
							GameObject.Find ("logTransform").gameObject.GetComponent<NetworkView>().RPC ("ApplyGlobalLogText", RPCMode.AllBuffered, GUILogTransform_scripts.playerName, msg);
						}
					}
					
				}
			}
		}
		
		//Server movement code
		if (Network.isServer || Network.player == owner) {//To also enable this on the client itself, use: "|| Network.player==owner){|"
			//Actually move the player using his/her input
			Vector3 moveDirection = new Vector3 (serverCurrentHInput, 0, serverCurrentVInput);
			transform.Translate (speed * moveDirection * Time.deltaTime);
		}
		
	}
	
	[RPC]
	void SendMovementInput (float HInput, float VInput)
	{
		//Called on the server
		serverCurrentHInput = HInput;
		serverCurrentVInput = VInput;
	}
	
	void OnSerializeNetworkView (BitStream stream, NetworkMessageInfo info)
	{
		if (stream.isWriting) {
			//This is executed on the owner of the networkview
			//The owner sends it's position over the network
			Vector3 pos = transform.position;
			stream.Serialize (ref pos);//"Encode" it, and send it
			
		} else {
			//Executed on all non-owners
			//receive a position and set the object to it
			Vector3 posReceive = Vector3.zero;
			stream.Serialize (ref posReceive); //"Decode" it and receive it
			
			//We've just recieved the current servers position of this object in 'posReceive'.
			
			transform.position = posReceive;
			//To reduce laggy movement a bit you could comment the line above and use position lerping below instead:	
			//transform.position = Vector3.Lerp(transform.position, posReceive, 0.9f); //"lerp" to the posReceive by 90%
			//It would be even better to save the last received server position and lerp to it in Update because it is executed more often than OnSerializeNetworkView
			
		}
	}
}