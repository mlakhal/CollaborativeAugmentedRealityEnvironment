// *********************************************************
// UDP SPEECH RECOGNITION
// *********************************************************
using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;

public class UDP_RecoServer : MonoBehaviour
{

	public static string strReceiveUDP = "";

	//public Transform cube;
	Thread receiveThread;
	UdpClient client;
	public int port = 26000; // DEFAULT UDP PORT !!!!! THE QUAKE ONE ;)
	private bool _tPaused = true;
	
	public void Start ()
	{
		
		Application.runInBackground = true;
		init ();  
	}
	// init
	private void init ()
	{
		receiveThread = new Thread (new ThreadStart (ReceiveData));
		receiveThread.IsBackground = true;
		receiveThread.Start ();
	}
	
	private void OnGUI ()
	{
		//--> Button for thread 
		if (GUI.Button (new Rect (150, 180, 60, 30), "Record")) {
			_tPaused = !_tPaused;
		}
	}
	
	private  void ReceiveData ()
	{
		client = new UdpClient (port);
		while (true) {
			if (!_tPaused) {
				try {
					IPEndPoint anyIP = new IPEndPoint (IPAddress.Broadcast, port);
					byte[] data = client.Receive (ref anyIP);
					strReceiveUDP = Encoding.UTF8.GetString (data);
					// ***********************************************************************
					// Simple Debug. Must be replaced with SendMessage for example.
					// ***********************************************************************
					Debug.Log (strReceiveUDP);
					// ***********************************************************************
				} catch (Exception err) {
					print (err.ToString ());
				}						
			} else {
				strReceiveUDP = "";
			}
		}
	}
	
	public string UDPGetPacket ()
	{
		return strReceiveUDP;
	}
	
	void OnDisable ()
	{
		if (receiveThread != null)
			receiveThread.Abort ();
		client.Close ();
	}
}
// *********************************************************
