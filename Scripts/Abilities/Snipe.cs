using UnityEngine;
using System.Collections;

public class Snipe : Ability {

	//long range archer attack
	//higher damage than normal

	public override string getName ()
	{
		return "Snipe";
	}

	public int _upScale;
	public override int upScale
	{
		get { return _upScale; }
		set { _upScale = value; }
	}

	public int _downScale;
	public override int downScale
	{
		get { return _downScale; }
		set { _downScale = value; }
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

	public float _projectileSpeed;
	public override float projectileSpeed
	{
		get { return _projectileSpeed; }
		set { _projectileSpeed = value; }
	}

	public float _projectileHeight;
	public override float projectileHeight
	{
		get { return _projectileHeight; }
		set { _projectileHeight = value; }
	}

	public int _maxCooldown;
	public override int maxCooldown ()
	{
		return _maxCooldown;
	}

	private int _cooldown;
	public override int cooldown {
		get { return _cooldown; }
		set { _cooldown = value; }
	}

	public int _minRange;
	public override int minRange ()
	{
		return _minRange;
	}

	public int _maxRange;
	public override int maxRange ()
	{
		return _maxRange;
	}

	public int _minHeight;
	public override int minHeight ()
	{
		return _minHeight;
	}

	public int _maxHeight;
	public override int maxHeight ()
	{
		return _maxHeight;
	}

	public int flatDamage;
	public float powerModifier;
	public override int getDamage (int power)
	{
		return flatDamage + (int)(power*powerModifier);
	}

	new void Start () {
		base.Start ();

		_upScale = 0;
		_downScale = 0;

		_projectileSpeed = 1.5f;
		_projectileHeight = 2f;

		_maxCooldown = 4;

		_minRange = 3;
		_maxRange = 10;
		_minHeight = 6;
		_maxHeight = 6;

		flatDamage = 2;
		powerModifier = 1f;
	}

	public override void HitTarget (Unit caster, Unit target) {
		target.TakeDamage (getDamage(caster.currentPower));
	}
}
