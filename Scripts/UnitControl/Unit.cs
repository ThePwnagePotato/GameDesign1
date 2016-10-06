using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Unit : MonoBehaviour
{

	public abstract string getName ();

	public abstract bool isFriendly ();

	public abstract bool isAlive { get; set; }

	public abstract GameObject[] abilities { get; set; }

	public abstract List<StatusEffect> statusEffects ();

	public abstract int maxHealth { get; set; }

	public abstract int currentHealth { get; set; }

	public abstract int power { get; set; }

	public abstract int defense { get; set; }

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

		for (int i = 0; i < abilities.Length; i++) {
			Instantiate (abilities[i], this.transform);
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
		int tempDef = defense;

		// Go through the list of statuseffects, get the ones that affect the DEF stat
		// then apply that effect to the tempDef value
		foreach (StatusEffect effect in statusEffects ()) {
			if (effect.GetEffectType () == EffectType.DEFBUFF) {
				tempDef += effect.power;
			} else if (effect.GetEffectType () == EffectType.DEFDEBUFF) {
				tempDef -= effect.power;
			}
		}

		// calculate the final damage using the tempDef value
		int finalDamage = System.Math.Max (damage - tempDef, 1);

		//check for status effects

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


	private enum Direction
	{
		NE,
		SE,
		SW,
		NW,
		NONE

	}

	public List<Vector3> possibleMoveList = new List<Vector3> ();
	private int[,] heightMap;

	// Returns all the possible locations the unit can move to with the current move stats
	// uses the recursive method
	public List<Vector3> UpdatePossibleMoves (int[,] heightMap)
	{
		possibleMoveList.Clear ();

		this.heightMap = heightMap;

		PositionSearch (transform.position, currentMoves, currentMovesUp, currentMovesDown, currentMovesSide, Direction.NONE);

		return possibleMoveList;
	}



	// checks all the possible moves from a position, then recursively searches using that next position and moves left.
	private void PositionSearch (Vector3 currentPosition, int possibleMoves, int possibleMovesUp, int possibleMovesDown, int possibleMovesSide, Direction direction)
	{
		int currentX = (int)currentPosition.x;
		int currentY = (int)currentPosition.y;
		int currentZ = (int)currentPosition.z;

		if (direction != Direction.SW && (currentX > 0)) {
			// x-, , only up

			// get the height of the block in this direction
			int newHeight = heightMap [currentX - 1, currentZ];

			// if there is no block there, skip everything
			if (newHeight > 0) {

				// check how much the height difference is.
				int heightDifference = newHeight - currentY;

				//if that difference is positive, check the possibleMovesUp
				if (heightDifference > 0 && possibleMovesUp >= heightDifference) {
					// if so, this block can be moved to
					// make the new position vector and add that to the possibleMoveList
					Vector3 newPosition = new Vector3 (currentX - 1, newHeight, currentZ);
					AddNonDuplicate (possibleMoveList, newPosition);

					//if there is a possible move after this, search again
					if (possibleMoves - heightDifference > 0) {
						PositionSearch (newPosition, possibleMoves - 1, possibleMovesUp - heightDifference, possibleMovesDown, possibleMovesSide, Direction.NE);
					}
				} 
				// if the difference is 0 (no vertical movement), check possibleMovesSide
				else if (heightDifference == 0 && possibleMovesSide > 0) {
					// if so, this block can be moved to
					// make the new position vector and add that to the possibleMoveList
					Vector3 newPosition = new Vector3 (currentX - 1, newHeight, currentZ);
					AddNonDuplicate (possibleMoveList, newPosition);

					//if there is a possible move after this, search again
					if (possibleMoves > 1) {
						PositionSearch (newPosition, possibleMoves - 1, possibleMovesUp, possibleMovesDown, possibleMovesSide - 1, Direction.NE);
					}
				}
			}
			/*
			 * not possible to go down in this direction
			 
			// if the difference is negative, check possibleMovesDown
			else if (heightDifference < 0 && possibleMovesDown >= -heightDifference) {
				// if so, this block can be moved to
				// make the new position vector and add that to the possibleMoveList
				Vector3 newPosition = new Vector3 (currentX - 1, newHeight, currentZ);
				possibleMoveList.Add (newPosition);

				//if there is a possible move after this, search again
				if (possibleMoves > 1) {
					PositionSearch (newPosition, possibleMoves - 1, possibleMovesUp, possibleMovesDown - -heightDifference, possibleMovesSide, Direction.NE);
				}
			}
			*/

		} 
		if (direction != Direction.NW && (currentZ < boardManager.dimensions.z - 1)) {
			// z+, only down

			int newHeight = heightMap [currentX, currentZ + 1];
			if (newHeight > 0) {
				int heightDifference = newHeight - currentY;
				if (heightDifference < 0 && possibleMovesDown >= -heightDifference) {
					Vector3 newPosition = new Vector3 (currentX, newHeight, currentZ + 1);
					AddNonDuplicate (possibleMoveList, newPosition);
					if (possibleMoves - -heightDifference > 0) {
						PositionSearch (newPosition, possibleMoves - -heightDifference, possibleMovesUp, possibleMovesDown - -heightDifference, possibleMovesSide, Direction.SE);
					}
				} else if (heightDifference == 0 && possibleMovesSide > 0) {
					Vector3 newPosition = new Vector3 (currentX, newHeight, currentZ + 1);
					AddNonDuplicate (possibleMoveList, newPosition);
					if (possibleMoves > 1) {
						PositionSearch (newPosition, possibleMoves - 1, possibleMovesUp, possibleMovesDown, possibleMovesSide - 1, Direction.SE);
					}
				}
			}
		} 
		if (direction != Direction.NE && (currentX < boardManager.dimensions.x - 1)) {
			// x+, , only down

			int newHeight = heightMap [currentX + 1, currentZ];
			if (newHeight > 0) {
				int heightDifference = newHeight - currentY;
				if (heightDifference < 0 && possibleMovesDown >= -heightDifference) {
					Vector3 newPosition = new Vector3 (currentX + 1, newHeight, currentZ);
					AddNonDuplicate (possibleMoveList, newPosition);
					if (possibleMoves - -heightDifference > 0) {
						PositionSearch (newPosition, possibleMoves - -heightDifference, possibleMovesUp, possibleMovesDown - -heightDifference, possibleMovesSide, Direction.SW);
					}
				} else if (heightDifference == 0 && possibleMovesSide > 0) {
					Vector3 newPosition = new Vector3 (currentX + 1, newHeight, currentZ);
					AddNonDuplicate (possibleMoveList, newPosition);
					if (possibleMoves > 1) {
						PositionSearch (newPosition, possibleMoves - 1, possibleMovesUp, possibleMovesDown, possibleMovesSide - 1, Direction.SW);
					}
				} 
			}
		} 
		if (direction != Direction.SE && (currentZ > 0)) {
			// z-, only up

			int newHeight = heightMap [currentX, currentZ - 1];
			if (newHeight > 0) {
				int heightDifference = newHeight - currentY;
				if (heightDifference > 0 && possibleMovesUp >= heightDifference) {
					Vector3 newPosition = new Vector3 (currentX, newHeight, currentZ - 1);
					AddNonDuplicate (possibleMoveList, newPosition);
					if (possibleMoves - heightDifference > 0) {
						PositionSearch (newPosition, possibleMoves - heightDifference, possibleMovesUp - heightDifference, possibleMovesDown, possibleMovesSide, Direction.NW);
					}
				} else if (heightDifference == 0 && possibleMovesSide > 0) {
					Vector3 newPosition = new Vector3 (currentX, newHeight, currentZ - 1);
					AddNonDuplicate (possibleMoveList, newPosition);
					if (possibleMoves > 1) {
						PositionSearch (newPosition, possibleMoves - 1, possibleMovesUp, possibleMovesDown, possibleMovesSide - 1, Direction.NW);
					}
				} 
			}
		}
	}

	// loop through all the elements of the list
	// if a vector equal to the new vector is found, return
	// if none is found, add the new vector to the list
	private void AddNonDuplicate (List<Vector3> list, Vector3 element)
	{
		foreach (Vector3 vector in list) {
			if (vector == element) {
				return;
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

		// choose correct sprite
		if (dz + dx > 0) spriteRenderer.sprite = sprites[0];
		else spriteRenderer.sprite = sprites[1];
		if (dx > 0 || dz < 0) spriteRenderer.flipX = false;
		else spriteRenderer.flipX = true;

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
	/*
	private IEnumerator ExecuteMovement (Vector3 endTarget, GameState gameState)
	{
		Vector3 startOrigin = transform.position; // saved to move unit on unitMap in boardManager at end of animation

		int dx, dz; // used for path calculation
		if (endTarget.x == transform.position.x) dx = 0;
		else dx = endTarget.x > transform.position.x ? 1 : -1;
		if (endTarget.z == transform.position.z) dz = 0;
		else dz = endTarget.z > transform.position.z ? 1 : -1;

		// choose correct sprite
		if (dz + dx > 0) spriteRenderer.sprite = sprites[0];
		else spriteRenderer.sprite = sprites[1];
		if (dx < 0 || dz > 0) spriteRenderer.flipX = false;
		else spriteRenderer.flipX = true;

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
		// modify unitMap to reflect new situation
		boardManager.unitMap[(int)startOrigin.x,(int)startOrigin.z] = null;
		boardManager.unitMap[(int)endTarget.x,(int)endTarget.z] = this;
		// signal that animation is done to GameManager:
		gameState.active = false;
	}
	*/
}

