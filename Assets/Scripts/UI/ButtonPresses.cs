using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonPresses : MonoBehaviour
{
	#region Declarations

	public static ButtonPresses Instance;
	public static bool menuShowing = false, isMainMenu = false, inTutorial = false, FromOptionsLeaderboard = false;
	private OptionsDetails QualityOptions, OptionsSaved;
	private Text QualityText;
	private Toggle t_antialias, t_bloom, t_grain, t_shadow;
	private Slider t_mouse;
	private int t_qualityLevel;
	public bool NoPause = false, StartScreenFade = false, GoToMainMenuNow = false;
	private GameObject MoreOptionsPanel, optionsPanel, helptip, TabMenu, ShortCutDialog, tutDialog, player;
	private Transform successObjTarget, canvasObj, UI;
	public Sprite[] headlights, CriteriaBox1Sprites;
	private Image lightIndicator, dockIndicator;
	private float ttime = 0.35f;

	//	[System.Serializable]
	public class OptionsDetails
	{
		//		[Header("Dynamic bool")]
		public bool antialiasing = false, bloom = false, grain = false, showShadow = false;
		public int qualityLevel = 4;
		public float mouseSens = 0.5f;
	}

	#endregion

	#region Mono Behaviour

	void Awake ()
	{
		Instance = this;
		QualityOptions = new OptionsDetails ();
//		new ScoreConnection ();

		canvasObj = GameObject.Find ("Canvas").transform;
		isMainMenu = Application.loadedLevel == 0;
		if (!isMainMenu) {
			Instance.lightIndicator = canvasObj.FindChild ("UI/Other/LightIndicator/Image").gameObject.GetComponent<Image> ();
			Instance.dockIndicator = canvasObj.FindChild ("UI/Other/DockIndicator/Image").gameObject.GetComponent<Image> ();
		}
	}

	// Use this for initialization
	void Start ()
	{
		optionsPanel = canvasObj.transform.FindChild ("OptionsPanel").gameObject;
		MoreOptionsPanel = optionsPanel.transform.FindChild ("MoreOptions").gameObject;
		ShortCutDialog = optionsPanel.transform.FindChild ("ShortcutDialog").gameObject;
		UI = canvasObj.FindChild ("UI").transform;
		successObjTarget = canvasObj.FindChild ("GameSuccessImage/OnlineScoreDisplay/Panel");
		player = GameObject.Find ("Player").gameObject;
		helptip = canvasObj.FindChild ("HelpTip").gameObject;
		//------------------------------------
		inTutorial = false;
		menuShowing = false;

		initSettings ();

		if (isMainMenu) {
			ScoreConnection.ReceiveRemoteVersion ();
			Button start = Instance.canvasObj.transform.FindChild ("MainMenu/Start").GetComponent<Button> ();
			start.Select ();
		} else {

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
//			print ("this level:"+Application.loadedLevel+" ,next allowed:"+LevelManager.GetNextLevel ());
			if (LevelManager.GetNextLevel () != Application.loadedLevel + 1) {
				optionsPanel.transform.FindChild ("PrevNextGroup/Next").GetComponent<Button> ().interactable = false;
			}
			//options Criteria
			FromOptionsLeaderboard = true;
			initialiseCriteriaIcons (true);
			ToggleDateRangeClicksOptions (2);

			Transform tut0 = canvasObj.FindChild ("Tutorial0");
			tutDialog = tut0.FindChild ("Dialog").gameObject;
			tut0.gameObject.SetActive ((Application.loadedLevel == 1));
			//-------sucess menus
			int lev = Application.loadedLevel;
			if (lev == 1) {
				canvasObj.FindChild ("GameSuccessImage/TopBar/PrevLevel").gameObject.GetComponent<Button> ().interactable = false;
				tut0.FindChild ("Dialog/No").gameObject.GetComponent<Button> ().Select ();
			}
//			else if (lev==Application.levelCount-1) 
//				canvasObj.FindChild ("GameSuccessImage/TopBar/NextLevel").gameObject.GetComponent<Button>().interactable=false;
		}
		
	}

	void Update ()
	{
		if (!GameEvents.StopListeningKeys) {
			if (!NoPause && Input.GetKeyDown (KeyCode.Escape)) {
				if (Application.loadedLevel > 0) {
					GameObject[] dialogs = {tutDialog, TabMenu, ShortCutDialog, MoreOptionsPanel,
						optionsPanel, helptip
					};
					bool wasActive = false;
					foreach (var di in dialogs) {
						if (di != null && di.activeInHierarchy) {
							wasActive = true;
							
							if (di == TabMenu)
								ToggleTabPanel ();
							else if (di == ShortCutDialog)
								hideShortcutDialog ();
							else if (di == MoreOptionsPanel)
								hideMoreSettings ();
							else if (di == optionsPanel)
								ToggleActionPanel ();
							else if (di == helptip)
								HideHelpTips ();
							else if (di == tutDialog)
								RejectTutorial ();
							break;
						}
					}
					if (!wasActive) {
						ToggleActionPanel ();
						menuShowing = true;
					} else {
						menuShowing = false;
					}
				} else {
					if (MoreOptionsPanel.activeInHierarchy) {
						hideMoreSettingsOnly ();
					} else {
						showMoreSettingsOnly ();
					}
					
				}
			}
			if (Input.GetKeyDown (KeyCode.T)) {
				if (!menuShowing && !helptip.activeInHierarchy)
					ShowHelpTips ();
				else
					HideHelpTips ();
			}
			if (Input.GetKeyDown (KeyCode.Tab)) {
				ToggleTabPanel ();
			}
		}
		if (Input.GetKeyUp (KeyCode.Return) && GameEvents.LevelSuccess) {
			Instance.LoadLevel (true);
		}
		if (GoToMainMenuNow) {
			GoToMainMenuNow = false;
			MainMenu ();
		}
	}

	#endregion

	#region Main Menu

	public void OpenGamePage(){
		Application.OpenURL ("https://aneilator.itch.io/docker");
	}

	public static IEnumerator WaitForVersion (WWW www, string receiving)
	{
		yield return www;
		// check for errors
		string data = www.text;
		bool successful = www.error == null && data != null && data != "";

		Text vtext = Instance.optionsPanel.transform.FindChild ("About/Version").GetComponent<Text> ();
		float yourVersion = ScoreConnection.GetCurrentGameVersion ();
		vtext.text = "Version: " + yourVersion + "\r\n";

		if (receiving == "check-update") {
			Debug.Log ("RESPONSE: " + data);
			if (successful) {

				float remoteVersion = ScoreConnection.AboutStringParseVersion (data);
//				System.DateTime date = ScoreConnection.AboutStringParseTime (data);

				Text updTxt = Instance.optionsPanel.transform.FindChild ("About/vtest").GetComponent<Text> ();
				GameObject updateReminder = Instance.canvasObj.FindChild ("Update_Reminder").gameObject;
				if (yourVersion < remoteVersion) {
					print ("UPDATE AVAILABLE!");
					updateReminder.SetActive (true); 
					updTxt.text = "Updated v" + remoteVersion + " available!";
				} else {
					print ("No Update Available");
					updTxt.text = "No update available!";
				}
			} else {
				print ("Could not check for update!");
			}
			

		}
		
	}
	//---------------------------------------------------------------------------------
	public void showLevelSelection ()
	{
		GameObject lv = canvasObj.FindChild ("LevelSelection").gameObject;
		lv.SetActive (true);
		lv.GetComponent<Animator> ().Play ("LevelSelectionEnter");
		lv.GetComponent<Animator> ().Rebind ();
		lv.transform.FindChild ("Actual/Area/Grid/Level#1/Button").gameObject.GetComponent<Button> ().Select ();
	}

	public void hideLevelSelection ()
	{
		GameObject lv = canvasObj.FindChild ("LevelSelection").gameObject;
		lv.SetActive (true);
		lv.GetComponent<Animator> ().Play ("LevelSelectionExit");
	}

	//--------------------------------------------------------------------------------------------------------

	#endregion

	#region Game Dialogs

	public void ToggleTabPanel ()
	{
		if (!menuShowing) {
			if (inTutorial) {
				ToggleTabLeaderboard ();
			} else if (Time.timeScale != 0) {
				GameEvents.PauseGame ();
			} else if (TabMenu.activeInHierarchy) {
				GameEvents.UnPauseGame ();
			}
			TabMenu.GetComponent<CanvasGroup> ().alpha = 1f;
			Animator anim = Instance.canvasObj.FindChild ("TabMenu/OnlineScoreDisplay").GetComponent<Animator> ();
			bool rev = anim.GetBool ("reverse");
			
			//hide
			if (TabMenu.activeInHierarchy) {
				Instance.StartCoroutine (AnimationScript.FadeOutPanel_Disable (TabMenu, 0.1f));
				Instance.StartCoroutine (AnimationScript.FadeInPanel_Enable (UI.gameObject, 1f, 0.1f, true));
				if (!rev)
					ToggleTabLeaderboard ();
			} 
			//show
			else {
				TabMenu.SetActive (true);
				anim.enabled = false;
				UI.gameObject.GetComponent<CanvasGroup> ().alpha = 0f;
				//			if (rev) ToggleTabLeaderboard ();
			}
			
			//		menuShowing = TabMenu.activeInHierarchy;
			//		MoreOptionsPanel.gameObject.SetActive (false);
		}
	}

	public void ToggleActionPanel ()
	{
		
		
		if (!menuShowing) {
			Animator anim = optionsPanel.GetComponent<Animator> ();
			anim.enabled = true;
			anim.SetBool ("isExiting", false);
			anim.Rebind ();
			Button resume = Instance.optionsPanel.transform.FindChild ("OptionsBox/Resume").GetComponent<Button> ();
			resume.Select ();
		}
		optionsPanel.SetActive (!optionsPanel.activeInHierarchy);
		menuShowing = optionsPanel.activeInHierarchy;
		if (Time.timeScale != 0) {
			GameEvents.PauseGame ();
		} else if (!menuShowing && !TabMenu.activeInHierarchy) {
			GameEvents.UnPauseGame ();
		}
	}

	public void showMoreSettings ()
	{
//		StartCoroutine(AnimationScript.FadeInPanel_Enable(MoreOptionsPanel, 1f, ttime, false));
		MoreOptionsPanel.SetActive (true);
		Animator anim = MoreOptionsPanel.GetComponent<Animator> ();
		anim.enabled = true;

		ResetToPrevSettings (OptionsSaved);
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

	public void showMoreSettingsOnly ()
	{
		menuShowing = true;
		optionsPanel.SetActive (true);
//		menuShowing = optionsPanel.activeInHierarchy;

		MoreOptionsPanel.SetActive (true);
//		ResetToPrevSettings (OptionsSaved);
	}

	public void hideMoreSettingsOnly ()
	{
		optionsPanel.SetActive (false);
		menuShowing = false;

		QualityOptions = OptionsSaved;
		MoreOptionsPanel.SetActive (false);

	}

	public void showShortcutDialog ()
	{
		ShortCutDialog.SetActive (true);
		if (!isMainMenu) {
			Animator anim = ShortCutDialog.GetComponent<Animator> ();
			anim.enabled = true;
		}
	}

	public void hideShortcutDialog ()
	{
		ShortCutDialog.SetActive (false);
	}

	public void ShowHelpTips ()
	{
		//		GameObject lv = GameObject.Find("Canvas").transform.FindChild("HelpTip").gameObject;
		StartCoroutine (AnimationScript.FadeInPanel_Enable (helptip, 1f, ttime, false));
//		helptip.SetActive (true);
//		Animator anim = helptip.GetComponent<Animator> ();
//		anim.enabled = true;
//		anim.SetBool ("isExiting", false);
//		anim.Rebind ();
		
		menuShowing = true;
		
	}

	public void HideHelpTips ()
	{
		
//		helptip.SetActive (false);
		menuShowing = false;
		StartCoroutine (AnimationScript.FadeOutPanel_Disable (helptip, ttime));
		//		Animator anim = helptip.GetComponent<Animator> ();
		//		anim.enabled = true;
		//		anim.SetBool ("isExiting", true);
		//		StartCoroutine (SetInactiveAfter (helptip, 1f));
	}

	public void ToggleAbout ()
	{
		GameObject about = Instance.optionsPanel.transform.FindChild ("About").gameObject;
		about.SetActive (!about.activeInHierarchy);
	}

	public void ResetSettings ()
	{
		Text lv = canvasObj.FindChild ("LevelSelection/Actual/Reset/Text").gameObject.GetComponent<Text> ();
		if (lv.text.Equals ("Reset Progress"))
			lv.text = "Click again to Confirm";
		else {
			PlayerPrefs.DeleteAll ();
			GameEvents.RestartLevel ();
		}
	}

	#endregion

	#region settings prefs

	void initSettings ()
	{
		
		t_mouse = MoreOptionsPanel.transform.FindChild ("Line1/Slider").gameObject.GetComponent<Slider> ();
		QualityText = MoreOptionsPanel.transform.FindChild ("Line2/Switcher/QualityText").gameObject.GetComponent<Text> ();
		t_antialias = MoreOptionsPanel.transform.FindChild ("Line3/Toggle").gameObject.GetComponent<Toggle> ();
		t_bloom = MoreOptionsPanel.transform.FindChild ("Line4/Toggle").gameObject.GetComponent<Toggle> ();
		t_grain = MoreOptionsPanel.transform.FindChild ("Line5/Toggle").gameObject.GetComponent<Toggle> ();
		t_shadow = MoreOptionsPanel.transform.FindChild ("Line6/Toggle").gameObject.GetComponent<Toggle> ();
		
		try {
			
			//first time default:
			bool firstTime = PlayerPrefs.GetInt ("Game_first_time") == 0 ? true : false;
			PlayerPrefs.SetInt ("Game_first_time", 1);
			if (firstTime) {
				PlayerPrefs.SetInt ("Settings_bloom", 1);
				PlayerPrefs.SetInt ("Settings_antialias", 1);
				PlayerPrefs.SetInt ("Settings_qualitylevel", 5);
			}
			//--------------------
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
			print ("ERROR occurred retrieving settings \n" + ex);
		} finally {
			
			//-----------
			ResetToPrevSettings (QualityOptions);
			saveSettings ();
			ApplyMoreSettings (true);
			
		}
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
			t_qualityLevel++;
		} else if (!isNext && t_qualityLevel > 0) {
			t_qualityLevel--;
		}
		QualityText.text = getQualityText (t_qualityLevel);
	}

	#endregion

	#region tutorial

	public void AcceptTutorial ()
	{
		inTutorial = true;
//		tutDialog.SetActive (false);
		StartCoroutine (AnimationScript.FadeOutPanel_Disable (tutDialog, 0.7f));
		StartCoroutine (TutorialController ());
	}

	public void RejectTutorial ()
	{
		inTutorial = false;
		StartCoroutine (AnimationScript.FadeOutPanel_Disable (tutDialog, 0.7f));
//		tutDialog.SetActive (false);
		StartCoroutine (ShowSimpleHelpMsg ());

	}

	IEnumerator ShowSimpleHelpMsg ()
	{
		float ttime = 0.7f;
		GameObject tut0 = Instance.canvasObj.FindChild ("Tutorial0").gameObject;
		Text objectiveMain = tut0.transform.FindChild ("Objective_info").gameObject.GetComponent<Text> ();
		Text objective = tut0.transform.FindChild ("Objective").gameObject.GetComponent<Text> ();
		Text tipInfo = tut0.transform.FindChild ("Tip_Info").gameObject.GetComponent<Text> ();
		Instance.StartCoroutine (AnimationScript.ChangeText (objectiveMain, "OBJECTIVE:", ttime));
		Instance.StartCoroutine (AnimationScript.ChangeText (objective, "Dock with the satellite", ttime));
		yield return new WaitForSeconds (8f);
		Instance.StartCoroutine (AnimationScript.ChangeText (objectiveMain, "", ttime));
		Instance.StartCoroutine (AnimationScript.ChangeText (objective, "", ttime));
		yield return new WaitForSeconds (6f);
		Instance.StartCoroutine (AnimationScript.ChangeText (tipInfo, "Need tips? Press T", ttime));
		yield return new WaitForSeconds (10f);
		Instance.StartCoroutine (AnimationScript.ChangeText (tipInfo, "", ttime));
		yield return new WaitForSeconds (1f);
		//END------------------
		tut0.SetActive (false);
		yield break;
	}

	void SetUpTutorial ()
	{
		float size = 100f;
		CameraScript cameraScript = Camera.main.GetComponent<CameraScript> ();
		cameraScript.minX = -size;
		cameraScript.minY = -size;
		cameraScript.maxX = size;
		cameraScript.maxY = size;
		cameraScript.DrawOutline (false);

		float dl = 0.3f;
		Vector3 newCamPos = Camera.main.transform.position;
		newCamPos.z = -600f;
		Instance.StartCoroutine (AnimationScript.AnimatePrefabXYZ (Camera.main.gameObject, newCamPos, 0.6f, dl));

		GameObject.Find ("CenterPuller").GetComponent<SpringJoint2D> ().frequency = 0.14f;
		GameObject parent = GameObject.Find ("AsteroidGroup");
		Instance.StartCoroutine (AnimationScript.AnimatePrefabPos (parent, 1.5f, 0.8f, dl += 0.7f));

		GameObject satellite = GameObject.Find ("Satellite").gameObject;
		Collider2D[] colliders = satellite.GetComponentsInChildren<Collider2D> ();
		foreach (Collider2D col in colliders)
			col.enabled = false;
		Vector3 newPos = satellite.transform.position;
		newPos.z = 300f;

		Instance.StartCoroutine (AnimationScript.AnimatePrefabXYZ (satellite, newPos, 0.6f, dl += 0.2f));

		Vector3 newPosPlayer = player.transform.position;
		newPosPlayer.x = 0f;
		newPosPlayer.y = -50f;
		Instance.StartCoroutine (AnimationScript.AnimatePrefabXYZ (player, newPosPlayer, 1.3f, dl += 0.5f));
		Instance.StartCoroutine (AnimationScript.AnimatePrefabRotation (player, new Vector3 (0f, 0f, 90f), 1.3f, dl));
		GameEvents.CheatOn (false);

		Instance.StartCoroutine (AnimationScript.AnimatePrefabZ (Camera.main.gameObject, -150f, 3f, dl += 1.5f));
	}

	IEnumerator TutorialController ()
	{
		float ttime = 0.7f;
		SetUpTutorial ();

		yield return new WaitForSeconds (5f);
		GameObject tut0 = Instance.canvasObj.FindChild ("Tutorial0").gameObject;
		Text objectiveMain = tut0.transform.FindChild ("Objective_info").gameObject.GetComponent<Text> ();
		Instance.StartCoroutine (AnimationScript.ChangeText (objectiveMain, "OBJECTIVE:", ttime));

		yield return new WaitForSeconds (1f);
		Text objective = tut0.transform.FindChild ("Objective").gameObject.GetComponent<Text> ();
		objective.text = "Hold W to accelerate forwards";
		//----------1
//		GameEvents.PlayerFuelUnlimited(true);
//		DimFuel(true);
		//----------1
		while (!Input.GetKey (KeyCode.W))
			yield return new WaitForEndOfFrame ();

		//-----------2
		yield return new WaitForSeconds (1.5f);
		Instance.StartCoroutine (AnimationScript.ChangeText (objective, "Let Go", ttime));
		yield return new WaitForSeconds (2.5f);
//		/*
		//-----------3
		Instance.StartCoroutine (AnimationScript.ChangeText (objective, "Hold S to accelerate backwards", ttime));
		while (!Input.GetKey (KeyCode.S))
			yield return new WaitForEndOfFrame ();
		yield return new WaitForSeconds (1.5f);
		Instance.StartCoroutine (AnimationScript.ChangeText (objective, "Let Go", ttime));
		yield return new WaitForSeconds (2.5f);
		//-----------3
		Instance.StartCoroutine (AnimationScript.ChangeText (objective, "Hold A to accelerate left\n and D to accelerate right", ttime));
		while (!Input.GetKey (KeyCode.A) && !Input.GetKey (KeyCode.D))
			yield return new WaitForEndOfFrame ();
		yield return new WaitForSeconds (2f);
		//-----------3
		Instance.StartCoroutine (AnimationScript.ChangeText (objective, "Going good. Now lets learn to rotate...", ttime));
		yield return new WaitForSeconds (3f);
		//-----------3
		Instance.StartCoroutine (AnimationScript.ChangeText (objective, "Hold Q to rotate left", ttime));
		while (!Input.GetKey (KeyCode.Q))
			yield return new WaitForEndOfFrame ();
		yield return new WaitForSeconds (2f);
		//-----------3
		Instance.StartCoroutine (AnimationScript.ChangeText (objective, "Hold E to rotate right", ttime));
		while (!Input.GetKey (KeyCode.E))
			yield return new WaitForEndOfFrame ();
		yield return new WaitForSeconds (2f);
		Instance.StartCoroutine (AnimationScript.ChangeText (objective, "You're a natural!", ttime));
		yield return new WaitForSeconds (2f);
		Instance.StartCoroutine (AnimationScript.ChangeText (objective, "That's all from maneuvering!", ttime));
		yield return new WaitForSeconds (3f);

		//-----------3
		Instance.StartCoroutine (AnimationScript.ChangeText (objective,
			"1. Click and Drag mouse to pan camera around     (or Arrow Keys)\n" +
			"2. Scroll mouse to zoom in-out                            (or RCtrl/RCmd + Arrow Keys)\n" +
			"3. Right click to reset camera                               (or Tap C)\n\n" +
			"When you are ready to proceed, press ENTER.", ttime));
		while (!Input.GetKey (KeyCode.Return))
			yield return new WaitForEndOfFrame ();
		//-----------3
		Instance.StartCoroutine (AnimationScript.ChangeText (objective, "Great!", ttime));
		yield return new WaitForSeconds (2f);
		Instance.StartCoroutine (AnimationScript.ChangeText (objective, "Now, let's see if you can...\n" +
		"Collect at least 6 red orbs\n" +
		"\n" +
		"(Use the maneuvering keys to move around)", ttime));

		Vector3 newPosPlayer = player.transform.position;
		newPosPlayer.x = 0f;
		newPosPlayer.y = 0f;
		Vector3 angle = new Vector3 (0f, 0f, 90f);
//		print(player.GetComponent<Rigidbody2D>().velocity);
		Instance.StartCoroutine (AnimationScript.AnimatePrefabXYZ (player, newPosPlayer, 1.5f, 0.8f));
		Instance.StartCoroutine (AnimationScript.AnimatePrefabRotation (player, angle, 1.5f, 0.8f));
//		print(player.GetComponent<Rigidbody2D>().velocity);

		while (!GameEvents.LevelFail && GameEvents.Score < 6) {
			yield return new WaitForEndOfFrame ();
		}
		//--------------
		Instance.StartCoroutine (AnimationScript.ChangeText (objective, "Impressive!", ttime));
		yield return new WaitForSeconds (3f);
		Instance.StartCoroutine (AnimationScript.ChangeText (objective, "One more thing...", ttime));
		yield return new WaitForSeconds (3f);
		Instance.StartCoroutine (AnimationScript.ChangeText (objective, 
			"Stabilisers assist beginners by automatically bringing them down to a stop\n" +
			"For best experience, the stabilisers are turned off by default in future levels\n\n" +
			"Press X to disable stabilisers\n", ttime));
		while (!Input.GetKey (KeyCode.X))
			yield return new WaitForEndOfFrame ();
		//----------kk
		Instance.StartCoroutine (AnimationScript.ChangeText (objective, "Now your movement is frictionless. Try it!\n\nPress Enter when you are ready to proceed", ttime));
		while (!Input.GetKey (KeyCode.Return))
			yield return new WaitForEndOfFrame ();
		Instance.StartCoroutine (AnimationScript.ChangeText (objective, "But before we begin, a quick guide of game options...", ttime));
		yield return new WaitForSeconds (3f);
		Instance.StartCoroutine (AnimationScript.ChangeText (objective, "Your main aim in each level is to dock with the satellite...\n" +
		"Press TAB to see Level objectives and leaderboard", ttime));
		while (!Input.GetKey (KeyCode.Tab))
			yield return new WaitForEndOfFrame ();
		Instance.StartCoroutine (AnimationScript.ChangeText (objectiveMain, "", ttime));
		Instance.StartCoroutine (AnimationScript.ChangeText (objective, "", ttime));
		yield return new WaitForSeconds (1.5f);
		Instance.StartCoroutine (AnimationScript.ChangeText (objectiveMain, "OBJECTIVE:", ttime));
		Instance.StartCoroutine (AnimationScript.ChangeText (objective, "The icons on the left show game objectives\n" +
		"     1. Dock\n     2. Score 10 (Collect orbs)\n     3. Time: within 15 seconds\n\n" +
		"If you complete all 3 objectives, you get a gold medal\n" +
		"When you have a gold medal, you can add your best time on the online leaderboard (right)\n\n" +
		"Press TAB again to proceed", ttime));
		while (!Input.GetKey (KeyCode.Tab))
			yield return new WaitForEndOfFrame ();
//		ToggleTabPanel ();
		Instance.StartCoroutine (AnimationScript.ChangeText (objective, "That's it for now...", ttime));
		yield return new WaitForSeconds (3f);
		Instance.StartCoroutine (AnimationScript.ChangeText (objective, "Let's Dock...", ttime));
		yield return new WaitForSeconds (3f);

		//-----------SATELLITE
//		*/
		Instance.StartCoroutine (AnimationScript.ChangeText (objective, 
			"To dock with the satellite, advance ahead until your nose is within its docking area.\n", ttime));
		//------
		Vector3 newPosPlayer2 = player.transform.position;
		newPosPlayer2.x = 0f;
		newPosPlayer2.y = -30f;
		Instance.StartCoroutine (AnimationScript.AnimatePrefabXYZ (player, newPosPlayer2, 1.5f, 1.5f));
		Instance.StartCoroutine (AnimationScript.AnimatePrefabRotation (player, new Vector3 (0f, 0f, 90f), 1.5f, 1.5f));

		yield return new WaitForSeconds (3f);
		GameObject satellite = GameObject.Find ("Satellite").gameObject;
		Collider2D[] colliders = satellite.GetComponentsInChildren<Collider2D> ();
		foreach (Collider2D col in colliders)
			col.enabled = true;
		Vector3 newPos = satellite.transform.position;
		newPos.z = 0f;
		
		Instance.StartCoroutine (AnimationScript.AnimatePrefabXYZ (satellite, newPos, 0.6f, 0f));

		//-----------3
		while (!GameEvents.LevelFail && !GameEvents.docked) {
			yield return new WaitForEndOfFrame ();
		}
		//-----------3
//		Instance.StartCoroutine (AnimationScript.ChangeText (objective, "Wow!", ttime));
//		yield return new WaitForSeconds (3f);
		Instance.StartCoroutine (AnimationScript.ChangeText (objective, "Congratulations. You have finished your training...\n\n" +
		"If you'd like to play around more, You can press F to undock\n" +
		"Whenever you feel ready, press SPACE BAR to start Level 1!", ttime));

		while (!Input.GetKey (KeyCode.F) && !Input.GetKey (KeyCode.Return)) {
			yield return new WaitForEndOfFrame ();
		}
		Instance.StartCoroutine (AnimationScript.ChangeText (objective, "", ttime));
//		Instance.StartCoroutine (AnimationScript.ChangeText (objective, "Brilliant!", ttime));
//		yield return new WaitForSeconds (3f);
//		Instance.StartCoroutine (AnimationScript.ChangeText (objective, "Here we go!", ttime));
//		yield return new WaitForSeconds (3f);
//		GameEvents.RestartLevel ();
//		GameEvents.PlayerFuelUnlimited(false);
//		DimFuel(false);
	}

	#endregion

	#region Game Commands

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
		if (IsNext) {
			if (Application.loadedLevel == Application.levelCount - 1) {
				Transform lev = GameObject.Find ("Level").transform;
				Destroy (lev.FindChild ("AsteroidGroup").gameObject);

//				player.GetComponent<Collider2D> ().enabled = false;
//				MonoBehaviour[] colls = player.GetComponents<MonoBehaviour> ();
//				for (int c = 0; c < colls.Length; c++) Destroy(colls[c]);
//					colls [c].enabled = false;
				Destroy (player.GetComponent<CenterOfMass> ());
				Destroy (player.GetComponent<GeyserControl> ());
				Destroy (player.GetComponent<Moves> ());
				Destroy (player.GetComponent<HealthLoss> ());
				Destroy (player.GetComponent<SpringJoint2D> ());
				Destroy (player.GetComponent<Rigidbody2D> ());
				Destroy (player.transform.FindChild ("outline").gameObject);
				Destroy (player.transform.FindChild ("Nose").gameObject);
				Destroy (player.transform.FindChild ("Trail").gameObject);

				GameObject satellite = lev.FindChild ("Dockables/Satellite").gameObject;

				Destroy (satellite.GetComponent<AnimationMovements> ());
				Destroy (satellite.GetComponent<SpringJoint2D> ());
				Destroy (satellite.GetComponent<Rigidbody2D> ());
				Destroy (satellite.transform.FindChild ("outline").gameObject);
				Destroy (satellite.transform.FindChild ("touchdown").gameObject);
				Destroy (satellite.transform.FindChild ("DockArea").gameObject);


//				player.transform.Rotate (new Vector3 (0, 0, 90f));
//				satellite.transform.Rotate (new Vector3 (0, 0, 90f));

				Instance.StartCoroutine (
					AnimationScript.FadeOutPanel (
						Instance.canvasObj.FindChild ("GameSuccessImage").gameObject.GetComponent<CanvasGroup> ()
					, 2f));
				lev.GetComponent<Animator> ().enabled = true;

			} else {
				GameEvents.LevelNext ();
			}
		} else
			GameEvents.LevelPrev ();
	}

	#endregion

	#region UI Icons

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

	//--------------------------------------------Cheats-----------------------------------//
	public static void DimHealth (bool dim)
	{
		
		Instance.UI.FindChild ("Health").GetComponent<CanvasGroup> ().alpha = dim ? 0.2f : 1f;
	}

	public static void DimFuel (bool dim)
	{
		Instance.UI.FindChild ("Fuel").GetComponent<CanvasGroup> ().alpha = dim ? 0.2f : 1f;
	}

	#endregion

	#region Success Screen

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
//			print ("not valid");
			name = "coolperson";
		}
		ScoreConnection.AddScore (name, bestTime);

		ToggleConfirmSendScore ();
