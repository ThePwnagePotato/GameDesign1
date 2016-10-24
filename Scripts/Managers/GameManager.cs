using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// StateType describes what kind of state the game is in
public enum GameStateType
{
	ROOT,				// The lowest GameState on the stack is always ROOT

	PLAYERTURN,			// select unit
	SELECTEDUNIT,		// can move, select ability
	SELECTEDABILITY,	// select target
	ANIMATION, 			// animation

	ENEMYTURN,
	ENEMYMOVING,
	ENEMYWAITFORATTACK,


	DIALOGUE,
	ESCMENU

	// Stats view, skill view
}


/* the GameState class contains information about its type, its evoker and whether it is still active
 * The inclusion of the "active" boolean gives the evoker the possiblity to set a flag for when this gamestate should be removed
 * e.g.: Suppose CASTABILITY is pushed, but then ESCMENU is pushed before CASTABILITY is finished.
 * When CASTABILITY finishes, its evoker might then try to pop it off the GameStack, but would pop ESCMENU instead. This is undesirable.
 * Instead, CASTABILITY's evoker sets "active" to "false" and the GameManager takes responsibility for popping it off the stack once it's on top again.
 */
public class GameState {
	public GameStateType type;
	public GameObject evoker;
	public bool active;

	public GameState(GameStateType type) {
		this.type = type;
		active = true;
	}

	public GameState(GameStateType type, GameObject evoker) {
		this.type = type;
		this.evoker = evoker;
		active = true;
	}
}

public class GameManager : MonoBehaviour
{
	//handles game logic, order, turns etc.
	//gives info to other classes

	[Header("Dependencies")]
	public GameObject highlighter;
	public BoardManager boardManager;

	public GameObject selectedUIHolder;
	public SelectedUI selectedUI;
	public GameObject playerTurnUIHolder;
	public FriendlyUnitsUI friendlyUnitsUI;

	private List<GameObject> spawnedObjects;
	public Stack<GameState> gameStack;

	void Start ()
	{
		gameStack = new Stack<GameState>();
		spawnedObjects = new List<GameObject>();
		Push (new GameState(GameStateType.ROOT));
		Push (new GameState(GameStateType.PLAYERTURN));
	}

	void Update ()
	{
		// if top gamestate is done, pop it
 		if (gameStack.Count > 0 && !gameStack.Peek ().active)
			Pop();
	}

	public void Push (GameState gameState) {
		switch (gameState.type) {
		case GameStateType.ROOT:
			gameStack.Push (gameState);
			break;
		case GameStateType.PLAYERTURN:
			Debug.Log ("TURN: Player turn started");
			playerTurnUIHolder.SetActive (true);
			foreach (Unit unit in boardManager.friendlyUnits) {
				if (unit.isAlive)
					unit.ResetTurn ();
			}
			friendlyUnitsUI.updateValues ();
			gameStack.Push (gameState);
			break;
		case GameStateType.ENEMYTURN:
			gameStack.Push (gameState);
			if (boardManager.enemyUnits.Count == 0) {
			} else {
				Debug.Log ("TURN: Enemy turn started");
				foreach (Unit unit in boardManager.enemyUnits) {
					EnemyUnit enemyUnit = (EnemyUnit)unit;
					if (enemyUnit.isAlive) {
						enemyUnit.ResetTurn ();
						enemyUnit.DoTurn ();
					}
				}
			}
			gameState.active = false;
			break;

		case GameStateType.SELECTEDUNIT:
			playerTurnUIHolder.SetActive (false);
			gameState.evoker.GetComponent<SpriteRenderer> ().color = new Color (1, 1, 0, 1); // make sprite yellow
			// activate unit info (selected UI)
			selectedUIHolder.SetActive (true);
			selectedUI.updateValues (gameState.evoker.GetComponent<Unit>());
			List<ReachableTile> traversable = gameState.evoker.GetComponent<Unit> ().GetPossibleMoves ();
			foreach (ReachableTile tile in traversable) {
				GameObject newHighlight = Instantiate (highlighter, tile.position, Quaternion.identity) as GameObject;
				if (tile.straight) {
					newHighlight.GetComponentInChildren<SpriteRenderer> ().color = new Color (1, 1, 0, 1);
				}
				spawnedObjects.Add (newHighlight);
			}
			gameStack.Push (gameState);
			break;
		case GameStateType.SELECTEDABILITY:
			// first wipe the tiles that show possible movement
			foreach (GameObject spawned in spawnedObjects) {
				DestroyObject (spawned);
			}
			spawnedObjects.Clear ();
			// now add tiles that show possible targets
			List<Vector3> targets = gameState.evoker.GetComponent<Ability> ().getPossibleTargets ();
			foreach (Vector3 target in targets) {
				GameObject newHighlight = Instantiate (highlighter, target, Quaternion.identity) as GameObject;
				newHighlight.GetComponentInChildren<SpriteRenderer> ().color = new Color (1, 0, 0, 1);
				spawnedObjects.Add (newHighlight);
			}
			gameStack.Push (gameState);
			break;
		default:
			break;
		}
	}

	public void Pop () {
		if (gameStack.Count == 0)
			return;
		GameState gameState = gameStack.Peek ();
		switch (gameState.type) {
		case GameStateType.ROOT:
			Debug.Log ("ERROR: ROOT popped!");
			break;
		case GameStateType.ANIMATION:
			gameStack.Pop ();
			break;
		case GameStateType.PLAYERTURN:
			Debug.Log ("TURN: Player turn ended");
			friendlyUnitsUI.Clear ();
			playerTurnUIHolder.SetActive (false);
			foreach (Unit unit in boardManager.friendlyUnits) {
				if(unit.isAlive) unit.EndTurn();
			}
			gameStack.Pop ();
			break;
		case GameStateType.ENEMYTURN:
			Debug.Log ("TURN: Enemy turn ended");
			foreach (Unit unit in boardManager.enemyUnits) {
				if (unit.isAlive)
					unit.EndTurn ();
			}
			gameStack.Pop ();
			GameState newState = new GameState (GameStateType.PLAYERTURN);
			Push (newState);
			break;
		case GameStateType.SELECTEDUNIT:
			gameState.evoker.GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 1); // "unselectify" - make sprite regular color
			selectedUI.Clear ();
			selectedUIHolder.SetActive (false); // disable "selected UI" 
			ResetSpawnedObjects ();
			playerTurnUIHolder.SetActive (true);
			gameStack.Pop ();
			break;
		case GameStateType.SELECTEDABILITY:
			ResetSpawnedObjects ();
			gameStack.Pop ();
			break;
		default:
			break;
		}
	}

	// End player turn and start enemy turn
	public void EndPlayerTurn () {
		while (gameStack.Peek ().type != GameStateType.ROOT)
			Pop ();
		GameState newState = new GameState (GameStateType.ENEMYTURN);
		Push (newState);
	}

	void ResetSpawnedObjects () {
		foreach (GameObject spawned in spawnedObjects) {
			DestroyObject (spawned);
		}
		spawnedObjects.Clear ();
	}
}