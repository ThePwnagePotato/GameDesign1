using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SaveDisplay : MonoBehaviour {

	public int saveSlot;
	public Text saveText;
	public Text chapterText;

	// Use this for initialization
	void Update () {
		if (SaveData.saveData.saveSlots [saveSlot] != null) {
			chapterText.text = "Chapter " + (SaveData.saveData.saveSlots [saveSlot].chapter + 1).ToString ();
			float tp = SaveData.saveData.saveSlots [saveSlot].timePlayed;
			string time = ((int)(tp / 3600)).ToString () + ":" + ((int)((tp % 3600)) / 60).ToString ();
			saveText.text = "Save " + (saveSlot + 1).ToString() + " ~ " + time;
		} else {
			saveText.text = "Save " + (saveSlot + 1).ToString() + " ~ --:--";
			chapterText.text = "EMPTY";
		}
	}
}