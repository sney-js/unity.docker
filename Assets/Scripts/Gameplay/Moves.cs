
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Moves : MonoBehaviour
{
	
	private Rigidbody2D body;
	public float speed = 5f;
	public float rotSpeed = 75f;
	private Slider fuelSlider;
	public float initialFuel = 1000;
	private Text fuelText;
	public GameObject thrustersObj;
	public static bool allowMoving = true;
	public GameObject Rotator3D;
	private float rotTotal = 0;
	public BurnerValues Burner;
	public float fuelDensity = 10000f;
	public bool unlimitedFuel = false;
	private Thrusters thrusterScript;
	private float fuelGuage;
	private float initialMass;
	[Range (1, 2)]
	public int
		navType = 1;
	private bool stabilizeOn;

	//------------------------------------------------Editor Classes----------------------------------------

	[System.Serializable]
	public class BurnerValues
	{
		public float BurnerLess = 0.2f, BurnerNormal = 1.0f, BurnerHigh = 1.8f;
		//		[HideInInspector]
	}

	//-----------------------------------------------START----------------------------------------------------

	void Start ()
	{ 
		if (thrustersObj != null) {
			Transform tt = thrustersObj.transform.Find ("Flames");

			Transform[] lThrus = { tt.FindChild ("FlameNE"), tt.FindChild ("FlameSE") };
			Transform[] rThrus = { tt.FindChild ("FlameNW"), tt.FindChild ("FlameSW") };
			Transform[] fThrus = { tt.FindChild ("FlameSR"), tt.FindChild ("FlameSL") };
			Transform[] bThrus = { tt.FindChild ("FlameNR"), tt.FindChild ("FlameNL") };
			Transform[] spinQThrus = { tt.FindChild ("FlameNE"), tt.FindChild ("FlameSW") };
			Transform[] spinEThrus = { tt.FindChild ("FlameNW"), tt.FindChild ("FlameSE") };

			thrusterScript = new Thrusters (thrustersObj, lThrus, rThrus, fThrus, bThrus, spinQThrus, spinEThrus);

		}
		allowMoving = true;
		body = GetComponent<Rigidbody2D> ();
		fuelSlider = GameObject.Find ("Canvas/UI/Fuel/FuelSlider").GetComponent<Slider> ();
		fuelText = GameObject.Find ("Canvas/UI/Fuel/Guage").GetComponent<Text> ();
		fuelGuage = initialFuel;
		GameEvents.maxFuel = fuelGuage;
		fuelSlider.maxValue = fuelGuage;
		SetFuelLevel ();
		initialMass = gameObject.GetComponent<Rigidbody2D> ().mass;
		initialMass += (initialFuel / fuelDensity);
		stabilizeOn = Application.loadedLevel == 1 || GameManager.GetStabiliserPref() ? true : false;

	}

	void Update ()
	{
		if (Input.GetKeyUp (KeyCode.X)) {
			stabilizeOn = !stabilizeOn;
			GameManager.SetStabiliserPref (stabilizeOn ? 1 : 0);
//			print ("X key up!!! " + stabilizeOn);
			
			StartCoroutine (AnimationScript.FlashScreen (null, 1.5f, 2, "Stabilisation " + (stabilizeOn ? "On" : "Off")));
		
			
		}
	}

	void FixedUpdate ()
	{
		if (thrustersObj != null) {
			thrusterScript.noAllFlame ();
		}
		//-----------------------------------major minor------------------------------
		float amount = Burner.BurnerNormal;
//		if (Input.GetKey (KeyCode.LeftShift)) {
//			amount = Burner.BurnerHigh;
//		}
//		if (Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.LeftAlt)) {
//			amount = Burner.BurnerLess;
//		}
		amount = getThrusterAmount (amount);
		//-----------------------------------------------------------------------------
		if (allowMoving && (fuelGuage > 0 || unlimitedFuel)) {
			navigate (amount);
//			if (!unlimitedFuel && Time.time - lastMassCheck > 1f) {
//				gameObject.GetComponent<Rigidbody2D> ().mass = initialMass + ((fuelGuage - initialFuel) / fuelDensity);
//				lastMassCheck = Time.time;
//				print ("decreasing");
//			}
		}
	}

	private float lastMassCheck = 0f;

	void navigate (float amount)
	{
		if (MInput.CONTROL == MInput.KEYBOARD2) {
			navigateAlternate (amount);
			return;
		}
		if (MInput.CONTROL == MInput.MIXED) {
			navigateFollow (amount);
			return;
		} else if (MInput.CONTROL == MInput.KEYBOARD1) {
		
			/*
		 * Input.GetAxis("Vertical") > 0 // gets forward
			Input.GetAxis("Vertical") < 0 // gets backward
			Input.GetAxis("Horizontal") > 0 // gets right
			Input.GetAxis("Horizontal") < 0 // gets left
			*/
			if (HeldMoveKeys ()) {

				float delta = amount / 30;
				rotTotal = Mathf.Clamp (rotTotal, -5f, 5f);
				Vector3 position = body.transform.position;

				if (MInput.sleft) {
					body.AddForceAtPosition (body.transform.up * (speed * amount), position);
					if (rotTotal - delta >= -3) {
						rotTotal += -delta;
						Rotator3D.transform.Rotate (new Vector3 (0f, delta, 0f));
					}
					ThrusterAt (0, amount);
				}
		
				if (MInput.fwd) {
					body.AddForceAtPosition (body.transform.right * (speed * amount), position);
			
					ThrusterAt (1, amount);
				}
		
				if (MInput.sright) {
					body.AddForceAtPosition (-body.transform.up * (speed * amount), position);
					if (rotTotal + delta <= 3) {
						rotTotal += delta;
						Rotator3D.transform.Rotate (new Vector3 (0f, -delta, 0f));
					}
					ThrusterAt (2, amount);
				}
		
				if (MInput.bck) {
					body.AddForceAtPosition (-body.transform.right * (speed * amount), position);
					ThrusterAt (3, amount);
				}
				//  rotation-------------------------------------------------------
		
				if (MInput.rright) {
					body.AddTorque (-rotSpeed * amount);
					ThrusterAt (5, amount);
				} 
				if (MInput.rleft) {
					body.AddTorque (rotSpeed * amount);
					ThrusterAt (4, amount);
				}
				//-------------------------------------------------------------------

//		if (HeldMoveKeys ()) {
				GameEvents.startCounting = true;
			} else if (stabilizeOn && (body.velocity != Vector2.zero || body.angularVelocity != 0f)) {
				Stabilize (amount);
			} else {
				stabilizing = false;
			}
		}
	}

	void navigateAlternate (float amount)
	{
		/*
		 * Input.GetAxis("Vertical") > 0 // gets forward
			Input.GetAxis("Vertical") < 0 // gets backward
			Input.GetAxis("Horizontal") > 0 // gets right
			Input.GetAxis("Horizontal") < 0 // gets left
			*/
		float lim = speed * 30f;
//		float delta = amount / 30;
		rotTotal = Mathf.Clamp (rotTotal, -5f, 5f);
//		Vector3 position = body.transform.position;
		if (MInput.sleft) {
			body.velocity = Vector2.MoveTowards (body.velocity, new Vector2 (-lim, 0), speed * amount * Time.deltaTime);
//			ThrustDirectionNormalise(amount,-1f);
		}
		
		if (MInput.fwd) {
			body.velocity = Vector2.MoveTowards (body.velocity, new Vector2 (0, lim), speed * amount * Time.deltaTime);
//			ThrustDirectionNormalise(amount,-1f);
		}
		
		if (MInput.sright) {
			body.velocity = Vector2.MoveTowards (body.velocity, new Vector2 (lim, 0), speed * amount * Time.deltaTime);
//			ThrustDirectionNormalise(amount,-1f);
		}
		
		if (MInput.bck) {
			body.velocity = Vector2.MoveTowards (body.velocity, new Vector2 (0, -lim), speed * amount * Time.deltaTime);
//			ThrustDirectionNormalise(amount,-1f);
		}
		//  rotation-------------------------------------------------------
		
		if (MInput.rright) {
			body.AddTorque (-rotSpeed * amount);
			ThrusterAt (5, amount);
		} 
		if (MInput.rleft) {
			body.AddTorque (rotSpeed * amount);
			ThrusterAt (4, amount);
		}
		//-------------------------------------------------------------------
		if (HeldMoveKeys ()) {
			GameEvents.startCounting = true;
		}

	}

	void navigateFollow (float amount)
	{
		if (HeldMoveKeys ()) {
			GameEvents.startCounting = true;
		}
		if (Input.GetMouseButton (0)) {
			
			Vector3 mousePos = Input.mousePosition - Camera.main.transform.position;
			mousePos.z = Camera.main.transform.position.z;
			print (mousePos);
			Vector3 pos = Camera.main.ScreenToWorldPoint (mousePos);
			pos.z = 0;
//			pos.x = transform.position.x- pos.x;
//			pos.y = transform.position.y-pos.y;
			print ("2: " + pos);
//			pos -= transform.position;
			StartCoroutine (AnimationScript.AnimatePrefabXYZ (gameObject, pos, 1f, 0f));
//			this.transform.position = pos;
		}

	}

	float getThrusterAmount (float amount)
	{
		float am = amount;
//		float sp = body.velocity.magnitude;
//		am = Mathf.Clamp(sp,0.1f,2f);
//		if (sp>am) am = sp*0.5f;
//		print("am: "+am +" ,sp: "+sp);
		am = 2f;
		return am;
	}

	public static bool HeldMoveKeys ()
	{

		return MInput.rleft || MInput.fwd || MInput.rright
		|| MInput.sleft || MInput.bck || MInput.sright
		|| Input.GetKeyUp (KeyCode.X);
	}

	void StabilizeRotation (float amount)
	{
		if (fuelGuage > 0 && body.angularVelocity != 0) {
			reduceFuel (amount);
			body.angularVelocity = Mathf.MoveTowards (body.angularVelocity, 0f, 3f * rotSpeed * amount * Time.deltaTime);
			
			if (body.angularVelocity > 0)
				ThrusterAt (5, amount);
			else if (body.angularVelocity < 0)
				ThrusterAt (4, amount);
			
			ThrustDirectionNormalise (amount, 1);
		}
	}

	bool stabilizing = false;
	void Stabilize (float amount)
	{
		if (fuelGuage > 0 || unlimitedFuel) {
			stabilizing = true;
			if (body.velocity != Vector2.zero || body.angularVelocity != 0) {
				reduceFuel (amount * 0.3f);
			}
			body.velocity = Vector2.MoveTowards (body.velocity, Vector2.zero, speed * amount * Time.deltaTime);
			body.angularVelocity = Mathf.MoveTowards (body.angularVelocity, 0f, 3f * rotSpeed * amount * Time.deltaTime);

			if (body.angularVelocity > 0)
				ThrusterAt (5, amount);
			else if (body.angularVelocity < 0)
				ThrusterAt (4, amount);

			ThrustDirectionNormalise (amount, 1);
		}
	}

	void ThrustDirectionNormalise (float amount, float inv)
	{
		Vector2 vl = body.velocity;
		float rt = body.transform.eulerAngles.z;
		//		VelocityThrusterDirection(body.transform.eulerAngles.z, body.velocity, amount);
		
		Vector2 norm = RotateV2 (vl, inv == -1f ? rt : -rt);

		float maxAmountX = amount, maxAmountY = amount;
		if (Mathf.Abs (norm.x) <= Mathf.Abs (norm.y))
			maxAmountX = Mathf.Abs (norm.x) / Mathf.Abs (norm.y) * amount;
		else
			maxAmountY = Mathf.Abs (norm.y) / Mathf.Abs (norm.x) * amount;
		if (norm.x > 0 && norm.y != 0)
			ThrusterAt (3, maxAmountX);
		if (norm.x < 0 && norm.y != 0)
			ThrusterAt (1, maxAmountX);
		if (norm.y > 0 && norm.x != 0)
			ThrusterAt (2, maxAmountY);
		if (norm.y < 0 && norm.x != 0)
			ThrusterAt (0, maxAmountY);
	}

	/**
	 *location = 0:left,1:fwd,2:right,3:bck,4:lrot,5:rrot 
	 */
	void ThrusterAt (int location, float amount)
	{
		if (thrustersObj != null) {
			switch (location) {
			case 0:
				thrusterScript.showDir ("LEFT", amount);
				break;
			case 1:
				thrusterScript.showDir ("FWD", amount);
				break;
			case 2:
				thrusterScript.showDir ("RIGHT", amount);
				break;
			case 3:
				thrusterScript.showDir ("BCK", amount);
				break;
			case 4:
				thrusterScript.showDir ("LROT", amount);
				break;
			case 5:
				thrusterScript.showDir ("RROT", amount);
				break;
			}
		}
		if (fuelSlider != null && !stabilizing) {
			reduceFuel (amount / 2f);
		}
	}

	public static Vector2 RotateV2 (Vector2 v, float degrees)
	{
		float sin = Mathf.Sin (degrees * Mathf.Deg2Rad);
		float cos = Mathf.Cos (degrees * Mathf.Deg2Rad);
			
		float tx = v.x;
		float ty = v.y;
		v.x = (cos * tx) - (sin * ty);
		v.y = (sin * tx) + (cos * ty);
		return v;
	}

	public void reduceFuel (float amount)
	{
		if (!unlimitedFuel) {
			fuelGuage -= amount;
			fuelGuage = Mathf.Clamp (fuelGuage, 0, initialFuel);
			SetFuelLevel ();
		}
	}

	float lastFuelUpdate = 0f;

	void SetFuelLevel ()
	{
		GameEvents.FuelReading = fuelGuage;
		if (Time.time > lastFuelUpdate + 0.06f) {
			lastFuelUpdate = Time.time;
			fuelSlider.value = fuelGuage;
			fuelText.text = ((int)fuelGuage).ToString ();
		}
	}

}

