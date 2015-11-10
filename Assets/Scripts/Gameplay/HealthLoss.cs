using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HealthLoss : MonoBehaviour
{

	// Use this for initialization

	private Slider healthSlider;
	private float durability = 18f;
	private int healthBars = 3;

//	bool flash = true;
	public bool showOnSlider = false;
	private float health;
	private bool prevShowonSlider = false;
	private Image DamageOverlay;
	public bool loseHealth = true;

	void Start ()
	{ 
		healthSlider = GameObject.Find("Canvas/UI/Health/HealthSlider").GetComponent<Slider>();
		DamageOverlay = GameObject.Find("Canvas/DamageImage").gameObject.GetComponent<Image>();
		health = durability;
		if (showOnSlider & gameObject.name == "Player") {
			GameEvents.HealthReading = health;
			GameEvents.maxHealth = health;
		}

	}

	void Update ()
	{
		if (showOnSlider != prevShowonSlider) {
			prevShowonSlider = showOnSlider;
			if (showOnSlider) {
				healthSlider.maxValue = durability;
				healthSlider.value = health;
				healthSlider.maxValue = durability;
			}
		}
//		if (!showOnSlider) FlashScreen();
	}

	void OnCollisionEnter2D (Collision2D other)
	{
		Rigidbody2D rOther = other.gameObject.GetComponent<Rigidbody2D> ();
		Rigidbody2D rMy = gameObject.GetComponent<Rigidbody2D> ();
		
		var vFinal = rOther.mass * other.relativeVelocity / (rMy.mass + rOther.mass);
		var impulse = vFinal * rMy.mass;
//		print ("col:"+impulse);
		if (DockAreaTrigger.inDockArea && impulse.magnitude < 3f) {} 
		else if (loseHealth) {
			ReduceHealth(impulse.magnitude);
		}


	}

	void ReduceHealth(float amount){

		float ratio = durability/healthBars +0.01f;
		float reduceBy = ratio*((int) (amount/ratio))+ratio;
		Debug.Log ("Health [" + gameObject.name + "] = " + amount+ ":NORM: "+reduceBy);
		if (showOnSlider && //amount>0.8f &&
		    !GameEvents.LevelFail && !GameEvents.LevelSuccess) {
			
//			health -= reduceBy;
			health -= amount;
			healthSlider.value = health;

			if (gameObject.name == "Player") {
				GameEvents.HealthReading = healthSlider.value;
			}
			StartCoroutine( AnimationScript.FlashScreen(DamageOverlay,0.5f));
//			StartCoroutine(CameraShaker.Instance.MyShake(amount, Mathf.Pow(amount, 1f/3f)));
			float norm = Mathf.Abs (Mathf.Log(amount*2f)*2f);
			norm = norm/3f;
			float timeNorm = norm / 1.5f;
//			print("amount="+amount+" ,norm:"+norm+ " ,timeNorm="+timeNorm);
			CameraShaker.Instance.ShakeOnce(norm, 2f, 0.1f, Mathf.Clamp(norm, 0.5f, 1.5f));
		}
	}




}