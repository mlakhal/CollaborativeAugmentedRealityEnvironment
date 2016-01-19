using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class screenShot_scripts : MonoBehaviour {
	
	private static int imageSmooth = 5;
	
	
	public static void saveFile(){
		SaveFileName sfn = new SaveFileName ();
		
		sfn.structSize = Marshal.SizeOf (sfn);
		
		sfn.filter = "All Files\0*.*\0\0";
		
		sfn.file = new string (new char[256]);
		
		sfn.maxFile = sfn.file.Length;
		
		sfn.fileTitle = new string (new char[64]);
		
		sfn.maxFileTitle = sfn.fileTitle.Length;
		
		sfn.initialDir = UnityEngine.Application.dataPath;
		
		sfn.title = "Attache file to your email";
		
		sfn.defExt = "png";
		sfn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
		//OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR
		
		if (DllSave.GetSaveFileName (sfn)) {
			//Debug.Log("Nom du fichier a sauvegardé : "+sfn.file);
			Application.CaptureScreenshot (sfn.file, imageSmooth);
		}
	}

}
