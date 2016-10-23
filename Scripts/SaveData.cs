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
		if (saveData == null) {
			DontDestroyOnLoad (gameObject);
			LoadFromDisk ();
			saveData = this;
		} else if (saveData != this)
			Destroy (this.gameObject);
	}

	public void StartNewSave () {
		currentSave = new Save (startChapter, startSkillPoints, characterAmount, startPlaytime);
	}

	public void SaveToInstance (int saveSlot) {
		currentSave.timePlayed += Time.time - lastLoad;
		lastLoad = Time.time;
		saveSlots [saveSlot] = new Save (currentSave);
	}

	public bool LoadFromInstance (int saveSlot) {
		if (saveSlots [saveSlot] != null) {
			currentSave = new Save (saveSlots [saveSlot]);
			lastLoad = Time.time;
			return true;
		}
		return false;
	}

	public void SaveToDisk() {
		BinaryFormatter bf = new BinaryFormatter ();
		for (int i = 0; i < saveSlotAmount; i++) {
			if (saveSlots [i] != null) {
				FileStream file = File.Open (Application.persistentDataPath + "/saveData" + i.ToString () + ".dat", FileMode.Open);
				bf.Serialize (file, saveSlots [i]);
				file.Close ();
			}
		}
	}

	public void LoadFromDisk() {
		BinaryFormatter bf = new BinaryFormatter ();
		for (int i = 0; i < 3; i++) {
			if (File.Exists (Application.persistentDataPath + "/saveData" + i.ToString () + ".dat")) {
				FileStream file = File.Open (Application.persistentDataPath + "/saveData" + i.ToString () + ".dat", FileMode.Open);
				saveSlots [i] = (Save) bf.Deserialize (file);
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

	public Save(int startChapter, int startSkillPoints, int characterAmount, float startPlaytime) {
		chapter = startChapter;
		skillPoints = startSkillPoints;
		unitStats = new int[characterAmount];
		for (int i = 0; i < characterAmount; i++) unitStats[i] = startSkillPoints;
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
