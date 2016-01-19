using UnityEngine;
using System.Collections;

public class Rotate_scripts : MonoBehaviour
{

	// Public variables

	public NetworkPlayer owner;
	public float RotationSpeed = 25;
	public bool down = false;
	
	// Variables that handle rotation variation
	// i.e: if the delta rotation is greater than something update!
	public float variation = 10.0f;
	/* Rotations */
	private Vector3 clientRotation = Vector3.zero;
	private Vector3 updateRotation = Vector3.zero;
	private Vector3 serverRotation = Vector3.zero;
	private bool rotate = false;


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
	void SetRotation (NetworkPlayer player)
	{
		owner = player;
		if (player == Network.player) {
			//Hey thats us! We can control this player: enable this script (this enables Update());
			enabled = true;
		}
	}


	// Update is called once per frame
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
				// If we press "R" start rotation, "T" stop
				if (Input.GetKey (KeyCode.R))
					down = true;
				else if (Input.GetKey (KeyCode.T))
					down = false;
				// Do action when rotation is actif
				if (down) {
					Vector3 input = new Vector3(Input.GetAxis ("Mouse X"), Input.GetAxis ("Mouse Y"), 0);
					clientRotation = input;
					if (Network.isServer) {
						//Too bad a server can't send an rpc to itself using "RPCMode.Server"!
						//This is a Unity "feature", see `Tips`
						SendRotation ( clientRotation);
					} else if (Network.isClient) {
						SendRotation ( clientRotation); //Use this (and line 64) for simple "prediction"
						GetComponent<NetworkView>().RPC ("SendRotation", RPCMode.Server, clientRotation);
					}

					// Verify if we can update the view
					if (Mathf.Abs (updateRotation.x - transform.rotation.eulerAngles.x) > variation
					    && Mathf.Abs (updateRotation.y - transform.rotation.eulerAngles.y) > variation
					    && Mathf.Abs (updateRotation.z - transform.rotation.eulerAngles.z) > variation) {
						string msg = "has done a rotation. Angle :"+transform.rotation.eulerAngles.ToString()+".\nAt : "+System.DateTime.Now;;
						GameObject.Find("logTransform").gameObject.GetComponent<NetworkView>().RPC("ApplyGlobalLogText", RPCMode.AllBuffered, GUILogTransform_scripts.playerName, msg);
						updateRotation = transform.rotation.eulerAngles;
					}

				}
			}
		}
		//Server movement code
		if (Network.isServer || Network.player==owner) {//To also enable this on the client itself, use: "|| Network.player==owner){|"
			//Actually rotate the player using his/her input
			if (rotate) {
				transform.Rotate ((serverRotation.x * RotationSpeed * Time.deltaTime),
				                  (serverRotation.y * RotationSpeed * Time.deltaTime), 0, Space.World);
				rotate = false;
			}
		}
	}
	
	[RPC]
	public void SendRotation (Vector3 position_)
	{
		// Update the server rotation
		serverRotation = position_;
		rotate = true;
	}

	void OnSerializeNetworkView (BitStream stream, NetworkMessageInfo info)
	{
		if (stream.isWriting) {
			//This is executed on the owner of the networkview
			//The owner sends it's rotation over the network
			Vector3 rot = transform.rotation.eulerAngles;
			stream.Serialize (ref rot);//"Encode" it, and send it
			
		} else {
			//Executed on all non-owners
			//receive a rotation and set the object to it
			Vector3 rotReceive = Vector3.zero;
			stream.Serialize (ref rotReceive); //"Decode" it and receive it
			
			//We've just recieved the current servers rotation of this object in 'rotReceive'.

			transform.eulerAngles = new Vector3(rotReceive.x, rotReceive.y, rotReceive.z);

			//To reduce laggy movement a bit you could comment the line above and use position lerping below instead:	
			//transform.position = Vector3.Lerp(transform.position, posReceive, 0.9f); //"lerp" to the posReceive by 90%
			//It would be even better to save the last received server position and lerp to it in Update because it is executed more often than OnSerializeNetworkView
			
		}
	}
}