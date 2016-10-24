using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SkillWatcher : MonoBehaviour {
	public Text skillText;

	// Use this for initialization
	void Update () {
		skillText.text = "Skill: " + SaveData.saveData.currentSave.skillPoints.ToString();
	}
}
