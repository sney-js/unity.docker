using UnityEngine;
using System.Collections;

[RequireComponent(typeof( Rigidbody2D))]
public class AnimationMovements : MonoBehaviour
{
	[Range(-1,5)]
	public int Movement=-1;
	//0-left 1-right 2-forward 3-back 4-rotateLeft 5-rotateRight
	public bool rotLeft, rotRight;
	public float force=500f;
	private Rigidbody2D body;
	public float OffsetTime=0f;
	public float RunTime=3f;

	// Use this for initialization
	void Start ()
	{
		print("force:"+force+" ,rotLeft:"+rotLeft);
		body = GetComponent<Rigidbody2D> ();
		if (RunTime==0)AddForce();
		else StartCoroutine(PhysicMovement());
//		StartCoroutine(LineDraw());

	}

//	void FixedUpdate(){
////		if (transform.position.x ==343) print(transform.eulerAngles.z);
//
//	}

	IEnumerator LineDraw(){
		Vector3 prevPos = transform.position;

//		TrailRenderer lr = gameObject.AddComponent<TrailRenderer>();
//		lr.material = new Material(Shader.Find("Particles/Additive"));
//		lr.SetColors(Color.yellow, Color.red);
//		lr.SetWidth(1F, 1F);
//		lr.SetVertexCount(1000);
//		lr.
		int i=0;
		while (true) {
			if (Movement==-1) break;
//			Debug.DrawLine(prevPos, transform.position);
//			Gizmos.DrawLine (prevPos, transform.position);
			prevPos = transform.position;
//			lr.SetPosition(i, prevPos);
			yield return new WaitForSeconds(1f);
			i++;
			if (i>=1000) i=0;
		}

	}

	IEnumerator PhysicMovement(){
		yield return new WaitForSeconds(OffsetTime);
//		print ("STARTED ANIMATING");
		float startTime = Time.time;

		while (Time.time < startTime + RunTime) {
			AddForce();
//			print("force:"+force+" ,rotLeft:"+rotLeft);
			yield return new WaitForSeconds(Time.deltaTime);
		}

//		print ("STOPPED ANIMATING");

	}

	// Update is called once per frame
	void AddForce ()
	{
		Vector3 position = body.transform.position;
		switch (Movement) {
		case 0: body.AddForceAtPosition (body.transform.up * (force), position); break;
		case 1: body.AddForceAtPosition (-body.transform.up * (force), position); break;
		case 2: body.AddForceAtPosition (body.transform.right * (force), position); break;
		case 3: body.AddForceAtPosition (-body.transform.right * (force), position); break;
		case 4: body.AddTorque (-force); break;
		case 5: body.AddTorque (-force); break;
		}
	
		if (rotLeft) body.AddTorque (-force);
		if (rotRight) body.AddTorque (force);
	}


}
