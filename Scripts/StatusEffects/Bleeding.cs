using UnityEngine;
using System.Collections;

public class Bleeding : StatusEffect
{
	//deals damage every turn

	public override string GetName ()
	{
		return "Bleeding";
	}

	public override string[] getDescription ()
	{
		return new string[] {
			"Blood drips from your wounds, dealing damage every turn"
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
	}
	public override void OnTurnEnd () {
		//bleed, do damage
		target.TakeDamage(5 + (int) (_power / 2));
	}


}
