using UnityEngine;
using System.Collections;

public class Blizzard : Ability {

	//3x3 aoe damage
	//applies slow

	public GameObject slowness;

	public override string getName ()
	{
		return "Blizzard";
	}

	public override string[] getDescription ()
	{
		return new string[] {
			"Damages a 3x3 area",
			"All enemies hit are slowed"
		};
	}

	public float _upScale;
	public override float upScale
	{
		get { return _upScale; }
		set { _upScale = value; }
	}

	public float _downScale;
	public override float downScale
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

	public int _downRange;
	public override int downRange()
	{
		return _downRange;
	}

	public int _sideRange;
	public override int sideRange()
	{
		return _sideRange;
	}

	public int _upRange;
	public override int upRange()
	{
		return _upRange;
	}

	public bool _dealsDamage;
	public override bool dealsDamage () {
		return _dealsDamage;
	}

	public int flatDamage;
	public float powerModifier;
	public override int getDamage (int power)
	{
		//standard damage
		int damage = getRawDamage(power);
		//randomness
		damage = (int)((Random.value * 0.2 + 0.9) * damage);
		//crit
		if (Random.value < critChance) {
			damage = (int)(damage * 1.5);
		}

		return damage;
	}

	public override int getRawDamage (int power)
	{
		//standard damage
		int damage = flatDamage + (int)(power * powerModifier);
		return damage;
	}

	public override void HitTarget (Unit caster, Vector3 targetPosition) {
		Unit[,] unitMap = gameManager.boardManager.unitMap;

		int x = (int) targetPosition.x;
		int z = (int) targetPosition.z;

		//for (int xRange = x - 1; xRange <= x + 1; x++) {
		//	for (int zRange = z - 1; zRange <= z + 1; z++) {
		//		if (xRange >= 0 && zRange >= 0 && xRange < gameManager.boardManager.dimensions.x && zRange < gameManager.boardManager.dimensions.z) {
					Unit target = unitMap [x, z];
					if (target != null) {
						int finalPower = caster.currentPower;
						foreach (StatusEffect effect in caster.GetComponentsInChildren<StatusEffect> ()) {
							finalPower = effect.OnDoDamage (finalPower);
						}

						int finalDamage = getDamage (finalPower);

						if (target.transform.position.y > caster.transform.position.y) {
							finalDamage = (int) (finalDamage * _upScale);
						} else if (target.transform.position.y < caster.transform.position.y) {
							finalDamage = (int) (finalDamage * _downScale);
						}

						target.TakeDamage (finalDamage);

						DamageDisplayer dd = target.GetComponentInChildren<DamageDisplayer> ();
						if (dd != null) {
							dd.ShowText ("Slowed");
						}

						GameObject addEffect = Instantiate (slowness, target.gameObject.transform) as GameObject;
						addEffect.GetComponent<StatusEffect> ().initialize(caster, caster.currentPower, 3);
					}
		//		}
		//	}
		//}
	}
}
