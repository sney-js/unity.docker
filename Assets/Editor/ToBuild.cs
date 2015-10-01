using UnityEngine;
using System.Collections;
using UnityEditor;

public class ToBuild : MonoBehaviour {

	private static int totLevels = 12; //0-11
	private static string installLocation = "Executables/", appName = "Docker";

	// Use this for initialization
	static void BuildMac () {

		ExecuteBuild (1);
	}

	static void BuildLinux() {
		
		ExecuteBuild (0);
		
	}

	static void BuildWindows() {
		
		ExecuteBuild (2);
		
	}

	static void ExecuteBuild(int platform){
		string folder = "Default";
		BuildTarget target = BuildTarget.StandaloneOSXUniversal;

		switch (platform) {
		case 0: folder = "Linux"; target = BuildTarget.StandaloneLinux64;
			break;
		case 1: folder = "MacOS";target = BuildTarget.StandaloneOSXUniversal;
			break;
		case 2: folder = "Windows"; target = BuildTarget.StandaloneWindows64;
			break;
		}

		string execPath = installLocation + folder + "/" + appName;

		try {
			BuildPipeline.BuildPlayer (getAllScenes (), execPath, target, BuildOptions.None);
			Debug.Log("::::::: Build Successful. Location of build: "+execPath);
		} catch (System.Exception ex) {
			Debug.Log("::::::: An error occurred while Building. Source: ToBuild.ExecuteBuild()");
			EditorApplication.Exit(1);
		}
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
