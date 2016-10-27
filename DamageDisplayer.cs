using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DamageDisplayer : MonoBehaviour {

	public GameObject regularDamageObject;

	public void ShowRegularDamage(int damage) {
		GameObject newDamage = Instantiate (regularDamageObject, transform) as GameObject;
		newDamage.transform.position = transform.position;
		newDamage.GetComponentInChildren<Text> ().text = damage.ToString();
	}
}
