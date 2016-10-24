using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class MapFriendlyUnitsUI : MonoBehaviour {

	public GameObject unitHolder;
	public GameObject smallStatWindow;
	public float unitHolderYOffset;
	public MapBoardManager boardManager;

	private List<GameObject> unitList;

	public void updateValues() {
		if (unitList == null) unitList = new List<GameObject> ();
		for (int i = 0; i < boardManager.friendlyUnits.Count; i++) {
			GameObject newWindow = Instantiate (smallStatWindow) as GameObject;
			newWindow.transform.SetParent (unitHolder.transform, false);
			newWindow.transform.position += i*unitHolderYOffset*Vector3.up;
			newWindow.GetComponent<MapSmallStatUI> ().UpdateValues (boardManager.friendlyUnits[i]); // to do
			unitList.Add (newWindow);
		}
	}

	public void Clear () {
		foreach (GameObject unitHolder in unitList) {
			Destroy (unitHolder);
		}
		unitList.Clear ();
	}
}