using UnityEngine;
using System.Collections;

public class Pulsar : MonoBehaviour
{

	public float speed = 0f;
	public bool XValue = false;
	public bool YValue = false;
	public bool ZValue = true;
	// Update is called once per frame

	void LateUpdate ()
	{
		if (Time.timeScale > 0) {
			Vector3 rot = Vector3.zero;
			rot.x += XValue ? 1 : 0;
			rot.y += YValue ? 1 : 0;
			rot.z += ZValue ? 1 : 0;
			rot *= speed;
			transform.Rotate (rot);
		}
	}



}
