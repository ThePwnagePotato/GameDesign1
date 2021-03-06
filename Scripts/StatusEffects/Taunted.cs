﻿using UnityEngine;
using System.Collections;

public class Taunted : StatusEffect {

	//fixes the enemy's target to a specific unit

	public override string GetName ()
	{
		return "Taunted";
	}

	public override string[] getDescription ()
	{
		return new string[] {
			"Forced to target " + _evoker.getName ()
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

		if (target is EnemyUnit) {
			EnemyUnit unit = (EnemyUnit) target;
			unit.targetUnit = evoker;
		}
	}
		
}
