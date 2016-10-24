using UnityEngine;
using System.Collections;

public class TankBuff : StatusEffect {

	//Lowers power stat temporarily

	public GameObject fatigued;

	public override string GetName ()
	{
		return "Fatigued";
	}

	public override string[] getDescription ()
	{
		return new string[] {
			"This unit's body is energized, raising all its stats"
		};
	}

	public override bool IsPositive ()
	{
		return true;
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

		//raise stats
		target.currentDefense += power;
		target.currentPower += power;
		target.currentMoves += Mathf.Max(3, power / 7);
		target.currentMovesUp += Mathf.Max(2, power / 10);;
		target.currentMovesDown += Mathf.Max(2, power / 10);;
		target.currentMovesSide += Mathf.Max(2, power / 10);;
	}
	public override void OnRemoval () {
		GameObject effect = Instantiate (fatigued, target.gameObject.transform) as GameObject;
		target.statusEffects().Add (effect);
		effect.GetComponent<StatusEffect> ().initialize(_evoker, _power, 1);
	}
}
