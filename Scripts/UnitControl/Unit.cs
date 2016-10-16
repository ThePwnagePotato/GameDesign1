using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// class that is used for path calculation/display
public class ReachableTile {
	// the position of this path element
	public Vector3 position;
	// the tile is reachable by moving in a straight line (without going through enemy units or passing through <=0-value's on the heightmap)
	public bool straight;

	// this stores how much movement points the unit has left after moving
	public int totalMove;
	public int upMove;
	public int downMove;
	public int sideMove;

	public ReachableTile (Vector3 position, bool straight, int currentMoves, int currentMovesUp, int currentMovesDown, int currentMovesSide) {
		this.position = position;
		this.straight = straight;
		totalMove = currentMoves;
		upMove = currentMovesUp;
		downMove = currentMovesDown;
		sideMove = currentMovesSide;
	}
}

enum Direction {
	NONE,
	X,
	Z
}

public abstract class Unit : MonoBehaviour
{

	public abstract string getName ();

	public abstract bool isFriendly ();

	public abstract bool isAlive { get; set; }

	public abstract List<GameObject> abilities { get; set; }

	public abstract List<GameObject> statusEffects ();

	public abstract bool canMove { get; set; }

	public abstract bool canAttack { get; set; }

	public abstract int maxHealth { get; set; }

	public abstract int currentHealth { get; set; }

	public abstract int power { get; set; }

	public abstract int currentPower { get; set; }

	public abstract int defense { get; set; }

	public abstract int currentDefense { get; set; }

	public abstract int totalMoves { get; set; }

	public abstract int currentMoves { get; set; }

	public abstract int totalMovesUp { get; set; }

	public abstract int currentMovesUp { get; set; }

	public abstract int totalMovesDown { get; set; }

	public abstract int currentMovesDown { get; set; }

	public abstract int totalMovesSide { get; set; }

	public abstract int currentMovesSide { get; set; }

	public abstract float movementSpeed { get; set; }

	public abstract SpriteRenderer spriteRenderer { get; set; } 

	public abstract Sprite[] sprites { get; set; }

	public abstract BoardManager boardManager { get; set; }

	public abstract GameManager gameManager { get; set; }

	public void Start ()
	{
		// connect dependencies
		boardManager = GameObject.FindGameObjectWithTag ("BoardManager").GetComponent<BoardManager> ();
		if (boardManager == null)
			Debug.Log ("Unit: BoardManager not found)");
		gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
		if (gameManager == null)
			Debug.Log ("Unit: GameManager not found)");

		// setting initial state to being alive
		isAlive = true;

		// add itself to the unitMap and relevant lists. This is for testing purposes, normally the boardManager does this while spawning unit
		Debug.Log("TEST: Adding itself to unitmap");
		boardManager.AddToUnitMap(this);
		if (isFriendly ())
			boardManager.friendlyUnits.Add (this);
		else
			boardManager.enemyUnits.Add (this);

		for (int i = 0; i < abilities.Count; i++) {
			GameObject newAbility = Instantiate (abilities[i], this.transform) as GameObject;
			newAbility.transform.position = this.transform.position;
		}
	}

	/* damages the characted, calls Die() if health goes below 0
	 * minimum damage is 1
	 * 
	 * add damager unit?
	 * return int finalDamage to be displayed?
	 * */
	public void TakeDamage (int damage)
	{
		// Go through the list of statuseffects, get the ones that affect the DEF stat
		// then apply that effect to the tempDef value

		foreach (GameObject effectObject in statusEffects ()) {
			effectObject.GetComponent<StatusEffect>().OnTakeDamage ();
		}

		// calculate the final damage using the tempDef value
		int finalDamage = Mathf.Max (damage - currentDefense, 1);

		currentHealth = currentHealth - finalDamage;

		// do in another class?
		// return sa bool?
		if (currentHealth < 0) {
			Die ();
		}
	}
	/* remove from board
	 * 
	 * want this? put in game manager?
	 * */
	public void Die ()
	{

	}

