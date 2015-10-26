using UnityEngine;
using System.Collections;

public class AsteroidShower : MonoBehaviour
{

	// Use this for initialization
	public int seed = 123;
	public bool drawAgain = false;
	public GameObject[] prefs;
	public bool AtStart=true;
//	public GameObject asteroid2D;
//	public bool inCircle=false;
	public int numberOfObjects = 5;	
	public int numberOfObjectsAtStart = 5;
	public float rangeXY = 100;
	public float zValue = 0f;
	public string namePref;
	public GameObject parentObj;
	public float defaultScale = 1f;
	public bool RotateAndScale = false;
	public Material OptionalMaterial;
	public float Velocity = 5f;
	public float VelocityVariance = 1.2f;
	public bool WithVelocity = false;
//	public float ProbVel=0.2f;
	public float velDirection = 2; //NESW=0123
	public bool AsteroidScaling = true;
	public bool SpawnMode = true;
	public float PerSpawnTime = 1f;
	public float Probability = 0.2f;
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
		if (AtStart) {
			for (; total < numberOfObjectsAtStart; total++) {

				SpawnOne ();

			}
			AtStart=false;
		}
	}

	void SpawnOne ()
	{
		//-----------------------random rock---------------------------------------
		GameObject theRock = getRandRock ();
		
		//-------------------------------position, size, rotation-----------------------------------------------
		Vector3 pos;
		float xran = Random.Range (-600f, 850f);
		float yran = rangeXY * Random.Range(AtStart?0.4f:0.9f, 1.1f) - (xran/4f);
		pos = new Vector3 (xran, yran,zValue);

		GameObject myRock = (GameObject)Instantiate (theRock, pos, Quaternion.identity);
		myRock.name = total + namePref;

		
		myRock.transform.parent = parentObj.transform;
		myRock.layer = 0;

		float rando = Random.Range (0.8f, 1.2f);
		if (AsteroidScaling) {
			rando = Random.Range (0.3f, 2f);
			if (rando > 1.8)
				rando = Random.Range (2f, 8f);
		}

		myRock.transform.localScale = new Vector3 (defaultScale * rando, defaultScale * rando, defaultScale * rando);

		if (RotateAndScale) {
			GameObject rock3D = myRock.transform.GetChild (0).gameObject;
			Vector3 center = rock3D.GetComponent<Renderer> ().bounds.center;
			//			                              new Vector3(360f*Random.value,360f*Random.value,360f*Random.value));
			rock3D.transform.RotateAround (center, Vector3.up, 360f * Random.value);
			rock3D.transform.RotateAround (center, Vector3.forward, 360f * Random.value);
			rock3D.transform.RotateAround (center, Vector3.right, 360f * Random.value);
			
			if (zValue != 0) {
				myRock.GetComponent<CircleCollider2D> ().enabled = false;	
				Destroy (myRock.GetComponent<Rigidbody2D> ());
			}

			myRock.GetComponent<Rigidbody2D> ().mass = 0.5f * Mathf.Pow (rando, 1.5f);
			myRock.gameObject.tag = "Rock";

			if (OptionalMaterial != null) {
				rock3D.GetComponent<Renderer> ().material = OptionalMaterial;	
			}
		}

		if (WithVelocity) {
			Vector3 vel = new Vector3 (0f, -Velocity * 
			                      Random.Range (1, VelocityVariance), 0f);

			myRock.GetComponent<Rigidbody2D> ().velocity = vel;
		}


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
			Start ();
			drawAgain = false;
		}
		if (SpawnMode && Time.time > startTime + PerSpawnTime) {
			startTime = Time.time;
			if (Random.value < Probability && parentObj.transform.childCount < numberOfObjects) {
				total++;
				SpawnOne ();
//				if (parentObj.transform.childCount > numberOfObjects) {
//					DeleteOne ();
//				}
			}
		}

	}

}
