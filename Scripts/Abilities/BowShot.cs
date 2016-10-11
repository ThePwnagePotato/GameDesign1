using UnityEngine;
using System.Collections;

public class BowShot : Ability
{
	//basic archer attack

	public override string getName ()
	{
		return "Shoot";
	}

	public GameObject _model;
	public override GameObject model
	{
		get { return _model; }
		set { _model = value; }
	}

	private GameManager _gameManager;
	public override GameManager gameManager
	{
		get { return _gameManager; }
		set { _gameManager = value; }
	}

	public float _projectileSpeed = 1.5f;
	public override float projectileSpeed
	{
		get { return _projectileSpeed; }
		set { _projectileSpeed = value; }
	}

	public float _projectileHeight = 1;
	public override float projectileHeight
	{
		get { return _projectileHeight; }
		set { _projectileHeight = value; }
	}

	public int maxCoolDown = 0;
	public override int maxCooldown ()
	{
		return maxCoolDown;
	}

	private int _cooldown;
	public override int cooldown {
		get { return _cooldown; }
		set { _cooldown = value; }
	}

	public int _minRange;
	public override int minRange ()
	{
		return 1;
	}

	public int _maxRange;
	public override int maxRange ()
	{
		return 6;
	}

	public int _minHeight;
	public override int minHeight ()
	{
		return -4;
	}

	public int _maxHeight;
	public override int maxHeight ()
	{
		return 4;
	}

	public int flatDamage = 2;
	public float powerModifier = 1;
	public override int getDamage (int power)
	{
		return flatDamage + (int)(power*powerModifier);
	}

	new void Start () {
		base.Start ();
	}

	public override void HitTarget (Unit caster, Unit target) {
		target.TakeDamage (getDamage(caster.currentPower));
	}
}