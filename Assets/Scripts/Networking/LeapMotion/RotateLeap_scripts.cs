using UnityEngine;
using Leap;

public class RotateLeap_scripts : MonoBehaviour
{
	public NetworkPlayer owner;
	private float speed = 5;
	private Vector3 serverRotation = Vector3.zero;
	private bool rotate = false;

	/// <summary>
	/// The Leap controller.
	/// </summary>
	private Controller controller;
	
	/// <summary>
	/// The current frame captured by the Leap Motion.
	/// </summary>
	Frame CurrentFrame
	{
		get { return ( IsReady ) ? controller.Frame() : null; }
	}
	
	/// <summary>
	/// Gets the hands data captured from the Leap Motion.
	/// </summary>
	/// <value>
	/// The hands data captured from the Leap Motion.
	/// </value>
	HandList Hands
	{
		get { return ( CurrentFrame != null && CurrentFrame.Hands.Count > 0 ) ? CurrentFrame.Hands : null; }
	}
	
	/// <summary>
	/// Gets a value indicating whether the Leap Motion is ready.
	/// </summary>
	/// <value>
	///   <c>true</c> if this instance is ready; otherwise, <c>false</c>.
	/// </value>
	bool IsReady
	{
		get { return ( controller != null && controller.Devices.Count > 0 && controller.IsConnected ); }
	}
	
	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>

	void Awake ()
	{
		controller = new Controller();
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
	
	/// <summary>
	/// Update function called every frame.
	/// </summary>
	void Update()
	{
		Hand mainHand; // The front most hand captured by the Leap Motion Controller
		
		// Check if the Leap Motion Controller is ready
		if ( !IsReady || Hands == null )
		{
			return;
		}
		
		mainHand = Hands.Frontmost;

		Vector3 rotation = new Vector3(mainHand.Direction.Pitch, mainHand.Direction.Yaw, mainHand.PalmNormal.Roll );
		networkView.RPC ("SendRotation", RPCMode.Server, rotation);

		if (rotate && Network.isClient) {
			transform.rotation = Quaternion.Euler (speed * serverRotation.x, speed * serverRotation.y, speed * serverRotation.z);
			rotate = false;
		}
		// For relative orientation
		// transform.rotation *= Quaternion.Euler( mainHand.Direction.Pitch, mainHand.Direction.Yaw, mainHand.PalmNormal.Roll );
		
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