/*using UnityEngine;
using System.Collections;

public class Stab : Ability
{
	public override string getName ()
	{
		return "Stab";
	}

	public override int maxCooldown ()
	{
		return 0;
	}

	private int _cooldown = 0;
	public override int cooldown {
		get { return _cooldown; }
		set { _cooldown = value; }
	}

	public override int minRange ()
	{
		return 1;
	}

	public override int maxRange ()
	{
		return 2;
	}

	public override int minHeight ()
	{
		return 0;
	}

	public override int maxHeight ()
	{
		return 0;
	}

	public override int damage (int power)
	{
		return 1 + power;
	}

	public override Vector3 getTrajectory (float time, Unit caster, Unit target)
	{
		//TODO
		return new Vector3 (1, 2, 3);
	}

}
*/