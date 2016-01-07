using UnityEngine;
using System.Collections;

public class Pulsar : MonoBehaviour
{

	public float speed = 0f;
	public float XValue = 100f;
	public float YValue = 50f;
	public float ZValue = 0f;
	// Update is called once per frame

	void LateUpdate ()
	{
		float t = Time.smoothDeltaTime;
		t = Mathf.Sin (t * Mathf.PI * 0.5f);
		Vector3 rot = Vector3.zero;
		rot.y = Mathf.Sin(2*YValue);
		rot.x = Mathf.Cos(XValue);
		rot.z = Mathf.Cos(2*ZValue);
		rot *= speed;
		transform.Rotate (rot);
	}



}
