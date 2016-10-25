using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class SelectedUI : MonoBehaviour {

	public GameObject abilityHolder;
	public GameObject abilityWindow;
	public float abilityHolderYOffset;
	public Sprite buttonSprite;
	public Sprite windowSprite;

	public Text hpText;
	public Text maxHpText;
	public Text nameText;
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
	public Text MOVText;
	public Text ACTText;
	public Slider hpSlider;

	private Unit selectedUnit;
	public Unit getUnit () { return selectedUnit; }

	private List<GameObject> abilityList;

	public void updateValues(Unit selected) {
		selectedUnit = selected;
		if (abilityList == null) abilityList = new List<GameObject> ();
		else foreach (GameObject abilityHolder in abilityList) {
			Destroy (abilityHolder);
		}
		abilityList.Clear ();
		nameText.text = selected.getName().ToString();
		hpText.text = selected.currentHealth.ToString();
		maxHpText.text = selected.maxHealth.ToString();
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
		hpSlider.maxValue = selected.maxHealth;
		hpSlider.value = selected.currentHealth;
		if (selected.canMove)
			MOVText.text = "MOV";
		else
			MOVText.text = "";
		if (selected.canAttack)
			ACTText.text = "ACT";
		else
			ACTText.text = "";
		Ability[] abilitiesOnUnit = selected.GetComponentsInChildren<Ability> ();
		for (int i = 0; i < abilitiesOnUnit.Length; i++) {
			GameObject newAbility = Instantiate (abilityWindow) as GameObject;
			newAbility.transform.SetParent (abilityHolder.transform, false);
			newAbility.transform.position += i*abilityHolderYOffset*Vector3.up;
			AbilityUI abilityUI = newAbility.GetComponent<AbilityUI> (); 
			abilityUI.UpdateValues (abilitiesOnUnit[i], selected);
			abilityList.Add (newAbility);
			if(selected.isFriendly() && abilitiesOnUnit[i].cooldown <= 0) newAbility.GetComponentInChildren<Image> ().sprite = buttonSprite;
			else newAbility.GetComponentInChildren<Image> ().sprite = windowSprite;
		}
	}

	public void Clear () {
		foreach (GameObject abilityHolder in abilityList) {
			Destroy (abilityHolder);
		}
		abilityList.Clear ();
	}
}
