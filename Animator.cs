using UnityEngine;
using System.Collections;

public enum AnimationState
{
	ROOT,
	NONE,
	IDLE,
	ATTACK
}

public class Animator : MonoBehaviour
{

	public bool animEnabled = true;
	public float idleAnimInterval = 0.5f;
	public GameObject deathAnimation;

	private SpriteRenderer spriteRenderer;
	private AnimationState currAnim;
	private AnimationState lastAnim;
	private float lastAnimTime;
	private int frame = 0;
	private Unit unit;

	// Use this for initialization
	void Awake ()
	{
		lastAnimTime = -1000;
		lastAnim = AnimationState.ROOT;
		currAnim = AnimationState.NONE;
		spriteRenderer = GetComponent<SpriteRenderer> ();
		unit = GetComponentInParent<Unit> ();
	}

	void Update ()
	{
		if (animEnabled) {
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
				if (unit.sprites.Length < 2)
					break;
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
			case AnimationState.ATTACK:
				if (unit.sprites.Length < 3)
					break;
				spriteRenderer.sprite = unit.sprites [2];
				break;
			}
		}
	}

	public void DeathAnimation () {
		GameObject ani = Instantiate (deathAnimation, spriteRenderer.transform) as GameObject;
		ani.transform.position = spriteRenderer.transform.position;
		StartCoroutine (DeathFade());
	}

	IEnumerator DeathFade() {
		float fadeTimeOut = 2f;
		float deathTime = Time.time + fadeTimeOut;
		if (fadeTimeOut != 0) {
			while (Time.time < deathTime) {
				float alpha = (deathTime - Time.time) / fadeTimeOut;
				spriteRenderer.color = new Color (spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
				yield return new WaitForFixedUpdate ();
			}
		}
	}

	public void ChangeAnimation (AnimationState newAnim)
	{
		currAnim = newAnim;
	}

	public void PlayAttackAnimation ()
	{
		ChangeAnimation (AnimationState.ATTACK);
		StartCoroutine (AttackAnimationTimer());
	}

	IEnumerator AttackAnimationTimer ()
	{
		yield return new WaitForSeconds (0.5f);
		ChangeAnimation (AnimationState.IDLE);
	}

	public void FlipVertical (bool flip)
	{
		spriteRenderer.flipX = (flip ? true : false);
	}

	public void SetEnable (bool enable)
	{
		if (enable) {
			this.animEnabled = true;
			spriteRenderer.enabled = true;
			return;
		}
		this.animEnabled = false;
		spriteRenderer.enabled = false;
	}
}
