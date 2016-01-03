using UnityEngine;
using System.Collections;

public class Pulsar : MonoBehaviour
{

	float rotationsPerMinute = 50f;
	float sleep = 0f;
	// Update is called once per frame
	void Update ()
	{
		if (Time.time > sleep + 2f) {
			float t = Time.smoothDeltaTime;
			t = Mathf.Sin (t * Mathf.PI * 0.5f);
			transform.Rotate (new Vector3 (0f, 6.0f * rotationsPerMinute * t, 0f));
		} 
		else if (Mathf.Approximately (transform.eulerAngles.y, 180f)) {
			sleep = Time.time;
		}
	}
}
