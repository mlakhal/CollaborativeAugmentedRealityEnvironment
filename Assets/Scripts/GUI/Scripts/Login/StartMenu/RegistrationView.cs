using UnityEngine;
using System.Collections;

public class RegistrationView : View {

	public static string NAME = "Registration";
	
	public GUISkin guiSkin;
	
	public GUIStyle header2Style;
	public GUIStyle formFieldStyle;
	public GUIStyle errorMessageStyle;
	
	public bool error = false;
	public string errorMessage = "";
	
	public RegistrationData data = new RegistrationData();
	
	//public var registrationHandler;
	//public var cancelHandler;
	
	private bool blockUI = false;
	
	public void setBlockUI(bool blockUI){
		this.blockUI = blockUI;
	}
	
	public void render() { 
		
		var screenWidth = Screen.width;
		var screenHeight = Screen.height;
		
		var xShift = (screenWidth - 360)/2;
		var yShift = (screenHeight - 300)/2;
		
		GUI.skin = guiSkin;
		
		// Disable UI in case of blockUI is true or any error:
		if(error || blockUI){
			GUI.enabled = false;
		} else {
			GUI.enabled = true;
		}
		
		// Message label:
		GUI.Label(new Rect(0, yShift + 0, screenWidth, 30), "Enter Registration Data", header2Style);
		
		// Login label and text filed:
		GUI.Label(new Rect(xShift, yShift + 50, 100, 30), "Login:", formFieldStyle);
		data.login = GUI.TextField(new Rect(xShift + 110, yShift + 50, 250, 30), data.login, 16);
		
		// Password label and text filed:
		GUI.Label(new Rect(xShift, yShift + 100, 100, 30), "Password:", formFieldStyle);
		data.password = GUI.PasswordField(new Rect(xShift + 110, yShift + 100, 250, 30), data.password, "*"[0], 16);
		
		// Confirm password label and text filed:
		GUI.Label(new Rect(xShift - 50, yShift + 150, 150, 30), "Confirm Password:", formFieldStyle);
		data.passwordConfirm = GUI.PasswordField(new Rect(xShift + 110, yShift + 150, 250, 30), data.passwordConfirm, "*"[0], 16);
		
		// Email label and text filed::
		GUI.Label(new Rect(xShift, yShift + 200, 100, 30), "Your Email:", formFieldStyle);
		data.email = GUI.TextField(new Rect(xShift + 110, yShift + 200, 250, 30), data.email, 32);
		
		// Register button:
		if(GUI.Button(new Rect(xShift + 50, yShift + 250, 120, 30), "Register")) {
		//	registrationHandler();
		}
		
		// Cancel button:
		if(GUI.Button(new Rect(xShift + 190, yShift + 250, 120, 30), "Cancel")) {
			//cancelHandler();
		}
		
		// Enabling UI:
		GUI.enabled = true;
		
		// Show errors:
		showErrors();
	}
	
	
	// In case of registration error render error window:
	private void showErrors() {
		if(error) {
			//int screenWidth = Screen.width;
			//int screenHeight = Screen.height;
			//windowRect = GUI.Window (0, Rect((screenWidth - 400)/2, (screenHeight - 300)/2, 400, 300), 
			  //                       renderErrorWindow, "Registration Error");
		}
	}
	
	// Render error window content:
	private void renderErrorWindow(int windowId) {
		GUI.Label(new Rect(10, 30, 380, 230), errorMessage, errorMessageStyle);
		if(GUI.Button(new Rect((400 - 120)/2, 260, 120, 30), "OK")) {
			error = false;
			errorMessage = "";
		}
	}
}