	//resets all temporary (current) values, then adds StatusEffects to them
	//this allows easy display of effects onto the actual stats (if currentpower = power + 3, then +3 power from effects)
	//set values under 0 to 0 at the end to make sure all effects are properly added (-2 going under 0 and then +3 will be fine)
	public void ResetTurn() {
		currentPower = power;
		currentDefense = defense;
		currentMoves = totalMoves;
		currentMovesUp = totalMovesUp;
		currentMovesDown = totalMovesDown;
		currentMovesSide = totalMovesSide;
		canMove = true;
		canAttack = true;

		foreach (GameObject effectObject in statusEffects ()) {
			StatusEffect effect = effectObject.GetComponent<StatusEffect> ();
			effect.OnTurnStart ();
		}

		if (currentPower < 0) { currentPower = 0; }
		if (currentDefense < 0) { currentDefense = 0; }
		if (currentMoves < 0) { currentMoves = 0; }
		if (currentMovesUp < 0) { currentMovesUp = 0; }
		if (currentMovesDown < 0) { currentMovesDown = 0; }
		if (currentMovesSide < 0) { currentMovesSide = 0; }
	}

	//tick abilities
	//remove statuseffects
	public void EndTurn() {
		foreach (GameObject abilityObject in abilities) {
			Ability ability = abilityObject.GetComponent<Ability> ();

			if (ability.cooldown < 0) {
				ability.cooldown = ability.maxCooldown ();
			} else if (ability.cooldown > 0) {
				ability.cooldown--;
			}
		}

		for (int i = statusEffects ().Count - 1; i >= 0; i--) {
			GameObject effectObject = statusEffects () [i];
			StatusEffect effect = effectObject.GetComponent<StatusEffect> ();
			effect.OnTurnEnd ();
					
			if (effect.duration <= 0) {
				statusEffects ().RemoveAt (i);
				Destroy (effect.gameObject);
			}
		}
	}


	private enum Direction
	{
		X,
		Z,
		NONE

	}

	// loop through all the elements of the list
	// if a vector equal to the new vector is found, return
	// if none is found, add the new vector to the list
	private void AddNonDuplicate (List<ReachableTile> list, ReachableTile element)
	{
		foreach (ReachableTile tile in list) {
			if (tile.position == element.position) {
				if (!tile.straight && element.straight) {
					tile.straight = true;
					return;
				}
				else return;
			}
		}

		list.Add (element);
	}

	// animates and executes the move of this unit to some target position
	public void Move (Vector3 target)
	{
		if (target.x != transform.position.x && target.z != transform.position.z) { // if not a single direction
			Debug.Log ("Illegal move given to " + name);
			return;
		}
		// first create and push a ANIMATION gamestate to restrict input and whatever
		GameState gameState = new GameState (GameStateType.ANIMATION, this.gameObject);
		gameManager.Push (gameState);
		// start actual coroutine
		StartCoroutine (ExecuteMovement (target, gameState));
	}

