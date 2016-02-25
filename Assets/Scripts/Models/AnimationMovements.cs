using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class AnimationMovements : MonoBehaviour
{
	[Range(-1,3)]
	public int
		Movement = -1;
	//0-left 1-right 2-forward 3-back 4-rotateLeft 5-rotateRight
	public bool rotLeft, rotRight;
	public float force = 500f;
	private Rigidbody2D body;
	public float OffsetTime = 0f;
	public float RunTime = 3f;
	public bool random = false;

	// Use this for initialization
	void Start ()
	{
//		print ("force:" + force + " ,rotLeft:" + rotLeft);
		body = GetComponent<Rigidbody2D> ();
		if (RunTime == 0)
			AddForce ();
		else
			StartCoroutine (PhysicMovement ());
//		StartCoroutine (PhysicMovement ());

	}

	IEnumerator PhysicMovement ()
	{
		if (RunTime == 0) {
			AddForce ();
			yield return null;
		} else {
			float startTime = Time.time;
			while (Time.time < startTime + RunTime) {
				yield return new WaitForEndOfFrame ();
				AddForce ();
			}
		}

	}

	void AddForce ()
	{
		Vector3 position = body.transform.position;
		if (!random) {
			switch (Movement) {
			case 0:
				body.AddForceAtPosition (body.transform.up * (force), position);
				break;
			case 1:
				body.AddForceAtPosition (-body.transform.up * (force), position);
				break;
			case 2:
				body.AddForceAtPosition (body.transform.right * (force), position);
				break;
			case 3:
				body.AddForceAtPosition (-body.transform.right * (force), position);
				break;
			}
		} else {
			Vector2 rando = new Vector2 (Random.Range (-10, 10), Random.Range (-10, 10));
			body.AddForceAtPosition (rando * (force), position);
		}

		if (random)force/=5f;
		if (rotLeft)
			body.angularVelocity = -force;
		if (rotRight)
			body.angularVelocity = force;
	}


}
