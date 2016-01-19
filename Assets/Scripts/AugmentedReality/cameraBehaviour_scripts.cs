using UnityEngine;
using System;
using System.Collections;
using jp.nyatla.nyartoolkit.cs.markersystem;
using jp.nyatla.nyartoolkit.cs.core;
using NyARUnityUtils;
using System.IO;

/// <summary>
/// AR camera behaviour.
/// This sample shows simpleLite demo.
/// 1.Connect webcam to your computer.
/// 2.Start sample program
/// 3.Take a "HIRO" marker on capture image
/// 
/// </summary>
public class cameraBehaviour_scripts : MonoBehaviour
{
	// Public static variables
	public static bool webSelected = false;
	public NyARUnityMarkerSystem _ms;
	public NyARUnityWebCam _ss;
	public int mid;//marker id
	public float speed = 20;
	private GameObject _bg_panel;
	private bool show = true;
		/*** Cars Transformation variables ***/
	// Lamburghini
		// Scale
	private Vector3 scaleL_ = Vector3.zero;
		// Translation
	private float aH = 0;
	private float aV = 0;
		// Rotation
	private float rlX = 0;
	private float rlY = 0;
	private float rlZ = 0;
	// Bugati
		// Scale
	private Vector3 scaleB_ = Vector3.zero;
		// Translation
	private float aHb = 0;
	private float aVb = 0;
	private float aYb = 0;
		// Rotation
	private float rbX = 0;
	private float rbY = 0;
	private float rbZ = 0;
	// Mustung
		// Scale
	private Vector3 scaleM_ = Vector3.zero;
		// Translation
	private float aHm = 0;
	private float aVm = 0;
		// Rotation
	private float rmX = 0;
	private float rmY = 0;
	private float rmZ = 0;

	void OnGUI ()
	{
		if (show)
			GUILayout.Window (1, new Rect (10, 10, Screen.width - 20, Screen.height - 20), Window, "", GUIStyle.none);
	}
	
	void Window (int id)
	{
		GUI.Box (new Rect (0, 0, Screen.width - 20, Screen.height - 20), "");

		GUILayout.Label ("Select webcam to start recording");
		WebCamDevice[] devices = WebCamTexture.devices;
		for (int i=0; i< devices.Length; i++) {
			if (GUILayout.Button (devices [i].name)) {
				WebCamTexture w = new WebCamTexture (devices [i].name, 320, 240, 15);
				//Make WebcamTexture wrapped Sensor.
				this._ss = NyARUnityWebCam.createInstance (w);
				//Make configulation by Sensor size.
				NyARMarkerSystemConfig config = new NyARMarkerSystemConfig (this._ss.width, this._ss.height);
				
				this._ms = new NyARUnityMarkerSystem (config);
				//mid=this._ms.addARMarker("./Assets/Data/patt.hiro",16,25,80);
				//This line loads a marker from texture
				mid = this._ms.addARMarker ((Texture2D)(Resources.Load ("MarkerHiro", typeof(Texture2D))), 16, 25, 80);
				
				//setup background
				this._bg_panel = GameObject.Find ("Plane");
				this._bg_panel.GetComponent<Renderer> ().material.mainTexture = w;
				this._ms.setARBackgroundTransform (this._bg_panel.transform);
				
				//setup camera projection
				this._ms.setARCameraProjection (this.GetComponent<Camera> ());
				//start sensor
				this._ss.start ();
				// Begin recording on the screen
				webSelected = true;
				show = false;
				return;
			}
		}
		
	}

