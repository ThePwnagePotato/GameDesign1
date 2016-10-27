using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DialogueUIHolder : MonoBehaviour {

	private string[] chapters;
	public Text chapterText;

	public GameObject storyHolder;
	public GameObject storyButton;

	private int currChap;

	void FillChapters() {
		chapters = new string[3];

		chapters [0] = "There is chaos in the 9 worlds. The tree of life, Yggdrasil, is dying. However not just the tree is affected but many of the inhabitants of the 9 worlds are dying. The disease is causing the tree and the people to slowly turn to ashes. Some people have gone to look for the cause and a cure. ";
		chapters [1] = "After a “civilized conversation” the party searches the library. They discovers that the plague has happened before and the cause could be found at the oldest wells of the three wells the tree uses for nourishment. To get to this well they must travel through the cold and desolate hills of Niflheim.";
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

	public void OpenStory() {
		storyButton.SetActive (false);
		storyHolder.SetActive (true);
	}

	public void HideStory() {
		storyButton.SetActive (true);
		storyHolder.SetActive (false);
	}
}
