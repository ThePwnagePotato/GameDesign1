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

	public abstract Animator animator { get; set; } 

	public abstract Sprite[] sprites { get; set; }

	public abstract BoardManager boardManager { get; set; }

	public abstract GameManager gameManager { get; set; }

	public bool finishedAbility = false;

	public void Awake ()
	{
		currentHealth = maxHealth;
		isAlive = true;
		canMove = true;
		canAttack = true;
		// connect dependencies
		boardManager = GameObject.FindGameObjectWithTag ("BoardManager").GetComponent<BoardManager> ();
		if (boardManager == null)
			Debug.Log ("Unit: BoardManager not found)");
		gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
		if (gameManager == null)
			Debug.Log ("Unit: GameManager not found)");
		animator = GetComponentInChildren<Animator> ();
		animator.ChangeAnimation (AnimationState.IDLE);

		// setting initial state to being alive
		isAlive = true;
	}

	public void Start () {
		isAlive = true;
		currentHealth = maxHealth;
		currentDefense = defense;
		currentPower = power;
		currentMoves = totalMoves;
		currentMovesDown = totalMovesDown;
		currentMovesSide = totalMovesSide;
		currentMovesUp = totalMovesUp;
		canMove = true;
		canAttack = true;
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
		foreach (StatusEffect effect in GetComponentsInChildren<StatusEffect> ()) {
			effect.OnTakeDamage ();
		}

		// calculate the final damage using the tempDef value
		int finalDamage = Mathf.Max (damage - currentDefense, 1);

		DamageDisplayer dd = GetComponentInChildren<DamageDisplayer> ();
		dd.ShowRegularDamage (finalDamage);

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
		isAlive = false;
		animator.DeathAnimation ();
		//remove from unitmap
		boardManager.unitMap[(int)transform.position.x, (int)transform.position.z] = null;
		//remove from friendly/enemy list
		if (isFriendly ()) {
			boardManager.friendlyUnits.Remove (this);
		} else {
			boardManager.enemyUnits.Remove (this);
		}
		GetComponentInChildren<Collider> ().enabled = false;
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

		foreach (StatusEffect effect in GetComponentsInChildren<StatusEffect> ()) {
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
		foreach (Ability ability in GetComponentsInChildren<Ability> ()) {
			if (ability.cooldown < 0) {
				ability.cooldown = ability.maxCooldown ();
			} else if (ability.cooldown > 0) {
				ability.cooldown--;
			}
		}
			
		StatusEffect[] effectList = GetComponentsInChildren<StatusEffect> ();
		for (int i = effectList.Length - 1; i >= 0; i--) {
			StatusEffect effect = effectList[i];

			effect.OnTurnEnd ();
					
			if (effect.duration <= 0) {
				effect.OnRemoval ();
				Destroy (effect.gameObject);
			}
		}
	}
		
	Vector3 movingPosition;
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
	protected IEnumerator ExecuteMovement (Vector3 endTarget, GameState gameState)
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
				animator.FlipVertical(false);
			else
				animator.FlipVertical(true);
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
				movingPosition = projectile.transform.position;
				while (Mathf.Abs ((projectile.transform.position - switchPoint).magnitude) >= 0.1f) { // while projectile is not at switchpoint
					if (parabolaFirst) { // if the parabola is first, then move in parabola to switchpoint
						projectile.transform.position = movingPosition;
						// we use the fraction of (how much the projectile has moved in the x,z plane) / (how much needs to be moved in the x,z plane)
						// to calculate the height of the parabola at that point
						Vector3 newPos = projectile.transform.position; // we calculate the new position by first taking the current position
						newPos.y = switchPoint.y; // reset y to what y would be without the added parabola height
						newPos = Vector3.MoveTowards (newPos, switchPoint, movementSpeed * 0.5f); // newPos is now the newPosition without the height of the parabola
						float progress = ((newPos - origin).magnitude) / switchPointDistance; // A value from 0-1 that represent what fraction of the parabola has been travelled
						float height = 0.5f; // this is the max height of the parabola
						newPos = newPos + Vector3.up * (-4 * Mathf.Pow (progress, 2) + 4 * progress) * height; // we now add the height of the parabola to get the real position
						movingPosition = newPos;
					} else { // if ascent is fist, then move to switchpointin straight line
						projectile.transform.position = movingPosition;
						movingPosition = Vector3.MoveTowards (projectile.transform.position, switchPoint, movementSpeed*2);
					}
					yield return new WaitForFixedUpdate ();
				}
				movingPosition = projectile.transform.position;
				while (Mathf.Abs ((projectile.transform.position - target).magnitude) >= 0.1f) { // while projectile is not at target
					if (parabolaFirst) {
						projectile.transform.position = movingPosition;
						// in the second phase descent towards target
						movingPosition = Vector3.MoveTowards (projectile.transform.position, target, movementSpeed*2);
					} else { // after ascending, move in parabola to target
						projectile.transform.position = movingPosition;
						// we use the fraction of (how much the projectile has moved in the x,z plane) / (how much needs to be moved in the x,z plane)
						// to calculate the height of the parabola at that point
						Vector3 newPos = projectile.transform.position; // we calculate the new position by first taking the current position
						newPos.y = target.y; // reset y to what y would be without the added parabola height
						newPos = Vector3.MoveTowards (newPos, target, movementSpeed * 0.5f); // newPos is now the newPosition without the height of the parabola
						float progress = ((newPos - switchPoint).magnitude) / switchPointDistance; // A value from 0-1 that represent what fraction of the parabola has been travelled
						float height = 0.5f; // this is the max height of the parabola
						newPos = newPos + Vector3.up * (-4 * Mathf.Pow (progress, 2) + 4 * progress) * height; // we now add the height of the parabola to get the real position
						movingPosition = newPos;
					}
					yield return new WaitForFixedUpdate ();
				}
			} else { // we do not need to jump, just move in a flat plane
				movingPosition = projectile.transform.position;
				while (transform.position != target) {
					projectile.transform.position = movingPosition;
					// ?????????????????????
					Vector3 temp = Vector3.MoveTowards (projectile.transform.position, target, 0.1f);
					//projectile.transform.position = Vector3.MoveTowards (projectile.transform.position, target, 0.1f);
					movingPosition = temp;

					yield return new WaitForFixedUpdate ();
				}

			}

			transform.position = target; // make sure that the unit is exactly at target position
		}
		// let sprite face camera after movements
		// modify unitMap to reflect new situation
		boardManager.unitMap[(int)startOrigin.x,(int)startOrigin.z] = null;
		boardManager.unitMap[(int)endTarget.x,(int)endTarget.z] = this;
		// signal that animation is done to GameManager:
		gameState.active = false;

		//isMoving = false;
	}

	protected enum Direction
	{
		X,
		Z,
		NONE

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

	private void InitiateTargetSearch(Vector3 targetPosition, Vector3 position, int currentMoves, int currentMovesUp, int currentMovesDown, int currentMovesSide, bool straight, Direction direction) {
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

	private void PositionSearch (Vector3 position, int currentMoves, int currentMovesUp, int currentMovesDown, int currentMovesSide, bool straight, Direction direction) {
		Unit a = boardManager.unitMap [(int)position.x, (int)position.z];
		if (currentMoves < 0 || currentMovesUp < 0 || currentMovesDown < 0 || currentMovesSide < 0 || (int)position.y <= 0
			|| boardManager.unitMap[(int)position.x, (int)position.z] != null && boardManager.unitMap[(int)position.x, (int)position.z] != this) {
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
		
	// loop through all the elements of the list
	// if a vector equal to the new vector is found, return
	// if none is found, add the new vector to the list
	private void AddNonDuplicate (List<ReachableTile> list, ReachableTile element)
	{
		ReachableTile toDelete = null;
		foreach (ReachableTile tile in list) {
			if (tile.position == element.position) {
				if (tile.totalMove < element.totalMove) {
					toDelete = tile;
					break;
				} 
				else return;
			}
		}
		if (toDelete != null) {
			list.Remove (toDelete);
		}
		list.Add (element);
	}

	// for easy movement managing to tiles
	public void SetMoveStats (ReachableTile tile) {
		this.currentMoves = tile.totalMove;
		this.currentMovesUp = tile.upMove;
		this.currentMovesDown = tile.downMove;
		this.currentMovesSide = tile.sideMove;
	}
}

