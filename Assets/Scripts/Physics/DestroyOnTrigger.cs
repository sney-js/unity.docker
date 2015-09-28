using UnityEngine;
using System.Collections;

public class DestroyOnTrigger : MonoBehaviour {


	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.tag=="Rock"){
			Destroy(other.gameObject);
		}

	}
}
