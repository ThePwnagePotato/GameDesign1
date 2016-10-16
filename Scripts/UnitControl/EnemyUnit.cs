using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class EnemyUnit : Unit {

	public abstract Unit targetUnit { get; set; }

	/* in a turn:
	 * - Decide what to do (move / attack)
	 * - Get Target unit (closest vs lowest health?)
	 * - Move towards target unit if not in range
	 * - bool with always higher/lower for positioning?
	 * - Attack
	*/

	new public void ResetTurn() {
		//resets targetUnit before calling the base function, so that the taunt status effect works
		targetUnit = null;
		base.ResetTurn();
	}


	List<Ability> availableAbilities = new List<Ability> ();
	Ability maxRangeAbility = null;
	Ability highestDamageAbility = null;

	public void DoTurn() {
		availableAbilities.Clear ();
		maxRangeAbility = null;
		highestDamageAbility = null;

		//get available abilities (cooldown <= 0)
		foreach (GameObject abilityObject in abilities) {
			Ability ability = abilityObject.GetComponent<Ability>();
			if (ability.cooldown <= 0) {
				availableAbilities.Add (ability);

				//find the longest range ability, and highest damage ability.
				if (maxRangeAbility == null || ability.maxRange () > maxRangeAbility.maxRange ()) {
					maxRangeAbility = ability;
				}
				if (highestDamageAbility == null || ability.getDamage (currentPower) > highestDamageAbility.getDamage (currentPower)) {
					highestDamageAbility = ability;
				}
			}
		}
		//if there are no available abilities, set canAttack to false (canAttack is reset to true in ResetTurn())
		if (availableAbilities.Count == 0) {
			canAttack = false;
		}

		//if the enemy does not have a target yet, acquire one
		if (targetUnit == null) {

		}

	}

	//first look for killable units in range
	//then for the lowest health
	private void getTargetUnit () {
		foreach (Unit unit in gameManager.boardManager.friendlyUnits) {

		}
	}


	//distance < highest damage range
	//distance < max range
	//distance < moves + max range
	private bool isTargetReachable (Vector3 targetPosition) {

		return false;
	}

	//first look through all targets
	//get highest health killable
	//get lowest health while highest damageable
	//get lowest health damageable

	//make moveto method, calls move mulitple times

	private Dictionary<ReachableTile, List<Vector3>> possibleMoveList = new Dictionary<ReachableTile, List<Vector3>> ();

	void MoveTo (Vector3 endTarget) {
		
	}
}
