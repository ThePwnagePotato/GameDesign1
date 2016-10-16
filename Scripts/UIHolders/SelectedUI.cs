using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class SelectedUI : MonoBehaviour {

	public GameObject abilityHolder;
	public GameObject abilityWindow;
	public float abilityHolderYOffset;

	public Text hpText;
	public Text powText;
	public Text defText;
	public Text totalMoveText;
	public Text totalUpMoveText;
	public Text totalDownMoveText;
	public Text totalSideMoveText;
	public Text moveText;
	public Text upMoveText;
	public Text downMoveText;
	public Text sideMoveText;

	private List<GameObject> abilityList;

	public void updateValues(Unit selected) {
		hpText.text = selected.maxHealth.ToString();
		powText.text = selected.power.ToString();
		defText.text = selected.defense.ToString();
		totalMoveText.text = selected.totalMoves.ToString();
		totalUpMoveText.text = selected.totalMovesUp.ToString();
		totalDownMoveText.text = selected.totalMovesDown.ToString();
		totalSideMoveText.text = selected.totalMovesSide.ToString();
		moveText.text = selected.currentMoves.ToString();
		upMoveText.text = selected.currentMovesUp.ToString();
		downMoveText.text = selected.currentMovesDown.ToString();
		sideMoveText.text = selected.currentMovesSide.ToString();

		if (abilityList == null) abilityList = new List<GameObject> ();
		Ability[] abilitiesOnUnit = selected.GetComponentsInChildren<Ability> ();

		for (int i = 0; i < abilitiesOnUnit.Length; i++) {
			GameObject newAbility = Instantiate (abilityWindow) as GameObject;
			newAbility.transform.SetParent (abilityHolder.transform, false);
			newAbility.transform.position += i*abilityHolderYOffset*Vector3.up;
			newAbility.GetComponent<AbilityUI> ().UpdateValues (abilitiesOnUnit[i], selected);
			abilityList.Add (newAbility);
		}
	}

	public void Clear () {
		foreach (GameObject abilityHolder in abilityList) {
			Destroy (abilityHolder);
		}
		abilityList.Clear ();
	}
}
