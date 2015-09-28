using UnityEngine;
using System.Collections;

public class ScoreConnection : MonoBehaviour {
	public static ScoreConnection instance;
	private string secretKey = "2pAk&&";
	void Start () {
		instance = this;
	}

	
	public static void ReceiveScore (int level, string dateRange) {
		string url = "http://arc-nova.net78.net/ReceiveScores.php";
		if (dateRange==null) dateRange = "year";


		WWWForm form = new WWWForm();
		form.AddField("LEVEL", Application.loadedLevel);
		form.AddField("SCORETYPE", GetScoreType(Application.loadedLevel));
		form.AddField("DATERANGE", dateRange);
		
		WWW receive = new WWW(url, form);
		if (instance!=null)
		instance.StartCoroutine(instance.WaitForRequest(receive, "displaying"));
	}

	
	public static void AddScore (string name, float score) {
		string url = "http://arc-nova.net78.net/AddScore.php";
		string scoreText = score.ToString("0.000");
//		name = WWW.EscapeURL(name);
		string hash = ComputeHash(name + scoreText + instance.secretKey);
//		print("Hash: "+hash);
		WWWForm form = new WWWForm();
		form.AddField("LEVEL", Application.loadedLevel);
		form.AddField("USERNAME", name);
		form.AddField("SCORE", scoreText);
		form.AddField("SCORETYPE", GetScoreType(Application.loadedLevel));
		form.AddField("HASH", hash);
		
		WWW addUser = new WWW(url, form);
		if (instance!=null)
		instance.StartCoroutine(instance.WaitForRequest(addUser, "adding"));
	}

	IEnumerator WaitForRequest(WWW www, string receiving)
	{
		ButtonPresses.ScoreDisplayResults("Loading...", "Loading...");
		yield return www;
		
		// check for errors
		string data = www.data;
		bool successful =  www.error==null && data!=null && data!="";
		if (receiving=="displaying") instance.DisplayResultsUI(data, successful);
		else if (receiving=="adding") instance.ScoreAddedUI(data,successful);
   
	}    

	public static string GetScoreType (int level) {
		return "time";
	}

	public static string[] GetDateRanges(){
		string[] s = {"week", "month", "year"}; 
		return s; 
	}

	public void DisplayResultsUI(string data, bool successful){
		if (successful){
			string[] entry = data.Split('#');
			string names = "";
			string score = "";
			string date ="";
			for (int i = 1; i < entry.Length; i++) {
				if (i%3==1) names+=entry[i]+"\n";
				if (i%3==2) score+=entry[i]+"\n";
				if (i%3==0) date+=entry[i]+"\n";
			}

			ButtonPresses.ScoreDisplayResults(names, score);
		}else{
//			ButtonPresses.ChangeSendScoreText("Error");			

			ButtonPresses.ScoreDisplayResults("Cannot Connect", "");
		}
	}

	public void ScoreAddedUI(string data, bool successful){
		Debug.Log("RESPONSE: "+data);
		if (successful){
			print("SENT FROM 2");
			ButtonPresses.ChangeSendScoreText(true);
			GameManager.SetLevelSavedSentTime(GameManager.GetLevelSavedTime());
		}else{
			print("SENT FROM 3");
			ButtonPresses.ChangeSendScoreText(false);
		}
		ButtonPresses.RefreshScores();
	}

	public static string ComputeHash(string s){
		// Form hash
		System.Security.Cryptography.MD5 h = System.Security.Cryptography.MD5.Create();
		byte[] data = h.ComputeHash(System.Text.Encoding.Default.GetBytes(s));
		// Create string representation
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		for (int i = 0; i < data.Length; ++i) {
			sb.Append(data[i].ToString("x2"));
		}
		return sb.ToString();
	}

	public static string Md5Sum(string strToEncrypt)
	{
		System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
		byte[] bytes = ue.GetBytes(strToEncrypt);
		
		// encrypt bytes
		System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
		byte[] hashBytes = md5.ComputeHash(bytes);
		
		// Convert the encrypted bytes back to a string (base 16)
		string hashString = "";
		
		for (int i = 0; i < hashBytes.Length; i++)
		{
			hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
		}
		
		return hashString.PadLeft(32, '0');
	}

}
