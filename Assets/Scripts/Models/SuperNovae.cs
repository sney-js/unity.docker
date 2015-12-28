using UnityEngine;
using System.Collections;

public class SuperNovae : MonoBehaviour {
//	private Transform tx;
	private ParticleSystem.Particle[] points;
	public bool Perlin=true;
	public int seed = 100;
	public int starsMax = 6;
	public float starSize = 2000;
	[Range(1f,5f)]
	public float StarSizeNoise = 2f;
	[Range(0f,360f)]
	public float RotationNoise = 360f;
	public float RangeXY = 5f;
	public float RangeZ = 600f;
	[Range(1f, 100f)]
	public float DepthZ;
	public bool createNow = false;
	//	public Gradient gradient;
	int total;

	//--------------------------------------------Trailer-----------------------------------//
	public Color32 col1, col2, col3, col4, col5;
	int currColor;
	private Color32[] cols= new Color32[5];
	public float interval=1;
	bool isTrailer;
	//--------------------------------------------X-----------------------------------//
	void Start ()
	{
		isTrailer=false;
		if (isTrailer){
		//		cols[0] = col1; cols[1]= col2;cols[2]= col3;cols[3]= col4;cols[4]= col5;
		//		currColor=0;
		//		StartCoroutine(throwPlayer(10f));
		}
		CreateStars();
	}
	IEnumerator throwPlayer (float overTime)
	{
		float startTime = Time.time;
		while (Time.time < startTime + overTime) {
			yield return new WaitForSeconds(interval);
			CreateStars();
		}
	}

	//----------------------------------------------------------------------------
	private void CreateStars ()
	{
		total = 0;
		Random.seed = seed==-1?Random.Range(0,100):seed;
//		tx = transform.parent.transform;
		points = new ParticleSystem.Particle[starsMax];
		int i = 0;
		//print(cols[currColor].r);
		while (i<starsMax) {
			//			points[i].position = Random.insideUnitSphere.normalized * starDistance + tx.position;;//GetRandomPos();
			Vector3 pos = GetRandomPos ();
			if (pos != Vector3.one) {
				AddParticle(pos);
				i++;
			}
		}
		if (isTrailer){
			currColor++;
			if (currColor>=cols.Length) currColor=0;
		}
		GetComponent<ParticleSystem> ().SetParticles (points, points.Length);
	}

	
	void AddParticle (Vector3 pos)
	{
		int i = total;
		points [i].position = pos;
		points [i].color = 
		GetComponent<ParticleSystem> ().startColor;//getRandomColor();
		if (isTrailer){	
			points [i].color = cols[currColor];
//			print(cols[currColor]);
		}
		points [i].rotation = Random.Range(0f,RotationNoise);
		//		float size = starSize * Random.Range (1f, StarSizeNoise);
		
		float rando = Random.Range (1f, StarSizeNoise);

		
		points [i].size = starSize*rando;
		total++;
	}

	
	private Vector3 GetRandomPos ()
	{
		
		float randX = Random.Range (-RangeXY, RangeXY);
		float randY = Random.Range (-RangeXY, RangeXY);
		float vz = Random.Range (RangeZ, RangeZ * DepthZ);
		return new Vector3 (randX, randY, vz);
		
	}
	// Update is called once per frame
	void Update ()
	{
		if (createNow) { 
			CreateStars ();
			createNow = false;
		}
	}
}