	// This function launches and animates the projectile trajectory and impact.
	// It then applies its effect to the target position and marks its associated ANIMATION GameState as inactive
	private IEnumerator ExecuteMovement (Vector3 endTarget, GameState gameState)
	{
		Vector3 startOrigin = transform.position; // saved to move unit on unitMap in boardManager at end of animation

		int dx, dz; // used for path calculation
		if (endTarget.x == transform.position.x) dx = 0;
		else dx = endTarget.x > transform.position.x ? 1 : -1;
		if (endTarget.z == transform.position.z) dz = 0;
		else dz = endTarget.z > transform.position.z ? 1 : -1;

		// choose correct sprite (don't do anything if no movement)
		if (transform.position != endTarget) {
			if (dx > 0 || dz < 0)
				spriteRenderer.flipX = false;
			else
				spriteRenderer.flipX = true;
		}

		// while not at target position
		while (transform.position != endTarget) {
			// check height of current and next position to know whether we need to "jump"
			int currHeight = boardManager.heightMap[(int)transform.position.x, (int)transform.position.z]+1;
			int nextHeight = boardManager.heightMap[(int)transform.position.x + dx, (int)transform.position.z + dz]+1;

			// this unit is the "projectile", the script is almost identical to moving a projectile
			GameObject projectile = this.gameObject;
			// the target for this "step"
			Vector3 target = new Vector3 (transform.position.x + dx, nextHeight, transform.position.z + dz);

			if (nextHeight != currHeight) { // we need to jump
				Vector3 origin = transform.position;
				// The trajectory is split up in two phases: In one phase the projectile moves straight up or down to get to the same height as the target
				// In the second phase the projectile moves horizontally in a parabola towards the target.
				// The order of the phases depends on how the height of the origin relates to the height of the target
				bool parabolaFirst = origin.y > target.y ? true : false; // calculate which phase is first
				Vector3 switchPoint; // calculate the switching point between the two phases
				float switchPointDistance; // distance beteen switchPoint and origin/target will be used later
				if (parabolaFirst) {
					switchPoint = target;
					switchPoint.y = origin.y;
					switchPointDistance = (switchPoint - origin).magnitude;
				} else {
					switchPoint = origin;
					switchPoint.y = target.y;
					switchPointDistance = (switchPoint - target).magnitude;
				}
				while (Mathf.Abs ((projectile.transform.position - switchPoint).magnitude) > 0.1f) { // while projectile is not at switchpoint
					if (parabolaFirst) { // if the parabola is first, then move in parabola to switchpoint
						// we use the fraction of (how much the projectile has moved in the x,z plane) / (how much needs to be moved in the x,z plane)
						// to calculate the height of the parabola at that point
						Vector3 newPos = projectile.transform.position; // we calculate the new position by first taking the current position
						newPos.y = switchPoint.y; // reset y to what y would be without the added parabola height
						newPos = Vector3.MoveTowards (newPos, switchPoint, movementSpeed * 0.5f); // newPos is now the newPosition without the height of the parabola
						float progress = ((newPos - origin).magnitude) / switchPointDistance; // A value from 0-1 that represent what fraction of the parabola has been travelled
						float height = 0.5f; // this is the max height of the parabola
						newPos = newPos + Vector3.up * (-4 * Mathf.Pow (progress, 2) + 4 * progress) * height; // we now add the height of the parabola to get the real position
						projectile.transform.position = newPos;
					} else { // if ascent is fist, then move to switchpointin straight line
						projectile.transform.position = Vector3.MoveTowards (projectile.transform.position, switchPoint, movementSpeed*2);
					}
					yield return new WaitForFixedUpdate ();
				}
				while (Mathf.Abs ((projectile.transform.position - target).magnitude) > 0.1f) { // while projectile is not at target
					if (parabolaFirst) {
						// in the second phase descent towards target
						projectile.transform.position = Vector3.MoveTowards (projectile.transform.position, target, movementSpeed*2);
					} else { // after ascending, move in parabola to target
						// we use the fraction of (how much the projectile has moved in the x,z plane) / (how much needs to be moved in the x,z plane)
						// to calculate the height of the parabola at that point
						Vector3 newPos = projectile.transform.position; // we calculate the new position by first taking the current position
						newPos.y = target.y; // reset y to what y would be without the added parabola height
						newPos = Vector3.MoveTowards (newPos, target, movementSpeed * 0.5f); // newPos is now the newPosition without the height of the parabola
						float progress = ((newPos - switchPoint).magnitude) / switchPointDistance; // A value from 0-1 that represent what fraction of the parabola has been travelled
						float height = 0.5f; // this is the max height of the parabola
						newPos = newPos + Vector3.up * (-4 * Mathf.Pow (progress, 2) + 4 * progress) * height; // we now add the height of the parabola to get the real position
						projectile.transform.position = newPos;
					}
					yield return new WaitForFixedUpdate ();
				}
			} else { // we do not need to jump, just move in a flat plane
				while (transform.position != target) {
					projectile.transform.position = Vector3.MoveTowards (projectile.transform.position, target, movementSpeed);
					yield return new WaitForFixedUpdate ();
				}
			}
			transform.position = target; // make sure that the unit is exactly at target position
		}
		// let sprite face camera after movements
		spriteRenderer.sprite = sprites [0];
		// modify unitMap to reflect new situation
		boardManager.unitMap[(int)startOrigin.x,(int)startOrigin.z] = null;
		boardManager.unitMap[(int)endTarget.x,(int)endTarget.z] = this;
		// signal that animation is done to GameManager:
		gameState.active = false;
	}

