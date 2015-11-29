using UnityEngine;
using System.Collections;

public class LightAnimation : MonoBehaviour {


	// Use this for initialization
	void Start () {
		StartCoroutine(FadeIn());
	}
	
	// Update is called once per frame
	IEnumerator FadeIn () {
		while (!GameEvents.startCounting) {
			yield return new WaitForEndOfFrame();
		}

		Light lit = GetComponent<Light>();
		float startTime = Time.time;
		while(Time.time < startTime + 5f)
		{
			lit.intensity = Mathf.SmoothStep(0f, 1f, (Time.time - startTime) / 5f);
			yield return null;
		}
	}

	
}
