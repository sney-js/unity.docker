using UnityEngine;
using System.Collections;

public class LightIntensity : MonoBehaviour {
//	public static LightIntensity instance;
	public Light lightObj;
	public Material NoseLight;
//	public bool flicker;
//	public float time;
	public float deltaAnim=0.2f;
	public float delta=1f;
	public float maxIntensity=2f;
	float currOrig;
	private int currLightLevel;
	// Use this for initialization
	private float nextActionTime = 0.0f; public float period = 0.1f;
	private static bool toggleCalled;
	public int StartingAmount=0;

	void Start () {
		toggleCalled=false;
		currOrig=lightObj.intensity;
		NoseLight.color = new Color(0f,0f,0f,1f);
		currLightLevel=0;
		for (int i = 0; i < StartingAmount; i++) {
			ToggleHeadlight();
		}
		ButtonPresses.ChangeHeadlightIcon(currLightLevel);
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if ((Input.GetKeyDown(KeyCode.H) || toggleCalled ) && !GameEvents.StopListeningKeys){

			ToggleHeadlight();
		}

//		if (flicker) {
//			if (Time.time > nextActionTime ) { 
//				nextActionTime += Random.Range(0, time*5); 
//				animate();
//				// execute block of code here 
//			} 
////			InvokeRepeating("animate", 0, time);
//		}
	}

	public static void Toggle(){
		toggleCalled=true;
	}
	void ToggleHeadlight(){
		toggleCalled=false;
		float intensity = lightObj.intensity+delta;
//		print("dd");
		Color nosecol = NoseLight.color;
		nosecol.r+=0.3f;
		nosecol.g+=0.3f;
		nosecol.b+=0.3f;
		currLightLevel++;
		if (intensity>maxIntensity) {
			intensity=0;
			currLightLevel=0;
			nosecol = new Color(0f,0f,0f,1f);
		}
		NoseLight.color = nosecol;
		
		lightObj.intensity=intensity;	
		currOrig=lightObj.intensity;
		ButtonPresses.ChangeHeadlightIcon(currLightLevel);

	}

	void animate(){
		float current = lightObj.intensity;
		float intensityD=Random.Range(current-deltaAnim, current+deltaAnim);
		lightObj.intensity=Mathf.Clamp(intensityD, currOrig-deltaAnim,currOrig+deltaAnim);
	}
}
