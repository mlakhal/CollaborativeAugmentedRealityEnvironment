using UnityEngine;
using System.Collections;

public class LoginView : View {

	public static string NAME = "Login";
	
	public GUISkin guiSkin;
	
	public GUIStyle header1Style;
	public GUIStyle header2Style;
	public GUIStyle header2ErrorStyle;
	public GUIStyle formFieldStyle;
	
	public LoginData data = new LoginData();
	
	public bool error = false;
	public string errorMessage = "";
	public string loginURL = "";
	public loginHandler_scripts lh;

	//public var enterGameHandler;
	//public var openRegistrationHandler;
	
	private bool blockUI = false;
	
	public void render() {
		
		int screenWidth = Screen.width;
		int screenHeight = Screen.height;
		
		int xShift = (screenWidth - 260)/2;
		int yShift = (screenHeight - 260)/2;
		
		GUI.skin = guiSkin;
		
		// Disabling UI if blockUI is true: 
		GUI.enabled = !blockUI;
		
		// Main label:
		GUI.Label(new Rect(0, yShift, screenWidth, 30), "Welcome to CARE System", header1Style);
		
		// Message label:
		if(error) {
			GUI.Label(new Rect(0, yShift + 70, screenWidth, 30), errorMessage, header2ErrorStyle);
		} else {
			GUI.Label(new Rect(0, yShift + 70, screenWidth, 30), "Please, fill in all the fields", header2Style);
		}

		// Ip label and ip text field:
		GUI.Label(new Rect(xShift, yShift + 120, 100, 30), "IP:", formFieldStyle);
		data.ip = GUI.TextField(new Rect(xShift + 110, yShift + 120, 150, 30), data.ip, 16);
		
		// Port label and port text field:
		GUI.Label(new Rect(xShift, yShift + 170, 100, 30), "Port:", formFieldStyle);
		data.port = GUI.TextField(new Rect(xShift + 110, yShift + 170, 150, 30), data.port, 16);

		// Login label and login text field:
		GUI.Label(new Rect(xShift, yShift + 220, 100, 30), "Login:", formFieldStyle);
		data.login = GUI.TextField(new Rect(xShift + 110, yShift + 220, 150, 30), data.login, 16);
		
		// Password label and password text field:
		GUI.Label(new Rect(xShift, yShift + 270, 100, 30), "Password:", formFieldStyle);
		data.password = GUI.PasswordField(new Rect(xShift + 110, yShift + 270, 150, 30), data.password, "*"[0], 16);

		lh.ip = data.ip;
		lh.port = data.port;
		lh.login = data.login;
		lh.password = data.password;

		// Login button:
		if(GUI.Button(new Rect(xShift + 30, yShift + 320, 120, 30), "Login")) {
			lh.loginSystem();
		}

		
		// Enabling UI: 
		
		GUI.enabled = true;
		
	}
	
	public void setBlockUI(bool blockUI) {
		this.blockUI = blockUI;
	}
	
}
