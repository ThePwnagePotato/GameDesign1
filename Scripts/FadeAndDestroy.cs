using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeAndDestroy : MonoBehaviour {

	[Header("Targets")]
	public GameObject target;
	public SpriteRenderer[] spriteRenderers;
	public Text[] texts;
	public Image[] images;

	[Header("Settings")]
	public float lifeTime = 1;
	public float fadeTimeIn = 0.5f;
	public float fadeTimeOut = 0.5f;

	// Use this for initialization
	void Start () {
		StartCoroutine (LiveFadeDie());
	}

	IEnumerator LiveFadeDie () {
		float startTime = Time.time + fadeTimeOut;
		if (fadeTimeIn != 0) {
			while (Time.time < startTime) {
				float b = Time.time;
				float alpha = 1 - (startTime - Time.time) / fadeTimeOut;
				foreach (SpriteRenderer a in spriteRenderers)
					a.color = new Color (a.color.r, a.color.g, a.color.b, alpha);
				foreach (Text a in texts)
					a.color = new Color (a.color.r, a.color.g, a.color.b, alpha);
				foreach (Image a in images)
					a.color = new Color (a.color.r, a.color.g, a.color.b, alpha);
				yield return new WaitForFixedUpdate ();
			}
		}
		yield return new WaitForSeconds(lifeTime);
		float deathTime = Time.time + fadeTimeOut;
		if (fadeTimeOut != 0) {
			while (Time.time < deathTime) {
				float alpha = (deathTime - Time.time) / fadeTimeOut;
				foreach (SpriteRenderer a in spriteRenderers)
					a.color = new Color (a.color.r, a.color.g, a.color.b, alpha);
				foreach (Text a in texts)
					a.color = new Color (a.color.r, a.color.g, a.color.b, alpha);
				foreach (Image a in images)
					a.color = new Color (a.color.r, a.color.g, a.color.b, alpha);
				yield return new WaitForFixedUpdate ();
			}
		}
		Destroy (gameObject);
	}
}
