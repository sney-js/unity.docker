using UnityEngine;
using System.Collections;

public class InfiniteStars : MonoBehaviour
{

	private Transform tx;
	private ParticleSystem.Particle[] points;
	public bool Perlin=true;
	public int seed = 100;
	public int starsMax = 1000;
	public float starSize = 5;
	[Range(1f,5f)]
	public float StarSizeNoise = 2f;
	public float sizeXY = 5f;
	public float sizeZ = 600f;
	[Range(1f,2f)]
	public float DepthZ = 2f;
	private int total;
	[Range(0,0.95f)]
	public float prob = 0.75f;
	[Range(0,0.95f)]
	public float prob2 = 0.75f;
	[Range(0.00001f, 0.1f)]
	public float zoom = 0.1f; // play with this to zoom into the noise field
	[Range(0.00001f, 0.1f)]
	public float zoom2 = 0.002f; // play with this to zoom into the noise field
	public bool createNow = false;
//	public Gradient gradient;

	void Start ()
	{
		total = 0;
		if (seed!=-1)	Random.seed = seed;
		tx = transform.parent.transform;
		if (!Perlin)CreateStars();
		else CreateStarsPerlin ();
		GetComponent<ParticleSystem> ().SetParticles (points, points.Length);
	}
	
	//--------------------------------------------------------------------

	private void CreateStarsPerlin ()
	{
		Vector2 shift = new Vector2 (0, 0); // play with this to shift map around
		points = new ParticleSystem.Particle[starsMax];
		int i = 0;
		while (i<starsMax) {
			float x = Random.Range (0, sizeXY * 2f);
			float y = Random.Range (0, sizeXY * 2f);
//			for(int y = 0; y < mapheight; y++){

			Vector2 posParent = zoom2 * (new Vector2 (x, y)) + shift;
			float noiseParent = Mathf.PerlinNoise (posParent.x, posParent.y);


			Vector2 pos = zoom * (new Vector2 (x, y)) + shift;
			float noise = Mathf.PerlinNoise (pos.x, pos.y);



			x -= sizeXY;
			y -= sizeXY;
			if (noise > prob && noiseParent > prob2) {

				float cutpoint = ((1-prob)/2+prob); //0.7<0.85<1
		
				float dz = DepthZ;
				dz = (1-(noise-prob));//*(2f-DepthZ);
//				else dz = (1+(cutpoint-noise))*DepthZ;

//				if (Random.value>0.5){
//					dz=dz>1?1-(dz-1):1+(1-dz);
//				}
				dz = (Mathf.Clamp(dz, 2f-DepthZ, DepthZ));
				float z = sizeZ*dz;
				AddParticle (new Vector3 (x, y, z));
				i++;
			}
			else if (Random.value>0.95f){
				float z = sizeZ;
				AddParticle (new Vector3 (x, y, z));
				i++;
			}

		}
	}

	void AddParticle (Vector3 pos)
	{
		int i = total;
		points [i].position = pos;
		points [i].color = GetComponent<ParticleSystem> ().startColor;//getRandomColor();
		points [i].rotation = GetComponent<ParticleSystem> ().startRotation;
//		float size = starSize * Random.Range (1f, StarSizeNoise);

		float rando = Random.Range(0.5f, 1f+StarSizeNoise/5f);//Range (1f, StarSizeNoise/5f+1f);
		if (Random.value>0.98f){
			rando = Random.Range (rando, StarSizeNoise);
//			if (Random.value>0.6f) rando = StarSizeNoise*3f;
		}

		points [i].size = starSize*rando;
		total++;
	}
	//----------------------------------------------------------------------------
	private void CreateStars ()
	{
		points = new ParticleSystem.Particle[starsMax];
		int i = 0;
		while (i<starsMax) {
//			points[i].position = Random.insideUnitSphere.normalized * starDistance + tx.position;;//GetRandomPos();
			Vector3 pos = GetRandomPos ();
			if (pos != Vector3.one) {
				AddParticle(pos);
				i++;
			}
		}
	}

	private Vector3 GetRandomPos ()
	{

		float randX = Random.Range (-sizeXY, sizeXY);
		float randY = Random.Range (-sizeXY, sizeXY);
		//+tx.position.x;
//		float rat1 = Mathf.PerlinNoise (randX, randY);
//
//		randX = Random.Range (-sizeXY, sizeXY);
//		randY = Random.Range (-sizeXY, sizeXY);
//		float rat2 = Mathf.PerlinNoise (randX, randY);
////		print ("x"+Mathf.Floor(vx%50)+"perlin:"+Mathf.Floor(rat*10));
//		float vy = (rat2 - 0.5f) * 2f * sizeXY;// Random.Range(-sizeXY,sizeXY);//+tx.position.y;
//
//		//+tx.position.x;
////		rat = Mathf.PerlinNoise(rand, seedX);
//		float vx = (rat1 - 0.5f) * 2f * sizeXY;
//
//		if (rat1 > prob) {
//			//		if (rat>prob) 
//
//		}
//		if (Random.value>0.3)
//		else
//			tmp = new Vector3(Random.Range(-1400f,1400f),Random.Range(-1700f,1700f),Random.Range(200f,255f));

					float vz = Random.Range (sizeZ, sizeZ * DepthZ);
					return new Vector3 (randX, randY, vz);

	}

	Color getRandomColor ()
	{
		Color cl = new Color (1, 1, 1, 1);
		if (Random.value > 0.3) {
			cl.r = 1.0f;
			cl.b = 1.0f;
			cl.g = 1.0f;
			return cl;
		}

		cl.b = Random.Range (0.4f, 1.0f);

		if (cl.b >= 0.7f) {
			cl.r = Random.Range (0.7f, 0.8f);
			cl.g = Random.Range (0.7f, 0.8f);
		} else if (cl.b < 0.7f) {
			cl.b = Random.Range (0.7f, 0.9f);
			cl.r = Random.Range (0.8f, 1.0f);
			cl.g = Random.Range (0.6f, 0.9f);
		}

		return cl;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (createNow) { 
//			CreateStars(); 
//			setPos();
			Start ();
			createNow = false;
		}
	}
}