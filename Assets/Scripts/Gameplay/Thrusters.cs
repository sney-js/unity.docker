using UnityEngine;
using System.Collections;

public class Thrusters
{

	Transform tt;
	private bool retracted = false;
	private Transform[] thrusters; //1:NE,2:NW,3:SE,4:SW,5:N1,6:N2,7:
//	public GameObject outline;
	Transform[] lThrus, rThrus, fThrus, bThrus, spinQThrus, spinEThrus;
	private float normalThrust1_Life, normalThrust1_Size;
	private float normalThrust2_Life, normalThrust2_Size;
	private AudioSource tt_sound;
	private float maxAmount = 2f;

	public Thrusters (GameObject thrustObj, Transform[] lThrus, Transform[] rThrus, Transform[] fThrus,
	                  Transform[] bThrus, Transform[] spinQThrus, Transform[] spinEThrus)
	{
		tt = thrustObj.transform.Find ("Flames");
		tt_sound = tt.gameObject.GetComponent<AudioSource> ();
		this.lThrus = lThrus; 
		this.rThrus = rThrus; 
		this.fThrus = fThrus;
		this.bThrus = bThrus; 
		this.spinQThrus = spinQThrus; 
		this.spinEThrus = spinEThrus;

		normalThrust1_Size = bThrus [0].gameObject.GetComponent<ParticleSystem> ().startSize;
		normalThrust1_Life = bThrus [0].gameObject.GetComponent<ParticleSystem> ().startSpeed;

		normalThrust2_Size = bThrus [0].FindChild ("Glow").gameObject.GetComponent<ParticleSystem> ().startSize;
		normalThrust2_Life = bThrus [0].FindChild ("Glow").gameObject.GetComponent<ParticleSystem> ().startSpeed;

		maxAmount = GameObject.Find ("Player").gameObject.GetComponent<Moves> ().Burner.BurnerHigh;
	}

//	void Update(){
//		toggleRetract();
//	}

	public void toggleRetract (Transform outline)
	{
		if (!retracted) {
			outline.localScale = new Vector3 (1f, 0.67f, 1f);
			retracted = true;

		} else {
			outline.localScale = new Vector3 (1f, 1f, 1f);
			retracted = false;
		}

	}

	bool accelerating = false;

	public void noAllFlame ()
	{
//		Transform t = tt.Find ("Flames");
		foreach (Transform child in tt) {
			hide (child);
		}
		if (!SoundScript.NoThrusSound) {
			if (tt_sound.isPlaying && !accelerating) {
//			float currVol = tt_sound.volume;
				tt_sound.volume -= 0.05f;
				if (tt_sound.volume <= 0) {
					//			tt_sound.volume= 0f;
					tt_sound.Pause ();
//				Debug.Log("PAUSED");
				}
			}
			if (tt_sound.isPlaying && accelerating) {
				accelerating = false;
			}
		}



	}

	void hide (Transform t)
	{
		t.gameObject.GetComponent<ParticleSystem> ().Stop ();
		Light light = t.FindChild ("glowLight").gameObject.GetComponent<Light> ();
		light.intensity = Mathf.Lerp (light.intensity, 0f, 0.4f);
		//		t.gameObject.GetComponent<Renderer> ().enabled = false;
	}

	bool fadeStart = false;

	void showArray (Transform[] thrus, float amount)
	{
		for (int i=0; i<thrus.Length; i++) {
			show (thrus [i], amount);
		}
		if (!SoundScript.NoThrusSound) {
			accelerating = true;
			if (!tt_sound.isPlaying) {
				tt_sound.Play ();
				fadeStart = true;
//			Debug.Log("STARTED");
			}
			float maxVol = Mathf.Clamp01 (amount / maxAmount);
//		Debug.Log("maxVol="+maxVol+",curr="+tt_sound.volume);

			if (fadeStart) {
//			tt_sound.volume= Mathf.Lerp(0f, Mathf.Clamp01(0.75f*amount),0.3f);
				float iter = 0.05f;
			 
				if (tt_sound.volume >= maxVol) {
					tt_sound.volume = maxVol;
					fadeStart = false;
//				Debug.Log("LERPED");
				}
				tt_sound.volume += iter;
			} else {
				tt_sound.volume = maxVol;
			}
		}

	}

	public void showDir (string s, float amount)
	{
//		Transform t = tt.Find ("Flames");

		if (s.Equals ("FWD")) {
			showArray (fThrus, amount);
		}
		if (s.Equals ("BCK")) {
			showArray (bThrus, amount);
		}
		
		if (s.Equals ("LEFT")) {
			showArray (lThrus, amount);
		}
		if (s.Equals ("RIGHT")) {
			showArray (rThrus, amount);
		}
		if (s.Equals ("RROT")) {
			showArray (spinEThrus, amount);
		}
		if (s.Equals ("LROT")) {
			showArray (spinQThrus, amount);
		}

	}
	
	void show (Transform t, float amount)
	{
//		if (amount < 0.5)
//			amount = 0.5f;

		ParticleSystem flame = t.gameObject.GetComponent<ParticleSystem> ();
		flame.startSpeed = normalThrust1_Life * amount;
		flame.startSize = normalThrust1_Size * amount;
		flame.Play ();

//		if (amount < 0.8)
//			amount = 0.8f;
		ParticleSystem flameGlow = t.FindChild ("Glow").gameObject.GetComponent<ParticleSystem> ();
		flameGlow.startSpeed = normalThrust2_Life * amount;
		flameGlow.startSize = normalThrust2_Size * amount;
		flameGlow.Play ();

		Light light = t.FindChild ("glowLight").gameObject.GetComponent<Light> ();
		light.intensity = Mathf.Lerp (0f, amount, 0.4f);

	}

}
