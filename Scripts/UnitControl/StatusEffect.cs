using UnityEngine;
using System.Collections;

public abstract class StatusEffect : MonoBehaviour {

	public abstract string GetName ();
	public abstract bool IsPositive ();

	public abstract int power { get; set; }
	public abstract int duration { get; set; }

	public abstract Unit evoker { get; set; }
	public abstract Unit target { get; set; }

	/*
	public StatusEffect (Unit evoker, Unit target, int power, int duration) {
		this.power = power;
		this.duration = duration;

		this.evoker = evoker;
		this.target = target;
	}
	*/

	// Lowers duration by one. If 0 or less afterwards, return true
	public bool UpdateDuration () {
		return --duration <= 0;
	}

	//methods can access the evoker and target units, can just return input to do nothing

	//input[0] = currentHealth
	//input[1] = currentPower
	//input[2] = currentDefense
	//input[3] = currentMoves
	//input[4] = currentMovesUp
	//input[5] = currentMovesDown
	//input[6] = currentMovesSide
	//input[7] = canMove
	//input[8] = canAttack
	public virtual void OnTurnStart () {
		duration--;
	}

	public abstract void OnTurnEnd ();

	public abstract void OnMovement ();

	public abstract void OnAbility ();

	public abstract void OnDoDamage ();

	public abstract void OnTakeDamage ();

	public void initialize (Unit evoker, int power, int duration) {
		this.evoker = evoker;
		this.power = power;
		this.duration = duration;
	}

	// Use this for initialization
	void Start () {
	
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