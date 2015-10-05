using UnityEngine;
using System.Collections;

public class AutoFade : MonoBehaviour
{
	private static AutoFade m_Instance = null;
	private int m_LevelIndex = 0;
	private bool m_Fading = false;
	
	private static AutoFade Instance {
		get {
			if (m_Instance == null) {
				m_Instance = (new GameObject ("AutoFade")).AddComponent<AutoFade> ();
			}
			return m_Instance;
		}
	}

	public static bool Fading {
		get { return Instance.m_Fading; }
	}

	Texture loadingTexture;

	private void Awake ()
	{
		DontDestroyOnLoad (this);
		m_Instance = this;
		loadingTexture = Resources.Load<Texture> ("Images/UI/background1");
	}
	

	private IEnumerator Fade (float aFadeOutTime, float aFadeInTime)
	{

		Rect rect = new Rect (0, 0, Screen.width, Screen.height);

		Color transparent = new Color (1, 1, 1, 0);
		Color opaque = new Color (1, 1, 1, 1f);
		Color col = transparent;

		float startTime = Time.time;
		while (Time.time < startTime + aFadeInTime) {
			yield return new WaitForEndOfFrame ();
			col.a += Mathf.Clamp01 (0.06f);

			GUI.color = col;
			GUI.DrawTexture (rect, loadingTexture, ScaleMode.ScaleAndCrop);
			yield return null;
		}

		GameManager.LoadLevelNum (m_LevelIndex);
		col.a = 1f;

		startTime = Time.time;
		while (Time.time < startTime + 0.1f) {
			yield return new WaitForEndOfFrame ();
			GUI.color = opaque;
			GUI.DrawTexture (rect, loadingTexture, ScaleMode.ScaleAndCrop);
			yield return null;
		}

		startTime = Time.time;
		while (Time.time < startTime + aFadeOutTime) {
			yield return new WaitForEndOfFrame ();
			col.a -= Mathf.Clamp01 (0.06f);
			GUI.color = col;
			GUI.DrawTexture (rect, loadingTexture, ScaleMode.ScaleAndCrop);
			yield return null;
		}

		m_Fading = false;
	}

	private void StartFade (float aFadeOutTime, float aFadeInTime)
	{
		m_Fading = true;
		StartCoroutine (Fade (aFadeOutTime, aFadeInTime));
	}

	public static void LoadLevel (int aLevelIndex)
	{
		if (Fading)
			return;
		Instance.m_LevelIndex = aLevelIndex;
		Instance.StartFade (0.25f, 0.25f);
	}
}

