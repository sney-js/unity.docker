using UnityEngine;
using System.Collections;

public class SoundScript : MonoBehaviour
{

	public static SoundScript instance;
	public AudioClip ThudDock;
	public AudioClip Success;
	public AudioClip SuccessPost;
	public AudioClip IntenseEscape;
	public AudioClip SuccessTextIter;
	private AudioClip UnPauseSound;
	AudioSource t_audio;
	AudioSource t_audio_single;
	public bool isMainMenu;
	Transform player;
	void Start ()
	{
		t_audio = GetComponents<AudioSource> ()[0];
		t_audio_single = GetComponents<AudioSource> ()[1];
		player=GameObject.Find("Level/Player").transform;
	}

	void  Update(){
		if (isMainMenu) {
			float value = 0f;
			value = (Camera.main.WorldToViewportPoint(player.position).x - 0.5f)*2f;
//			print ("VALUE: "+value);
			Camera.main.GetComponent<AudioSource>().panStereo=value;
		}

	}

	void Awake ()
	{
		instance = this;
	}
	// Use this for initialization
	public static void docked (float impact)
	{
		PlayOnce(instance.ThudDock, false);
	}
	public static void TextIncrease(float pitch){
		instance.t_audio_single.pitch = pitch;
		instance.t_audio_single.volume = 0.1f;
		instance.t_audio_single.PlayOneShot(instance.SuccessTextIter);
	}

	public static void PlayOnce(AudioClip clip, bool changePitch){
		try {
			if (changePitch) instance.t_audio_single.pitch = Random.Range(0.8f, 1.2f);
			else instance.t_audio_single.pitch=1f;
			instance.t_audio_single.PlayOneShot(clip);
			instance.t_audio_single.volume=0.6f;
		} catch (System.Exception ex) {}
	}

	public static void UnPausePlay(float volume){
		instance.t_audio_single.volume =volume;
		instance.t_audio_single.pitch=1f;
		instance.t_audio_single.clip = instance.UnPauseSound;
		if (!instance.t_audio_single.isPlaying) instance.t_audio_single.Play();
		if (volume<=0) instance.t_audio_single.Stop();
	}

	public static void SuccessMusic(){

		instance.t_audio.clip=instance.Success;
		instance.t_audio.volume=1f;
		instance.t_audio.PlayDelayed(1f); 
//		instance.audio.Stop()//Delayed (1f);

		instance.StartCoroutine(PostSuccessMusic(instance.Success.length+3f));
	}

	public static void FailedMusic(){		
		instance.StartCoroutine(instance.FailPitch(0.5f));
	}

	IEnumerator FailPitch(float overTime){
		float startTime = Time.time;
		AudioSource main = Camera.main.GetComponent<AudioSource>();
		while (Time.time < startTime + overTime) {
			main.pitch = Mathf.SmoothStep (1f, 0f, (Time.time - startTime) / overTime);
			yield return null;
		}
	}

	public static IEnumerator PostSuccessMusic(float overTime) {
//		yield return new instance.PostSuccessMusic(waitTime);
		instance.StartCoroutine(FadeSound(Camera.main.GetComponent<AudioSource>(), 1f, 0f, 0f));

		yield return new WaitForSeconds(overTime);

		instance.t_audio.clip=instance.SuccessPost;
		instance.t_audio.loop = true;
		instance.t_audio.volume=0f;
		instance.t_audio.Play();
		float startTime = Time.time;
		while (Time.time < startTime + 5f) {
			instance.t_audio.volume+=0.1f;
			yield return null;
		}

	}

	public static void MusicIntenseEscape(){
		if (instance.t_audio.clip==instance.IntenseEscape && instance.t_audio.isPlaying){}
		else{
			instance.t_audio.clip=instance.IntenseEscape;
			instance.t_audio.volume=0.5f;
			instance.t_audio.Play();
		}
	}

	public static void StopMusic(){
//		instance.audio.clip=instance.IntenseEscape;

		instance.StartCoroutine( FadeSound(instance.t_audio, 3f, 0f, 0f));

//		instance.audio.Stop();
	}


	public static IEnumerator FadeSound (AudioSource audio, float overTime, float to, float WaitFor)
	{
		yield return new WaitForSeconds(WaitFor);
		float startTime = Time.time;
		float from = audio.volume;	


		if (!audio.isPlaying) audio.Play();

		while (Time.time < startTime + overTime) {
			audio.volume = Mathf.Lerp (from, to, (Time.time - startTime) / overTime);
			yield return null;
		}
		if (to==0f)	{
			audio.Stop();
//		print("MUSIC STOPPED.");
		}
	}
}
