using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Collectibles : MonoBehaviour
{

	private bool callOnce;
	public AudioClip CollectSound;
	private Text scoretext;

	void Start ()
	{
		callOnce = true;
		scoretext = GameObject.Find ("Canvas").transform.FindChild ("UI/Score").GetComponent<Text> ();
	}
	
	void OnTriggerEnter2D (Collider2D other)
	{
		bool found = false;
		if (other.tag.Equals ("Player") && !GameEvents.docked)
			found = true;
		if (found & callOnce) {
			GameEvents.Score++;
//			StartCoroutine( AnimationScript.FlashScreen(null, 1f, 1, null));
			SoundScript.PlayOnce (CollectSound, true);
			if (transform.FindChild ("Particles").gameObject != null) {
				ParticleSystem sysP = transform.FindChild ("Particles").gameObject.GetComponent<ParticleSystem> ();
				if (sysP != null) {
					sysP.Stop ();
					found = false;
					callOnce = false;
					StartCoroutine (animateScore ());
				}
			}
			Invoke ("destroyGroup", 0.7f);
		}

	}

	IEnumerator animateScore ()
	{
		float max = 75f;
		float min = 65f;
		yield return new WaitForEndOfFrame ();
		float startTime = Time.time;
		float overTime = 0.1f;
		while (Time.time < startTime + overTime) {
			scoretext.fontSize = (int)Mathf.Lerp (min, max, (Time.time - startTime) / overTime);
			yield return null;
		}

		startTime = Time.time;
		overTime = 0.3f;
		while (Time.time < startTime + overTime) {
			scoretext.fontSize = (int)Mathf.Lerp (max, min, (Time.time - startTime) / overTime);
			yield return null;
		}
	}

	void destroyGroup ()
	{
		Destroy (gameObject);
	}
}
