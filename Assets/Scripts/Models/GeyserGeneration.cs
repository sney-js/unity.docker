using UnityEngine;
using System.Collections;

public class GeyserGeneration : MonoBehaviour
{

	// Use this for initialization
	public int seed = 123;
	public bool drawAgain = false;
	public GameObject parentObj;
	public string namePref = "_item";
	public int numberOfObjects = 5;
	public GameObject[] prefs;

	public bool AtStart = true;
	public bool SpawnMode = true;

	public float zValue = 0f;
	public float OffsetTime = 4f;

	public bool WithVelocity = false;
	[Range(0,1)] public float ProbVel = 0.2f;
	public float PerSpawnTime = 0.2f;
	[Range(0,1)] public float	Probability = 1f;
	public bool SpawnFollowPlayer=false;
	public Rigidbody2D Player;
	private float startTime;
	private int total = 0;

	private GameObject getRandRock ()
	{
		return prefs [Random.Range (0, prefs.Length - 1)];
	}

	void Start ()
	{
		startTime = Time.time;
		Random.seed = seed;
//		Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Asteroids/");
//		Debug.Log(sprites.Length);
		if (AtStart) {
			for (; total < numberOfObjects; total++) {

				SpawnOne ();

			}
		}
	}

	Vector2 CalculatePlayerPos(){
		Vector2 pos = new Vector2(Player.transform.position.x, Player.transform.position.y);
		float t = OffsetTime;
		Vector2 u = Player.velocity;
		//r=ut+0.5.at^2+r0
		Vector2 newPos;
		newPos.x = u.x*t + 0*0.5f*(t*t)+pos.x;
		newPos.y = u.y*t + 0*0.5f*(t*t)+pos.y;

//		print("ut:"+u.x*t+"+:"+pos.x);
//		print("R0: "+pos+ ", R1:"+newPos);
		return newPos;
	}

	void SpawnOne ()
	{
		//-----------------------random rock---------------------------------------
		GameObject theRock = getRandRock ();
		
		//-------------------------------position, size, rotation-----------------------------------------------
		Vector3 pos;
		Vector3 playerFuturePos  = CalculatePlayerPos();
		pos = new Vector3 (playerFuturePos.x, playerFuturePos.y, zValue);
			

		GameObject myRock = (GameObject)Instantiate (theRock, pos, Quaternion.identity);
		myRock.name = total + namePref;

		
		myRock.transform.parent = parentObj.transform;
//		myRock.transform.localPosition=pos;
		myRock.layer = 0;


	}

	void DeleteOne ()
	{

		GameObject.Destroy (parentObj.transform.GetChild (0).gameObject);
		


	}

	void Update ()
	{
		if (drawAgain) {
			foreach (Transform child in parentObj.transform) {
				GameObject.Destroy (child.gameObject);
			}
			total = 0;
			Start ();
			drawAgain = false;
		}
		if (SpawnMode && Time.time > startTime + PerSpawnTime && GameEvents.startCounting) {
			startTime = Time.time;
			if (Random.value < Probability) {
				total++;
				SpawnOne ();
				if (parentObj.transform.childCount > numberOfObjects) {
					DeleteOne ();
				}
			}
		}

	}

}
