using UnityEngine;
using System.Collections;

public class ComboBox_scripts : MonoBehaviour
{
	
	// Public static variables
	public static int selectedItem_ = -1;
	public static int[] newNetIDStatic;
	public static int newNetID = -1;
	public static string newSpawnObject = "";
	public static bool spawn = false;
	public static int lenght = -1;

	// Public variables
	public int[] netIDs;
	public string[] items;
	public Rect Box;
	public string selectedItem = "None";
	public string clientType = "";

	// Private variables
	private bool editing = false;
	private int cpt = 0;

	private void Start(){
		Box.x = Screen.width - 205;
	}

	private void Update ()
	{
		lenght = items.Length;
		// If we have just Spawn new gameObject!
		if (spawn && clientType == "worker") {
			//networkView.RPC("spawnObject", RPCMode.Others, null);
			string[] oldItems = (string[])items.Clone ();
			items = new string[oldItems.Length + 1];
			
			for (int i=0; i< items.Length - 2; i++) {
				items [i] = oldItems [i];
			}
			items [items.Length - 2] = newSpawnObject;
			items [items.Length - 1] = "AR-car";
			/////////////////////////////////////////
			
			int[] oldNetView = (int[])netIDs.Clone ();
			netIDs = new int[netIDs.Length + 1];
			
			for (int i=0; i< netIDs.Length - 1; i++) {
				netIDs [i] = oldNetView [i];
			}
			netIDs [netIDs.Length - 1] = newNetID;
			newNetIDStatic = (int[])netIDs.Clone ();
			/////////////////////////////////////////
			spawn = false;
			cpt++;
		}
	}

	private void OnGUI ()
	{
		if (cameraBehaviour_scripts.webSelected) {
			if (clientType == "worker") {
				if (GUI.Button (Box, selectedItem)) {
					editing = true;
				}
				
				if (editing) {
					for (int x = 0; x < items.Length; x++) {
						if (GUI.Button (new Rect (Box.x, (Box.height * x) + Box.y + Box.height, Box.width, Box.height), items [x])) {
							selectedItem = items [x];
							selectedItem_ = x;
							editing = false;
						}
					}
				}
			}
		}
	}
	
}
