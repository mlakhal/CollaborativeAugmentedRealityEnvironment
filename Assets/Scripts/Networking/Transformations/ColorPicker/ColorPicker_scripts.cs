using UnityEngine;
using System.Collections;

public class ColorPicker_scripts : MonoBehaviour
{
	public bool en_ = false;

	// Public variables
	public NetworkPlayer owner;
	
	// Private variables
	private Color clientColor ;
	private Color serverColor;

	private string lastColor = "";
	private string currentColor = "";

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
	void SetColor (NetworkPlayer player)
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
		if (Network.player == owner && ComboBox_scripts.selectedItem_ != -1 && ComboBox_scripts.selectedItem_ != ComboBox_scripts.lenght - 1) {
			string id = GetComponent<NetworkView>().viewID.ToString ();
			int networkID_ = -1; 
			int.TryParse (id.Replace ("AllocatedID: ", ""), out networkID_);		
			bool enable_ = ComboBox_scripts.newNetIDStatic [ComboBox_scripts.selectedItem_].Equals (networkID_);
			// Select only the gameObject from the combo box
			if (en_) {
				switch (UDPVoiceRecognition_scripts.strReceiveUDP) {
				case "blue":
					clientColor = Color.blue;
					break;
				case "green":
					clientColor = Color.green;
					break;
				case "red":
					clientColor = Color.red;
					break;
				case "black":
					clientColor = Color.black;
					break;
				case "yellow":
					clientColor = Color.yellow;
					break;
				}
			
				//		clientColor = picker.currentColor;
				if (Network.isServer) {
					//Too bad a server can't send an rpc to itself using "RPCMode.Server"!
					//This is a Unity "feature", see `Tips`
					Vector3 color_ = new Vector3 (clientColor.r, clientColor.g, clientColor.b);
					SendClorInput (color_);
				} else if (Network.isClient) {
					Vector3 color_ = new Vector3 (clientColor.r, clientColor.g, clientColor.b);
					SendClorInput (color_); //Use this (and line 64) for simple "prediction"
					GetComponent<NetworkView>().RPC ("SendClorInput", RPCMode.Server, color_);

					// Verify if we can update the Log view
					currentColor= UDPVoiceRecognition_scripts.strReceiveUDP;
					if(currentColor != lastColor){
						lastColor = currentColor;
						string msg = "has change the color of the object to : "+currentColor+".\nAt : "+System.DateTime.Now;
						GameObject.Find("logTransform").gameObject.GetComponent<NetworkView>().RPC("ApplyGlobalLogText", RPCMode.AllBuffered, GUILogTransform_scripts.playerName, msg);
					}

				}
				
			}
		}
		
		//Server movement code
		if (Network.isServer || Network.player == owner) {//To also enable this on the client itself, use: "|| Network.player==owner){|"
			GetComponent<Renderer>().material.color = serverColor;
		}
		
	}
	
	[RPC]
	void SendClorInput (Vector3 colorInput)
	{
		//Called on the server
		Color color_ = new Color (colorInput.x, colorInput.y, colorInput.z);
		serverColor = color_;
	}
	
	void OnSerializeNetworkView (BitStream stream, NetworkMessageInfo info)
	{
		if (stream.isWriting) {
			//This is executed on the owner of the networkview
			//The owner sends it's color over the network
			
			//send color
			Vector3 tempcolor = new Vector3 (gameObject.GetComponent<Renderer>().material.color.r, gameObject.GetComponent<Renderer>().material.color.g, gameObject.GetComponent<Renderer>().material.color.b);
			
			stream.Serialize (ref tempcolor);//"Encode" it, and send it
			
		} else {
			//Executed on all non-owners
			//receive a color and set the object to it
			
			//get color
			Vector3 tempcolor = Vector3.zero;
			Color colorReceive = Color.black;
			stream.Serialize (ref tempcolor);
			
			colorReceive = new Color (tempcolor.x, tempcolor.y, tempcolor.z, 1.0f);
			
			//We've just recieved the current servers position of this object in 'posReceive'.
			
			GetComponent<Renderer>().material.color = colorReceive;
			//To reduce laggy movement a bit you could comment the line above and use position lerping below instead:	
			//transform.position = Vector3.Lerp(transform.position, posReceive, 0.9f); //"lerp" to the posReceive by 90%
			//It would be even better to save the last received server position and lerp to it in Update because it is executed more often than OnSerializeNetworkView
			
		}
	}
}