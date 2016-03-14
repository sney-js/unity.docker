using UnityEngine;
using UnityEngine.UI;


using System.Collections;

public class GameEvents : MonoBehaviour
{

	public static GameEvents Instance;
	public WinTriggers WinWithTriggers;
	public loseTriggers LoseWithTriggers;
	public bool noWinLose = false;
	public static float maxFuel, maxHealth, FuelReading, HealthReading ;
	public static int Score;
	public static float timeTaken;
	public static bool dockedWithTarget, docked, dockedWithSecondary, startCounting;
	public static bool LevelSuccess, LevelFail;
	public static bool StopListeningKeys, StopListeningKeysMain;
	private Text timeText, scoreText, infoText;
//	private float currTimeSpeed;
//	private bool prevDocked = false;
	private int failMsgID;
	private float prevScore;
	private bool infoMsgSet = false;
	private GameObject PauseIcon;
	private GameObject player;
	bool cheated;

	//-----------------------------------CLASSES---------------------------------------------------

	[System.Serializable]
	public class WinTriggers
	{
		public bool dockSuccess = true, timeWin = true, scoreWin = true, FullHealthWin, NoDockSecWin, FuelWin;
		public GameObject DockTo;
		public int MinScore = 10, MustScore = -1, MinFuel = 2000;
		public float MinTime = 80f;
		public GameObject DockSecondary;
		public bool JustWinNow = false;
	}

	[System.Serializable]
	public class loseTriggers
	{
		public bool noHealth, noFuel, hasCountdown;
		public float CountdownTimer = 60;
	}

	//---------------------------------------START-------------------------------------------

	void Awake ()
	{
		Instance = this;
	}

	void init ()
	{
		StopListeningKeys = false;
		StopListeningKeysMain = false;
		startCounting = false;
		LevelFail = false;
		LevelSuccess = false;
		dockedWithTarget = false;
		dockedWithSecondary = false;

		docked = false;
		Score = 0;
		prevScore = -1;
		timeTaken = 0;
		failMsgID = 0;
	}

	void Start ()
	{

		init ();

		player = GameObject.Find ("Player").gameObject;
		Transform UI = GameObject.Find ("Canvas/UI").transform;
		timeText = UI.Find ("Time").gameObject.GetComponent<Text> ();
		scoreText = UI.Find ("Score").gameObject.GetComponent<Text> ();
		infoText = GameObject.Find ("Canvas/InfoOverImage/FailText").gameObject.GetComponent<Text> ();
		PauseIcon = UI.FindChild ("PauseIcon").gameObject;
		scoreText.enabled = WinWithTriggers.scoreWin || WinWithTriggers.MustScore > 0;
		scoreText.text = Score.ToString ("00");

		if (LoseWithTriggers.hasCountdown)
			timeTaken = LoseWithTriggers.CountdownTimer;
		
		updateTime (timeTaken);
		
	}

	void Update ()
	{
		if (!noWinLose) {
//		print ("Success: " + GameEvents.LevelSuccess);
			GameFeaturesChecks ();
			checkForFail ();
			if (!LevelSuccess && !LevelFail) {
				if (startCounting) {
					if (LoseWithTriggers.hasCountdown) {
						timeTaken -= Time.deltaTime;
					} else {
						timeTaken += Time.deltaTime;
					} 
					updateTime (Mathf.Clamp (timeTaken, 0, float.PositiveInfinity));
					if ((WinWithTriggers.scoreWin || WinWithTriggers.MustScore > 0) && prevScore != Score) {
						scoreText.text = Score.ToString ("00");
						prevScore = Score;
					}
				}
			}
		}
	}

	//----------------------------------------------------------------------------------------------------

