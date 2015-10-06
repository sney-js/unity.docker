using UnityEngine;
using System.Collections;

public class SpringWell : MonoBehaviour {

	public bool ScreenBounded = false, CameraBouded = true;
	private Vector3[] screenBounds;
	public GameObject player;
	private SpringJoint2D spring;
	public static bool PullNow = false;
	public static bool CancelPull = false;
	// Use this for initialization
	void Start () {
		if (player == null)
			player = GameObject.Find ("Player").gameObject;

		spring = gameObject.GetComponent<SpringJoint2D>();
		spring.connectedBody = player.GetComponent<Rigidbody2D>();

		if (ScreenBounded) {
			
			Vector3 lb = Camera.main.ScreenToWorldPoint (new Vector3 (0f, 0f, -Camera.main.transform.position.z));
			Vector3 rt = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width, Screen.height, -Camera.main.transform.position.z));
			
			screenBounds = new Vector3[2];
			screenBounds [0] = lb;
			screenBounds [1] = rt;
		}
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (ScreenBounded) adjustBounds();
		else if (CameraBouded)  {
			spring.enabled=PullNow&&!CancelPull;

		} 
	}

	void adjustBounds ()
	{
		Vector3 pos = player.transform.position;
		if (pos.x>screenBounds[1].x || pos.x<screenBounds[0].x || pos.y>screenBounds[1].y || pos.y<screenBounds[0].y) {
			spring.enabled=true;
		}
		else {
			spring.enabled=false;
		}
	}

}
