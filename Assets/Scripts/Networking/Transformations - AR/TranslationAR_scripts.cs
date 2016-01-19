using UnityEngine;
using System.Collections;
using NyARUnityUtils;

public class TranslationAR_scripts : MonoBehaviour
{
	/*  This file is part of the "Ultimate Unity networking project" by M2H (http://www.M2H.nl)
     *  This project is available on the Unity Store. You are only allowed to use these
     *  resources if you've bought them from the Unity Assets Store.
     */
	
	public static bool reload = false;

	// Public variables
	public bool active_ = false;
	public Transform object_ = null;
	public float speed = 100;
	//Last input value, we're saving this to be able to save network messages/bandwidth.
	private float lastClientHInput = 0;
	private float lastClientVInput = 0;
	
	//The input values the server will execute on this object
	public float serverCurrentHInput = 0;
	public float serverCurrentVInput = 0;
	public float serverCurrentYInput = 0;
	
	
	void Update ()
	{
		if ((ComboBox_scripts.selectedItem_ == -1 || ComboBox_scripts.selectedItem_ == ComboBox_scripts.lenght - 1) && active_ ){
			float HInput = Input.GetAxis ("Horizontal");
			float VInput = Input.GetAxis ("Vertical");
			float Y = Input.GetAxis("Mouse Y");
			
			//Is our input different? Do we need to update the server?
			if (lastClientHInput != HInput || lastClientVInput != VInput) {
				lastClientHInput = HInput;
				lastClientVInput = VInput;
				
				if (Network.isClient) {
					
					//SendMovementInput (HInput, VInput); //Use this (and line 64) for simple "prediction"
					GetComponent<NetworkView>().RPC ("SendMovementInput", RPCMode.Server, HInput, VInput, Y);

					// Synchronization 
					GetComponent<NetworkView>().RPC ("getIndex", RPCMode.Server, Network.player);
					GetComponent<NetworkView>().RPC ("init_index", RPCMode.Server, HInput, VInput);
					GetComponent<NetworkView>().RPC ("near_time", RPCMode.Server, null);
					// Run the algorithms 
					GetComponent<NetworkView>().RPC ("Synchronized_algorithm", RPCMode.Server, null);
				}
				
			}
		}
		
	}

	[RPC]
	void reloadObject(bool reload_){
		reload = reload_;
		GetComponent<NetworkView>().RPC ("clientReload", RPCMode.Others, reload_);
	}

	[RPC]
    void clientReload(bool reload_){
		reload = reload_;
	}

	[RPC]
	void SendMovementInput (float HInput, float VInput, float Y)
	{
		//Called on the server
		serverCurrentHInput = HInput;
		serverCurrentVInput = VInput;
		serverCurrentYInput = Y;

		GetComponent<NetworkView>().RPC ("sendClientInput", RPCMode.Others, HInput, VInput, Y);
	}

	[RPC]
	void sendClientInput(float HInput, float VInput, float Y){
		serverCurrentHInput = HInput;
		serverCurrentVInput = VInput;
		serverCurrentYInput = Y;
	}

	void OnSerializeNetworkView (BitStream stream, NetworkMessageInfo info)
	{
		if (stream.isWriting) {
			//This is executed on the owner of the networkview
			//The owner sends it's position over the network
			//Vector3 pos = GameObject.Find ("Audi").transform.position;
			Vector3 pos = transform.position;
			stream.Serialize (ref pos);//"Encode" it, and send it
			
		} else {
			//Executed on all non-owners
			//receive a position and set the object to it
			Vector3 posReceive = Vector3.zero;
			stream.Serialize (ref posReceive); //"Decode" it and receive it
			
			//We've just recieved the current servers position of this object in 'posReceive'.
			transform.position = posReceive;

		}
	}
}