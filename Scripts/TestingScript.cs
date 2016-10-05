using UnityEngine;
using System.Collections;

public class TestingScript : MonoBehaviour {

	public GameObject dialogueHolder;

	public void toggleDialogue () {
		dialogueHolder.SetActive(!dialogueHolder.activeSelf);
	}
}
