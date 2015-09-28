using UnityEngine;
using System.Collections;

public class DockAreaTrigger : MonoBehaviour {

	public static bool inDockArea=false;

	// Use this for initialization
	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.gameObject.name == "Nose") 
		inDockArea=true;
	}
	
	void OnTriggerExit2D (Collider2D other)
	{
//		if (other.gameObject.name == "Nose") {
		inDockArea = false;
	}
	
	void OnTriggerStay2D (Collider2D other)
	{
		if (other.gameObject.name == "Nose") 
			inDockArea=true;	
		}

}
