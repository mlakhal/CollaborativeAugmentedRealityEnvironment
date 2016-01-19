using UnityEngine;
using System.Collections;

public class SpawnControl : MonoBehaviour {

	public static bool spawnC = false;

	private void Update(){
		if (spawnC) {
			string name = GameObject.Find("guiVertical").GetComponent<Spawn_scripts>().clientName;
			if(name == "Worker-01"){
				GetComponent<Spawn_scripts>().enabled = false;
			}
			if(name == "Worker-02"){
				GetComponent<SpawnWheel>().enabled = false;
			}
			spawnC = false;
		}
	}

}
