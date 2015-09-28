using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SliderColor : MonoBehaviour {

	private Slider slider;
	public Color MaxHealthColor = Color.green;
	public Color MinHealthColor = Color.red;
	public Image Fill;
	// Use this for initialization
	void Start () {
		slider = gameObject.GetComponent<Slider>();
	}
	
	// Update is called once per frame
	void Update () {
		Fill.color = Color.Lerp(MinHealthColor, MaxHealthColor, (float)slider.value / slider.maxValue);
	}
}

