using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapScreenController : MonoBehaviour
{
	[Header ("Dependencies")]
	public UnitManager unitManager;

	void Update ()
	{
		// if player clicks right mouse button
		if ((Input.GetMouseButtonDown (1)  || Input.GetKeyDown("left shift"))
			&& unitManager.gameStack.Peek ().type != MapStateType.PLAYERTURN
			&& unitManager.gameStack.Peek ().type != MapStateType.ROOT) {
			unitManager.Pop ();
		}
	}
}
