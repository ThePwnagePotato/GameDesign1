using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DialogueUIHolder : MonoBehaviour {

	private string[] chapters;
	public Text chapterText;

	private int currChap;

	void FillChapters() {
		chapters = new string[3];

		chapters [0] = "Story 0";
		chapters [1] = "Story 1";
		chapters [2] = "Story 2";

	}

	// Use this for initialization
	void Start () {
		FillChapters ();
		if (SaveData.saveData.currentSave.chapter < chapters.Length) {
			currChap = SaveData.saveData.currentSave.chapter;
			ShowChapter (currChap);
		}
	}

	void ShowChapter(int index) {
		chapterText.text = chapters[index];
	}
	
	public void Next () {
		if (currChap < chapters.Length-1 && currChap < SaveData.saveData.currentSave.chapter) {
			ShowChapter (++currChap);
		}
	}

	public void Prev () {
		if (currChap > 0) {
			ShowChapter (--currChap);
		}
	}
}
