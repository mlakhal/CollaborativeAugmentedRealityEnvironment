using UnityEngine;
using System.Collections;

public class getPicker_scripts : MonoBehaviour {

	public HSVPicker pPicker;
	public static HSVPicker picker;

	private void Start(){
		picker = pPicker;
	}

}
