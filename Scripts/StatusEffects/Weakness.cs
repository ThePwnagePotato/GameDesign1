
using UnityEngine;
using System.Collections;

public class Weakness : StatusEffect
{
	//Lowers power stat temporarily

	public override string GetName ()
	{
		return "Weakness";
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
		target.currentDefense -= power;
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
		

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
