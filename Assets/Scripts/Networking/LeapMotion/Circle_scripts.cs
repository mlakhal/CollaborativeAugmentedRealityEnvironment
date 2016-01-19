using UnityEngine;
using Leap;

public class Circle_scripts : MonoBehaviour
{

	// Public variables
	public NetworkPlayer owner;
	public float variation = 30.0f;
	public float scale = 1;

	private Controller controller;
	private bool zoom_ = false;

	
	//Last input value, we're saving this to be able to save network messages/bandwidth.
	private Vector3 lastClientScale = Vector3.zero;
	private Vector3 updateVector = Vector3.zero;
	
	//The input values the server will execute on this object
	private Vector3 serverCurrentScale = Vector3.zero;
	
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
	void SetScale (NetworkPlayer player)
	{
		owner = player;
		if (player == Network.player) {
			//Hey thats us! We can control this player: enable this script (this enables Update());
			enabled = true;
		}
	}


	private void Start(){
		controller = new Controller ();
		controller.EnableGesture (Gesture.GestureType.TYPECIRCLE);
		controller.EnableGesture(Gesture.GestureType.TYPEKEYTAP);

		lastClientScale = transform.localScale;
		serverCurrentScale = lastClientScale;

	}

	private void Update(){

		//Client code
		if (Network.player == owner ) {
			string id = networkView.viewID.ToString ();
			int networkID_ = -1; 
			int.TryParse (id.Replace ("AllocatedID: ", ""), out networkID_);		
			bool enable_ = ComboBox_scripts.newNetIDStatic [ComboBox_scripts.selectedItem_].Equals (networkID_);
			// Select only the gameObject from the combo box
			if (enable_) {
				Frame frame = controller.Frame ();
				GestureList gestures = frame.Gestures ();
				for (int i=0; i< gestures.Count; i++) {
					Gesture gesture = gestures [i];
					if (gesture.Type == Gesture.GestureType.TYPECIRCLE ) {
						CircleGesture circle = new CircleGesture (gesture);
						if (circle.Pointable.Direction.AngleTo (circle.Normal) <= Mathf.PI / 2) {
							transform.localScale += new Vector3 (scale, scale, scale);
							networkView.RPC ("SendScale", RPCMode.Server, transform.lossyScale);
						} else {
							transform.localScale -= new Vector3 (scale, scale, scale);
						}
					}
					if (gesture.Type == Gesture.GestureType.TYPEKEYTAP) {
						zoom_ = !zoom_;
					}
				}
					// Verify if we can update the view
					if (Mathf.Abs (lastClientScale.x - transform.localScale.x) > variation
						&& Mathf.Abs (lastClientScale.y - transform.localScale.y) > variation
				    && Mathf.Abs (lastClientScale.z - transform.localScale.z) > variation ) {
						if (Network.isClient) {
							networkView.RPC ("SendScale", RPCMode.Server, transform.lossyScale);
							lastClientScale = transform.lossyScale;
						}
					}
				
					if (Mathf.Abs (lastClientScale.x - updateVector.x) > variation
						&& Mathf.Abs (lastClientScale.y - updateVector.y) > variation
						&& Mathf.Abs (lastClientScale.z - updateVector.z) > variation) {
						if (Network.isClient) {
							string msg = "has done a Scaling on object. Variation : " + transform.localScale.ToString () + ".\nAt : " + System.DateTime.Now;
							GameObject.Find ("logTransform").gameObject.networkView.RPC ("ApplyGlobalLogText", RPCMode.AllBuffered, GUILogTransform_scripts.playerName, msg);
							updateVector = lastClientScale;
						}
					}
			}
		}
		//Server movement code
		if (Network.isServer) {//To also enable this on the client itself, use: "|| Network.player==owner){|"
			//Actually move the player using his/her input
			transform.localScale = serverCurrentScale;
		}


	}

	[RPC]
	public void SendScale (Vector3 position_)
	{
		serverCurrentScale = position_;
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
