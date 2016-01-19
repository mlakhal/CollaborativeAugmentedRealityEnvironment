using UnityEngine;
using System.Collections;

public class RotateAR_scripts : MonoBehaviour
{


	/* Rotations */
	public static Vector3 clientRotation = Vector3.zero;
	public static Vector3 serverRotation = Vector3.zero;
	public static bool rotate = false;
	public  static float RotationSpeed = 5;

	// Public variables
	public int posOrNegX = 1;
	public int posOrNegY = 1;
	public int posOrNegZ = 1;
	public bool active_ = false;
	// Variables that handle rotation variation
	// i.e: if the delta rotation is greater than something update!
//	private float variation = 1.0f;


	// Update is called once per frame
	void Update ()
	{
		//Client code
		if ((ComboBox_scripts.selectedItem_ == -1 || ComboBox_scripts.selectedItem_ == ComboBox_scripts.lenght - 1) && active_) {
			// X rotation
			if (Input.GetKey (KeyCode.O)) {
				posOrNegX = 1;
				posOrNegY = 0;
				posOrNegZ = 0;
				GetComponent<NetworkView> ().RPC ("SendRotation", RPCMode.Server, posOrNegX, posOrNegY, posOrNegZ);
				return;
			}
			if (Input.GetKey (KeyCode.L)) {
				posOrNegX = -1;
				posOrNegY = 0;
				posOrNegZ = 0;
				GetComponent<NetworkView> ().RPC ("SendRotation", RPCMode.Server, posOrNegX, posOrNegY, posOrNegZ);
				return;
			}
			// Y rotation
			if (Input.GetKey (KeyCode.P)) {
				posOrNegY = 1;
				posOrNegX = 0;
				posOrNegZ = 0;
				GetComponent<NetworkView> ().RPC ("SendRotation", RPCMode.Server, posOrNegX, posOrNegY, posOrNegZ);
				return;
			}
			if (Input.GetKey (KeyCode.M)) {
				posOrNegY = -1;
				posOrNegX = 0;
				posOrNegZ = 0;
				GetComponent<NetworkView> ().RPC ("SendRotation", RPCMode.Server, posOrNegX, posOrNegY, posOrNegZ);
				return;
			}
			// Z rotation
			if (Input.GetKey (KeyCode.I)) {
				posOrNegZ = 1;
				posOrNegX = 0;
				posOrNegY = 0;
				GetComponent<NetworkView> ().RPC ("SendRotation", RPCMode.Server, posOrNegX, posOrNegY, posOrNegZ);
				return;
			}
			if (Input.GetKey (KeyCode.K)) {
				posOrNegZ = -1;
				posOrNegX = 0;
				posOrNegY = 0;
				GetComponent<NetworkView> ().RPC ("SendRotation", RPCMode.Server, posOrNegX, posOrNegY, posOrNegZ);
				return;
			}
			// No player input
			if (!Input.GetKey (KeyCode.M) && !Input.GetKey (KeyCode.P)&& !Input.GetKey (KeyCode.L)&& !Input.GetKey (KeyCode.O)
			    && !Input.GetKey (KeyCode.K)&& !Input.GetKey (KeyCode.I)) {
				posOrNegX = 0;
				posOrNegY = 0;
				posOrNegZ = 0;
			}
		}
	}
	
	[RPC]
	public void SendRotation (int rotValueX, int rotValueY, int rotValueZ)
	{
		GetComponent<NetworkView> ().RPC ("clientRotate", RPCMode.Others, true, rotValueX, rotValueY, rotValueZ);
	}

	[RPC]
	void clientRotate (bool rotate_, int rotValueX, int rotValueY, int rotValueZ)
	{
		rotate = rotate_;
		posOrNegX = rotValueX;
		posOrNegY = rotValueY;
		posOrNegZ = rotValueZ;
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

			transform.eulerAngles = new Vector3 (rotReceive.x, rotReceive.y, rotReceive.z);

			//To reduce laggy movement a bit you could comment the line above and use position lerping below instead:	
			//transform.position = Vector3.Lerp(transform.position, posReceive, 0.9f); //"lerp" to the posReceive by 90%
			//It would be even better to save the last received server position and lerp to it in Update because it is executed more often than OnSerializeNetworkView
			
		}
	}
}