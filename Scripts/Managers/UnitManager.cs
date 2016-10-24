using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

// StateType describes what kind of state the game is in
public enum MapStateType
{
	ROOT,				// The lowest MapState on the stack is always ROOT

	PLAYERTURN,			// select unit
	SELECTEDUNIT,		// can move, select ability
	ABILITYCONFIRM,
	SAVESELECT,
	OVERWRITECONFIRM,
	ESCMENU

	// Stats view, skill view
}


/* the MapState class contains information about its type, its evoker and whether it is still active
 * The inclusion of the "active" boolean gives the evoker the possiblity to set a flag for when this gamestate should be removed
 * e.g.: Suppose CASTABILITY is pushed, but then ESCMENU is pushed before CASTABILITY is finished.
 * When CASTABILITY finishes, its evoker might then try to pop it off the GameStack, but would pop ESCMENU instead. This is undesirable.
 * Instead, CASTABILITY's evoker sets "active" to "false" and the GameManager takes responsibility for popping it off the stack once it's on top again.
 */
public class MapState {
	public MapStateType type;
	public GameObject evoker;
	public bool active;

	public MapState(MapStateType type) {
		this.type = type;
		active = true;
	}

	public MapState(MapStateType type, GameObject evoker) {
		this.type = type;
		this.evoker = evoker;
		active = true;
	}
}

public class UnitManager : MonoBehaviour
{
	//handles game logic, order, turns etc.
	//gives info to other classes

	[Header("Dependencies")]

	public GameObject selectedUIHolder;
	public MapSelectedUI selectedUI;
	public GameObject playerTurnUIHolder;
	public MapFriendlyUnitsUI friendlyUnitsUI;
	public MapBoardManager mapBoardManager;
	public Stack<MapState> gameStack;

	void Start ()
	{
		gameStack = new Stack<MapState>();
		Push (new MapState(MapStateType.ROOT));
		Push (new MapState(MapStateType.PLAYERTURN));
	}

	void Update ()
	{
		// if top gamestate is done, pop it
		if (gameStack.Count > 0 && !gameStack.Peek ().active)
			Pop();
	}


	public GameObject abilityConfirmHolder;
	public Text abilityConfirmText;
	int[] characterStats;
	int characterIndex;
	public void InitiateConfirmAbilityLearn (int[] characterStats, int characterIndex, string unitName, string abilityName) {
		if(gameStack.Peek().type == MapStateType.SELECTEDUNIT) {
		this.characterIndex = characterIndex;
		this.characterStats = characterStats;
		abilityConfirmHolder.SetActive (true);
		abilityConfirmText.text = "Consume one skill point to teach\n" + abilityName + " to " + unitName + "?";
		Push (new MapState(MapStateType.ABILITYCONFIRM));
		}
	}
	public void ConfirmAbilityLearn (bool confirm) {
		if (confirm) {
			characterStats [characterIndex]++;
		}
		Pop ();
	}

	public GameObject saveSlotHolder;
	public void InitiateSaveSlots () {
			saveSlotHolder.SetActive (true);
		Push (new MapState(MapStateType.SAVESELECT));
	}

	public GameObject overwriteConfirmHolder;
	public Text overwriteConfirmText;
	int saveSlotIndex;
	public void InitiateConfirmOverwrite (int saveSlotIndex) {
		if(gameStack.Peek().type == MapStateType.SAVESELECT) {
			this.saveSlotIndex = saveSlotIndex;
			overwriteConfirmHolder.SetActive (true);
			overwriteConfirmText.text = "Overwrite save slot " + (saveSlotIndex+1).ToString() + "?";
			Push (new MapState(MapStateType.OVERWRITECONFIRM));
		}
	}
	public void ConfirmOverwrite (bool confirm) {
		if (confirm) {
			SaveData.saveData.SaveToInstance(saveSlotIndex);
			SaveData.saveData.SaveToDisk ();
		}
		Pop ();
	}

	public void Push (MapState mapState) {
		switch (mapState.type) {
		case MapStateType.ROOT:
			gameStack.Push (mapState);
			break;
		case MapStateType.ABILITYCONFIRM:
			gameStack.Push (mapState);
			break;
		case MapStateType.SAVESELECT:
			while (gameStack.Peek ().type != MapStateType.PLAYERTURN) {
				Pop ();
			}
			gameStack.Push (mapState);
			break;
		case MapStateType.OVERWRITECONFIRM:
			while (gameStack.Peek ().type != MapStateType.PLAYERTURN) {
				Pop ();
			}
			gameStack.Push (mapState);
			break;
		case MapStateType.PLAYERTURN:
			Debug.Log ("TURN: Player turn started");
			playerTurnUIHolder.SetActive (true);
			friendlyUnitsUI.updateValues ();
			gameStack.Push (mapState);
			break;

		case MapStateType.SELECTEDUNIT:
			playerTurnUIHolder.SetActive (false);
			// activate unit info (selected UI)
			selectedUIHolder.SetActive (true);
			selectedUI.updateValues (mapState.evoker.GetComponent<Unit>());
			gameStack.Push (mapState);
			break;
		default:
			break;
		}
	}

	public void Pop () {
		if (gameStack.Count == 0)
			return;
		MapState mapState = gameStack.Peek ();
		switch (mapState.type) {
		case MapStateType.ROOT:
			Debug.Log ("ERROR: ROOT popped!");
			break;
		case MapStateType.SAVESELECT:
			saveSlotHolder.SetActive (false);
			gameStack.Pop ();
			break;
		case MapStateType.OVERWRITECONFIRM:
			overwriteConfirmHolder.SetActive (false);
			gameStack.Pop ();
			break;
		case MapStateType.ABILITYCONFIRM:
			abilityConfirmHolder.SetActive (false);
			gameStack.Pop();
			break;
		case MapStateType.PLAYERTURN:
			Debug.Log ("TURN: Player turn ended");
			friendlyUnitsUI.Clear ();
			playerTurnUIHolder.SetActive (false);
			gameStack.Pop ();
			break;
		case MapStateType.SELECTEDUNIT:
			selectedUI.Clear ();
			selectedUIHolder.SetActive (false); // disable "selected UI" 
			playerTurnUIHolder.SetActive (true);
			gameStack.Pop ();
			break;
		default:
			break;
		}
	}
}