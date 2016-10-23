using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// StateType describes what kind of state the game is in
public enum StartScreenType
{
	ROOT,
	// The lowest GameState on the stack is always ROOT

	MAINMENU,
	SAVESELECTOR,
	CONFIRM
}


/* the GameState class contains information about its type, its evoker and whether it is still active
 * The inclusion of the "active" boolean gives the evoker the possiblity to set a flag for when this gamestate should be removed
 * e.g.: Suppose CASTABILITY is pushed, but then ESCMENU is pushed before CASTABILITY is finished.
 * When CASTABILITY finishes, its evoker might then try to pop it off the GameStack, but would pop ESCMENU instead. This is undesirable.
 * Instead, CASTABILITY's evoker sets "active" to "false" and the GameManager takes responsibility for popping it off the stack once it's on top again.
 */
public class StartScreenState
{
	public StartScreenType type;
	public GameObject evoker;
	public bool active;

	public StartScreenState (StartScreenType type)
	{
		this.type = type;
		active = true;
	}

	public StartScreenState (StartScreenType type, GameObject evoker)
	{
		this.type = type;
		this.evoker = evoker;
		active = true;
	}
}

public class StartScreenManager : MonoBehaviour
{
	//handles game logic, order, turns etc.
	//gives info to other classes

	[Header ("Dependencies")]
	public GameObject mainMenuHolder;
	public GameObject saveSelectorHolder;
	public GameObject confirmHolder;
	public SceneLoader sceneLoader;
	public bool confirm;

	public Stack<StartScreenState> gameStack;

	void Start ()
	{
		gameStack = new Stack<StartScreenState> ();
		Push (new StartScreenState (StartScreenType.ROOT));
		Push (new StartScreenState (StartScreenType.MAINMENU));
		confirm = false;
	}

	void Update ()
	{
		// if top gamestate is done, pop it
		if (gameStack.Count > 0 && !gameStack.Peek ().active)
			Pop ();
	}

	public void Push (StartScreenState screenState)
	{
		switch (screenState.type) {
		case StartScreenType.MAINMENU:
			mainMenuHolder.SetActive (true);
			break;
		case StartScreenType.SAVESELECTOR:
			saveSelectorHolder.SetActive (true);
			break;
		case StartScreenType.CONFIRM:
			confirm = false;
			confirmHolder.SetActive (true);
			break;
		default:
			break;
		}
		gameStack.Push (screenState);
	}

	public void Pop ()
	{
		if (gameStack.Count == 0)
			return;
		StartScreenState gameState = gameStack.Peek ();
		switch (gameState.type) {
		case StartScreenType.MAINMENU:
			mainMenuHolder.SetActive (false);
			gameStack.Pop ();
			break;
		case StartScreenType.SAVESELECTOR:
			saveSelectorHolder.SetActive (false);
			gameStack.Pop ();
			gameStack.Push (new StartScreenState(StartScreenType.MAINMENU));
			break;
		case StartScreenType.CONFIRM:
			confirmHolder.SetActive (false);
			gameStack.Pop ();
			if (!confirm) {
				if (gameStack.Peek ().type == StartScreenType.MAINMENU)
					mainMenuHolder.SetActive (true);
				return;
			} else if (gameStack.Peek ().type == StartScreenType.MAINMENU)
				sceneLoader.Exit ();
			confirm = false;
			break;
		default:
			break;
		}
	}

	public void enterSaveSelect() {
		Pop ();
		StartScreenState newState = new StartScreenState (StartScreenType.SAVESELECTOR);
		Push (newState);
	}

	public void initiateConfirm () {
		mainMenuHolder.SetActive (false);
		StartScreenState newState = new StartScreenState (StartScreenType.CONFIRM);
		Push (newState);
	}

	public void setConfirm (bool confirmed) {
		confirm = confirmed;
		Pop ();
	}
}