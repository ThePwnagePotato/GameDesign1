using UnityEngine;
using System.Collections;

public class Fireball : Ability
{
	public string _name;


	public override string getName ()
	{
		return _name;
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

	public int _maxCooldown;
	public override int maxCooldown ()
	{
		return _maxCooldown;
	}

	private int _cooldown = 0;
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
		return _minHeight;;
	}

	public int _maxHeight;
	public override int maxHeight ()
	{
		return _maxHeight;
	}

	public int flatDamage;
	public float powerModifier;
	public override int damage (int power)
	{
		return flatDamage + (int)(power*powerModifier);
	}

	new void Start () {
		base.Start ();
	}
}