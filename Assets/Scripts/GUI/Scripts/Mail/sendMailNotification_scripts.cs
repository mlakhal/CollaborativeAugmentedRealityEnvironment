using UnityEngine;
using System.Collections;

public class sendMailNotification_scripts : MonoBehaviour
{
	
	// Public variable
	public Texture2D sendIcon;
	public Texture2D errorIcon;
	public bool show = false;
	public int i = -1;
	
	void OnGUI ()
	{
		if (show)
			switch (i) {
			case 1:
				StartCoroutine (notificationSendCoroutine ());
				break;
			case 2:
				StartCoroutine (notificationErrorCoroutine ());
				break;
			}
	}
	
	IEnumerator notificationSendCoroutine ()
	{
		float timeToWait = 3;
		notification ("Message sent!", sendIcon);
		yield return new WaitForSeconds (timeToWait);
		gameObject.GetComponent<sendMailNotification_scripts> ().enabled = false;
		show = false;
	}

	IEnumerator notificationErrorCoroutine ()
	{
		float timeToWait = 3;
		notification ("Error while sending!", errorIcon);
		yield return new WaitForSeconds (timeToWait);
		gameObject.GetComponent<sendMailNotification_scripts> ().enabled = false;
		show = false;
	}

	void notification (string text, Texture2D actionIcon)
	{
		
		
		GUILayout.BeginArea (new Rect (Screen.width / 2, Screen.height / 2, 170, 200));
		
		GUILayout.BeginHorizontal ();
		
		GUILayout.Label (text);    
		GUILayout.Label (actionIcon, GUILayout.ExpandWidth (true));
		
		GUILayout.EndHorizontal ();    
		
		
		GUILayout.EndArea ();
	}
}
