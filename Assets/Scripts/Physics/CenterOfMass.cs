using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class CenterOfMass : MonoBehaviour
{

	public Collider2D MassCollider;
	public float x, y;
	public bool ReturnToOrig=false;
	Vector2 currCOM;
	// Use this for initialization
	void Start ()
	{
		currCOM = gameObject.GetComponent<Rigidbody2D> ().centerOfMass;
		if (MassCollider != null) {
			gameObject.GetComponent<Rigidbody2D> ().centerOfMass = MassCollider.offset;
		} else {

			gameObject.GetComponent<Rigidbody2D> ().centerOfMass = new Vector2 (x, y);
		}
	}

	void Update(){
		if (ReturnToOrig){
			gameObject.GetComponent<Rigidbody2D> ().centerOfMass = currCOM;
			ReturnToOrig=false;
		}

	}
	

}
