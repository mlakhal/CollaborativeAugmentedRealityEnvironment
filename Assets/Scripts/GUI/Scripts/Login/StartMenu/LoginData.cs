using UnityEngine;
using System.Collections;

public class LoginData : MonoBehaviour {

	public string ip = "";
	public string port = "";
	public string login = "";
	public string password = "";
	
	public void clear() {
		ip = "";
		port = "";
		login = "";
		password = "";
	}
}
