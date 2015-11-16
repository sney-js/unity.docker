using UnityEngine;
using System.Collections;

public class PrefabGeneration : MonoBehaviour
{

	// Use this for initialization
	public int seed = 123;
	public bool drawAgain = false;
	public GameObject parentObj;
	public Material OptionalMaterial;
	public string namePref = "_item";
	public int numberOfObjects = 5;
	public GameObject[] prefs;

	public bool AtStart = true;
	public bool SpawnMode = true;

	public float rangeXY = 100;
	public float zValue = 0f;
	[Range(1,5)] 
	public float zNoise = 1f;

	public bool InCircle = false;
	public bool PerfectCircle = false;
	public float RadiusFrom = 0f;

	public float defaultScale = 1f;
	public bool RotateAndScale = false;
	public bool AsteroidScaling = false;

	public bool WithVelocity = false;
	[Range(0,1)] public float ProbVel = 0.2f;
	public float PerSpawnTime = 0.2f;
	[Range(0,1)] public float	Probability = 1f;
	
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

	void SpawnOne ()
	{
		//-----------------------random rock---------------------------------------
		GameObject theRock = getRandRock ();
		
		//-------------------------------position, size, rotation-----------------------------------------------
		Vector3 pos;
		if (!InCircle) {
			pos = new Vector3 (Random.Range (-rangeXY, rangeXY), Random.Range (-rangeXY, rangeXY), zValue*Random.Range(1f,zNoise));
		} else {
			if (PerfectCircle) {
				float pointNum = (total * 1.0f) / numberOfObjects;
				
				//angle along the unit circle for placing points
				float angle = pointNum * Mathf.PI * 2;
				
				float x = Mathf.Sin (angle) * rangeXY;
				float y = Mathf.Cos (angle) * rangeXY;
				pos = new Vector3 (x, y, zValue);
			} else {
				Vector2 circle = Random.insideUnitCircle;
				circle = circle.normalized * Random.Range (RadiusFrom, rangeXY);
				float xValue = circle.x;
				float yValue = circle.y;

//			float ringWidth = rangeXY - RadiusFrom;
//			 xValue = circle.x>=0?RadiusFrom+ circle.x*ringWidth:-RadiusFrom+ circle.x*ringWidth;
//			 yValue = circle.y>=0?RadiusFrom+ circle.y*ringWidth:-RadiusFrom+ circle.y*ringWidth;
				pos = new Vector3 (xValue, yValue, zValue);
			}
		}
		GameObject myRock = (GameObject)Instantiate (theRock, pos, Quaternion.identity);
		myRock.name = total + namePref;

		
		myRock.transform.parent = parentObj.transform;
		myRock.transform.localPosition=pos;
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
			if (Random.value < ProbVel) {
				Vector3 vel = new Vector3 (Random.Range (-3f, 3f), Random.Range (-3f, 3f), 0f);
				if (Random.value < ProbVel) {
					vel *= Random.Range (6f, 12f);
				}
				myRock.GetComponent<Rigidbody2D> ().velocity = vel;
			}
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
			total = 0;
			Start ();
			drawAgain = false;
		}
		if (SpawnMode && Time.time > startTime + PerSpawnTime) {
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
