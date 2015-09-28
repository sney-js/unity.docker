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
	public static bool dockedWithTarget,docked, dockedWithSecondary, startCounting;
	public static bool LevelSuccess, LevelFail;
	public static bool StopListeningKeys, StopListeningKeysMain;

	private Text timeText, scoreText, infoText;
	private float currTimeSpeed;
	private bool prevDocked = false;
	private int failMsgID;

	private float prevScore;
	private bool infoMsgSet = false;
	private GameObject PauseIcon;
	//-----------------------------------CLASSES---------------------------------------------------

	[System.Serializable]
	public class WinTriggers
	{
		public bool dockSuccess=true, timeWin=true, scoreWin=true, FullHealthWin, NoDockSecWin, FuelWin;
		public GameObject DockTo;
		public int MinScore = 10, MustScore=-1, MinFuel=2000;
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

		Transform UI = GameObject.Find ("Canvas/UI").transform;
		timeText = UI.Find ("Time").gameObject.GetComponent<Text> ();
		scoreText = UI.Find ("Score").gameObject.GetComponent<Text> ();
		infoText = GameObject.Find ("Canvas/InfoOverImage/FailText").gameObject.GetComponent<Text> ();
		PauseIcon = UI.FindChild ("PauseIcon").gameObject;
		scoreText.enabled = WinWithTriggers.scoreWin || WinWithTriggers.MustScore>0;
		scoreText.text = Score.ToString ("00");

		if (LoseWithTriggers.hasCountdown)
			timeTaken = LoseWithTriggers.CountdownTimer;
		
		updateTime (timeTaken);
		
	}

	void Update ()
	{
		if (!noWinLose) {
//		print ("Success: " + GameEvents.LevelSuccess);
			simulationFeatures ();
			if (!LevelSuccess && !LevelFail) {
				checkForFail ();
				if (startCounting) {
					if (LoseWithTriggers.hasCountdown) {
						timeTaken -= Time.deltaTime;
					} else {
						timeTaken += Time.deltaTime;
					} 
					updateTime (Mathf.Clamp (timeTaken, 0, float.PositiveInfinity));
					if ((WinWithTriggers.scoreWin || WinWithTriggers.MustScore>0) && prevScore != Score) {
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

	void simulationFeatures ()
	{

//		if (!StopListeningKeys)	SimulateSpeedCheck ();

		if (Input.GetKeyDown (KeyCode.Space) && !StopListeningKeysMain) {
			RestartLevel ();
		}

	}

	//--------------------------------------------------------------------------------------------------

	public void LevelMainMenu ()
	{
		if (Time.timeScale == 0)
			PauseGame ();
		AutoFade.LoadLevel (0, 0.33f, 1, Color.black);
	}

	public static void LevelNext ()
	{
		GameManager.Run1 = true;
		Time.timeScale=1;
//		if (Time.timeScale == 0)
//			PauseGame ();
		AutoFade.LoadLevel (LevelManager.GetNextLevel (), 0.33f, 1, Color.black);
	}

	public static void LevelPrev ()
	{
		GameManager.Run1 = true;
		Time.timeScale=1;
//		if (Time.timeScale == 0)
//			PauseGame ();
		AutoFade.LoadLevel (Application.loadedLevel - 1, 0.33f, 1, Color.black);
	}

	public static void PauseGame ()
	{
		if (Time.timeScale!=0) Instance.currTimeSpeed = Time.timeScale;
		Time.timeScale = 0;
		Instance.PauseIcon.SetActive (true);
	}

	public static void UnPauseGame ()
	{
//		if (Time.timeScale!=0) Instance.currTimeSpeed = Time.timeScale;
		Instance.PauseIcon.SetActive (false);
		Instance.StartCoroutine(Instance.UnpauseGradual(0.5f));
//		Time.timeScale = Instance.currTimeSpeed;
	}

	IEnumerator UnpauseGradual(float overTime){
		float startTime = Time.time;
		float myDeltaTime = Time.deltaTime; 
		float speed = 100f;
		float destScale = 1f;
		Time.timeScale=0.1f;
		while(Time.time < startTime + overTime)
		{
			if (Time.timeScale==0) break;
//			Time.timeScale = Mathf.MoveTowards(0.1f, destScale, (Time.time - startTime)/overTime);
			Time.timeScale = Mathf.SmoothStep(0.1f, destScale, (Time.time - startTime)/overTime);
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
		AutoFade.LoadLevel (Application.loadedLevel, 0.33f, 1, Color.black);
	}

	//------------------------------------------FAIL CHECKS--------------------------------------------
	void checkForFail ()
	{
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

		//------------------------win params----------------------------------
		bool success = false;
		if (WinWithTriggers.dockSuccess && dockedWithTarget) {
			success = true;
		}
		if (WinWithTriggers.JustWinNow) {
			WinWithTriggers.JustWinNow=false;
			Success();
		}
		//--------------------------no fail/success but docked-------------------
		if (!LevelFail && !LevelSuccess && dockedWithTarget & Score < WinWithTriggers.MustScore) {
			success=false;
			if (!infoMsgSet) setInfoText ("You must score at least " + WinWithTriggers.MustScore);	  
		}

		if (success) {
			Success ();
		}

	}

	/**
	 * msg = "" means fadeout whole group
	 * */
	void setInfoText (string msg)
	{
		Instance.infoText.text = msg;

		if (msg.Equals ("")) {
//			Instance.infoText.enabled=false;
			StartCoroutine (AnimationScript.FadeImage (
				Instance.infoText.transform.parent.GetComponent<Image> (), 0.5f, 0f));
		} else {
			infoMsgSet = true;
//			Instance.infoText.enabled=true;
			StartCoroutine (AnimationScript.FadeImage (
					Instance.infoText.transform.parent.GetComponent<Image> (), 0.5f, 0.3f));
		}
	}

	void updateTime (float time)
	{
		timeText.text = time.ToString ("0.0");// + " s";
	}

	//----------------------------------------PUBLIC CALLS--------------------------------------------------

	public static void Docked (GameObject dockedWith, bool IsTarget)
	{

		docked = true;
		if (dockedWith == Instance.WinWithTriggers.DockTo && IsTarget) {
			dockedWithTarget = true;
		}
		if (dockedWith == Instance.WinWithTriggers.DockSecondary) {
			dockedWithSecondary=true;
			print("DOCKEDSECOND");
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
		Instance.setInfoText ("");
		if (GameEvents.LevelFail) {
			undockedWith.GetComponent<HealthLoss> ().showOnSlider = false;
			GameObject.Find ("Player").gameObject.GetComponent<HealthLoss> ().showOnSlider = true;
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
		return Score>=Instance.WinWithTriggers.MinScore;
	}
	
	public static bool IsTimeChecked ()
	{
		return timeTaken<=Instance.WinWithTriggers.MinTime;
	}

	public static bool IsHealthFull ()
	{
		return HealthReading==maxHealth;
	}

	public static bool IsFuelChecked ()
	{
		return (maxFuel- FuelReading)<=Instance.WinWithTriggers.MinFuel;
	}

	public static string[] GetCriteriasNames(){
		string[] str = new string[3];
		str[0] = "DOCK";

		WinTriggers wt = Instance.WinWithTriggers;
		if (wt.scoreWin) str[1]="SCORE";
		else if (wt.FullHealthWin) str[1]="HEALTH";
		else if (wt.NoDockSecWin) str[1]="NODOCK";
		else if (wt.FuelWin) str[1]="FUEL";
		else str[1] = "NONE";
		str[2] = "TIME";
		return str;
	}

	//---------------------------------------GAME SUCCESS/FAIL------------------------------------------
	public static void Failed (int failId)
	{
		Debug.Log ("FAILED!!!");
		GameEvents.LevelFail = true;
		Instance.StartCoroutine (AnimationScript.LevelFailed (failId));
		SoundScript.FailedMusic ();
	}
	
	void Success ()
	{
		//medals : 0:none 1:bronze 2:silver 3:gold
//		Debug.Log ("SUCCESS!!!");
		LevelSuccess = true;
		StopListeningKeys = true;

		Moves.allowMoving=false;
		ButtonPresses.Success ();
		SoundScript.SuccessMusic ();

		//retrieve as much data
		AnimationScript.LevelSuccess (this);
		//saved!
		GameManager.SaveCriteriaData();
		ButtonPresses.ToggleScoreButton() ;
		//-------------------
		ScoreConnection.ReceiveScore (Application.loadedLevel, ScoreConnection.GetDateRanges () [2]);
	}

}

