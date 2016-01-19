using UnityEngine;
using System.Collections;

public class CanvasGUI_manager : MonoBehaviour {

	void OnPlayerConnected(NetworkPlayer newPlayer)
	{
		GetComponent<NetworkView>().RPC ("setCanvas", newPlayer, null);
	}

	[RPC]
	private void setCanvas(){
		Canvas_manager.show_ = true;
	}
}
