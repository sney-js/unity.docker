using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnimationScript : MonoBehaviour
{

	public static bool RestartCalled = false;
	public static int medal_s;
	public static AnimationScript instance;
	public static Transform myCanvas;
	public static float BestTime;



	public static IEnumerator ChangeText (Text text, string str, float overTime)
	{
		Color moreAlphaC = Color.white, normC = text.color;
		moreAlphaC.a = 0.7f;
		normC.a = 0f;
		
		float startTime = Time.time;
		while (Time.time < startTime + overTime) {
			text.color = Color.Lerp (moreAlphaC, normC, (Time.time - startTime) / overTime);
			yield return null;
		}
		text.text = str;
		startTime = Time.time;
		while (Time.time < startTime + overTime) {
			text.color = Color.Lerp (normC, moreAlphaC, (Time.time - startTime) / overTime);
			yield return null;
		}

	}

	public static IEnumerator AnimatePrefabPos (GameObject parent, float factor, float overTime, float delay)
	{
		yield return new WaitForSeconds (delay);
		int childCount = parent.transform.childCount;
		Vector3[] positions = new Vector3[childCount];
		for (int i = 0; i < childCount; i++) {
			positions [i] = parent.transform.GetChild (i).transform.position;
			positions [i] *= factor;
		}

		float startTime = Time.time;
//		Vector3 vel  = Vector3.zero;
		while (Time.time < startTime + overTime) {
			for (int i = 0; i < childCount; i++) {
				Transform child = parent.transform.GetChild (i).transform;
				Vector3 newV = child.transform.position * factor;
				newV.z = 0f;
				child.transform.position = Vector3.Lerp (child.transform.position, positions [i],
				                                         (Time.time - startTime) / overTime);
			}
			yield return null;
		}
	}

	public static IEnumerator AnimatePrefabXYZ (GameObject obj, Vector3 newVal, float overTime, float delay)
	{
		yield return new WaitForSeconds (delay);
		Vector3 curr = obj.transform.position;
//		Vector3 vel = Vector3.zero;
		Rigidbody2D rigid = obj.GetComponent<Rigidbody2D> ();
		if (rigid != null) {
			print("RIGID RESET");
			rigid.velocity = Vector2.zero;
			rigid.angularVelocity = 0f;
		}
//		vel.x = obj.GetComponent<Rigidbody2D>().velocity.x;
//		vel.y = obj.GetComponent<Rigidbody2D>().velocity.y;
		float startTime = Time.time;
		while (Time.time < startTime + overTime) {
			float t = (Time.time - startTime) / overTime;
			t = t * t * t * (t * (6f * t - 15f) + 10f);
			obj.transform.position = Vector3.Lerp (curr, newVal, t);
			yield return null;
		}
	}

	public static IEnumerator AnimatePrefabZ (GameObject obj, float newVal, float overTime, float delay)
	{
		yield return new WaitForSeconds (delay);
		float curr = obj.transform.position.z;
		float startTime = Time.time;
		while (Time.time < startTime + overTime) {
			float t = (Time.time - startTime) / overTime;
			t = t * t * t * (t * (6f * t - 15f) + 10f);
			Vector3 newV = obj.transform.position;
			newV.z = Mathf.Lerp (curr, newVal, t);
			obj.transform.position = newV;
			yield return null;
		}
	}

	public static IEnumerator AnimatePrefabRotation (GameObject obj, Vector3 newVal, float overTime, float delay)
	{
		yield return new WaitForSeconds (delay);
		Vector3 curr = obj.transform.localEulerAngles;
//		Vector3 vel = Vector3.zero;
		float startTime = Time.time;
		while (Time.time < startTime + overTime) {
//			obj.transform.localEulerAngles = Vector3.Lerp (curr, newVal, (Time.time - startTime) / overTime);
			float t = (Time.time - startTime) / overTime;
			t = t * t * t * (t * (6f * t - 15f) + 10f);
			float zz = Mathf.LerpAngle (curr.z, newVal.z, t);
			obj.transform.localEulerAngles = new Vector3 (newVal.x, newVal.y, zz);
			yield return null;
		}
	}
	
	public static IEnumerator FadeInPanel_Enable (GameObject obj, float toAlpha, float overTime, bool enableIB)
	{
			
		CanvasGroup overlay = obj.GetComponent<CanvasGroup> ();
		if (overlay == null) {
			obj.AddComponent<CanvasGroup>();
			overlay = obj.GetComponent<CanvasGroup> ();
		}
		overlay	.alpha = 0f;
		obj.SetActive (true);
		float startTime = Time.time;
		while (Time.time < startTime + overTime) {
			float t = (Time.time - startTime) / overTime;
			t = Mathf.Sin(t * Mathf.PI * 0.5f);
			overlay	.alpha = Mathf.Lerp (0f, toAlpha, t);
			yield return null;
		}
		if (enableIB) {
			overlay.interactable = true;
			overlay.blocksRaycasts = true;
		}
		overlay.alpha=1f;
//		yield break;
	}
	//https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/
	public static IEnumerator FadeOutPanel_Disable (GameObject obj, float overTime)
	{
		CanvasGroup cg = obj.GetComponent<CanvasGroup> ();
		if (cg == null) {
			obj.AddComponent<CanvasGroup>();
			cg = obj.GetComponent<CanvasGroup> ();
		}
		cg.alpha = 1.0f;
		float startTime = Time.time;
		while (Time.time < startTime + overTime) {
			float t = (Time.time - startTime) / overTime;
			t = Mathf.Sin(t * Mathf.PI * 0.5f);
			cg.alpha = Mathf.Lerp (1.0f, 0f, t);
			yield return null;
		}
		obj.SetActive (false);
//		yield break;
	}

	public static IEnumerator FadeOutPanel (CanvasGroup overlay, float overTime)
	{
		Animator anim = overlay.gameObject.GetComponent<Animator> ();
		anim.Stop ();
		float startTime = Time.time;
		while (Time.time < startTime + overTime) {
			overlay	.alpha = Mathf.Lerp (overlay.alpha, 0, (Time.time - startTime) / overTime);
			yield return null;
		}
		overlay.interactable = false;
		overlay.blocksRaycasts = false;
	}

	public static IEnumerator FlashScreen (Image overlay, float overTime, int color, string str)
	{
		if (ButtonPresses.isMainMenu)
			yield break;
		if (overlay == null)
			overlay = GameObject.Find ("Canvas/DamageImage").gameObject.GetComponent<Image> ();

		Text info = overlay.transform.FindChild ("Text").GetComponent<Text> ();
		info.text = str == null ? "" : str;

		Color col = new Color32 (255, 63, 90, 0);
		if (color == 1)
			col = new Color32 (255, 83, 150, 0);
		if (color == 2)
			col = new Color32 (55, 253, 250, 0);
		if (color == 3)
			col = new Color32 (255, 216, 255, 0);

		Color moreAlphaC = col, normC = col;
		moreAlphaC.a += 0.2f;
		normC.a = 0f;

		Color textC = info.color, textCAlpha = info.color;
		textC.a = 0.6f;
		textCAlpha.a = 0;

		float startTime = Time.time;
		while (Time.time < startTime + overTime) {
			overlay	.color = Color.Lerp (moreAlphaC, normC, (Time.time - startTime) / overTime);
			info.color = Color.Lerp (textC, textCAlpha, (Time.time - startTime) / overTime);
			yield return null;
		}
	}

	public static IEnumerator NudgeUI (GameObject obj, float endTime, float scaleFactor)
	{
//		float max = 75f;
//		float min = 65f;
		yield return new WaitForEndOfFrame ();
		float startTime = Time.time;
		float overTime = 0.1f;
		Vector3 defScale = obj.transform.localScale;
		Vector3 scaleTo = defScale * scaleFactor;

		while (Time.time < startTime + overTime) {
			obj.transform.localScale = Vector3.Lerp (defScale, scaleTo, (Time.time - startTime) / overTime);
			yield return null;
		}
//		print ("Scale set" + obj.transform.localScale);
		startTime = Time.time;
//		endTime = 0.3f;
		while (Time.time < startTime + overTime) {
			obj.transform.localScale = Vector3.Lerp (scaleTo, defScale, (Time.time - startTime) / overTime);
			yield return null;
		}
	}

//	public void FadeInDamageUI(float time){
//		Image damageImg = GameObject.Find("Canvas/DamageImage").GetComponent<Image>();
//		StartCoroutine( FadeImage(damageImg, time, 0.5f));
//
//	}

	public void FadeOutDamageUI (float time)
	{
		Image damageImg = GameObject.Find ("Canvas/DamageImage").GetComponent<Image> ();
		StartCoroutine (FadeImage (damageImg, time, 0f, 0f));
		
	}

	public static IEnumerator FadeImage (Image overlay, float overTime, float to, float after)
	{
		yield return new WaitForSeconds (after);
		float startTime = Time.time;
		Color moreAlphaC = overlay.color;
		Color normC = overlay.color;
//		moreAlphaC.a;
		normC.a = to;
//		overlay.color=moreAlphaC;
		if (to > 0f)
			overlay.gameObject.SetActive (true);
		while (Time.time < startTime + overTime) {
			overlay	.color = Color.Lerp (moreAlphaC, normC, (Time.time - startTime) / overTime);
			yield return null;
		}
//		if (to==0f) overlay.gameObject.SetActive(false);
	}

	public static IEnumerator LevelFailed (int msgID)
	{
		float FlashTime = 0.3f;

		CanvasGroup overlay = GameObject.Find ("Canvas").transform.FindChild ("GameOverImage").GetComponent<CanvasGroup> ();
		Text failText = overlay.transform.FindChild ("FailText").gameObject.GetComponent<Text> ();
		failText.text = GameManager.getMsg (msgID);

		float startTime = Time.time;
		float moreAlphaC = overlay.alpha;
		float normC = overlay.alpha;
		moreAlphaC += 0.5f;
		normC = 0.3f;

		while (Time.time < startTime + FlashTime) {
			overlay	.alpha = Mathf.Lerp (moreAlphaC, normC, (Time.time - startTime) / FlashTime);
			yield return null;
		}


		float sliderTime = 2f;
		Slider slider = overlay.transform.FindChild ("Slider").gameObject.GetComponent<Slider> ();
		slider.value = 0;
		startTime = Time.time;
		while (Time.time < startTime + sliderTime) {
			slider.value = Mathf.Lerp (0, 1, (Time.time - startTime) / sliderTime);
			if (GameEvents.LevelSuccess) {
				break;
			}
			yield return null;
		}
		yield return null;
		if (!GameEvents.LevelSuccess) {
			GameEvents.RestartLevel ();
		} else {
			startTime = Time.time;
			while (Time.time < startTime + FlashTime) {
				overlay	.alpha = Mathf.Lerp (normC, 0f, (Time.time - startTime) / FlashTime);
				yield return null;
			}
		}

	}

	public static void LevelSuccess (MonoBehaviour corr)
	{
		BestTime = GameManager.GetLevelSavedTime ();

		float timeTaken = GameEvents.timeTaken;
		float score = GameEvents.Score;
		float fuel = GameEvents.maxFuel - GameEvents.FuelReading;


//		float timeWin = GameEvents.GetWinCriteriaTime();
//		int scoreWin = GameEvents.GetWinCriteriaScore();
//		bool[] criteriasChecked = {true,GameEvents, timeTaken<=timeWin};
//		Color orange = new Color32(255,251,94,255);
//
		if (myCanvas == null)
			myCanvas = GameObject.Find ("Canvas").transform;
		Transform successScreen = myCanvas.FindChild ("GameSuccessImage");
		//-------------------------------------write values---------------------------------------
		ButtonPresses.initialiseCriteriaIcons (false);
		//----------------------------------------------------------------------------------------
		corr.StartCoroutine (CriteriaAnimOffset (0, 2.4f));

		Text bestTime = successScreen.FindChild ("LeftGroup/BestTime/Result").GetComponent<Text> ();
		Text resultTime = successScreen.FindChild ("LeftGroup/TimeGroup/Result").GetComponent<Text> ();
		resultTime.text = timeTaken.ToString ("0.000") + " s";
		bestTime.text = BestTime == float.PositiveInfinity ? "N/A" : BestTime.ToString ("0.000") + " s";
		float AnimTimeGroupStart = 3.7f;

		string[] crinames = GameEvents.GetCriteriasNames ();
		Text resultScore = successScreen.FindChild ("LeftGroup/ScoreGroup/Result").GetComponent<Text> (); 
		if (crinames [1].Equals ("SCORE")) {
			resultScore.text = score.ToString ();
			corr.StartCoroutine (IncreaseText (resultScore, score, 2.5f, GameEvents.IsScoreChecked (), 1, corr));
		} else if (crinames [1].Equals ("FUEL")) {
			resultScore.text = fuel.ToString ();
			successScreen.FindChild ("LeftGroup/ScoreGroup/Text").GetComponent<Text> ().text = "Fuel Used :";
			corr.StartCoroutine (IncreaseText (resultScore, fuel, 2.5f, GameEvents.IsFuelChecked (), 2, corr));
		} else {
			successScreen.FindChild ("LeftGroup/ScoreGroup").gameObject.SetActive (false);
			AnimTimeGroupStart = 2.6f;
			bool satisfyNoDock = (crinames [1].Equals ("NODOCK") && GameEvents.IsNoDock ());
			bool satisfyFullHealth = (crinames [1].Equals ("HEALTH") && GameEvents.IsHealthFull ());
//			bool satisfyFuel = (crinames[1].Equals("FUEL") && GameEvents.IsFuelChecked());
			if (satisfyNoDock || satisfyFullHealth) {
				corr.StartCoroutine (CriteriaAnimOffset (1, 2.5f));
			}
		}

		corr.StartCoroutine (IncreaseText (resultTime, timeTaken, AnimTimeGroupStart, 
		                                 GameEvents.IsTimeChecked (), 0, corr));


		//-------------------------------------------------------------------------------
		int medalReceived = GameManager.GetMedal ();

		Transform medalPanel = successScreen.FindChild ("LeftGroup/MedalPanel");
//		print("RECCC:"+medalReceived);
		switch (medalReceived) {
		case 3:
			medalPanel.FindChild ("GOLD").gameObject.SetActive (true);
			break;
		case 2:
			medalPanel.FindChild ("SILVER").gameObject.SetActive (true);
			break;
		case 1:
			medalPanel.FindChild ("BRONZE").gameObject.SetActive (true);
			break;
		default:
			break;
		}

		//----------------------------------finally-------------------------------------------------
		successScreen.gameObject.SetActive (true);
	}

	/**
	 * criteria type : 0=time, 1=score, 2=fuel
	 */
	public static IEnumerator IncreaseText (Text obj, float to, float offset,
	                                       bool criteriaAchieved, int CriteriaType, MonoBehaviour corr)
	{
		bool isTime = CriteriaType == 0;
		bool isScore = CriteriaType == 1;
		bool isFuel = CriteriaType == 2;

		float currTotal = 0f;
		float duration = 1f, steps = 1f / to;
		if (isTime || isFuel) {
			duration = 1.5f;
			steps = 0.05f;
		}


		Transform successScreen = myCanvas.FindChild ("GameSuccessImage");
		obj.text = isTime ? currTotal.ToString ("0.000") + "s" : currTotal.ToString ("0");


		yield return new WaitForSeconds (offset);

		successScreen.GetComponent<Animator> ().speed = 0f;
		float startTime = Time.time;
		float pitch = 0.7f;

		while (Time.time < startTime + duration) {
			pitch += 0.01f;
			if (isScore) {
				currTotal++;
			} else {
				currTotal += to / (duration / steps);
			}
			if (currTotal >= to) {
				obj.text = isTime ? to.ToString ("0.000") + "s" : to.ToString ("0");
				break;
			}
			obj.text = isTime ? currTotal.ToString ("0.000") + "s" : currTotal.ToString ("0");
			SoundScript.TextIncrease (pitch);
			yield return new WaitForSeconds (steps);
		}
		obj.text = isTime ? to.ToString ("0.000") + "s" : to.ToString ("0");

		if (!isTime && criteriaAchieved) {
			corr.StartCoroutine (CriteriaAnimOffset (1, 0.1f));
		}

		if (isTime) {
			if (criteriaAchieved)
				corr.StartCoroutine (CriteriaAnimOffset (2, 0.1f));
			if (BestTime > to) {
				successScreen.FindChild ("LeftGroup/BestTime/Result").GetComponent<Text> ().text = to.ToString ("0.000") + "s";
			}
			successScreen.GetComponent<Animator> ().speed = 1f;
		}
	}

	public static IEnumerator CriteriaAnimOffset (int criteriaNum, float time)
	{
		Transform cgroup = GameObject.Find ("Canvas").transform.FindChild ("GameSuccessImage/LeftGroup/CriteriaGroup");
		Animator anim = cgroup.FindChild ("Box" + criteriaNum).gameObject.GetComponent<Animator> ();
		yield return new WaitForSeconds (time);
		anim.enabled = true;


	}
}
