using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StartScreenController : MonoBehaviour
{
	[Header ("Dependencies")]
	public StartScreenManager startScreenManager;

	void Update ()
	{
		// if player clicks right mouse button
		if ((Input.GetMouseButtonDown (1)  || Input.GetKeyDown("left shift"))
			&& startScreenManager.gameStack.Peek ().type != StartScreenType.MAINMENU
			&& startScreenManager.gameStack.Peek ().type != StartScreenType.ROOT) {
			startScreenManager.Pop ();
			switch(startScreenManager.gameStack.Peek ().type) {
			case StartScreenType.MAINMENU:
				startScreenManager.mainMenuHolder.SetActive (true);
				break;
			case StartScreenType.SAVESELECTOR:
				startScreenManager.saveSelectorHolder.SetActive (true);
				break;
			case StartScreenType.CONFIRM:
				startScreenManager.confirmHolder.SetActive (true);
				break;
			}
		}
	}
}
