using UnityEngine;
using System.Collections;

public class ScaleAR_scripts : MonoBehaviour
{
	
	// Public variables
	public NetworkPlayer owner;
	public bool active_ = false;
	public float scale = 0.1F;

	// Private variables
	private bool zoom = false;

	// Variables that handle rotation variation
	// i.e: if the delta rotation is greater than something update!
	private float variation = 1.0f;
	
	
	//Last input value, we're saving this to be able to save network messages/bandwidth.
	private Vector3 lastClientScale = Vector3.zero;
	
	//The input values the server will execute on this object
	public Vector3 serverCurrentScale = Vector3.zero;
	
	private void Start ()
	{
		lastClientScale = transform.localScale;
		serverCurrentScale = lastClientScale;
	}
	
	// Update is called once per frame
	private void Update ()
	{
		//Client code
		if ((ComboBox_scripts.selectedItem_ == -1 || ComboBox_scripts.selectedItem_ == ComboBox_scripts.lenght - 1) && active_) {
			// If we press "+" zoom in, "-" shrink the object
			// Widen the object by 0.1
			if (Input.GetKey (KeyCode.KeypadPlus)) {
				transform.localScale += new Vector3 (scale, scale, scale);
			}
			if (Input.GetKey (KeyCode.KeypadMinus)) {
				transform.localScale -= new Vector3 (scale, scale, scale);
			}
			GetComponent<NetworkView> ().RPC ("SendScale", RPCMode.Server, transform.localScale);
		}
	}
	
	[RPC]
	public void SendScale (Vector3 position_)
	{
		GetComponent<NetworkView> ().RPC ("clientScale", RPCMode.Others, position_);
	}

	[RPC]
	void clientScale (Vector3 position)
	{
		serverCurrentScale = position;
	}

	void OnSerializeNetworkView (BitStream stream, NetworkMessageInfo info)
	{
		if (stream.isWriting) {
			//This is executed on the owner of the networkview
			//The owner sends it's scale over the network
			Vector3 scale = transform.lossyScale;
			stream.Serialize (ref scale);//"Encode" it, and send it
			
		} else {
			//Executed on all non-owners
			//receive a scale and set the object to it
			Vector3 scaleReceive = Vector3.zero;
			stream.Serialize (ref scaleReceive); //"Decode" it and receive it
			
			//We've just recieved the current servers position of this object in 'posReceive'.
			
			transform.localScale = scaleReceive;
			//To reduce laggy movement a bit you could comment the line above and use position lerping below instead:	
			//transform.position = Vector3.Lerp(transform.position, posReceive, 0.9f); //"lerp" to the posReceive by 90%
			//It would be even better to save the last received server position and lerp to it in Update because it is executed more often than OnSerializeNetworkView
			
		}
	}
}
