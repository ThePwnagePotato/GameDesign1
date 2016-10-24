using UnityEngine;
using System.Collections;

public class ProgressChecker : MonoBehaviour {

	public GameObject[] gameObjects;

	// Use this for initialization
	void Start () {
		int progress = SaveData.saveData.currentSave.chapter;
		for (int i = progress + 1; i < gameObjects.Length; i++)
			gameObjects [i].SetActive (false);
	}
}
