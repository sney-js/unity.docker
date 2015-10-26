using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class DestroyPlane : MonoBehaviour {

	public string tagToDestroy = "Rock";
	// Use this for initialization

	void OnTriggerExit2D(Collider2D coll){
		if (coll.gameObject.tag==tagToDestroy){
			GameObject.Destroy(coll.gameObject);
		}
	}
}
