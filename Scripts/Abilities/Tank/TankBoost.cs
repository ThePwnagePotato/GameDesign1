﻿using UnityEngine;
using System.Collections;

public class TankBoost : Ability {

	//buff all stats (mostly movement) for 1 turn
	//then debuff (fatigue) all stats

	public GameObject selfBuff;

	public override string getName ()
	{
		return "Bulk Up";
	}

	public override string[] getDescription ()
	{
		return new string[] {
			"Work out, buffing all stats for 1 turn",
			"Gain fatigue afterwards"
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
		return 0;
	}

	public override int getRawDamage (int power)
	{
		return 0;
	}

	public override void HitTarget (Unit caster, Vector3 targetPosition) {
		Unit target = gameManager.boardManager.unitMap[(int) targetPosition.x, (int) targetPosition.z];
		if (target != null) {
			DamageDisplayer dd = target.GetComponentInChildren<DamageDisplayer> ();
			if (dd != null) {
				dd.ShowText ("Boosted");
			}

			GameObject effect = Instantiate (selfBuff, target.gameObject.transform) as GameObject;
			effect.GetComponent<StatusEffect> ().initialize(caster, caster.currentPower, 1);
		}
	}
}
