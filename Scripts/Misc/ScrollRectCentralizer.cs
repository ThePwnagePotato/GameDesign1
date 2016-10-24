using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScrollRectCentralizer : MonoBehaviour {

	public ScrollRect scrollRect;
	public float normalizedHorizontalPosition = 0.5f;
	public float normalizedVerticalPosition = 0.5f;

	// Use this for initialization
	void Start () {
		scrollRect.verticalNormalizedPosition = normalizedVerticalPosition;
		scrollRect.horizontalNormalizedPosition = normalizedHorizontalPosition;
	}
}