	private List<ReachableTile> possibleMoveList = new List<ReachableTile> ();

	// Returns all the possible locations the unit can move to with the current move stats
	// uses the recursive method
	public List<ReachableTile> GetPossibleMoves ()
	{
		possibleMoveList.Clear ();

		if (!canMove)
			return possibleMoveList;

		PositionSearch (transform.position-Vector3.up, currentMoves, currentMovesUp, currentMovesDown, currentMovesSide, true, Direction.NONE);

		return possibleMoveList;
	}

	void InitiateTargetSearch(Vector3 targetPosition, Vector3 position, int currentMoves, int currentMovesUp, int currentMovesDown, int currentMovesSide, bool straight, Direction direction) {
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
		PositionSearch (targetPosition, currentMoves, currentMovesUp, currentMovesDown, currentMovesSide, straight, direction);
	}

	void PositionSearch (Vector3 position, int currentMoves, int currentMovesUp, int currentMovesDown, int currentMovesSide, bool straight, Direction direction) {
		if (currentMoves < 0 || currentMovesUp < 0 || currentMovesDown < 0 || currentMovesSide < 0
			|| (boardManager.unitMap[(int)position.x, (int)position.z] != null && boardManager.unitMap[(int)position.x, (int)position.z].isFriendly() != isFriendly())) {
			return;
		}
		// this is a legitimate movement
		AddNonDuplicate(possibleMoveList, new ReachableTile(position+Vector3.up, straight, currentMoves, currentMovesUp, currentMovesDown, currentMovesSide));
		if (position.x > 0) {
			Vector3 targetPosition = new Vector3 (position.x-1, boardManager.heightMap[(int)position.x-1, (int)position.z], position.z);
			InitiateTargetSearch (targetPosition, position, currentMoves, currentMovesUp, currentMovesDown, currentMovesSide, direction == Direction.Z ? false : true, Direction.X);
		}
		if (position.x < boardManager.dimensions.x - 1) {
			Vector3 targetPosition = new Vector3 (position.x+1, boardManager.heightMap[(int)position.x+1, (int)position.z], position.z);
			InitiateTargetSearch (targetPosition, position, currentMoves, currentMovesUp, currentMovesDown, currentMovesSide, direction == Direction.Z ? false : true, Direction.X);
		}
		if (position.z > 0) {
			Vector3 targetPosition = new Vector3 (position.x, boardManager.heightMap[(int)position.x, (int)position.z-1], position.z-1);
			InitiateTargetSearch (targetPosition, position, currentMoves, currentMovesUp, currentMovesDown, currentMovesSide, direction == Direction.X ? false : true, Direction.Z);
		}
		if (position.z < boardManager.dimensions.z - 1) {
			Vector3 targetPosition = new Vector3 (position.x, boardManager.heightMap[(int)position.x, (int)position.z+1], position.z+1);
			InitiateTargetSearch (targetPosition, position, currentMoves, currentMovesUp, currentMovesDown, currentMovesSide, direction == Direction.X ? false : true, Direction.Z);
		}
	}
}

