using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

	public Texture2D fadeOutTexture;
	public AudioSource[] audioSources;
	private float[] originalVolumes;
	public float fadeSpeed = 0.8f;
	public float audioFadeSpeed = 0.05f;

	private int drawDepth = -1000;
	private float alpha = 1.0f;
	private int fadeDir = -1;

	public void Start () {
		if (audioSources != null) {
			originalVolumes = new float[audioSources.Length];
			for (int i = 0; i < audioSources.Length; i++) {
				originalVolumes [i] = audioSources [i].volume;
			}
		}
	}

	public void Exit () {
		StartCoroutine (FadeAndExit());
	}

	private IEnumerator FadeAndExit () {
		float fadeTime = BeginFade (1);
		yield return new WaitForSeconds (fadeTime);
		Application.Quit ();
	}

	void OnGUI () {
		alpha += fadeDir * fadeSpeed * Time.deltaTime;
		alpha = Mathf.Clamp01 (alpha);
		GUI.color = new Color (GUI.color.r, GUI.color.g, GUI.color.b, alpha);
		GUI.depth = drawDepth;
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), fadeOutTexture);
	}

	public float BeginFade (int direction) {
		fadeDir = direction;
		return fadeSpeed;
	}

	void OnLevelWasLoaded () {
		BeginFade (-1);
	}

	public void LoadScene (string name) {
		StartCoroutine(FadeAndLoad(name));
	}

	IEnumerator fadeSound () {
		while (true) {
			for (int i = 0; i < audioSources.Length; i++) {
				audioSources [i].volume = Mathf.MoveTowards (audioSources [i].volume, 0, originalVolumes [i] * audioFadeSpeed);
			}
			yield return new WaitForFixedUpdate ();
		}
	}

	private IEnumerator FadeAndLoad (string name) {
		float fadeTime = BeginFade (1);
		if (audioSources != null && audioSources.Length > 0) StartCoroutine (fadeSound ());
		yield return new WaitForSeconds (fadeTime);
		SceneManager.LoadScene (name);
	}

	public void AttemptSlotContinue (string sceneName) {
		if (SaveData.saveData.currentSave != null) {
			LoadScene (sceneName);
		}
	}

	public void StartNewSave () {
		SaveData.saveData.StartNewSave ();
	}

	public void SaveToInstance (int saveSlot) {
		SaveData.saveData.SaveToInstance (saveSlot);
	}

	public void LoadFromInstance (int saveSlot) {
		SaveData.saveData.LoadFromInstance (saveSlot);
	}

	public void SaveToDisk() {
		SaveData.saveData.SaveToDisk ();
	}

	public void LoadFromDisk() {
		SaveData.saveData.LoadFromDisk ();
}
}
