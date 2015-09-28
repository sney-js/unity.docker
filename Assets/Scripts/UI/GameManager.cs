using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
//	public float FixedTimeStep=0.02f;
	private float FixedTimeStepChanged;
	private bool loading = true;
	public static bool Run1=true;

	Texture loadingTexture;


	void Awake ()
	{
//		DontDestroyOnLoad(gameObject);
//		FixedTimeStepChanged=FixedTimeStep;
		Time.timeScale=1;

	}

	void Start(){
		ChangeCursor(true);

		loadingTexture = Resources.Load<Texture> ("Images/UI/background1.jpg");

	}

	void ChangeCursor(bool isCustom){
		Texture2D cursor=null;
		if (isCustom){
			cursor = Resources.Load<Texture2D> ("Images/cursor.png");
		}else{
				Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
				print(cursor+"CURSOR"+Cursor.visible);
		}
	}
	public void ResetProgress(){
		PlayerPrefs.DeleteAll();
		Application.LoadLevel(Application.loadedLevel);

	}

	public static string GetLevelName(int lev){
		string str = lev +". ";
		switch (lev) {
			case 0: str += "MAIN MENU"; break;
			case 1: str += "LEARNER"; break;
			case 2: str += "CHAOS"; break;
			case 3: str += "PROTECTOR"; break;
			case 4: str += "MIGHTY"; break;
			case 5: str += "SURVIVOR"; break;
			case 6: str += "DESOLATE"; break;
			case 7: str += "FESTIVE"; break;
			case 8: str += "TERROR"; break;
			case 9: str += "COMPANION"; break;
			case 10: str += "DANCERS"; break;
			case 11: str += "UNFORGIVING"; break;
		}
		return str;
	}

	void Update ()
	{
		if(Application.isLoadingLevel)
			loading = true;
		else
			loading = false;
//		if (FixedTimeStepChanged!=FixedTimeStep){
//			Time.fixedDeltaTime = FixedTimeStep;
//			FixedTimeStepChanged=FixedTimeStep;
//		}

	}

	void OnGUI()
	{
		if(loading)
			GUI.DrawTexture (new Rect(0,0,Screen.width,Screen.height), loadingTexture, ScaleMode.ScaleAndCrop);
		//Frame Rate
		//		GUI.Label(new Rect(10,10, 100, 100), ((int)(1.0f / Time.smoothDeltaTime)).ToString());        
	}
	
	public void QuitGame ()
	{
		Application.Quit ();
	}

	public static void LoadLevelNum (int num)
	{
//		LoadingScreen.show();
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
			return "You went into the Sun"; 
		case 5:
			return "You got sucked into the black hole"; 
		case 6:
			return "You went into the Abyss"; 
		case 7:
			return "You were hit by a Geyser"; 
		default:
			return "FAILED!";
		}

	}

	//--------------------------------------------------------------------------------------------------

	public static int GetMedal(){
		//bronze would be if docked
		string lev = "Level#"+Application.loadedLevel;
		int medal=1;

		string[] crinames = GameEvents.GetCriteriasNames();
		if (ArrayContains(crinames, "SCORE")){
			if (GameEvents.IsScoreChecked() || GetLevelSavedScore()>=GameEvents.GetWinCriteriaScore() ) medal++;
		}
		if (ArrayContains(crinames, "NODOCK")){
			if (GameEvents.IsNoDock() || GetLevelSavedNoDock()) medal++;
		}
		if (ArrayContains(crinames, "HEALTH")){
			if (GameEvents.IsHealthFull() || GetLevelSavedHealth()) medal++;
		}
		if (ArrayContains(crinames, "FUEL")){
			if (GameEvents.IsFuelChecked() || GetLevelSavedFuel()) medal++;
		}
		if (ArrayContains(crinames, "TIME")){
			if (GameEvents.IsTimeChecked() || GetLevelSavedTime()<=GameEvents.GetWinCriteriaTime()) medal++;
		}

		medal = Mathf.Clamp(medal, 0,3);
//		medal = Mathf.Max(GetLevelSavedMedal(), medal);
		return medal;
	}



	public static void SaveCriteriaData(){
		//bronze would be if docked
		string lev = "Level#"+Application.loadedLevel;
		int medal=GetMedal();
		if (PlayerPrefs.GetInt(lev+"_Medal")<medal) PlayerPrefs.SetInt(lev+"_Medal", medal);

		string[] crinames = GameEvents.GetCriteriasNames();

		if (ArrayContains(crinames, "SCORE")){
			if (GetLevelSavedScore()<GameEvents.Score) PlayerPrefs.SetInt(lev+"_Score", GameEvents.Score);
		}
		if (ArrayContains(crinames, "NODOCK")){
			if (!GetLevelSavedNoDock()) PlayerPrefs.SetInt(lev+"_Dock2", GameEvents.IsNoDock()?1:0);
		}
		if (ArrayContains(crinames, "HEALTH")){
			if (!GetLevelSavedHealth()) PlayerPrefs.SetInt(lev+"_Health", GameEvents.IsHealthFull()?1:0);
		}
		if (ArrayContains(crinames, "FUEL")){
			if (!GetLevelSavedFuel()) PlayerPrefs.SetInt(lev+"_Fuel", GameEvents.IsFuelChecked()?1:0);
		}
		if (ArrayContains(crinames, "TIME")){
			float bestTime = GetLevelSavedTime ();
			if (bestTime>GameEvents.timeTaken) PlayerPrefs.SetFloat(lev+"_Time", GameEvents.timeTaken);
		}

	}


	public static float GetLevelSavedTime(){
		string lev = "Level#"+Application.loadedLevel;
		float time= PlayerPrefs.GetFloat(lev+"_Time");
		if (time==0f) return float.PositiveInfinity;
		else return time;
	}
	public static float GetLevelSavedSentTime(){
		string lev = "Level#"+Application.loadedLevel;
		float time= PlayerPrefs.GetFloat(lev+"_SentTime");
		if (time==0f) return float.PositiveInfinity;
		else return time;
	}
	public static int GetLevelSavedScore(){
		string lev = "Level#"+Application.loadedLevel;
		return PlayerPrefs.GetInt(lev+"_Score");
	}
	public static bool GetLevelSavedDocked(){
		string lev = "Level#"+Application.loadedLevel;
		return PlayerPrefs.GetInt(lev+"_Medal")>0;
	}
	public static bool GetLevelSavedNoDock(){
		string lev = "Level#"+Application.loadedLevel;
		return PlayerPrefs.GetInt(lev+"_Dock2")==1;
	}
	public static bool GetLevelSavedHealth(){
		string lev = "Level#"+Application.loadedLevel;
		return PlayerPrefs.GetInt(lev+"_Health")==1;
	}
	public static bool GetLevelSavedFuel(){
		string lev = "Level#"+Application.loadedLevel;
		return PlayerPrefs.GetInt(lev+"_Fuel")==1;
	}
	public static int GetLevelSavedMedal(){
		string lev = "Level#"+Application.loadedLevel;
		return PlayerPrefs.GetInt(lev+"_Medal");
	}


	public static void SetLevelSavedSentTime(float time){
		string lev = "Level#"+Application.loadedLevel;
		PlayerPrefs.SetFloat(lev+"_SentTime", time);
	}


	public static bool ArrayContains(string[] array, string word){
		for (int i = 0; i < array.Length; i++) {
			if (array[i].Equals(word)) return true;
		}
		return false;
	}

}
