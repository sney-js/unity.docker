using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DockedScript : MonoBehaviour
{


	public static bool attach = true;
	public GameObject MoverObject;
	public bool IsBigWheelGoal;
	private Image dockIndicator;
	private GameObject playerMain;
//	private Color indDefColor;
	private static bool undockedCalled;
	public bool IsTarget=true;

	void Start ()
	{
//		dockIndicator = GameObject.Find ("Canvas/UI/Other/DockIndicator").gameObject.GetComponent<Image> ();
		playerMain = GameObject.Find ("Player").gameObject;
//		indDefColor = dockIndicator.color;
		undockedCalled=false;
//		Debug.Log("Name: "+MoverObject.name);
		ButtonPresses.ChangeDockIcon(false);
	}

	void LateUpdate(){
		if (Input.GetKeyDown (KeyCode.F)){
			undockedCalled=true;
		}
		if (GameEvents.LevelFail || GameEvents.LevelSuccess){
			AudioSource au= GetComponent<AudioSource>();
			if (au!=null && au.volume>0) au.volume=Mathf.Lerp(0.66f, 0f, 0.5f);
		}


	}
	void OnTriggerEnter2D (Collider2D other)
	{

	}
	
	void OnTriggerExit2D (Collider2D other)
	{
		attach = true;
	}
	
	void OnTriggerStay2D (Collider2D other)
	{
		if (other.gameObject.name == "Nose") {
//			Debug.Log ("STAYING: " + other.gameObject.name);

			Transform myParent = gameObject.transform.parent; //Sink, PropShiled, PropSweeper
			Transform yourParent = other.transform.parent; //player
		
			SpringJoint2D myDJoints;
			if (!IsBigWheelGoal) myDJoints = myParent.GetComponent<SpringJoint2D> ();
			else{ //if not bigwheel, select first else select second
				myDJoints = myParent.GetComponents<SpringJoint2D> ()[1];
			}

			SpringJoint2D yourDJoints = yourParent.GetComponent<SpringJoint2D> ();
		
			if (myDJoints != null && yourDJoints != null) {
				if (attach & myDJoints .connectedBody != yourDJoints .gameObject.GetComponent<Rigidbody2D> ()) {


					myDJoints .connectedBody = yourDJoints .gameObject.GetComponent<Rigidbody2D> ();
					yourDJoints .connectedBody = myDJoints .gameObject.GetComponent<Rigidbody2D> ();

					myDJoints .connectedAnchor = yourDJoints .anchor;
					yourDJoints .connectedAnchor = myDJoints .anchor;

					dockSuccess (yourParent);

				} else {
					if (undockedCalled && !GameEvents.LevelFail && !GameEvents.LevelSuccess) {
						myDJoints .connectedBody = myDJoints .gameObject.GetComponent<Rigidbody2D> ();
						yourDJoints .connectedBody = yourDJoints .gameObject.GetComponent<Rigidbody2D> ();
				
						myDJoints .connectedAnchor = myDJoints .anchor;
						yourDJoints .connectedAnchor = yourDJoints .anchor;
						attach = false;
						undockedCalled=false;
						dockReleased (yourParent);

					} 
				}


			}
		}

	}

	public static void ReleaseCalled(){
		undockedCalled=true;

	}
	public void dockReleased (Transform player)
	{

		if (MoverObject != null) {
			changeDockIndicator (false);
			if (gameObject.GetComponent<AudioSource> () != null) {//satellite 
				StartCoroutine (SoundScript.FadeSound (gameObject.GetComponent<AudioSource> (), 2f, 0.5f, 5f));
			}
			if (gameObject.tag=="Shield"){
				playerMain.gameObject.GetComponent<HealthLoss>().loseHealth=true;
			}
//			if (IsTarget)	
			GameEvents.Undocked (MoverObject);
		}
	}

	public void dockSuccess (Transform player)
	{
		if (MoverObject != null) {
			changeDockIndicator (true);
			SoundScript.docked (1);
			if (gameObject.GetComponent<AudioSource> () != null) {
				StartCoroutine (SoundScript.FadeSound (gameObject.GetComponent<AudioSource> (), 0.5f, 0f, 0f));
			}
			if (gameObject.tag=="Shield"){
				playerMain.gameObject.GetComponent<HealthLoss>().loseHealth=false;
			}
			GameEvents.Docked (MoverObject, IsTarget);
		}
	}

	void changeDockIndicator (bool isOn)
	{
		ButtonPresses.ChangeDockIcon(isOn);
	}


}