//		Button scoreButton = Instance.successObjTarget.transform.FindChild ("SendScore").GetComponent<Button> ();
//		scoreButton.interactable = false;
//		scoreButton.transform.FindChild("Text").GetComponent<Text>().text="Time Added";
//		print ("TIME SENT: " + bestTime);
//		}else{
//			print("AN ERROR OCCURRED");
//		}

	}

	public static void ToggleScoreButton ()
	{
		float bestTime = GameManager.GetLevelSavedTime ();
		float sentTime = GameManager.GetLevelSavedSentTime ();

//		print ("BestTime:" + bestTime + " ,senttime:" + sentTime);
		bool visibile = GameManager.GetLevelSavedMedal () == 3 && bestTime < sentTime;
		Button scoreButton = Instance.successObjTarget.transform.FindChild ("SendScore").GetComponent<Button> ();
		Text info = Instance.successObjTarget.transform.FindChild ("SendScore/DisabledInfo").GetComponent<Text> ();
		scoreButton.interactable = visibile;
		if (visibile) {
			info.text = "";
		} else {
			info.text = GameManager.getNoSubmitInfo ();
		}
		if (!visibile && bestTime >= sentTime && sentTime > 0) {
			ChangeSendScoreText (true);
		} else {
		}
		if (LevelManager.GetNextLevel () != Application.loadedLevel + 1) {
			Instance.canvasObj.FindChild ("GameSuccessImage/TopBar/NextLevel").gameObject.
				GetComponent<Button> ().interactable = false;
		}
		GameObject tutmain = Instance.canvasObj.FindChild ("Tutorial0").gameObject;
		tutmain.SetActive (false);
//		print (tutmain.activeInHierarchy);

//		tootlitSubmit.isDisabled=vi
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

	#endregion
}
