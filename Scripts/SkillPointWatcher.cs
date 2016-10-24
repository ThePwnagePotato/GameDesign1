using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SkillPointWatcher : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<Text> ().text = ("Skill: " + SaveData.saveData.currentSave.skillPoints.ToString());
	}
}
