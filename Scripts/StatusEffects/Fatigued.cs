﻿using UnityEngine;
using System.Collections;

public class Fatigued : StatusEffect {

	//Lowers power stat temporarily

	public override string GetName ()
	{
		return "Fatigued";
	}

	public override string[] getDescription ()
	{
		return new string[] {
			"This unit's body is tired, lowering its movement stats"
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
		target.currentMoves = Mathf.Max(target.currentMoves - (power / 2), 1);
		target.currentMovesUp = Mathf.Max(target.currentMovesUp - (power / 2), 0);
		target.currentMovesDown = Mathf.Max(target.currentMovesDown - (power / 2), 0);
		target.currentMovesSide = Mathf.Max(target.currentMovesSide - (power / 2), 1);
	}

}
