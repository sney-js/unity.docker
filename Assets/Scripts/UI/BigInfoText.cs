using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BigInfoText : MonoBehaviour
{

	public static string msg;
	Text text;
//	private string prevMsg;
	// Use this for initialization
	public GameObject nose, sink;
	public int lengthOfLineRenderer = 20;
	private float nextActionTime = 0.0f; 
	public float period = 0.1f;

	void Start ()
	{
		msg = "";
//		prevMsg = msg;
		text = GetComponent<Text> ();	
//		StartCoroutine (MyRoutine (0.05f));

	}
	
	IEnumerator MyRoutine (float interval)
	{ 
		while (true) {
			float distance = Vector2.Distance (nose.transform.position, sink.transform.position);
			text.text = distance.ToString ("0.0");
			yield 
				return new WaitForSeconds (interval); 
		}
	}


	// Update is called once per frame
	void Update ()
	{
		if (Time.time > nextActionTime ) { 
			nextActionTime += period; // execute block of code here }
			float distance = Vector2.Distance (nose.transform.position, sink.transform.position);
			text.text = distance.ToString ("0.0");
		}



	}
}
