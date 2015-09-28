using UnityEngine;
using System.Collections;

public class LightAnimation : MonoBehaviour {


	// Use this for initialization
	void Start () {
		StartCoroutine(FadeIn());
	}
	
	// Update is called once per frame
	IEnumerator FadeIn () {
		bool ended=false, started=false;
		float startTime = Time.time;
		while (!ended){
			Light lit = GetComponent<Light>();
			if (GameEvents.startCounting){
				if (!started) {
					startTime = Time.time;
					started=true;
				}
				lit.intensity = Mathf.SmoothStep(0f, 1f, (Time.time - startTime) / 5f);
			}
			if (lit.intensity>=1f){
				ended=true;
				break;
			}
			yield return null;
		}
	}

	
}
