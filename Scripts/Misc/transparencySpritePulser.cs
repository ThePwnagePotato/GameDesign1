using UnityEngine;
using System.Collections;

// this script makes the associated sprite "pulse" in transparency
public class transparencySpritePulser : MonoBehaviour {

	public SpriteRenderer spriteRenderer;
	[Range(0, 0.05f)] public float pulseSpeed;
	[Range(0,1)] public float minAlpha;
	[Range(0,1)] public float maxAlpha;
	public int frameLinger; // how many frames the animation lingers at max

	private bool rising;
	private int counter;

	// Use this for initialization
	void Start () {
		spriteRenderer.color = new Color (spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, maxAlpha);
		rising = true;
		counter = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		float newAlpha = spriteRenderer.color.a + (rising ? pulseSpeed : -pulseSpeed);
		if (spriteRenderer.color.a < minAlpha) {
			newAlpha = minAlpha;
			rising = true;
		} else if (spriteRenderer.color.a > maxAlpha) {
			if (counter < frameLinger) {
				counter++;
				newAlpha = spriteRenderer.color.a;
			}
			else {
				counter = 0;
				rising = false;
				newAlpha = maxAlpha;
			}
		}
			spriteRenderer.color = new Color (spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, newAlpha);
	}
}
