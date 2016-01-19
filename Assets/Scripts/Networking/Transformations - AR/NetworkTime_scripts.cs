using UnityEngine;
using System.Collections;

public class NetworkTime_scripts : MonoBehaviour
{
	
	//attach to gameSetup object
	public static double time ;
	//this time is the same on EVERYONE.  Network.time is relative to the player. Network.time - info.timestamp WILL work correctly, because the timestamps are adjusted
	
	private NetworkView netView;
	
	//fixes for network.time not being same on all teh clients.  Syncs itself to server's network.time
	void Awake ()
	{
		netView = GetComponent<NetworkView>();
		if (!Network.isServer) {
			//DebugConsole.Message("Asking server for time");
			netView.RPC ("SyncDeltaTime", RPCMode.Server);
		} else {
			time = Network.time;
		}
	}
	
	//keep time updated
	void Update ()
	{
		if (Network.isServer)
			time = Network.time; //truthfully the server can always reference network.time while clients can reference netx.time, but this makes code a bit easier
			//else time += Time.deltaTime;
			else
			time = Network.time + deltaTime; //cause network.time is synched, just starts at different points, so just add the difference
	}
	
	[RPC]
	void SyncDeltaTime (NetworkMessageInfo info)
	{
		double foo = Network.time; //can't rpc doubles
		netView.RPC ("SetDeltaSync", info.sender, foo);
	}
	
	double deltaTime;
	//get the difference in time from local network.time to the server's time.
	[RPC]
	void SetDeltaSync (double newTime, NetworkMessageInfo info)
	{
		deltaTime = (newTime + (Network.time - info.timestamp)) - Network.time;  //make sure we compensate for lag.
		Debug.Log ("Delta from network.time to server's network.time is " + deltaTime);
	}
	
}
