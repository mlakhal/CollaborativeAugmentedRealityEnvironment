using UnityEngine;
using System.Collections;

public class color__ : MonoBehaviour {

	// Public variables
	public NetworkPlayer owner;
	public static bool enable__ = false;

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
	void SetColor_ (NetworkPlayer player)
	{
		owner = player;
		if (player == Network.player) {
			//Hey thats us! We can control this player: enable this script (this enables Update());
			enabled = true;
		}
	}

	private void Update (){
			
			//Client code
			if (Network.player == owner && ComboBox_scripts.selectedItem_ != -1 && ComboBox_scripts.selectedItem_ != ComboBox_scripts.lenght - 1) {
				string id = GetComponent<NetworkView>().viewID.ToString ();
				int networkID_ = -1; 
				int.TryParse (id.Replace ("AllocatedID: ", ""), out networkID_);		
				bool enable_ = ComboBox_scripts.newNetIDStatic [ComboBox_scripts.selectedItem_].Equals (networkID_);
				// Select only the gameObject from the combo box
				if (enable_) {
				GameObject.Find("Pistons_MeshPart0").GetComponent<ColorPicker_scripts>().en_ = true;
				}else{
				GameObject.Find("Pistons_MeshPart0").GetComponent<ColorPicker_scripts>().en_ = false;
			}
	}

}
}