using UnityEngine;
using System.Collections;

public class Rope : MonoBehaviour
{
	public GameObject Player;

	LineRenderer lr;
	public int size;
	// Use this for initialization
	void Start ()
	{
		lr = gameObject.GetComponent<LineRenderer> ();
//		StartCoroutine(DrawLineDamp(0.1f));
	}

	void LateUpdate(){
//		Vector3 mPos = transform.position;
		Vector3 loc = Player.transform.position;

//		lr.SetVertexCount (size+1);
//		lr.SetPosition (0, mPos);
		lr.SetPosition (size, loc);

	}

	void DrawLine (int i)
	{
		Vector3 mPos = transform.position;
		Vector3 loc = Player.transform.position;

//		for (int i = 1; i < size; i++) {

			float r = i / size;
			Vector3 newLoc = r * loc + (1 - r) * mPos;

			lr.SetPosition (i, newLoc);
//		}
	}

	IEnumerator DrawLineDamp (float waitTime)
	{
//		float startTime = Time.time;
		while (true) {
			for (int i = 1; i < size; i++) {
				DrawLine(i);
			}
			yield return new WaitForSeconds(waitTime);
		}
	}	
}

