using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HPBarWatcher : MonoBehaviour {

	public Slider slider;
	public Unit unit;

	// Use this for initialization
	void Start () {
		slider.maxValue = unit.maxHealth;
		Update ();
	}
	
	// Update is called once per frame
	void Update () {
		slider.value = unit.currentHealth;
	}
}
