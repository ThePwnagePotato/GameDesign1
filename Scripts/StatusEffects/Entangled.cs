﻿using UnityEngine;
using System.Collections;

public class Entangled : StatusEffect {

	//can not move (trapped by vines)
	//ups defense (vines protect you)
	//DoT? (prickly vines?)

	public override string GetName ()
	{
		return "Snared";
	}

	public override string[] getDescription ()
	{
		return new string[] {
			"Unable to move, but can still attack"
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

		target.canMove = false;
		target.currentDefense += power;
	}
		
}
