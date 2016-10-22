using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// this script makes the associated sprite "pulse" in transparency
public class transparencyImagePulser : MonoBehaviour {

	public Image targetImage;
	[Range(0, 0.05f)] public float pulseSpeed;
	[Range(0,1)] public float minAlpha;
	[Range(0,1)] public float maxAlpha;
	public int frameLinger; // how many frames the animation lingers at max

	private bool rising;
	private int counter;

	// Use this for initialization
	void Start () {
		targetImage.color = new Color (targetImage.color.r, targetImage.color.g, targetImage.color.b, maxAlpha);
		rising = true;
		counter = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		float newAlpha = targetImage.color.a + (rising ? pulseSpeed : -pulseSpeed);
		if (targetImage.color.a < minAlpha) {
			newAlpha = minAlpha;
			rising = true;
		} else if (targetImage.color.a > maxAlpha) {
			if (counter < frameLinger) {
				counter++;
				newAlpha = targetImage.color.a;
			}
			else {
				counter = 0;
				rising = false;
				newAlpha = maxAlpha;
			}
		}
			targetImage.color = new Color (targetImage.color.r, targetImage.color.g, targetImage.color.b, newAlpha);
	}
}
