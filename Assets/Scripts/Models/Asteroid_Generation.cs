using UnityEngine;
using System.Collections;

public class Asteroid_Generation : MonoBehaviour {

	// Use this for initialization
	public GameObject[] rocks;
//	public GameObject asteroid2D;
	public int numberOfObjects = 5;
//	public Sprite[] sprites;
	public float defaultScale =2f;
	public float range = 100;
	public int seed = 123;
	public float zValue=0f;
	public GameObject parentObj;
	

	private GameObject getRandRock() {
		return rocks[Random.Range(0, rocks.Length - 1)];
	}


	void Start() {
		Random.seed=seed;
//		Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Asteroids/");
//		Debug.Log(sprites.Length);
		for (int i = 0; i < numberOfObjects; i++) {

			//-----------------------random rock---------------------------------------
			GameObject theRock = getRandRock();

			//-------------------------------position, size, rotation-----------------------------------------------
//			GameObject tempAst = asteroid2D;
			Vector3 pos = new Vector3(Random.Range(-range, range), Random.Range(-range, range),zValue);

//			GameObject myAsteroid = (GameObject) Instantiate(tempAst, pos, Quaternion.identity);
			//new Vector3(0f,0f,0f)
			GameObject myRock =(GameObject) Instantiate(theRock, pos, Quaternion.identity);
			myRock.name = i + "_Rock";

			myRock.transform.parent = parentObj.transform;
			myRock.layer = 10;

//			myRock.transform.position=Vector3.zero;


			float rando = Random.Range(0.3f,2f);
			if (rando>1.8) rando = Random.Range(2f, 8f);
			myRock.transform.localScale = new Vector3(defaultScale*rando, defaultScale*rando, defaultScale*rando);


			GameObject rock3D = myRock.transform.GetChild(0).gameObject;
			Vector3 center = rock3D.GetComponent<Renderer>().bounds.center;
			//			                              new Vector3(360f*Random.value,360f*Random.value,360f*Random.value));
			rock3D.transform.RotateAround(center, Vector3.up,360f*Random.value);
			rock3D.transform.RotateAround(center, Vector3.forward,360f*Random.value);
			rock3D.transform.RotateAround(center, Vector3.right,360f*Random.value);

			if (zValue!=0){
				myRock.GetComponent<CircleCollider2D>().enabled=false;	
				Destroy( myRock.GetComponent<Rigidbody2D>());
			}
			//----------------------------------------Colour----------------------------------------
//			tempPrefab.GetComponent<SpriteRenderer>().color = GetRandColor();

			//----------------------------------Rigidbody, collision--------------------------------

			myRock.GetComponent<Rigidbody2D>().mass = 0.5f*Mathf.Pow(rando,1.5f);
			myRock.gameObject.tag = "Rock";
//			Destroy(tempPrefab.GetComponent<CircleCollider2D>());
//			tempPrefab.AddComponent<CircleCollider2D>();
//			Debug.Log(i+" = "+center);
//			Debug.Log(i+" DIFF = "+new Vector3(
//				pos.x -  center.x, pos.y-center.y, pos.z-center.z
//				));
//			myRock.transform.position = new Vector3(
//					2*pos.x -  center.x, 2*pos.y-center.y, 2*pos.z-center.z
//				);


			//---------------------------instantiate finally----------------------

		}
	}

	Color GetRandColor(){

		float qq = Random.Range(100f,200f)/255;

		Color col = new Color(qq,qq,qq,1);
		col.r = col.r*Random.Range(1.1f,1.2f);
		col.b = col.b*Random.Range(0.9f,1f);

		return col;
	}
	// Update is called once per frame
	void Update () {
	
	}

	/*
	 * 	void Start() {
		Random.seed=seed;
		Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Asteroids/");
		Debug.Log(sprites.Length);
		for (int i = 0; i < numberOfObjects; i++) {

			GameObject tempPrefab = prefab;

			//-----------------------random sprites---------------------------------------

			int rand = (int) Random.Range(0f,sprites.Length-1);
			tempPrefab.GetComponent<SpriteRenderer>().sprite = sprites[rand];

			//-------------------------------position, size, rotation-----------------------------------------------

			Vector3 pos = new Vector3(Random.Range(-range, range), Random.Range(-range, range),0f);

			float rando = Random.Range(0.3f,2f);
			if (rando>1.8) rando = Random.Range(2f, 8f);
			tempPrefab.transform.localScale = new Vector3(defaultScale*rando, defaultScale*rando, 1f);
			tempPrefab.transform.Rotate(new Vector3(0f,0f,360f*Random.value));

			//----------------------------------------Colour----------------------------------------
			tempPrefab.GetComponent<SpriteRenderer>().color = GetRandColor();

			//----------------------------------Rigidbody, collision--------------------------------

			tempPrefab.GetComponent<Rigidbody2D>().mass = 0.2f*2f*rando;
//			Destroy(tempPrefab.GetComponent<CircleCollider2D>());
//			tempPrefab.AddComponent<CircleCollider2D>();


			//---------------------------instantiate finally----------------------
			Instantiate(tempPrefab, pos, Quaternion.identity);
		}
	}
	*/
}
