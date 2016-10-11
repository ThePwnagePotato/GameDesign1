using UnityEngine;
using System.Collections;

public abstract class EnemyUnit : Unit {

	public abstract Unit targetUnit { get; set; }

	/* in a turn:
	 * - Get Target unit (closest vs lowest health?)
	 * - Move towards target unit if not in range
	 * - bool with always higher/lower for positioning?
	 * - Attack
	*/

	new public void ResetTurn() {
		targetUnit = null;
		base.ResetTurn();
	}

	public void DoTurn() {
		int maxRange = 0;

		//find the longest range ability, and highest damage ability.
		foreach (GameObject abilityObject in abilities) {
			Ability ability = abilityObject.GetComponent<Ability>();
			if (ability.maxRange () > maxRange) {
				maxRange = ability.maxRange ();
			}

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
}