	void SimulateSpeedCheck ()
	{
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			Time.timeScale = 0.02f;
		}
		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			Time.timeScale = 0.2f;
		}
		if (Input.GetKeyDown (KeyCode.Alpha3)) {
			Time.timeScale = 1;
		}
		if (Input.GetKeyDown (KeyCode.Alpha4)) {
			Time.timeScale = 2f;
		}
		if (Input.GetKeyDown (KeyCode.Alpha5)) {
			Time.timeScale = 8f;
		}
		if (Input.GetKeyDown (KeyCode.Alpha6)) {
			Time.timeScale = 40f;
		}
	}

	void GameFeaturesChecks ()
	{

//		if (!StopListeningKeys)	SimulateSpeedCheck ();

		if (Input.GetKeyDown (KeyCode.Space) && !StopListeningKeysMain) {
			RestartLevel ();
		}

		if (Input.GetKey (KeyCode.LeftControl) && Input.GetKey (KeyCode.LeftShift) && Input.GetKeyDown (KeyCode.K)) {
			GameEvents.CheatOn (true);
		}

	}

	public static void PlayerFuelUnlimited(bool isUnlimited){
		Instance.player.GetComponent<Moves> ().unlimitedFuel = isUnlimited;
	}

	public static void CheatOn (bool indicate)
	{
		Instance.cheated = true;
		Instance.LoseWithTriggers.noHealth = false;
		Instance.player.GetComponent<Moves> ().unlimitedFuel = true;
		ButtonPresses.DimFuel (true);
		ButtonPresses.DimHealth (true);
//		Camera.main.GetComponent<CameraScript> ().FollowsBounds = false;
		//				Image flash = Instance.infoText.transform.parent.GetComponent<Image> ();
		if (indicate)
		Instance.StartCoroutine (AnimationScript.FlashScreen (null, 1f, 0, "God mode active"));
	}

	public static void CheatOff (bool indicate)
	{
		Instance.cheated = false;
		Instance.LoseWithTriggers.noHealth = true;
		Instance.player.GetComponent<Moves> ().unlimitedFuel = false;
		ButtonPresses.DimFuel (false);
		ButtonPresses.DimHealth (false);
		Camera.main.GetComponent<CameraScript> ().FollowsBounds = true;
		//				Image flash = Instance.infoText.transform.parent.GetComponent<Image> ();
		if (indicate)
			Instance.StartCoroutine (AnimationScript.FlashScreen (null, 1f, 0, "God mode active"));
	}

	//--------------------------------------------------------------------------------------------------

	public static void LevelMainMenu ()
	{
		Time.timeScale = 1;
		AutoFade.LoadLevel (0);
	}

	public static void LevelNext ()
	{
		GameManager.Run1 = true;
		Time.timeScale = 1;
//		if (Time.timeScale == 0)
//			PauseGame ();
		AutoFade.LoadLevel (LevelManager.GetNextLevel ());
	}

	public static void LevelPrev ()
	{
		GameManager.Run1 = true;
		Time.timeScale = 1;
//		if (Time.timeScale == 0)
//			PauseGame ();
		AutoFade.LoadLevel (Application.loadedLevel - 1);
	}

	public static void PauseGame ()
	{
//		if (Time.timeScale != 0)
//			Instance.currTimeSpeed = Time.timeScale;
		Time.timeScale = 0;
		Instance.PauseIcon.SetActive (true);
	}

	public static void UnPauseGame ()
	{
//		if (Time.timeScale!=0) Instance.currTimeSpeed = Time.timeScale;
		Instance.PauseIcon.SetActive (false);
		Instance.StartCoroutine (Instance.UnpauseGradual (0.5f));
//		Time.timeScale = Instance.currTimeSpeed;
	}

	IEnumerator UnpauseGradual (float overTime)
	{
		float startTime = Time.time;
//		float myDeltaTime = Time.deltaTime; 
//		float speed = 100f;
		float destScale = 1f;
		Time.timeScale = 0.1f;
		while (Time.time < startTime + overTime) {
			if (Time.timeScale == 0)
				break;
//			Time.timeScale = Mathf.MoveTowards(0.1f, destScale, (Time.time - startTime)/overTime);
			Time.timeScale = Mathf.SmoothStep (0.1f, destScale, (Time.time - startTime) / overTime);
//			SoundScript.UnPausePlay(Mathf.Pow((1-Time.timeScale)*0.2f,2f));
			yield return null;
		}

	}

	public static void RestartLevel ()
	{
//		if (Time.timeScale == 0)
//			UnPauseGame ();
		Time.timeScale = 1;
//		LevelSuccess=false;
		AnimationScript.RestartCalled = true;
		GameManager.Run1 = false;
		AutoFade.LoadLevel (Application.loadedLevel);
	}

	//------------------------------------------FAIL CHECKS--------------------------------------------
	void checkForFail ()
	{
		if (!LevelFail && !LevelSuccess) {
			bool failed = false;
			//|| FuelReading <= 0
			if (LoseWithTriggers.noHealth && HealthReading <= 0) {
				Moves.allowMoving = false;
				failed = true;
				failMsgID = 1;
			} else if (LoseWithTriggers.hasCountdown && timeTaken <= 0) {
				failed = true;
				failMsgID = 2;
			}
			if (failed) {
				Failed (failMsgID);
				return;
			}
			if (!infoMsgSet && FuelReading <= 0f) {
				StartCoroutine (setInfoText ("You ran out of fuel!", 0f));	
				StartCoroutine (setInfoText ("", 2f));
			}
		}
		//------------------------win params----------------------------------
		if (!LevelSuccess) {
			bool success = false;
			if (WinWithTriggers.dockSuccess && dockedWithTarget) {
				success = true;
			}
			if (WinWithTriggers.JustWinNow) {
				WinWithTriggers.JustWinNow = false;
				Success ();
			}
			//--------------------------no success but docked-------------------
			if (dockedWithTarget) { 
				if (ButtonPresses.inTutorial){
					success=false;
					if (!infoMsgSet) {
						StartCoroutine (setInfoText ("        Docked!", 0.5f));	  
					}
				}
				else if (Score < WinWithTriggers.MustScore) {
					success = false;
					if (!infoMsgSet) {
						StartCoroutine (setInfoText ("You must score at least " + WinWithTriggers.MustScore, 0f));	  
					}
				}
				else if (cheated) {
					success = false;
					if (!infoMsgSet) {
						StartCoroutine (setInfoText ("You are in God Mode!", 0f));	
						StartCoroutine (setInfoText ("", 2f));	

					}
				}
			}
			if (success) {
				Success ();
			}
		}

		//--------------------------------------------Call Success-----------------------------------//

	}
	/**
	 * msg = "" means fadeout whole group
	 * */
	IEnumerator setInfoText (string msg, float invokeAfter)
	{
		yield return new WaitForSeconds (invokeAfter);
		Instance.infoText.text = msg;
		if (msg.Equals ("")) {
			StartCoroutine (AnimationScript.FadeImage (
				Instance.infoText.transform.parent.GetComponent<Image> (), 0.5f, 0f, 0f));
		} else {
			infoMsgSet = true;
			StartCoroutine (AnimationScript.FadeImage (
				Instance.infoText.transform.parent.GetComponent<Image> (), 0.5f, 0.3f, 0f));
		}
		yield return null;
	}

	void updateTime (float time)
	{
		string ttext = time.ToString ("0.0");// + " s";
		if (ttext.Length<=3) ttext += 0;
		timeText.text = ttext;
	}

	//----------------------------------------PUBLIC CALLS--------------------------------------------------

	public static void Docked (GameObject dockedWith, bool IsTarget)
	{

		docked = true;
		if (dockedWith == Instance.WinWithTriggers.DockTo && IsTarget) {
			dockedWithTarget = true;
		}
		if (dockedWith == Instance.WinWithTriggers.DockSecondary) {
			dockedWithSecondary = true;
//			print ("DOCKEDSECOND");
		}
//		if (GameEvents.LevelFail) {
//			dockedWith.GetComponent<HealthLoss> ().showOnSlider = true;
//			GameObject.Find ("Player").gameObject.GetComponent<HealthLoss> ().showOnSlider = false;
//		}

		//individual changes--------------------------------
		if (dockedWith.name == "Satellite") {
			GameObject satLight = dockedWith.transform.FindChild ("Lightings").FindChild ("PointLight").gameObject;
			satLight.GetComponent<Animator> ().enabled = false;
			satLight.GetComponent<Light> ().color = new Color (1f, 0.5f, 0.5f);
			satLight.GetComponent<Light> ().intensity = 1.5f;
//			print ("Set" + satLight.GetComponent<Light> ().color);
		}
	}

	public static void Undocked (GameObject undockedWith)
	{
		LevelSuccess = false;
		docked = false;
		dockedWithTarget = false;
//		dockedWithSecondary=false;
		Instance.infoMsgSet = false;
		Instance.StartCoroutine (Instance.setInfoText ("", 0f));
		if (GameEvents.LevelFail) {
//			undockedWith.GetComponent<HealthLoss> ().showOnSlider = false;

//			player.GetComponent<HealthLoss> ().showOnSlider = true;
		}

		if (undockedWith.name == "Satellite") {
			GameObject satLight = undockedWith.transform.FindChild ("Lightings").FindChild ("PointLight").gameObject;
			satLight.GetComponent<Animator> ().enabled = true;
			satLight.GetComponent<Light> ().color = new Color (0.6f, 0.9f, 1f);
		}
	}

	//-------------------------------------criterias--------------------------------------------

	public static int GetWinCriteriaScore ()
	{
		return Instance.WinWithTriggers.MinScore;
	}
	
	public static float GetWinCriteriaTime ()
	{
		return Instance.WinWithTriggers.MinTime;
	}

	public static int GetWinCriteriaFuel ()
	{
		return Instance.WinWithTriggers.MinFuel;
	}

	public static bool IsNoDock ()
	{
		return !dockedWithSecondary;
	}

	public static bool IsScoreChecked ()
	{
		return Score >= Instance.WinWithTriggers.MinScore;
	}
	
	public static bool IsTimeChecked ()
	{
		return timeTaken <= Instance.WinWithTriggers.MinTime;
	}

	public static bool IsHealthFull ()
	{
		return HealthReading == maxHealth;
	}

	public static bool IsFuelChecked ()
	{
		return (maxFuel - FuelReading) <= Instance.WinWithTriggers.MinFuel;
	}

	public static string[] GetCriteriasNames ()
	{
		string[] str = new string[3];
		str [0] = "DOCK";

		WinTriggers wt = Instance.WinWithTriggers;
		if (wt.scoreWin)
			str [1] = "SCORE";
		else if (wt.FullHealthWin)
			str [1] = "HEALTH";
		else if (wt.NoDockSecWin)
			str [1] = "NODOCK";
		else if (wt.FuelWin)
			str [1] = "FUEL";
		else
			str [1] = "NONE";
		str [2] = "TIME";
		return str;
	}

	//---------------------------------------GAME SUCCESS/FAIL------------------------------------------
	public static void Failed (int failId)
	{
		if (!LevelFail) {
			Debug.Log ("FAILED!!!");
			GameEvents.LevelFail = true;
			Instance.StartCoroutine (AnimationScript.LevelFailed (failId));
			SoundScript.FailedMusic ();

		}
	}
	
	void Success ()
	{
		//medals : 0:none 1:bronze 2:silver 3:gold
//		Debug.Log ("SUCCESS!!!");
//		if (!LevelSuccess){
		LevelSuccess = true;
		StopListeningKeys = true;

		Moves.allowMoving = false;
		ButtonPresses.Success ();
//		if (Application.loadedLevel == Application.levelCount - 1) {
//			GameObject.Find ("Level").GetComponent<Animator> ().enabled = true;
//			StartCoroutine (SuccessAnimation (15f));
//		} else {
		StartCoroutine (SuccessAnimation (0f));
//		}
	}

	IEnumerator SuccessAnimation (float time)
	{
		yield return new WaitForSeconds (time);
		SoundScript.SuccessMusic ();
		
		//retrieve as much data
		AnimationScript.LevelSuccess (this);
		//saved!
		GameManager.SaveCriteriaData ();
		ButtonPresses.ToggleScoreButton ();
		//-------------------
		ScoreConnection.ReceiveScore (Application.loadedLevel, ScoreConnection.GetDateRanges () [2]);
	}

}

