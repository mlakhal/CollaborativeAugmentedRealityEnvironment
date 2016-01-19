using UnityEngine;
using System.Collections;

public class handPosition_scripts : MonoBehaviour {

	public static Vector3 handPosition = Vector3.zero;

	private void Update(){
		handPosition = GetComponent<SkeletalHand>().palm.transform.position;
	}
}
