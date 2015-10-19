using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonPresses : MonoBehaviour
{

	public static ButtonPresses Instance;
	public GameObject optionsPanel;
	public static bool menuShowing;
	OptionsDetails QualityOptions, OptionsSaved;
	private GameObject MoreOptionsPanel;
	private Text QualityText;
	private GameObject ShortCutDialog;
	private Toggle t_antialias, t_bloom, t_grain, t_shadow;
	private Slider t_mouse;
	private int t_qualityLevel;
	public bool NoPause = false;
	public bool isMainMenu = false;
	private Transform UI;
	private GameObject helptip;
	public bool FlagHelpTip = false;
	private Transform successObjTarget;
	private Transform canvasObj;
	//-------------------------icons-----------------
	public Sprite[] headlights;
	public Sprite[] CriteriaBox1Sprites;
	Image lightIndicator, dockIndicator;
	private GameObject TabMenu;
	bool StartScreenFade = false;
//	[System.Serializable]
	public class OptionsDetails
	{
//		[Header("Dynamic bool")]
		public bool antialiasing = false, bloom = false, grain = false, showShadow = false;
		public int qualityLevel = 4;
		public float mouseSens = 0.5f;
	}


	void Awake ()
	{
		Instance = this;
		QualityOptions = new OptionsDetails ();
		new ScoreConnection ();
		if (!isMainMenu) {
			Instance.lightIndicator = GameObject.Find ("Canvas/UI/Other/LightIndicator/Image").gameObject.GetComponent<Image> ();
			Instance.dockIndicator = GameObject.Find ("Canvas/UI/Other/DockIndicator/Image").gameObject.GetComponent<Image> ();
		}
	}

	// Use this for initialization
	void Start ()
	{
		canvasObj = GameObject.Find ("Canvas").transform;
		menuShowing = optionsPanel.activeInHierarchy;

		MoreOptionsPanel = optionsPanel.transform.Find ("MoreOptions").gameObject;
		ShortCutDialog = optionsPanel.transform.FindChild ("ShortcutDialog").gameObject;
		UI = canvasObj.FindChild ("UI").transform;
		successObjTarget = canvasObj.FindChild ("GameSuccessImage/OnlineScoreDisplay/Panel");
	
		//-----------icons-------------------------
		helptip = canvasObj.FindChild ("HelpTip").gameObject;
		if (Application.loadedLevel == 1 && FlagHelpTip && GameManager.Run1) {          
			ShowHelpTips ();
		}

		initSettings ();

		if (!isMainMenu) {
//			Instance.lightIndicator = UI.transform.FindChild ("Other/LightIndicator/Image").gameObject.GetComponent<Image> ();
//			Instance.dockIndicator = UI.transform.FindChild ("Other/DockIndicator/Image").gameObject.GetComponent<Image> ();
			if (StartScreenFade) {
				canvasObj.FindChild ("StartScreen").gameObject.SetActive (GameManager.Run1);
				if (GameManager.Run1) {
					canvasObj.FindChild ("StartScreen/LevelTitle").GetComponent<Text> ().text = 
						GameManager.GetLevelName (Application.loadedLevel);
				}
			}

			TabMenu = canvasObj.FindChild ("TabMenu").gameObject;
			Text leveltext = TabMenu.transform.FindChild ("LeftGroup/MoreInfo/LevelText").GetComponent<Text> ();
			leveltext.text = "Level " + Application.loadedLevel;
			if (LevelManager.GetNextLevel () == Application.loadedLevel) {
				optionsPanel.transform.FindChild ("PrevNextGroup/Next").GetComponent<Button> ().interactable = false;
			}
			//options Criteria
			FromOptionsLeaderboard = true;
			initialiseCriteriaIcons (true);
			ToggleDateRangeClicksOptions (2);
		} else {
			ScoreConnection.ReceiveRemoteVersion();
		}
		
	}

	public static IEnumerator WaitForVersion (WWW www, string receiving)
	{
		print ("GOT HERE");
		yield return www;
		// check for errors
		string data = www.data;
		bool successful = www.error == null && data != null && data != "";
		if (receiving == "check-update") {
			Debug.Log ("RESPONSE: " + data);
			if (successful) {

				float remoteVersion = ScoreConnection.AboutStringParseVersion(data);
				System.DateTime date = ScoreConnection.AboutStringParseTime(data);


				if (ScoreConnection.GetCurrentGameVersion()<remoteVersion){
					print ("UPDATE AVAILABLE!");
				}else{
					print ("No Update Available");
				}
			} else {
				print ("An Error occurred retrieving remote version");
			}
			

		}
		
	}    
	//--------------------------------------------------------------------------------------------------------

	//WILL CRASH ON ANDROID BECAUSE IT RETURNS NULL RESOLUTIONS
	void initSettings ()
	{

		t_mouse = MoreOptionsPanel.transform.FindChild ("Line1/Slider").gameObject.GetComponent<Slider> ();
		QualityText = MoreOptionsPanel.transform.FindChild ("Line2/Switcher/QualityText").gameObject.GetComponent<Text> ();
		t_antialias = MoreOptionsPanel.transform.FindChild ("Line3/Toggle").gameObject.GetComponent<Toggle> ();
		t_bloom = MoreOptionsPanel.transform.FindChild ("Line4/Toggle").gameObject.GetComponent<Toggle> ();
		t_grain = MoreOptionsPanel.transform.FindChild ("Line5/Toggle").gameObject.GetComponent<Toggle> ();
		t_shadow = MoreOptionsPanel.transform.FindChild ("Line6/Toggle").gameObject.GetComponent<Toggle> ();

		try {
			if (PlayerPrefs.HasKey ("Settings_mouseSensitivity")) {
				QualityOptions.mouseSens = PlayerPrefs.GetFloat ("Settings_mouseSensitivity");
			}
			if (PlayerPrefs.HasKey ("Settings_qualitylevel")) {
				QualityOptions.qualityLevel = PlayerPrefs.GetInt ("Settings_qualitylevel");
			}
			QualityOptions.antialiasing = PlayerPrefs.GetInt ("Settings_antialias") == 1 ? true : false;
			QualityOptions.bloom = PlayerPrefs.GetInt ("Settings_bloom") == 1 ? true : false;
			QualityOptions.grain = PlayerPrefs.GetInt ("Settings_grain") == 1 ? true : false;
			QualityOptions.showShadow = PlayerPrefs.GetInt ("Settings_shadow") == 1 ? true : false;

//			PrintSettings ("INIT");

		} catch (System.Exception ex) { //ignore catch for now
			print ("ERROR occurred retrieving settings");
		} finally {

			//-----------
			ResetToPrevSettings (QualityOptions);
			saveSettings ();
			ApplyMoreSettings (true);

		}



		//FINAL
//		saveCurrSettings ();
	}

	public static void initialiseCriteriaIcons (bool isOptions)
	{
		Color orange = new Color32 (255, 251, 94, 255);
		int winScore = GameEvents.GetWinCriteriaScore ();
		int winFuel = GameEvents.GetWinCriteriaFuel ();
		float timeWin = GameEvents.GetWinCriteriaTime ();

		Transform cgroup;
		if (isOptions)
			cgroup = Instance.canvasObj.transform.FindChild ("TabMenu/LeftGroup/CriteriaGroup").transform;
		else
			cgroup = Instance.canvasObj.FindChild ("GameSuccessImage/LeftGroup/CriteriaGroup").transform;

		string[] crinames = GameEvents.GetCriteriasNames ();

		//box0:
		if (GameManager.GetLevelSavedDocked ()) {
			cgroup.FindChild ("Box0/Icon").GetComponent<Image> ().color = orange;
		}
		//box1:
		string box1Text = "OK";
		int box1spriteNum = 0;
		if (GameManager.ArrayContains (crinames, "SCORE")) {
			box1Text = winScore.ToString ();
			box1spriteNum = 0;
			if (GameManager.GetLevelSavedScore () >= winScore) {
				cgroup.FindChild ("Box1/Icon").GetComponent<Image> ().color = orange;
			}
		}
		if (GameManager.ArrayContains (crinames, "FUEL")) {
			box1Text = winFuel.ToString ();
			box1spriteNum = 3;
			if (GameManager.GetLevelSavedFuel ()) {
				cgroup.FindChild ("Box1/Icon").GetComponent<Image> ().color = orange;
			}
		}
		if (GameManager.ArrayContains (crinames, "NODOCK")) {
			box1Text = "Avoid";
			box1spriteNum = 1;
			if (GameManager.GetLevelSavedNoDock ())
				cgroup.FindChild ("Box1/Icon").GetComponent<Image> ().color = orange;
		}
		if (GameManager.ArrayContains (crinames, "HEALTH")) {
			box1Text = "No Damage";
			box1spriteNum = 2;
			if (GameManager.GetLevelSavedHealth ())
				cgroup.FindChild ("Box1/Icon").GetComponent<Image> ().color = orange;
		}
		cgroup.FindChild ("Box1/Text").GetComponent<Text> ().text = box1Text;
		cgroup.FindChild ("Box1/Icon").GetComponent<Image> ().sprite = Instance.CriteriaBox1Sprites [box1spriteNum];

		//box2:
		if (GameManager.ArrayContains (crinames, "TIME")) {
			if (GameManager.GetLevelSavedTime () <= timeWin) {
				cgroup.FindChild ("Box2/Icon").GetComponent<Image> ().color = orange;
			}
			cgroup.FindChild ("Box2/Text").GetComponent<Text> ().text = timeWin.ToString () + "s";
		}

		if (isOptions) {
			float BestTime = GameManager.GetLevelSavedTime ();
			Text bestTime = Instance.canvasObj.transform.FindChild ("TabMenu/LeftGroup/MoreInfo/TimeGroup/Result").GetComponent<Text> ();
			bestTime.text = BestTime == float.PositiveInfinity ? "N/A" : BestTime.ToString ("0.000") + " s";
		}

	}
	//-----------------------------------------------------------------------------------------------
	
	// Update is called once per frame
	void Update ()
	{
		if (!GameEvents.StopListeningKeys) {
			if (!NoPause && Input.GetKeyDown (KeyCode.Escape)) {
				if (Application.loadedLevel > 0) {

					hideMoreSettings ();
					if (ShortCutDialog.activeInHierarchy) {
						hideShortcutDialog ();
					}
					showActionPanel ();
				} else {
					if (MoreOptionsPanel.activeInHierarchy) {
						hideMoreSettingsOnly ();
					} else {
						showMoreSettingsOnly ();
					}

				}
			}
			if (Input.GetKeyDown (KeyCode.T)) {
				if (!helptip.activeInHierarchy)
					ShowHelpTips ();
				else
					HideHelpTips ();
			}
			if (Input.GetKeyDown (KeyCode.Tab)) {
				showTabPanel ();
			}
		}
	}

	public void showActionPanel ()
	{

		if (Time.timeScale != 0) {
			GameEvents.PauseGame ();
		} else if (menuShowing && !TabMenu.activeInHierarchy) {
			GameEvents.UnPauseGame ();
		}
		optionsPanel.SetActive (!optionsPanel.activeInHierarchy);
		menuShowing = optionsPanel.activeInHierarchy;
		MoreOptionsPanel.gameObject.SetActive (false);

	}
	
	public void showTabPanel ()
	{
		
		if (Time.timeScale != 0) {
			GameEvents.PauseGame ();
		} else if (TabMenu.activeInHierarchy && !menuShowing) {
			GameEvents.UnPauseGame ();
		}
		TabMenu.GetComponent<CanvasGroup> ().alpha = 1f;
		if (TabMenu.activeInHierarchy) {
			Instance.StartCoroutine (Instance.FadePanel (TabMenu, 0.1f));
		} else {
			TabMenu.SetActive (true);
		}
//		menuShowing = TabMenu.activeInHierarchy;
//		MoreOptionsPanel.gameObject.SetActive (false);
		
	}

	IEnumerator FadePanel (GameObject obj, float overTime)
	{
		float startTime = Time.time;

		CanvasGroup cg = obj.GetComponent<CanvasGroup> ();
		cg.alpha = 0.99f;
		while (Time.time < startTime + overTime) {
			if (cg.alpha == 1f)
				break;
			cg.alpha = Mathf.SmoothStep (0.99f, 0f, (Time.time - startTime) / overTime);
			yield return null;
		}
		obj.SetActive (false);
	}

	private void saveSettings ()
	{
		OptionsSaved = QualityOptions;
	}

	private void ResetToPrevSettings (OptionsDetails option)
	{
		QualityOptions = option;

		t_mouse.value = QualityOptions.mouseSens;

		QualityText.text = getQualityText (QualityOptions.qualityLevel);
		t_qualityLevel = QualityOptions.qualityLevel;
	
		t_antialias.isOn = QualityOptions.antialiasing;
		t_bloom.isOn = QualityOptions.bloom;
		t_grain.isOn = QualityOptions.grain;
		t_shadow.isOn = QualityOptions.showShadow;
	}

	public void showMoreSettings ()
	{
		MoreOptionsPanel.SetActive (true);
		ResetToPrevSettings (OptionsSaved);
	}

	public void showMoreSettingsOnly ()
	{
		optionsPanel.SetActive (true);
		menuShowing = optionsPanel.activeInHierarchy;

		MoreOptionsPanel.SetActive (true);
//		ResetToPrevSettings (OptionsSaved);
	}

	public void hideMoreSettingsOnly ()
	{
		optionsPanel.SetActive (false);
		menuShowing = optionsPanel.activeInHierarchy;

		QualityOptions = OptionsSaved;
		MoreOptionsPanel.SetActive (false);

	}

	public void hideMoreSettings ()
	{
		if (isMainMenu)
			hideMoreSettingsOnly ();
		else {
			QualityOptions = OptionsSaved;
			MoreOptionsPanel.SetActive (false);
		}
//		saveSettings();
//		print(MoreOptionsPanel);//.enabled=false;
	}

	public void ApplyMoreSettings (bool isForInitialisation)
	{
		setMouseSensitivity (t_mouse.value);
		//1:
		setQualityLevel (t_qualityLevel);
		//2:
		setQualityAntiAlias (t_antialias.isOn);
		//3:
		setQualityBloom (t_bloom.isOn);
		//		//4:
		setQualityGrain (t_grain.isOn);
		//		//5:
		setQualityShadow (t_shadow.isOn);
//		//		//6:
//		setQualityLevel (t_qualityLevel);
//		//2:
//		setQualityAntiAlias (QualityOptions.antialiasing);
//		//3:
//		setQualityBloom (QualityOptions.bloom);
//		//		//4:
//		setQualityGrain (QualityOptions.grain);
//		//		//5:
//		setQualityShadow (QualityOptions.showShadow);
//		//		//6:

//		PrintSettings ("APPLIED");
		saveSettings ();
//		hideMoreSettings ();
	}

	
	//-----------------------------------------set and save to prefs----------------------------------------
	
	public void setMouseSensitivity (float set)
	{
//		QualitySettings.SetQualityLevel (set);
		CameraScript.mouseSensitivity = set;
		PlayerPrefs.SetFloat ("Settings_mouseSensitivity", set);
		QualityOptions.mouseSens = set;
		
	}

	public void setQualityLevel (int set)
	{
		QualitySettings.SetQualityLevel (set);
		PlayerPrefs.SetInt ("Settings_qualitylevel", set);
		t_qualityLevel = set;
		QualityOptions.qualityLevel = set;
		
	}
	
	void setQualityAntiAlias (bool set)
	{
		//0,2,4,8
		int AA = (int)Mathf.Pow (2, (QualitySettings.GetQualityLevel () - 2));
		AA = Mathf.Clamp (AA, 2, 8);

		QualitySettings.antiAliasing = !set ? 0 : AA;
//		UnityStandardAssets.ImageEffects.Antialiasing alias = 
//			Camera.main.GetComponent<UnityStandardAssets.ImageEffects.Antialiasing> ();
//		alias.enabled = set;
		PlayerPrefs.SetInt ("Settings_antialias", set ? 1 : 0);
		QualityOptions.antialiasing = set;
	}
	
	void setQualityBloom (bool set)
	{
		UnityStandardAssets.ImageEffects.Bloom bloom = 
			Camera.main.GetComponent<UnityStandardAssets.ImageEffects.Bloom> ();
		bloom.enabled = set;
		PlayerPrefs.SetInt ("Settings_bloom", set ? 1 : 0);
		QualityOptions.bloom = set;
	}
	
	void setQualityGrain (bool set)
	{
		UnityStandardAssets.ImageEffects.NoiseAndGrain grain = 
			Camera.main.GetComponent<UnityStandardAssets.ImageEffects.NoiseAndGrain> ();
		grain.enabled = set;
		PlayerPrefs.SetInt ("Settings_grain", set ? 1 : 0);
		QualityOptions.grain = set;
	}
	
	void setQualityShadow (bool set)
	{
		
		float defShadpowDist = Mathf.Pow (2, QualityOptions.qualityLevel + 3);
		if (QualityOptions.qualityLevel == 6)
			defShadpowDist = 1024;
		QualitySettings.shadowDistance = set ? defShadpowDist : 0f;
		PlayerPrefs.SetInt ("Settings_shadow", set ? 1 : 0);
		QualityOptions.showShadow = set;
	}
	
	void PrintSettings (string msg)
	{
		print (msg);
		print ("[2]:" + QualityOptions.qualityLevel);
		print ("[3]:" + QualityOptions.antialiasing);
		print ("[4]:" + QualityOptions.bloom);
		print ("[5]:" + QualityOptions.grain);
		print ("[6]:" + QualityOptions.showShadow);
	}
	
	string getQualityText (int ind)
	{
		switch (ind) {
		case 0:
			return "Fastest";
		case 1:
			return "Fast";
		case 2:
			return "Simple";
		case 3: 
			return "Good";
		case 4:
			return "Beautiful";
		case 5:
			return "Fantastic";
		case 6:
			return "Extreme";
		default:
			return "Normal";
		}
		
	}
	
	//----------------------------toggles-----------------------------------------------

	public void SwitcherQualityLevel (bool isNext)
	{
		if (isNext && t_qualityLevel < 6) {
			t_qualityLevel ++;
		} else if (!isNext && t_qualityLevel > 0) {
			t_qualityLevel --;
		}
		QualityText.text = getQualityText (t_qualityLevel);
	}
	
	//-----------------------------------------------SHORTCUT WINDOW-----------------------------------------------
	public void showShortcutDialog ()
	{
		ShortCutDialog.SetActive (true);
	}

	public void hideShortcutDialog ()
	{
		ShortCutDialog.SetActive (false);
	}
	//---------------------------------------------------------------------------------
	public void showLevelSelection ()
	{
		GameObject lv = canvasObj.FindChild ("LevelSelection").gameObject;
		lv.SetActive (true);
		lv.GetComponent<Animator> ().Play ("LevelSelectionEnter");
		lv.GetComponent<Animator> ().Rebind ();

	}

	public void hideLevelSelection ()
	{
		GameObject lv = canvasObj.FindChild ("LevelSelection").gameObject;
		lv.SetActive (true);
		lv.GetComponent<Animator> ().Play ("LevelSelectionExit");
	}

	public void ShowHelpTips ()
	{
//		GameObject lv = GameObject.Find("Canvas").transform.FindChild("HelpTip").gameObject;

		helptip.SetActive (true);
		Animator anim = helptip.GetComponent<Animator> ();
		anim.enabled = true;
		anim.SetBool ("isExiting", false);
		anim.Rebind ();

		menuShowing = true;

	}

	IEnumerator SetInactiveAfter (GameObject obj, float time)
	{
		yield return new WaitForSeconds (time);
		obj.SetActive (false);
	}

	public void HideHelpTips ()
	{

		helptip.SetActive (true);
		Animator anim = helptip.GetComponent<Animator> ();
		anim.enabled = true;
		anim.SetBool ("isExiting", true);
		menuShowing = false;
		StartCoroutine (SetInactiveAfter (helptip, 1f));
	}

	//--------------------------------------------Cheats-----------------------------------//
	public static void DimHealth ()
	{
		Instance.UI.FindChild ("Health").GetComponent<CanvasGroup> ().alpha = 0.2f;
	}

	public static void DimFuel ()
	{
		Instance.UI.FindChild ("Fuel").GetComponent<CanvasGroup> ().alpha = 0.2f;
	}
	//----------------------------------------------PAUSE RESUME GAME-----------------------------------
//	public static void PauseGame(){
//		Time.timeScale = Time.timeScale == 0 ? 1 : 0;
//		Instance.UI.Find("PauseIcon").gameObject.SetActive (Time.timeScale == 0);
//	}
	public void RestartGame ()
	{
		GameEvents.RestartLevel ();

	}

	public void MainMenu ()
	{
		GameEvents.LevelMainMenu ();
//		GameManager.LoadLevelNum (0);
	}

	public void Undock ()
	{
		DockedScript.ReleaseCalled ();
	}

	public void ToggleHeadlight ()
	{
		LightIntensity.Toggle ();
		
	}

	public void LoadLevel (bool IsNext)
	{
		if (IsNext)
			GameEvents.LevelNext ();
		else 
			GameEvents.LevelPrev ();
	}

	public static void ChangeDockIcon (bool isOn)
	{
		Color curr = Instance.dockIndicator.color;
		curr.a = isOn ? 1f : 0.35f;
		Instance.dockIndicator.color = curr;
	}

	public static void ChangeHeadlightIcon (int level)
	{
//		print ("level:"+level);
		if (level == 0) {
			Instance.lightIndicator.enabled = false;
		} else {
			Instance.lightIndicator.enabled = true;
			Instance.lightIndicator.sprite = Instance.headlights [level - 1];
		}
	}
	//-----------------------------------Score Display----------------------------------
	public static bool FromOptionsLeaderboard = false;

//	public void ToggleOptionsLeaderboard(){
//		GameObject lb = Instance.optionsPanel.transform.FindChild ("OnlineScoreDisplay").gameObject;
//		lb.SetActive(!lb.activeInHierarchy);
//	}

	public static void ScoreDisplayResults (string names, string scores)
	{
		Text nameT, scoreT;
		if (FromOptionsLeaderboard) {
			nameT = Instance.canvasObj.FindChild ("TabMenu/OnlineScoreDisplay/Panel/Area/Target/Text_names").GetComponent<Text> ();
			scoreT = Instance.canvasObj.FindChild ("TabMenu/OnlineScoreDisplay/Panel/Area/Target/Text_scores").GetComponent<Text> ();
		} else {
			nameT = Instance.successObjTarget.transform.FindChild ("Area/Target/Text_names").GetComponent<Text> ();
			scoreT = Instance.successObjTarget.transform.FindChild ("Area/Target/Text_scores").GetComponent<Text> ();
		}
		nameT.text = names;

		scoreT.text = scores;
	}

	public void ToggleDateRangeClicksOptions (int id)
	{
		FromOptionsLeaderboard = true;
		ScoreConnection.ReceiveScore (Application.loadedLevel, ScoreConnection.GetDateRanges () [id]);
	}

	public void ToggleDateRangeClicks (int id)
	{
		FromOptionsLeaderboard = false;
		ScoreConnection.ReceiveScore (Application.loadedLevel, ScoreConnection.GetDateRanges () [id]);
	}

	public static void RefreshScores ()
	{
		for (int i = 0; i < 3; i++) {
			if (Instance.successObjTarget.transform.FindChild ("ToggleGroup/" + i).GetComponent<Toggle> ().isOn) {
				Instance.ToggleDateRangeClicks (i);
				break;
			}
		}
	}

	public void ToggleConfirmSendScore ()
	{
		GameObject cnf = Instance.successObjTarget.transform.FindChild ("ConfirmSend").gameObject;
		cnf.SetActive (!cnf.activeInHierarchy);
//		GameEvents.StopListeningKeys = cnf.activeInHierarchy;
		GameEvents.StopListeningKeysMain = cnf.activeInHierarchy;
//		ScoreConnection.AddScore
	}

	public void SendScore ()
	{
		FromOptionsLeaderboard = false;

		float bestTime = GameManager.GetLevelSavedTime ();
//		float sentTime = GameManager.GetLevelSavedSentTime();
//		if (bestTime < sentTime){
		Text inp = Instance.successObjTarget.transform.FindChild ("ConfirmSend/Panel/InputField/Text").GetComponent<Text> ();
		string name = inp.text;
		if (string.IsNullOrEmpty (inp.text)) {
			print ("not valid");
			name = "coolperson";
		}
		ScoreConnection.AddScore (name, bestTime);

		ToggleConfirmSendScore ();
//		Button scoreButton = Instance.successObjTarget.transform.FindChild ("SendScore").GetComponent<Button> ();
//		scoreButton.interactable = false;
//		scoreButton.transform.FindChild("Text").GetComponent<Text>().text="Time Added";
		print ("TIME SENT: " + bestTime);
//		}else{
//			print("AN ERROR OCCURRED");
//		}

	}

	public static void ToggleScoreButton ()
	{
		float bestTime = GameManager.GetLevelSavedTime ();
		float sentTime = GameManager.GetLevelSavedSentTime ();

		print ("BestTime:" + bestTime + " ,senttime:" + sentTime);
		bool visibility = GameManager.GetLevelSavedMedal () == 3 && bestTime < sentTime;
		Button scoreButton = Instance.successObjTarget.transform.FindChild ("SendScore").GetComponent<Button> ();
		scoreButton.interactable = visibility;
		if (!visibility && bestTime >= sentTime) {
//			print("SENT FROM 1");
			ChangeSendScoreText (true);
		}
	}

	public static void ChangeSendScoreText (bool TimeAdded)
	{
		Text scoreButton = Instance.successObjTarget.transform.FindChild ("SendScore/Text").GetComponent<Text> ();

		string str = TimeAdded ? "Time Added" : "Try Again";

		scoreButton.text = str;
		scoreButton.transform.parent.gameObject.GetComponent<Button> ().interactable = !TimeAdded;
	}


	//----------------------------------------------SUCESS------------------------------
	public static void Success ()
	{
		FromOptionsLeaderboard = false;
		Instance.UI.gameObject.GetComponent<CanvasGroup> ().alpha = 0f;
	}

	public void ToggleTabLeaderboard ()
	{
		Animator anim = Instance.canvasObj.FindChild ("TabMenu/OnlineScoreDisplay").GetComponent<Animator> ();

		anim.enabled = true;
		bool currReverse = anim.GetBool ("reverse");
		anim.SetBool ("reverse", !currReverse);

//		anim.Rebind ();
//		anim.speed=anim.speed==1?-1:1;
//		anim.Play("SlideLeft");
	}
}
