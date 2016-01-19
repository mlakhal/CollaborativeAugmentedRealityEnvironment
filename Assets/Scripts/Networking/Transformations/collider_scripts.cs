using UnityEngine;
using System.Collections;

public class collider_scripts : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		Destroy(other.gameObject);
	}
}
