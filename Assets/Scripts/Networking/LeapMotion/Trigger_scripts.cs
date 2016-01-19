using UnityEngine;
using System.Collections;

public class Trigger_scripts : MonoBehaviour {


	/*  This file is part of the "Ultimate Unity networking project" by M2H (http://www.M2H.nl)
     *  This project is available on the Unity Store. You are only allowed to use these
     *  resources if you've bought them from the Unity Assets Store.
     */
	public NetworkPlayer owner;
	public float speed = 5;
	public Vector3 serverCurrentPosition = new Vector3(0,0,150);
	//Last input value, we're saving this to be able to save network messages/bandwidth.
	private float lastClientHInput = 0;
	private float lastClientVInput = 0;
	
	//The input values the server will execute on this object
	private float updateLogGUIHInput = 0;
	private float updateLogGUIVInput = 0;
	private float variation = 10;

	private bool update_ = false;
	
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

	void OnTriggerEnter(Collider other) {

		if (Network.isClient) {
			Debug.Log("Trigger hna!");
			networkView.RPC ("SendMovementInput", RPCMode.Server, handPosition_scripts.handPosition);
		}

	}

	private void Update(){

		//Server movement code
		if ((Network.isServer || Network.player == owner) && update_) {//To also enable this on the client itself, use: "|| Network.player==owner){|"
			//Actually move the player using his/her input
			transform.position = serverCurrentPosition ;
			update_ = false;
		}

	}

	[RPC]
	void SendMovementInput (Vector3 position)
	{
		//Called on the server
		serverCurrentPosition = position;
		networkView.RPC ("SendMovementInput", RPCMode.Others, serverCurrentPosition);
		update_ = true;
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
