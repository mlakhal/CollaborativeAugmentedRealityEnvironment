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

public class UDPVoiceRecognition_scripts : MonoBehaviour
{

	// Public static variables
	public static bool runTh = false;
	public static bool startVoiceRecognition = false;
	public static string strReceiveUDP = "";

	Thread receiveThread;
	UdpClient client;
	public int port = 26000; // DEFAULT UDP PORT !!!!! THE QUAKE ONE ;)
	string LocalIP = String.Empty;
	string hostname;
	
	public void Update ()
	{
		if (runTh) {
			Application.runInBackground = true;
			init ();  
			runTh = false;
		}
	}
	// init
	private void init ()
	{
		receiveThread = new Thread (new ThreadStart (ReceiveData));
		receiveThread.IsBackground = true;
		receiveThread.Start ();
		hostname = Dns.GetHostName ();
		IPAddress[] ips = Dns.GetHostAddresses (hostname);
		if (ips.Length > 0) {
			LocalIP = ips [0].ToString ();
			Debug.Log (" MY IP : " + LocalIP);
		}
	}
	
	private  void ReceiveData ()
	{
		client = new UdpClient (port);
		while (true) {
			if (startVoiceRecognition) {
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
