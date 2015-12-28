using UnityEngine;
using System.Collections;

public class TinyGravity : MonoBehaviour
{

	public float StartingForce = 0.001f;
	private int totalObj;
//	private bool beingPulled = false;
	public ParticleSystem burnAnimation;
	public bool StartAtAwake=true;
	public GameObject BurnDemParent;

	void Start ()
	{
		burnAnimation.Stop ();
	}

	void CheckAndPull (Transform obj)
	{
		Vector3 direction = transform.position;// - obj.transform.position;
		direction.y = 0f;
		direction.z = 0f;
		Rigidbody2D rbod = obj.gameObject.GetComponent<Rigidbody2D> ();
		if (rbod != null)
			rbod.AddForce (StartingForce * direction);


	}

	void OnTriggerEnter2D (Collider2D other)
	{

		if (other.gameObject.name == "outline") {
			gameObject.GetComponent<Animation> ().Play ();
			SoundScript.MusicIntenseEscape ();
		}
//		print (other.tag);
	}

	void OnTriggerStay2D (Collider2D other)
	{
		if (StartAtAwake || GameEvents.startCounting){
		if (other.gameObject.name == "outline") {
			CheckAndPull (other.transform.parent.transform);
		} else if (other.tag == "Rock") {
			CheckAndPull (other.transform);
		}
		}

	}

	void OnTriggerExit2D (Collider2D other)
	{
		if (other.gameObject.name == "outline") {
			SoundScript.StopMusic ();

		}
		float distance = transform.position.x - other.transform.position.x;
		if (distance > 0) { //exxit from the sun's side
			GoneIntoTheSun (other.transform);
		} else { //from escape side
			if (other.gameObject.name == "outline") {
				gameObject.GetComponent<Animation> ().Stop ();
				Color curr = gameObject.GetComponent<SpriteRenderer>().color;
				curr.a=0f;
				gameObject.GetComponent<SpriteRenderer>().color = curr;
			}
		}

	}

	void GoneIntoTheSun (Transform other)
	{
		Vector3 positionAt = other.position;
		StartCoroutine(NewBurnDem(positionAt, 1f));

		if (other.tag == "Player") {
			SpringWell.CancelPull = true;
			Invoke ("callGameEventSunFail", 2.5f);
		}
	}

	void callGameEventSunFail ()
	{
		GameEvents.Failed (4);
	}

	IEnumerator NewBurnDem(Vector2 positionAt, float mass){
//		if (!burnAnimation.isPlaying){
//			burnAnimation.transform.position = positionAt;
//			burnAnimation.Play ();
//		}else{
			GameObject clone = Instantiate(burnAnimation.gameObject);
			clone.transform.parent = BurnDemParent.transform;

//			clone.transform.parent=burnAnimation.transform.parent;
			clone.transform.position = positionAt;
			clone.GetComponent<ParticleSystem>(). Play ();
//			print ("Waiting for: "+clone.duration);
			yield return new WaitForSeconds(5f);
//			print ("NOW DESTROYED!");
			GameObject.Destroy(clone);
//		}

	}

}
