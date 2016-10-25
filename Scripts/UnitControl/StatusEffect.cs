using UnityEngine;
using System.Collections;

public abstract class StatusEffect : MonoBehaviour {

	//usually: 	do effect on start of turn
	//			-1 duration at start of turn
	//			remove effects with <=0 duration at end of turn

	public abstract string GetName ();
	public abstract string[] getDescription ();
	public abstract bool IsPositive ();

	public abstract int power { get; set; }
	public abstract int duration { get; set; }

	public abstract Unit evoker { get; set; }
	public abstract Unit target { get; set; }

	// Lowers duration by one. If 0 or less afterwards, return true
	public bool UpdateDuration () {
		return --duration <= 0;
	}
		
	public virtual void OnTurnStart () {
		duration--;
	}

	public virtual void OnTurnEnd () {}

	public virtual void OnMovement () {}

	public virtual void OnAbility () {}

	public virtual int OnDoDamage (int power) { return power; }

	public virtual void OnTakeDamage () {}

	public virtual void OnRemoval () {}

	public void initialize (Unit evoker, int power, int duration) {
		this.evoker = evoker;
		this.power = power;
		this.duration = duration;
	}

	// Use this for initialization
	void Start () {
		target = GetComponentInParent<Unit> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

/*
public enum EffectType {
	DAMAGING,
	HEALING,

	POWDEBUFF,
	DEFDEBUFF,
	MOVDEBUFF,

	POWBUFF,
	DEFBUFF,
	MOVBUFF,
}
*/