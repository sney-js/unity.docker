using UnityEngine;
using System.Collections;

public class GavityWell : MonoBehaviour {

	public float StartingForce = 0.001f;
//	private int totalObj;
	private bool beingPulled = false;
	void Start ()
	{

	}
	
	void CheckAndPull (Transform obj)
	{
		Vector3 direction = transform.position - obj.transform.position;
//		direction.y = 0f;
//		direction.z = 0f;
		Rigidbody2D rbod = obj.gameObject.GetComponent<Rigidbody2D> ();
		if (rbod != null)
			rbod.AddForce (StartingForce * direction);
		
		
	}

	void OnTriggerStay2D (Collider2D other)
	{
		if (other.gameObject.name == "outline") {
			CheckAndPull (other.transform.parent.transform);
		} else if (other.tag == "Rock") {
			CheckAndPull (other.transform);
		}
		
	}

}
