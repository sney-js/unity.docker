using UnityEngine;
using System.Collections;

public class GeyserControl : MonoBehaviour {

//	public float intensity=1f;
	// Use this for initialization
	bool expelled;
	void Start(){

		expelled=false;
	}


	void OnTriggerStay2D (Collider2D other){
//		print("Collider found: "+other.name+"pos:"+other.gameObject.transform.localPosition.z);
		if (other.gameObject.tag=="Geyser" && other.gameObject.transform.localPosition.z <=-2400){
//			print("ENTER");
			if (!expelled){
				print("Expelled!");
				StartCoroutine(throwPlayer(0.5f));

			}
		}
	}

	IEnumerator throwPlayer (float overTime)
	{
		expelled=true;
		float startTime = Time.time;
		while (Time.time < startTime + overTime) {
			Vector3 pos = transform.position;
			pos.z = Mathf.SmoothStep (0f, -1500f, (Time.time - startTime) / overTime);
			transform.position = pos;
			yield return null;
		}
		Camera.main.GetComponent<CameraScript>().follow=false;
		GameEvents.Failed(7);
	}
}
