using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Remove_scripts : MonoBehaviour {

	// Public static variables
	public static bool remove = false;

	// Private variables
	private NetworkPlayer owner;

	void Update(){
		owner= gameObject.GetComponent<Trigger_scripts>().owner;
		//Client code
		if (Network.player == owner && remove) {
			string id = GetComponent<NetworkView>().viewID.ToString ();
			int networkID_ = -1; 
			int.TryParse (id.Replace ("AllocatedID: ", ""), out networkID_);		
			bool enable_ = ComboBox_scripts.newNetIDStatic [ComboBox_scripts.selectedItem_].Equals (networkID_);
			// Select only the gameObject from the combo box
			if (enable_) {
				GameObject.Find("guiVertical").GetComponent<Spawn_scripts>().removeObject(Network.player);
				GameObject.Find("comboBox").GetComponent<ComboBox_scripts>().items = new string[0];
				// Reinitialize the players array
				GameObject.Find("guiVertical").GetComponent<Spawn_scripts>().playerScripts = new List<Trigger_scripts> (); 
				GameObject.Find("guiVertical").GetComponent<Spawn_scripts>().scaleScripts = new List<Circle_scripts> ();
				//GameObject.Find("guiVertical").GetComponent<Spawn_scripts>().rotateScripts = new List<Rotate_scripts> ();
				remove = false;
			}
		}// End of client code responsible to remove gameOject on the network
	}
}
