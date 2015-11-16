using UnityEngine;
using System.Collections;

public class AnimSlow : MonoBehaviour {
	public bool slowNow = false;
	public Animator animtr;
	public float overTime = 2f;
	public bool goToMainMenu = false;
	// Use this for initialization
	void Start () {
		animtr = gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		if (slowNow){
			StartCoroutine(slowFunction());
			slowNow=false;
		}
		if (goToMainMenu){
			goToMainMenu=false;
			GameEvents.LevelMainMenu ();
		}
	}

	IEnumerator slowFunction(){
		float startTime = Time.time;
		while(Time.time < startTime + overTime)
		{
			animtr.speed = Mathf.SmoothStep(1,0, (Time.time - startTime)/overTime);
			print ("Slowing.."+animtr.speed);
			yield return null;
		}
	}
}
