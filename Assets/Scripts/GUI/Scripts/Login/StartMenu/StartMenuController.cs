using UnityEngine;
using System.Collections;

public class StartMenuController : MonoBehaviour {

	// Common GUI skin:
	public GUISkin guiSkin;
	
	// GUI styles for labels:
	public GUIStyle header1Style;
	public GUIStyle header2Style;
	public GUIStyle header2ErrorStyle;
	public GUIStyle formFieldStyle;
	public GUIStyle errorMessageStyle;
	
	// Active view name:
	string activeViewName = LoginView.NAME;
	
	// Map views by name:
	Hashtable viewByName;
	
	// Login view:
	LoginView loginView;
	
	// Registration view:
	RegistrationView registrationView;
	
	
	// Do we need block UI:
	bool blockUI = false;


	// This function will be called when scene loaded:
	void Awake () {   
		loginView = new LoginView();
		// Setup of login view:
		loginView.guiSkin = guiSkin;
		loginView.header1Style = header1Style;
		loginView.header2Style = header2Style;
		loginView.header2ErrorStyle = header2ErrorStyle;
		loginView.formFieldStyle = formFieldStyle;
		
		// Handler of registration button click:
		//loginView.openRegistrationHandler = function() {
			// Clear reistration fields:
		//	registrationView.data.clear();
			// Set active view to registration:
		//	activeViewName = RegistrationView.NAME;
		//};
		
		// Setup of login view:
/*		registrationView.guiSkin = guiSkin;
		registrationView.header2Style = header2Style;
		registrationView.formFieldStyle = formFieldStyle;
		registrationView.errorMessageStyle = errorMessageStyle;
*/		
		// Handler of cancel button click:
		/*registrationView.cancelHandler = function() {
			// Clear reistration fields:
			loginView.data.clear();
			// Set active view to registration:
			activeViewName = LoginView.NAME;
		};*/
		
		viewByName = new Hashtable();
		
		// Adding login view to views by name map:
		viewByName.Add(LoginView.NAME, loginView);
		//viewByName[RegistrationView.NAME] = registrationView;
	}
	
	// This function will draw UI components
	void OnGUI () {
		// Getting current view by active view name:
		LoginView currentView = (LoginView) viewByName[activeViewName];
		currentView.lh =  gameObject.GetComponent<loginHandler_scripts> ();

		// Set blockUI for current view:
		currentView.setBlockUI(blockUI);
		
		// Rendering current view:
		currentView.render();

		// Show box with "Wait..." when UI is blocked:
		var screenWidth = Screen.width;
		var screenHeight = Screen.height;
		if(blockUI) 
			GUI.Box(new Rect((screenWidth - 200)/2, (screenHeight - 60)/2, 200, 60), "Wait...");
		
	}

}
