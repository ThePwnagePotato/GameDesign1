using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveData : MonoBehaviour {
	public int saveSlotAmount = 3;
	public int characterAmount = 4;
	public int startChapter = 0;
	public int startSkillPoints = 0;
	public int startAbilityProgress = 0;
	public int startPlaytime = 0;

	public static SaveData saveData;
	public Save[] saveSlots;
	public Save currentSave;
	private float lastLoad;

	void Awake () {
		Debug.Log (Application.persistentDataPath);
		if (saveData == null) {
			DontDestroyOnLoad (gameObject);
			saveData = this;
			saveSlots = new Save[saveSlotAmount];
			for (int i = 0; i < 3; i++) {
				saveSlots [i] = null;
			}
			LoadFromDisk ();
			currentSave = new Save (startChapter, startSkillPoints, characterAmount, startAbilityProgress, startPlaytime);
		} else if (saveData != this)
			Destroy (this.gameObject);
	}

	public void StartNewSave () {
		saveData.currentSave = new Save (saveData.startChapter, saveData.startSkillPoints, saveData.characterAmount, saveData.startAbilityProgress, saveData.startPlaytime);
	}

	public void SaveToInstance (int saveSlot) {
		saveData.currentSave.timePlayed += Time.time - saveData.lastLoad;
		saveData.lastLoad = Time.time;
		saveData.saveSlots [saveSlot] = new Save (saveData.currentSave);
	}

	public bool LoadFromInstance (int saveSlot) {
		if (saveData.saveSlots [saveSlot] != null) {
			saveData.currentSave = new Save (saveData.saveSlots [saveSlot]);
			saveData.lastLoad = Time.time;
			return true;
		}
		saveData.currentSave = null;
		return false;
	}

	public void SaveToDisk() {
		BinaryFormatter bf = new BinaryFormatter ();
		for (int i = 0; i < saveData.saveSlotAmount; i++) {
			if (saveData.saveSlots [i] != null) {
				FileStream file = File.Create (Application.persistentDataPath + "/saveData" + i.ToString () + ".dat");
				bf.Serialize (file, saveData.saveSlots [i]);
				file.Close ();
			}
		}
	}

	public void LoadFromDisk() {
		BinaryFormatter bf = new BinaryFormatter ();
		for (int i = 0; i < 3; i++) {
			if (File.Exists (Application.persistentDataPath + "/saveData" + i.ToString () + ".dat")) {
				FileStream file = File.Open (Application.persistentDataPath + "/saveData" + i.ToString () + ".dat", FileMode.Open);
				saveData.saveSlots [i] = (Save) bf.Deserialize (file);
				file.Close();
			}
		}
	}
}

[Serializable]
public class Save {
	public int chapter;
	public int[] unitStats;
	public int skillPoints;
	public float timePlayed;

	public Save(int startChapter, int startSkillPoints, int characterAmount, int startAbilityProgress, float startPlaytime) {
		chapter = startChapter;
		skillPoints = startSkillPoints;
		unitStats = new int[characterAmount];
		for (int i = 0; i < characterAmount; i++) unitStats[i] = startAbilityProgress;
		timePlayed = startPlaytime;
	}

	public Save (Save other) {
		this.chapter = other.chapter;
		this.skillPoints = other.skillPoints;
		this.timePlayed = other.timePlayed;
		this.unitStats = new int[other.unitStats.Length];
		for (int i = 0; i < other.unitStats.Length; i++)
			this.unitStats [i] = other.unitStats [i];
	}
}
