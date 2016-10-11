using UnityEngine;
using System.Collections;

public class RootShot : Ability {

	//med range attack
	//Low damage
	//roots target for 2 turns (can attack but not move)
	//buffs target's defense for 2 turns (1)?

	public override string getName ()
	{
		return "Entangle";
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

	public float _projectileSpeed = 1f;
	public override float projectileSpeed
	{
		get { return _projectileSpeed; }
		set { _projectileSpeed = value; }
	}

	public float _projectileHeight = 1f;
	public override float projectileHeight
	{
		get { return _projectileHeight; }
		set { _projectileHeight = value; }
	}

	public int maxCoolDown = 5;
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
		return 5;
	}

	public int _minHeight;
	public override int minHeight ()
	{
		return -2;
	}

	public int _maxHeight;
	public override int maxHeight ()
	{
		return 2;
	}

	public int flatDamage = 2;
	public float powerModifier = 0.7f;
	public override int getDamage (int power)
	{
		return flatDamage + (int)(power*powerModifier);
	}

	new void Start () {
		base.Start ();
	}

	public override void HitTarget (Unit caster, Unit target) {
		target.TakeDamage (getDamage(caster.currentPower));
		target.statusEffects ().Add (new Entangled (caster, target, caster.currentPower, 2));
	}
}
