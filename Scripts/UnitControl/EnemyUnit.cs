using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class EnemyUnit : Unit {

	/*
	 *
	 * 
	 * TODO?: get target: killable units out of range
	 * */

	//should the unit get up close in your face, or stay at their range
	public bool stayClose;

	public Unit targetUnit;
	Targetable targetAction;
	ReachableTile targetActionTile;

	/* in a turn:
	 * - Decide what to do (move / attack)
	 * - Get Target unit (closest vs lowest health?)
	 * - Move towards target unit? if not in range
	 * - bool with always higher/lower for positioning?
	 * - Attack
	*/

	new public void ResetTurn() {
		//resets targetUnit before calling the base function, so that the taunt status effect works
		targetUnit = null;
		targetAction = Targetable.NULL;
		targetActionTile = null;
		base.ResetTurn();
	}

	Dictionary<ReachableTile, List<Vector3>> possibleMoveList = new Dictionary<ReachableTile, List<Vector3>> ();

	List<TargetAction> possibleTargets = new List<TargetAction> ();
	List<Ability> availableAbilities = new List<Ability> ();
	Ability maxRangeAbility = null;
	Ability highestDamageAbility = null;

	public void DoTurn() {
		availableAbilities.Clear ();
		possibleTargets.Clear ();
		maxRangeAbility = null;
		highestDamageAbility = null;

		//first get possible moves
		if (canMove) {
			GetPossibleMoves ();
		}

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
			SetTargetUnit ();
		} 

		//next get an action to do
		if (targetAction == Targetable.NULL) {
			TargetAction tAction = isTargetReachable (targetUnit);
			targetAction = tAction.action;
			targetActionTile = tAction.tile;
		}

		//do the action
		if (canMove && targetActionTile != null) {
			MoveTo (targetActionTile.position);
		}
		if (canAttack) {
			if (targetAction == Targetable.MAXRANGE) {
				maxRangeAbility.ActivateAbility (targetUnit.transform.position);
			} else if (targetAction == Targetable.MAXDAMAGE) {
				highestDamageAbility.ActivateAbility (targetUnit.transform.position);
			}
		}
	}






	//first look for killable units in range
	//then for highest damage possible (lower health higher chance)
	//then for in range (lower health higher chance)
	//then for closest unit
	private float closestDistance = 999;
	private Unit closestUnit = null;
	private Targetable maxTargetable = Targetable.UNABLE;
	private void SetTargetUnit () {
		//health to calculate score
		int highestHealth = 0;

		foreach (Unit unit in gameManager.boardManager.friendlyUnits) {
			//check if it is the closest unit
			float distance = Vector3.Distance (transform.position, unit.transform.position);
			if (distance < closestDistance) {
				closestUnit = unit;
			}

			//get targetability
			TargetAction isTargetable = isTargetReachable(unit);

			Targetable originalAction = isTargetable.action;

			//if the unit is not targetable, continue
			//if (isTargetable.action == Targetable.UNABLE) {
			//	continue;
			//}

			//check if target can be killed
			//get either maxRange damage or highestDamage damage
			if (isTargetable.action != Targetable.UNABLE && canAttack) {
				int rawDamage = (isTargetable.action == Targetable.MAXRANGE) ? maxRangeAbility.getRawDamage (currentPower) : highestDamageAbility.getRawDamage (currentPower);
				if (unit.currentHealth + unit.currentDefense - rawDamage < 0) {
					//if the unit is killable, set as so
					isTargetable.action = Targetable.KILLABLE;
				}
			}

			if (isTargetable.action < maxTargetable) {
				continue;
			} 
			//if there is a better target group, remove the rest and continue with only the better targets
			else if (isTargetable.action > maxTargetable) {
				highestHealth = 0;
				possibleTargets.Clear ();
				maxTargetable = isTargetable.action;
			}
			//add the unit to the list, also set highestHealth and add totalHealth
			isTargetable.action = originalAction;
			possibleTargets.Add (isTargetable);
			if (unit.currentHealth > highestHealth) {
				highestHealth = unit.currentHealth;
			}
		}

		//if there are no targets, return the closest unit
		//if there is one target, return that target
		if (possibleTargets.Count == 0) {
			targetUnit = closestUnit;
			targetAction = Targetable.UNABLE;
			return;
		} else if (possibleTargets.Count == 1) {
			targetUnit = possibleTargets [0].unit;
			targetAction = possibleTargets [0].action;
			targetActionTile = possibleTargets [0].tile;
			return;
		}
			
		// if there is more than 1 target 
		// if killable, choose one at random
		int randomUnitI;
		if (maxTargetable == Targetable.KILLABLE) {
			randomUnitI = Mathf.RoundToInt(Random.value * (possibleTargets.Count - 1));
			targetUnit = possibleTargets [randomUnitI].unit;
			targetAction = possibleTargets [randomUnitI].action;
			targetActionTile = possibleTargets [randomUnitI].tile;
			return;
		}
		// else give all of them a score based on their health
		// choose based on that score
		// the lower the health, the higher the score, mostly based on the difference between highest and lowest healths
		// adds all score together, generate and random int between 0 and scoreTotal, and remove individual scores until the random int reaches 0
		int scoreUpperbound = 2 * highestHealth;
		int scoreTotal = 0;
		foreach (TargetAction tScore in possibleTargets) {
			tScore.score = scoreUpperbound - tScore.currentHealth;
			scoreTotal += tScore.score;
		}
		randomUnitI = Mathf.RoundToInt(Random.value * scoreTotal);
		foreach (TargetAction tScore in possibleTargets) {
			randomUnitI -= tScore.score;
			if (randomUnitI <= 0) {
				targetUnit = possibleTargets [randomUnitI].unit;
				targetAction = possibleTargets [randomUnitI].action;
				targetActionTile = possibleTargets [randomUnitI].tile;
				return;
			}
		}
	}

	//private float closestTargetableDistance = 999;

	private TargetAction isTargetReachable (Unit targetUnit) {
		Vector3 targetPosition = targetUnit.transform.position;
		Targetable testTarget = Targetable.UNABLE;
		//first check current position
		testTarget = getTargetablility(testTarget, targetPosition, transform.position);

		ReachableTile finalTile = null;
		if (canMove) {
			//then check possibleMoveList if the unit can move
			//save the tile the unit would stand on 
			float finalDistance = Vector3.Distance (targetPosition, transform.position);
			Targetable lastBestTarget = testTarget;
			foreach (ReachableTile tile in possibleMoveList.Keys) {

				testTarget = getTargetablility (testTarget, targetPosition, tile.position);

				float currentDistance = Vector3.Distance (targetPosition, tile.position);

				//if the move is better than the previous 
				//if the move is the same value as the previous, but further away if needed, or closer if needed
				if (testTarget > lastBestTarget || !stayClose && testTarget == lastBestTarget && currentDistance > finalDistance ||
				   stayClose && testTarget == lastBestTarget && currentDistance < finalDistance) {
					lastBestTarget = testTarget;
					finalTile = tile;
					finalDistance = currentDistance;
				}
				//maximum highest damage, so stop after that
				//if (testTarget >= Targetable.MAXDAMAGE) {
				//	finalTile = tile;
				//	break;
				//}
			}
		}
		//create a new action and return it
		TargetAction action = new TargetAction (targetUnit, testTarget);
		action.tile = finalTile;
		return action;
	}

	private Targetable getTargetablility (Targetable value, Vector3 targetPosition, Vector3 position) {
		//check distance between tile and targetposition
		float distance = Vector3.Distance (position, targetPosition);

		//if the distance was larger than before, skip if the unit should stayClose
		//if (stayClose && distance > closestTargetableDistance) {
		//	return value;
		//}

		//set closestDistance to distance
		//closestTargetableDistance = distance;

		//check if abilities reach if attacking is possible
		if (canAttack) {
			int horDistance = System.Math.Abs ((int) position.x - (int)targetPosition.x) + System.Math.Abs ((int)position.z - (int)targetPosition.z);
			//if unable, check for maxrange
			if (value == Targetable.UNABLE) {
				if (horDistance <= maxRangeAbility.sideRange () &&
				   (int)targetPosition.y >= (int)position.y - maxRangeAbility.downRange () && (int)targetPosition.y <= (int)position.y + maxRangeAbility.upRange ()) {
					value = Targetable.MAXRANGE;
				}
			}
			//if maxrange, check for maxdamage
			if (value == Targetable.MAXRANGE) {
				if (horDistance <= highestDamageAbility.sideRange () &&
				   (int)targetPosition.y >= (int)position.y - highestDamageAbility.downRange () && (int)targetPosition.y <= (int)position.y + highestDamageAbility.upRange ()) {
					//if maxDamage has been found, break
					value = Targetable.MAXDAMAGE;
				}
			}
		}
		return value;
	}

	//first look through all targets
	//get highest health killable
	//get lowest health while highest damageable
	//get lowest health damageable

	private int pathIndex;
	List<Vector3> path;
	//moves the unit through a path to the endTarget, if the endTarget is in possibleMoveList
	void MoveTo (Vector3 endTarget) {
		//check if endTarget is in possibleMoveList
		ReachableTile endTile = null;
		foreach (ReachableTile reachableTile in possibleMoveList.Keys) {
			if (reachableTile.position == endTarget) {
				endTile = reachableTile;
				break;
			}
		}

		//if not, error and return
		if (endTile == null) {
			Debug.Log ("ERROR: " + getName () + " tried to reach a tile out of range!");
			return;
		}

		//if so, move through the path
		path = possibleMoveList [endTile];
		pathIndex = 0;
		isMoving = true;
		Move (path [pathIndex]);
	}

	void Update () {
		if (gameManager.gameStack.Peek ().type == GameStateType.ANIMATION) {
			return;
		}
		if (isMoving) {
			if (transform.position == path [pathIndex]) {
				if (pathIndex < path.Count - 1) {
					pathIndex++;
					Move (path [pathIndex]);
				} else {
					isMoving = false;
				}
			}
		}
	}

	new Dictionary<ReachableTile, List<Vector3>> GetPossibleMoves ()
	{
		possibleMoveList.Clear ();

		if (!canMove)
			return possibleMoveList;

		PositionSearch (transform.position-Vector3.up, currentMoves, currentMovesUp, currentMovesDown, currentMovesSide, true, Direction.NONE, new List<Vector3> ());

		return possibleMoveList;
	}

	void InitiateTargetSearch(Vector3 targetPosition, Vector3 position, int currentMoves, int currentMovesUp, int currentMovesDown, int currentMovesSide, bool straight, 
		Direction direction, List<Vector3> path) {

		currentMovesSide--;
		currentMoves--;
		if (targetPosition.y > position.y) {
			int heightDelta = (int)targetPosition.y - (int)position.y;
			currentMovesUp -= heightDelta;
			currentMoves -= heightDelta;
		}
		else if (targetPosition.y < position.y) {
			int heightDelta = (int)position.y - (int)targetPosition.y;
			currentMovesDown -= heightDelta;
			currentMoves -= heightDelta;
		}
		// calculate if this is still straight on the path
		PositionSearch (targetPosition, currentMoves, currentMovesUp, currentMovesDown, currentMovesSide, straight, direction, path);
	}

	void PositionSearch (Vector3 position, int currentMoves, int currentMovesUp, int currentMovesDown, int currentMovesSide, bool straight, 
		Direction direction, List<Vector3> path) {

		if (currentMoves < 0 || currentMovesUp < 0 || currentMovesDown < 0 || currentMovesSide < 0
			|| (boardManager.unitMap[(int)position.x, (int)position.z] != null && boardManager.unitMap[(int)position.x, (int)position.z].isFriendly() != isFriendly())) {
			return;
		}
		// this is a legitimate movement

		List<Vector3> pathCopy = new List<Vector3> (path);
		//if straight, remove the last vector in the path, since it is the same direction
		if (straight && pathCopy.Count > 0) {
			pathCopy.RemoveAt (pathCopy.Count - 1);
		}
		pathCopy.Add (position + Vector3.up);

		AddNonDuplicate(possibleMoveList, new KeyValuePair<ReachableTile, List<Vector3>>(new ReachableTile(position+Vector3.up, straight, currentMoves, currentMovesUp, currentMovesDown, currentMovesSide), 
			pathCopy));
		if (position.x > 0) {
			Vector3 targetPosition = new Vector3 (position.x-1, boardManager.heightMap[(int)position.x-1, (int)position.z], position.z);
			InitiateTargetSearch (targetPosition, position, currentMoves, currentMovesUp, currentMovesDown, currentMovesSide, direction == Direction.Z ? false : true, Direction.X, pathCopy);
		}
		if (position.x < boardManager.dimensions.x - 1) {
			Vector3 targetPosition = new Vector3 (position.x+1, boardManager.heightMap[(int)position.x+1, (int)position.z], position.z);
			InitiateTargetSearch (targetPosition, position, currentMoves, currentMovesUp, currentMovesDown, currentMovesSide, direction == Direction.Z ? false : true, Direction.X, pathCopy);
		}
		if (position.z > 0) {
			Vector3 targetPosition = new Vector3 (position.x, boardManager.heightMap[(int)position.x, (int)position.z-1], position.z-1);
			InitiateTargetSearch (targetPosition, position, currentMoves, currentMovesUp, currentMovesDown, currentMovesSide, direction == Direction.X ? false : true, Direction.Z, pathCopy);
		}
		if (position.z < boardManager.dimensions.z - 1) {
			Vector3 targetPosition = new Vector3 (position.x, boardManager.heightMap[(int)position.x, (int)position.z+1], position.z+1);
			InitiateTargetSearch (targetPosition, position, currentMoves, currentMovesUp, currentMovesDown, currentMovesSide, direction == Direction.X ? false : true, Direction.Z, pathCopy);
		}
	}

	void AddNonDuplicate (Dictionary<ReachableTile, List<Vector3>> list, KeyValuePair<ReachableTile, List<Vector3>> element)
	{
		foreach (ReachableTile tile in list.Keys) {
			//if the tile is already reachable and in less movements (path.Count), don't add a new one
			if (tile.position == element.Key.position) {
				if (list [tile].Count > element.Value.Count) {
					list [tile] = element.Value;
					return;
				} else
					return;
			}
		}

		list.Add (element.Key, element.Value);
	}

}

public enum Targetable
{
	NULL,
	UNABLE,
	MAXRANGE,
	MAXDAMAGE,
	KILLABLE
}

public class TargetAction
{
	public Unit unit;
	public int currentHealth;
	public Targetable action;
	public ReachableTile tile;
	public int score;

	public TargetAction (Unit unit, Targetable action) {
		this.unit = unit;
		this.currentHealth = unit.currentHealth;
		this.action = action;
		tile = null;
	}
}
