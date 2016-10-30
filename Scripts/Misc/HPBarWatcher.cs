using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HPBarWatcher : MonoBehaviour {

	public Slider slider;
	private Unit unit;
	public Text hpText;

	// Use this for initialization
	void Start () {
		unit = GetComponentInParent<Unit> ();
		slider.maxValue = unit.maxHealth;
		Update ();
	}
	
	// Update is called once per frame
	void Update () {
		if (unit.currentHealth <= 0)
			gameObject.SetActive (false);
		else {
			slider.value = unit.currentHealth;
			hpText.text = unit.currentHealth.ToString ();
		}
	}
}
