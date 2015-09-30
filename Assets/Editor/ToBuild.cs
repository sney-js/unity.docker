using UnityEngine;
using System.Collections;
using UnityEditor;

public class ToBuild : MonoBehaviour {

	private static int totLevels = 12; //0-11
	private static string installLocation = "Executables/", appName = "Docker";

	// Use this for initialization
	static void BuildMac () {

		BuildPipeline.BuildPlayer(getAllScenes(), installLocation + "MacOSx/"+appName, 
		                          BuildTarget.StandaloneOSXUniversal, BuildOptions.None);
	
	}

	static void BuildLinux() {
		
		BuildPipeline.BuildPlayer(getAllScenes(), installLocation + "Linux/"+appName, 
		                          BuildTarget.StandaloneLinux64, BuildOptions.None);
		
	}

	static string[] getAllScenes(){
		string sceneLocation = "Assets/Scenes/";
		string levName = "level_";
		string ext = ".unity";
		string[] scenes = new string[totLevels];
		for (int i = 0; i < totLevels; i++) {
			scenes[i] = sceneLocation+levName+(i.ToString())+ext;
		}
		return scenes;
	}

}