	// Update is called once per frame
	void Update ()
	{
		if (webSelected) {
			// Set camera rotation
			GameObject.Find ("cameraClient").transform.Rotate (Vector3.zero);
			//Update SensourSystem
			this._ss.update ();
			//Update marker system by ss
			this._ms.update (this._ss);
			//update Gameobject transform
			if (this._ms.isExistMarker (mid)) {

				int arCombo = ARComboBox_scripts.selectedItem_;

				switch (arCombo) {
				case 0:
					this._ms.setMarkerTransform (mid, GameObject.Find ("Bugatti-Veyron").transform);
					if (TranslationAR_scripts.reload) {
						GameObject.Find ("Bugatti-Veyron").transform.localPosition = Vector3.zero;
						GameObject.Find ("Bugatti-Veyron").GetComponent<NetworkView> ().RPC ("reloadObject", RPCMode.Server, false);
						aHb = 0;
						aVb = 0;
					} else {
						aHb += GameObject.Find ("Bugatti-Veyron").gameObject.GetComponent<TranslationAR_scripts> ().serverCurrentHInput;
						aVb += GameObject.Find ("Bugatti-Veyron").gameObject.GetComponent<TranslationAR_scripts> ().serverCurrentVInput;
						aYb += GameObject.Find ("Bugatti-Veyron").gameObject.GetComponent<TranslationAR_scripts> ().serverCurrentYInput;
						GameObject.Find ("Bugatti-Veyron").transform.Translate (speed * new Vector3 (-aHb,-aVb , -aYb));
					}

					// Rotation of the car
					Vector3 oldRotation = GameObject.Find ("Bugatti-Veyron").transform.eulerAngles;
					int posNegXB = GameObject.Find ("Bugatti-Veyron").GetComponent<RotateAR_scripts>().posOrNegX;
					int posNegYB = GameObject.Find ("Bugatti-Veyron").GetComponent<RotateAR_scripts>().posOrNegY;
					int posNegZB = GameObject.Find ("Bugatti-Veyron").GetComponent<RotateAR_scripts>().posOrNegZ;
					rbX += RotateAR_scripts.RotationSpeed * posNegXB;
					rbY += RotateAR_scripts.RotationSpeed * posNegYB;
					rbZ += RotateAR_scripts.RotationSpeed * posNegZB;
					Vector3 rotation_ = new Vector3 (oldRotation.x + rbX, oldRotation.y + rbY, oldRotation.z + rbZ);
					GameObject.Find ("Bugatti-Veyron").transform.eulerAngles = rotation_;//rotates coin on Y axis
					RotateAR_scripts.rotate = false;

					// Scaling
					scaleB_ = GameObject.Find ("Bugatti-Veyron").GetComponent<ScaleAR_scripts>().serverCurrentScale;
					GameObject.Find ("Bugatti-Veyron").transform.localScale = scaleB_;

					break;
				case 1:
					this._ms.setMarkerTransform (mid, GameObject.Find ("lamborghini").transform);
					if (TranslationAR_scripts.reload) {
						GameObject.Find ("lamborghini").transform.localPosition = Vector3.zero;
						GameObject.Find ("lamborghini").GetComponent<NetworkView> ().RPC ("reloadObject", RPCMode.Server, false);
						aH = 0;
						aV = 0;
					} else {
						aH += GameObject.Find ("lamborghini").gameObject.GetComponent<TranslationAR_scripts> ().serverCurrentHInput;
						aV += GameObject.Find ("lamborghini").gameObject.GetComponent<TranslationAR_scripts> ().serverCurrentVInput;
						GameObject.Find ("lamborghini").transform.Translate (speed * new Vector3 (-aH, 0, -aV));
					}

					// Rotation of the car
					Vector3 oldRotationLum = GameObject.Find ("lamborghini").transform.eulerAngles;
					int posNegXL = GameObject.Find ("lamborghini").GetComponent<RotateAR_scripts>().posOrNegX;
					int posNegYL = GameObject.Find ("lamborghini").GetComponent<RotateAR_scripts>().posOrNegY;
					int posNegZL = GameObject.Find ("lamborghini").GetComponent<RotateAR_scripts>().posOrNegZ;
					rlX += RotateAR_scripts.RotationSpeed * posNegXL;
					rlY += RotateAR_scripts.RotationSpeed * posNegYL;
					rlZ += RotateAR_scripts.RotationSpeed * posNegZL;
					Vector3 rotationLum_ = new Vector3 (oldRotationLum.x + rlX, oldRotationLum.y + rlY, oldRotationLum.z + rlZ);
					GameObject.Find ("lamborghini").transform.eulerAngles = rotationLum_;//rotates coin on Y axis
					RotateAR_scripts.rotate = false;

					// Scaling
					scaleL_ = GameObject.Find ("lamborghini").GetComponent<ScaleAR_scripts>().serverCurrentScale;
					GameObject.Find ("lamborghini").transform.localScale = scaleL_;

					break;
				case 2:
					this._ms.setMarkerTransform (mid, GameObject.Find ("Car").transform);
					if (TranslationAR_scripts.reload) {
						GameObject.Find ("Car").transform.localPosition = Vector3.zero;
						GameObject.Find ("Car").GetComponent<NetworkView> ().RPC ("reloadObject", RPCMode.Server, false);
						aHm = 0;
						aVm = 0;
					} else {
						aHm += GameObject.Find ("Car").gameObject.GetComponent<TranslationAR_scripts> ().serverCurrentHInput;
						aVm += GameObject.Find ("Car").gameObject.GetComponent<TranslationAR_scripts> ().serverCurrentVInput;
						GameObject.Find ("Car").transform.Translate (speed * new Vector3 (-aHm, 0, -aVm));
					}
					// Rotation of the car
					Vector3 oldRotationMus = GameObject.Find ("Car").transform.eulerAngles;
					int posNegXM = GameObject.Find ("Car").GetComponent<RotateAR_scripts>().posOrNegX;
					int posNegYM = GameObject.Find ("Car").GetComponent<RotateAR_scripts>().posOrNegY;
					int posNegZM = GameObject.Find ("Car").GetComponent<RotateAR_scripts>().posOrNegZ;
					rmX += RotateAR_scripts.RotationSpeed * posNegXM;
					rmY += RotateAR_scripts.RotationSpeed * posNegYM;
					rmZ += RotateAR_scripts.RotationSpeed * posNegZM;
					Vector3 rotationMus_ = new Vector3 (oldRotationMus.x + rmX, oldRotationMus.y + rmY, oldRotationMus.z + rmZ);
					GameObject.Find ("Car").transform.eulerAngles = rotationMus_;//rotates coin on Y axis
					RotateAR_scripts.rotate = false;

					// Scaling
					scaleM_ = GameObject.Find ("Car").GetComponent<ScaleAR_scripts>().serverCurrentScale;
					GameObject.Find ("Car").transform.localScale = scaleM_;

					break;
				}

				/*Vector3 position_ = GameObject.Find ("wheel(Clone)").gameObject.GetComponent<Trigger_scripts>().serverCurrentPosition;
				this._ms.setMarkerTransform (mid, GameObject.Find ("wheel(Clone)").transform);
				GameObject.Find ("wheel(Clone)").transform.position = position_;

				this._ms.setMarkerTransform (mid, GameObject.Find ("RigidHand(Clone)").transform);
				this._ms.setMarkerTransform (mid, GameObject.Find ("CleanRobotLeftHand(Clone)").transform);
				this._ms.setMarkerTransform (mid, GameObject.Find ("CleanRobotRightHand(Clone)").transform);

				/*float H= GameObject.Find ("wheel(Clone)").gameObject.GetComponent<Trigger_scripts>().serverCurrentHInput;
				float V= GameObject.Find ("wheel(Clone)").gameObject.GetComponent<Trigger_scripts>().serverCurrentVInput;
				//ici this._ms.setMarkerTransform (mid, GameObject.Find ("wheel(Clone)").transform);
				GameObject.Find ("wheel(Clone)").transform.Translate( new Vector3(20*H, 0, 20*V) );
				/*Vector3 pos = Vector3.zero;
				Quaternion rot = Quaternion.identity;
				_ms.getMarkerTransform(mid,ref pos,ref rot);
				GameObject.Find("Audi").transform.Translate(-pos);
				Vector3 _rot = new Vector3( -rot.x, -rot.y, -rot.z);
				GameObject.Find("Audi").transform.Rotate(_rot);*/

			} else {
				// hide Game object
				GameObject.Find ("Bugatti-Veyron").transform.localPosition = new Vector3 (-1000, -1000, -1000);
				GameObject.Find ("lamborghini").transform.localPosition = new Vector3 (-1000, -1000, -1000);
				GameObject.Find ("Car").transform.localPosition = new Vector3 (-1000, -1000, -1000);
				//GameObject.Find ("wheel(Clone)").transform.localPosition = new Vector3 (-100, -100, -100);
			}
		}
	}

	void OnDisconnectedFromServer (NetworkDisconnection info)
	{
		_ss.stop ();
	}

}