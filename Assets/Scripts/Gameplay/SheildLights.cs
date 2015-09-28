using UnityEngine;
using System.Collections;

public class SheildLights : MonoBehaviour {

	public Light[] lights;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.gameObject.name == "Nose") {
			for (int i = 0; i < lights.Length; i++) {
				StartCoroutine(FadeLight(lights[i], 4f, 3f,0f));
			}
		}

	}
	
	void OnTriggerExit2D (Collider2D other)
	{
		if (other.gameObject.name == "Nose") {
			for (int i = 0; i < lights.Length; i++) {
				StartCoroutine(FadeLight(lights[i], 0f, 3f,0f));
			}
		}
	}

	public static IEnumerator FadeLight(Light light, float to, float overTime, float WaitFor){
		yield return new WaitForSeconds(WaitFor);
		float startTime = Time.time;
		float from = light.intensity;	
				
		while (Time.time < startTime + overTime) {
			light.intensity = Mathf.Lerp (from, to, (Time.time - startTime) / overTime);
			yield return null;
		}

	}
}
