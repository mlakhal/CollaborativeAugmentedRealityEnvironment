using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.IO;

public class Synchronize_scripts : MonoBehaviour
{

	public int index;
	Vector3 v_ = Vector3.zero;
	private double time_ = 0.000002;
	private StreamWriter sr;
	private StreamReader sre ;


	// Synchronization 
	private int maxConnection = 16;
	private int index_tab = 0;
	private double[] time;
	private Vector3[] vectors;
	private List<string> listIp = null;
	List<int> index_ = new List<int> ();
	float x = 0, z = 0;
	Vector3 ve = Vector3.zero;
	int n;
	
	private void init_tab ()
	{
		time = new double[maxConnection];
		vectors = new Vector3[maxConnection];
		
		for (int i=0; i< maxConnection; i++) {
			time [i] = 0;
			vectors [i] = Vector3.zero;
		}
	}
	
	void Awake ()
	{
		init_tab ();
	}
	
	[RPC]
	public void getIndex (NetworkPlayer np)
	{
		index = Connect_scripts.playerListIP.IndexOf (np.ipAddress);
	}
	
	void Update ()
	{
		if (Connect_scripts.playerListIP != null) {
			if (index_tab < maxConnection)
				index_tab = Connect_scripts.playerListIP.Count;
			else
				index_tab = -1;

			listIp = Connect_scripts.playerListIP;
		}

	}
	
	[RPC]
	void init_index (float HInput, float VInput)
	{
		Vector3 v = new Vector3 (HInput, 0, VInput);
		vectors [index] = v;
		time [index] = NetworkTime_scripts.time  / 1000000;
	}
	
	[RPC]
	void Synchronized_algorithm ()
	{
		n = index_.Count;
		x = 0;
		z = 0;
		foreach (int i in index_) {
			ve = vectors [i];
			x += ve.x;
			z += ve.z;
		}
		
		x /= n;
		z /= n;
		v_.x = x;
		v_.z = z;
		
		//Called on the server
		GetComponent<NetworkView>().RPC ("SendMovementInput", RPCMode.Server, v_.x, v_.z);

		// Update the Log GUI if two player or more manipulate the same object at delta time
		if (n > 1) {
			string msg = "Synchronization between '" + n + "' worker's input.\nRunning the algorithm...\nFinished at:" + System.DateTime.Now;
			GameObject.Find ("logTransform").gameObject.GetComponent<NetworkView> ().RPC ("ApplyGlobalLogText", RPCMode.AllBuffered, GUILogTransform_scripts.playerName, msg);
		}

	}
	
	[RPC]
	private void near_time ()
	{
		Debug.LogWarning ("Index : "+index);
		index_.Clear();
		for (int i=0; i<index_tab; i++) {
			Debug.LogWarning ("time ["+i+"]: "+time [i]);
			if (i != index && ((time [index] - time [i] < time_) || (time [i] - time [index] > -time_))){
				index_.Add (i);
			}
		}
		index_.Add (index);
	}

	
}
