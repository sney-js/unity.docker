using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
	//	public float FixedTimeStep=0.02f;
	private float FixedTimeStepChanged;
	//	private bool loading = true;
	public static bool Run1 = true;
	public static GameManager instance;
	Texture loadingTexture;
	//	bool drawn = false;
	int controlType;

	void Awake ()
	{
//		DontDestroyOnLoad(this);
//		FixedTimeStepChanged=FixedTimeStep;
//		controlType = MInput.KEYBOARD1;

		Time.timeScale = 1;
		Application.targetFrameRate = 60;
	}

	void Start ()
	{
//		ChangeCursor(true);
		instance = this;
		if (loadingTexture == null) {
			loadingTexture = Resources.Load<Texture> ("Images/UI/background1");
		}
	}

	void ChangeCursor (bool isCustom)
	{
		Texture2D cursor = null;
		if (isCustom) {
			cursor = Resources.Load<Texture2D> ("Images/cursor_32");
//			print ("CURSOR :"+cursor.name);
		} else {
			Cursor.SetCursor (cursor, Vector2.zero, CursorMode.Auto);
			print (cursor + "CURSOR" + Cursor.visible);
		}
	}

	//	public static void ResetProgress(){
	//		PlayerPrefs.DeleteAll();
	//		Application.LoadLevel(Application.loadedLevel);
	//	}

	public static string GetLevelName (int lev)
	{
		string[] levNames = {"MAIN MENU", 
			"LEARNER", 
			"CHAOS",
			"ILLUSION",
			"MIGHTY", 
			"DESOLATE", 
			"FESTIVE",
			"TERROR", 
			"SURVIVOR", 
			"TENACIOUS",
			"PATIENT", 
			"DESTROYER"
		};
		string str = lev + ". ";
		str += levNames[lev];
		return str;
	}

	//	void Update ()
	//	{
	//		loading = Application.isLoadingLevel;
	//		if (FixedTimeStepChanged!=FixedTimeStep){
	//			Time.fixedDeltaTime = FixedTimeStep;
	//			FixedTimeStepChanged=FixedTimeStep;
	//		}
	//	}

	void OnGUI ()
	{
//		if(Application.isLoadingLevel) {
////			GUI.DrawTexture (new Rect(0,0,Screen.width,Screen.height), loadingTexture, ScaleMode.ScaleAndCrop);
//			Graphics.DrawTexture (new Rect(0,0,Screen.width,Screen.height), loadingTexture);
//			print("DRAWING TEXTURE: "+loadingTexture.name);
//			drawn=true;
//		}
		//Frame Rate
		//		GUI.Label(new Rect(10,10, 100, 100), ((int)(1.0f / Time.smoothDeltaTime)).ToString());        
	}

	public static IEnumerator FadeLoad (float overTime)
	{
		float startTime = Time.time;

		Color transparent = new Color (0, 0, 0, 0);
		Color black = new Color (0, 0, 0, 1);
		GUI.color = transparent;
//		Graphics.DrawTexture (new Rect(0,0,Screen.width,Screen.height), loadingTexture);
		while (Time.time < startTime + overTime) {
			GUI.color = Color.Lerp (GUI.color, black, (Time.time - startTime) / overTime);
			print (GUI.color);
			yield return null;
		}


		startTime = Time.time;
		while (Time.time < startTime + overTime) {
			print (GUI.color);
			GUI.color = Color.Lerp (GUI.color, transparent, (Time.time - startTime) / overTime);
			yield return null;
		}
	}

	public void QuitGame ()
	{
		Application.Quit ();
	}

	public static void LoadLevelNum (int num)
	{
//		LoadingScreen.show();
//		instance.StartCoroutine(FadeLoad(1f));
		Application.LoadLevel (num);

//		AutoFade.LoadLevel (num, 0.33f, 1, Color.black);
	}

	public void LoadLevelNumUI (int num)
	{
		//		LoadingScreen.show();
		Application.LoadLevel (num);
		
		//		AutoFade.LoadLevel (num, 0.33f, 1, Color.black);
	}

	public static string getMsg (int msgID)
	{
		switch (msgID) {
		case 1:
			return "You took too much damage"; 
		case 2:
			return "You ran out of time"; 
		case 3:
			return "Your score is not high enough"; 
		case 4:
			return "";//You went into the Sun"; 
		case 5:
			return "";//You got sucked into the black hole"; 
		case 6:
			return "You went into the Abyss"; 
		case 7:
			return "";//You got hit!"; 
		default:
			return "FAILED!";
		}

	}

	//--------------------------------------------------------------------------------------------------

	#region Player prefs

	public static int GetMedal ()
	{
		//bronze would be if docked
//		string lev = "Level#"+Application.loadedLevel;
		int medal = 1;

		string[] crinames = GameEvents.GetCriteriasNames ();
		if (ArrayContains (crinames, "SCORE")) {
			if (GameEvents.IsScoreChecked () || GetLevelSavedScore () >= GameEvents.GetWinCriteriaScore ())
				medal++;
		}
		if (ArrayContains (crinames, "NODOCK")) {
			if (GameEvents.IsNoDock () || GetLevelSavedNoDock ())
				medal++;
		}
		if (ArrayContains (crinames, "HEALTH")) {
			if (GameEvents.IsHealthFull () || GetLevelSavedHealth ())
				medal++;
		}
		if (ArrayContains (crinames, "FUEL")) {
			if (GameEvents.IsFuelChecked () || GetLevelSavedFuel ())
				medal++;
		}
		if (ArrayContains (crinames, "TIME")) {
			if (GameEvents.IsTimeChecked () || GetLevelSavedTime () <= GameEvents.GetWinCriteriaTime ())
				medal++;
		}

		medal = Mathf.Clamp (medal, 0, 3);
//		medal = Mathf.Max(GetLevelSavedMedal(), medal);
		return medal;
	}

	public static string getNoSubmitInfo ()
	{
		float bestTime = GameManager.GetLevelSavedTime ();
		float sentTime = GameManager.GetLevelSavedSentTime ();

		if (GetMedal () < 3) {
			return "You must get a Gold medal first";
		} else if (bestTime >= sentTime) {
			return "You've already sent your best time";
		} else {
			return "";
		}
	}

	public static void SaveCriteriaData ()
	{
		//bronze would be if docked
		string lev = "Level#" + Application.loadedLevel;
		int medal = GetMedal ();
		if (PlayerPrefs.GetInt (lev + "_Medal") < medal)
			PlayerPrefs.SetInt (lev + "_Medal", medal);

		string[] crinames = GameEvents.GetCriteriasNames ();

		print (lev + ",medal:"+medal+" ,saved: "+PlayerPrefs.GetInt (lev + "_Medal"));

		if (ArrayContains (crinames, "SCORE")) {
			if (GetLevelSavedScore () < GameEvents.Score)
				PlayerPrefs.SetInt (lev + "_Score", GameEvents.Score);
		}
		if (ArrayContains (crinames, "NODOCK")) {
			if (!GetLevelSavedNoDock ())
				PlayerPrefs.SetInt (lev + "_Dock2", GameEvents.IsNoDock () ? 1 : 0);
		}
		if (ArrayContains (crinames, "HEALTH")) {
			if (!GetLevelSavedHealth ())
				PlayerPrefs.SetInt (lev + "_Health", GameEvents.IsHealthFull () ? 1 : 0);
		}
		if (ArrayContains (crinames, "FUEL")) {
			if (!GetLevelSavedFuel ())
				PlayerPrefs.SetInt (lev + "_Fuel", GameEvents.IsFuelChecked () ? 1 : 0);
		}
		if (ArrayContains (crinames, "TIME")) {
			float bestTime = GetLevelSavedTime ();
			if (bestTime > GameEvents.timeTaken)
				PlayerPrefs.SetFloat (lev + "_Time", GameEvents.timeTaken);
		}

	}


	public static float GetLevelSavedTime ()
	{
		string lev = "Level#" + Application.loadedLevel;
		float time = PlayerPrefs.GetFloat (lev + "_Time");
		if (time == 0f)
			return float.PositiveInfinity;
		else
			return time;
	}

	public static float GetLevelSavedSentTime ()
	{
		string lev = "Level#" + Application.loadedLevel;
		float time = PlayerPrefs.GetFloat (lev + "_SentTime");
		if (time == 0f)
			return float.PositiveInfinity;
		else
			return time;
	}

	public static int GetLevelSavedScore ()
	{
		string lev = "Level#" + Application.loadedLevel;
		return PlayerPrefs.GetInt (lev + "_Score");
	}

	public static bool GetLevelSavedDocked ()
	{
		string lev = "Level#" + Application.loadedLevel;
		return PlayerPrefs.GetInt (lev + "_Medal") > 0;
	}

	public static bool GetLevelSavedNoDock ()
	{
		string lev = "Level#" + Application.loadedLevel;
		return PlayerPrefs.GetInt (lev + "_Dock2") == 1;
	}

	public static bool GetLevelSavedHealth ()
	{
		string lev = "Level#" + Application.loadedLevel;
		return PlayerPrefs.GetInt (lev + "_Health") == 1;
	}

	public static bool GetLevelSavedFuel ()
	{
		string lev = "Level#" + Application.loadedLevel;
		return PlayerPrefs.GetInt (lev + "_Fuel") == 1;
	}

	public static int GetLevelSavedMedal ()
	{
		string lev = "Level#" + Application.loadedLevel;
		return PlayerPrefs.GetInt (lev + "_Medal");
	}

	
	public static void SetLevelSavedSentTime (float time)
	{
		string lev = "Level#" + Application.loadedLevel;
		PlayerPrefs.SetFloat (lev + "_SentTime", time);
	}

	public static bool GetStabiliserPref ()
	{
		return PlayerPrefs.GetInt ("_stabiliser") == 1;
	}

	public static void SetStabiliserPref (int active)
	{
//		string lev = "Level#"+Application.loadedLevel;
		PlayerPrefs.SetInt ("_stabiliser", active);
	}


	public static bool ArrayContains (string[] array, string word)
	{
		for (int i = 0; i < array.Length; i++) {
			if (array [i].Equals (word))
				return true;
		}
		return false;
	}

	#endregion
}
