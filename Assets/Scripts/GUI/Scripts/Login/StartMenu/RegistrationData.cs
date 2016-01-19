using UnityEngine;
using System.Collections;

public class RegistrationData : MonoBehaviour {

	public string login = "";
	public string password = "";
	public string passwordConfirm = "";
	public string email = "";
	
	public void clear() {
		login = "";
		password = "";
		passwordConfirm = "";
		email = "";
	}
}
