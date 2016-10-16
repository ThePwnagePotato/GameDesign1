using UnityEngine;
using System.Collections;

public class Slowness : StatusEffect {

	//Lowers power stat temporarily

	public override string GetName ()
	{
		return "Slowed";
	}

	public override string[] getDescription ()
	{
		return new string[] {
			"This unit's movement is slowed"
		};
	}

	public override bool IsPositive ()
	{
		return false;
	}

	private int _power;
	public override int power {
		get { return _power; }
		set { _power = value; }
	}

	private int _duration;
	public override int duration {
		get { return _duration;	}
		set { _duration = value; }
	}

	private Unit _evoker;
	public override Unit evoker {
		get { return _evoker; }
		set { _evoker = value; }
	}

	private Unit _target;
	public override Unit target {
		get { return _target; }
		set { _target = value; }
	}

	public override void OnTurnStart () {
		base.OnTurnStart ();

		//subtract power of the effect from power of the unit
		target.currentMoves -= 2;
		target.currentMovesUp -= 1;
		target.currentMovesDown -= 1;
		target.currentMovesSide -= 1;
	}
	public override void OnTurnEnd () {

	}
	public override void OnMovement () {

	}
	public override void OnAbility () {

	}
	public override void OnDoDamage () {

	}
	public override void OnTakeDamage () {

	}
	public override void OnRemoval () {

	}

}
