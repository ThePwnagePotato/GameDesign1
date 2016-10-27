using UnityEngine;
using System.Collections;

public enum AnimationState {
	ROOT,
	NONE,
	IDLE
}

public class Animator : MonoBehaviour {

	public bool animEnabled = true;
	public float idleAnimInterval = 0.5f;

	private SpriteRenderer spriteRenderer;
	private AnimationState currAnim;
	private AnimationState lastAnim;
	private float lastAnimTime;
	private int frame = 0;
	private Unit unit;

	// Use this for initialization
	void Awake () {
		lastAnimTime = -1000;
		lastAnim = AnimationState.ROOT;
		currAnim = AnimationState.NONE;
		spriteRenderer = GetComponent<SpriteRenderer> ();
		unit = GetComponentInParent<Unit> ();
		StartCoroutine (FrameUpdater());
	}

	IEnumerator FrameUpdater () {
		while (true) {
			while (animEnabled) {
				if (lastAnim != currAnim) {
					lastAnim = currAnim;
					// do something when changing animation
					lastAnimTime = -1000;
					frame = 0;
					if (currAnim == AnimationState.IDLE)
						frame = 1;
				}

				switch (currAnim) {
				case AnimationState.IDLE:
					if (unit.sprites.Length < 2) break;
					if (lastAnimTime < Time.time - idleAnimInterval) {
						lastAnimTime = Time.time;
						if (frame == 0) {
							frame = 1;
							spriteRenderer.sprite = unit.sprites [1];
						} else {
							frame = 0;
							spriteRenderer.sprite = unit.sprites [0];
						}
					}
					break;
				}
				yield return new WaitForFixedUpdate ();
			}
			yield return new WaitForFixedUpdate ();
		}
	}

	public void ChangeAnimation (AnimationState newAnim) {
		currAnim = newAnim;
	}

	public void FlipVertical (bool flip) {
		spriteRenderer.flipX = (flip ? true : false);
	}

	public void SetEnable (bool enable) {
		if (enable) {
			this.animEnabled = true;
			spriteRenderer.enabled = true;
			return;
		}
		this.animEnabled = false;
		spriteRenderer.enabled = false;
	}
}
