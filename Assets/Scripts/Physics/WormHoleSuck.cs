using UnityEngine;
using System.Collections;

public class WormHoleSuck : MonoBehaviour
{

	public float StretchAmount = 0.3f;
	public float transformPull = 1f;
	public Collider2D Sucker;
	public bool started = false;
	// Use this for initialization
	void Start ()
	{
//		Sucker.enabled=false;
//		started = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!started && (GameEvents.startCounting) ) {
			Sucker.enabled = true;
			started = true;
		}
//		print ("update");
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (started) {

			if (other.gameObject.name == "outline" && other.gameObject.tag == "Player") {
				StretchAnim (other.transform.parent.transform);
				print ("Sucked player");
			} else if (other.gameObject.name == "outline" && other.gameObject.tag == "Satellite") {
				StretchAnim (other.transform.parent.transform);
				print ("Sucked sat");
			} else if (other.gameObject.tag != "blackhole") {
//			CheckAndPull (other.transform);
				StretchAnim (other.transform);
			}
		}
		
	}

	void StretchAnim (Transform obj)
	{

		Vector3 currScale = obj.transform.localScale;
		currScale.z += StretchAmount;

		if (obj.gameObject.GetComponent<CircleCollider2D> ()) {
			obj.gameObject.GetComponent<CircleCollider2D> ().enabled = false;
		} else if (obj.gameObject.name == "outline") {
			Collider2D[] colls = obj.parent.GetComponentsInChildren<Collider2D> ();
			foreach (Collider2D item in colls) {
				item.enabled = false;
			}
		}
		StartCoroutine (ByeBye (obj));
	}

	IEnumerator ByeBye (Transform obj)
	{
		float startTime = Time.time;
		Vector3 currPos = obj.transform.position;
		Vector3 currScale = obj.transform.localScale;
		float startScale = currScale.z;
		float overtime = 1.5f;
		while (Time.time < startTime + overtime) {
//			currPos.z+=transformPull;
			currPos.z = Mathf.SmoothStep (0f, 3100f, (Time.time - startTime) / overtime);

			currScale.z = Mathf.SmoothStep (startScale, startScale * 30f, (Time.time - startTime) / overtime);

			try {
				obj.transform.localScale = currScale;
				obj.transform.position = currPos;
			} catch (System.Exception) {
			}

			yield return null;
		}
		try {
			if (obj.tag == "Player") {
				Destroy (obj.gameObject);
				GameEvents.Failed (5);
			} else {
				Destroy (obj.gameObject);
			}
		} catch (System.Exception) {
		}
	}

}
