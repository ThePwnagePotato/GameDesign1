using UnityEngine;
using System.Collections;

public class Weakness : StatusEffect
{

	public override string GetName ()
	{
		return "Weakness";
	}

	public override bool IsPositive ()
	{
		return false;
	}

	public override EffectType GetEffectType ()
	{
		return EffectType.POWBUFF;
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

	public Weakness (int power, int duration) : base (power, duration)
	{
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
