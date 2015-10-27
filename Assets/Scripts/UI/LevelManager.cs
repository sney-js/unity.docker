using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelManager : MonoBehaviour {

	public Transform LevelGrid;
//	private static int totLevels;
	public Sprite[] levelScreenshots;
	public int totLevels=12;
	// Use this for initialization
	void Start () {
//		totLevels = LevelGrid.childCount;
		Sprite[] screens = Resources.LoadAll<Sprite>("Images/UI/ScreenshotsLevel/");

		PlayerPrefs.SetInt("Level#" + 4 + "_Medal", 3);
		PlayerPrefs.SetInt("Level#" + 1 + "_Medal", 3);
		PlayerPrefs.SetInt("Level#" + 2 + "_Medal", 3);
		PlayerPrefs.SetInt("Level#" + 3 + "_Medal", 3);
		PlayerPrefs.SetInt("Level#" + 4 + "_Medal", 3);
		PlayerPrefs.SetInt("Level#" + 5 + "_Medal", 3);
		PlayerPrefs.SetInt("Level#" + 6 + "_Medal", 3);
		PlayerPrefs.SetInt("Level#" + 7 + "_Medal", 3);
		PlayerPrefs.SetInt("Level#" + 8 + "_Medal", 3);
		PlayerPrefs.SetInt("Level#" + 9 + "_Medal", 3);
		PlayerPrefs.SetInt("Level#" + 10 + "_Medal",3);

		Color bronze = normColor(124,76,76,1);
		Color silver = normColor(173,173,173,1);
		Color gold = normColor(255,220,60,1);
		Color trans = normColor(0,0,0,0);

		bool allGold=HasAllGold();
		bool allSilver=HasAllSilver();
		bool ok=true;

		//------------------------------instantiate-------------------------------------------
		for (int i = 1; i < totLevels; i++) {
			GameObject level= (GameObject) Instantiate(LevelGrid.transform.GetChild(0).gameObject);
			level.name = "Level#"+(i+1);
			level.transform.parent = LevelGrid.transform;
		}

		//---------------------------------------------------------------------------------------------------

		for (int i = 0; i < totLevels; i++) {

			Transform level = LevelGrid.transform.GetChild(i);
			//---------------------------set Text/Image----
			level.FindChild("Text").GetComponent<Text>().text = GameManager.GetLevelName(i+1);

			if (i==totLevels-2) {
				level.FindChild("ReqText").gameObject.SetActive(true);
				level.FindChild("ReqText").GetComponent<Text>().text = "Required: All Silver";
			}
			else if (i==totLevels-1) {
				level.FindChild("ReqText").gameObject.SetActive(true);
				level.FindChild("ReqText").GetComponent<Text>().text = "Required: All Gold";
			}

			Sprite myscreen = screens[0];
			for (int j = 0; j < screens.Length; j++) {
				if (screens[j].name==(i+1).ToString()){
					myscreen=screens[j];
					break;
				}
			}

			level.FindChild("Image").GetComponent<Image>().sprite = myscreen;
			//-----------------------------------------------

			string key = "Level#"+(i+1)+"_Medal";
			int achievedMedal = PlayerPrefs.GetInt(key);

			//----------------------------MEDAL-----------------------------------------------
			Color medColor = achievedMedal==0?trans:achievedMedal==1?bronze:achievedMedal==2?silver:gold;
			level.Find("Medal").gameObject.GetComponent<Image>().color = medColor;
			if (achievedMedal<3){
				allGold=false;
				if (achievedMedal<2){
					allSilver=false;
				}
			}

			//----------------------------------------LOCK/BUTTON---------------------------------------------
			bool levAllowed = IsLevelAllowed(i+1) && ok;
			if (!levAllowed) ok=false;
//			print("Level "+(i+1)+" Allowed?="+levAllowed);

//			bool lockRelease = ((achievedMedal==0 && !ok) || (i==totLevels-1 && !allGold) || (i==totLevels-2 && !allSilver))
//				&& (i!=0);

			Button button = level.FindChild("Button").gameObject.GetComponent<Button>();
			button.interactable = levAllowed;

			level.Find("Lock").gameObject.SetActive(!levAllowed);

			int levNum = i+1;
			button.onClick.AddListener(() => AutoFade.LoadLevel(levNum));
//			button.onClick.AddListener(() => GameManager.LoadLevelNum(levNum));

		}

	}

	public static bool IsLevelAllowed(int lev){
		if (lev==1) return true;

		int tot = Application.levelCount;

		string key = "Level#"+(lev-1)+"_Medal";
		int prevLevMedal = PlayerPrefs.GetInt(key);

		if (lev<tot-2){
		   return (prevLevMedal>0);
		}else{
			return (lev==tot-2 && HasAllSilver()) || (lev==tot-1 && HasAllGold());
		}
	}

	public static int GetNextLevel(){
		bool HasSilver = HasAllSilver();
		bool HasGold = HasAllGold();

		int lev = Application.loadedLevel;
		int tot = Application.levelCount-1;
		if (lev == tot - 2 && HasSilver) {
			return lev + 1;
		}
		else if (lev==tot-2 && !HasSilver) return 0;
		else if (lev==tot-1 && HasGold) return lev+1;
		else if (lev==tot-1 && !HasGold) return 0;
		else {
			if (GameManager.GetLevelSavedDocked()) return lev+1;
			return lev;
		}
	}

	public static bool HasAllSilver(){
		int tot = Application.levelCount;
		for (int i = 1; i < tot -2; i++) {
			string key = "Level#"+(i)+"_Medal";
			if (PlayerPrefs.GetInt(key)<2){
				return false;
			}
		}
		return true;
	}

	public static bool HasAllGold(){
		int tot = Application.levelCount;
		for (int i = 1; i < tot-1; i++) {
			string key = "Level#"+(i)+"_Medal";
			if (PlayerPrefs.GetInt(key)<3){
				return false;
			}
		}
		return true;
	}

	public static Color normColor(float r, float g, float b, float a){
		Color color = new Color(r/255, g/255, b/255, a);
		return color;
	}


	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.F7)){
			int totLevels = LevelGrid.childCount;

			for (int i = 0; i < totLevels; i++) {
				Transform level = LevelGrid.transform.GetChild(i);
				level.Find("Button").gameObject.GetComponent<Button>().interactable=true;
				level.Find("Lock").gameObject.SetActive(false);
			}
		}
	}
}
