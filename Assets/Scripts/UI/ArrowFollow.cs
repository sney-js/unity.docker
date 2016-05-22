using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ArrowFollow : MonoBehaviour
{


	public GameObject goToTrack;
	private GameObject fromTrack;
	public Text text;
	public GameObject blob;
	public float min = 0.02f, max = 0.98f;
	private float nextActionTime = 0.0f, nextActionTimeText = 0.0f;
	private float period = 0.02f;
	// Use this for initialization
	void Start ()
	{
		if (goToTrack == null)
			goToTrack = GameObject.Find ("Level/Dockables/Satellite");
		if (fromTrack == null)
			fromTrack = GameObject.Find ("Level/Player");
//			fromTrack = blob;
		min = 0.02f;
		max = 0.98f;
		nextActionTime = 0.0f;
		nextActionTimeText = 0.0f;
	}

	public static float distance = 400;
	
	// Update is called once per frame
	void LateUpdate ()
	{
		if (Time.time > nextActionTime) { 
			nextActionTime += period; 

			if (Time.time > nextActionTimeText) { 
				nextActionTimeText += (0.1f); 
				distance = Vector2.Distance (fromTrack.transform.position, goToTrack.transform.position);
				Vector3 fromDist = Camera.main.ScreenToWorldPoint (blob.transform.position);
				fromDist.z = 0f;
				float distanceBlob = Vector2.Distance (fromDist, goToTrack.transform.position);
				text.text = distanceBlob.ToString ("0.0");
			}

			Vector3 v3Screen = Camera.main.WorldToViewportPoint (goToTrack.transform.position);
//		Debug.Log("sat at: "+v3Screen);
			if (v3Screen.x > -0.01f && v3Screen.x < 1.01f && v3Screen.y > -0.01f && v3Screen.y < 1.01f) {
//			gameObject.GetComponent<SpriteRenderer>().enabled=false;
				if (text.IsActive ()) {
					blob.SetActive (false);
					text.gameObject.SetActive (false);
				}
			} else {
//			gameObject.GetComponent<SpriteRenderer>().enabled=true;
//			gameObject.SetActive(false);
				if (!text.IsActive ()) {
					text.gameObject.SetActive (true);
					blob.SetActive (true);
				}

				Vector3 v3Screen2 = v3Screen;

				v3Screen.x = Mathf.Clamp (v3Screen.x, min, max);
				v3Screen.y = Mathf.Clamp (v3Screen.y, min, max);
				v3Screen2.x = Mathf.Clamp (v3Screen2.x, min + 0.04f, max - 0.04f);
				v3Screen2.y = Mathf.Clamp (v3Screen2.y, min + 0.04f, max - 0.04f);


//			transform.position = Camera.main.ViewportToWorldPoint(v3Screen);
				text.transform.position = Camera.main.ViewportToScreenPoint (v3Screen2);
				blob.transform.position = Camera.main.ViewportToScreenPoint (v3Screen);
//			text.transform.position = Camera.main.WorldToViewportPoint (v3Screen);
//			transform.rotation = Quaternion.Slerp(
//				transform.rotation,
//				Quaternion.LookRotation(),
//				Time.deltaTime * 2f);
			}
		}
	}
}
