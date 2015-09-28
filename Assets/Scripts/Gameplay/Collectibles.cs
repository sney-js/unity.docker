using UnityEngine;
using System.Collections;

public class Collectibles : MonoBehaviour {

	private bool callOnce;
	public AudioClip CollectSound;
//	public GameObject[] collectedBy;
//	private int arrSize =0 ;
	// Use this for initialization
	void Start () {
		callOnce=true;
//		arrSize = collectedBy.Length;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D other){
		bool found=false;
//		Debug.Log(other.tag);
//		for (int i = 0; i < arrSize; i++) {
//			if (collectedBy[i]==other){
//				found = true;
//				break;
//			}
//		}
		if (other.tag.Equals("Player") && !GameEvents.docked)found=true;
//		print ("found="+found);
		if (found & callOnce){
			GameEvents.Score++;
//			AudioClip deltaPitch = CollectSounds[0].
			SoundScript.PlayOnce(CollectSound, true);
			transform.FindChild("Particles").GetComponent<ParticleSystem>().Stop();
			Invoke("destroyGroup", 0.7f);
			found=false;
			callOnce=false;
		}

	}

	void destroyGroup(){
		Destroy(gameObject);
	}
}
