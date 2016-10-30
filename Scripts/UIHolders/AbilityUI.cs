using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour {

	// the ability this abilityUI represents
	private Ability ability;
	private Unit unit;
	private GameManager gameManager;

	public Text nameText;
	public Text powText;
	public Text cooldownText;
	public Text upScaleText;
	public Text downScaleText;
	public Text maxMoveText;
	public Text minMoveText;
	public Text totalUpMoveText;
	public Text totalDownMoveText;
	public Text totalSideMoveText;

	void Start() {
		gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
	}

	public void PushAbilitySelect () {
		if (gameManager.gameStack.Peek ().type == GameStateType.SELECTEDABILITY)
			gameManager.Pop ();
		if (gameManager.gameStack.Peek ().type == GameStateType.SELECTEDUNIT
			&& gameManager.gameStack.Peek().evoker.GetComponent<Unit>() == unit) {
				GameState gameState = new GameState (GameStateType.SELECTEDABILITY, ability.gameObject);
				gameManager.Push (gameState);
		}
	}

	public void AttemptAbilityLearn () {
		UnitManager unitManager = GameObject.FindGameObjectWithTag ("UnitManager").GetComponent<UnitManager>();
		for (int i = 0; i < unitManager.mapBoardManager.mainUnits.Length; i++) {
			if (unitManager.mapBoardManager.mainUnits [i] == unit) {
				if (SaveData.saveData.currentSave.unitStats [i] < unit.abilities.Count && SaveData.saveData.currentSave.skillPoints > 0) {
					unitManager.InitiateConfirmAbilityLearn (SaveData.saveData.currentSave.unitStats, i, unit.getName (), ability.getName ());
				}
			}
		}
	}

	public void UpdateValues(Ability selectedAbility, Unit selectedUnit) {
		ability = selectedAbility;
		unit = selectedUnit;
		nameText.text = selectedAbility.getName ();
		powText.text = selectedAbility.getRawDamage(selectedUnit.power).ToString();
		upScaleText.text = selectedAbility.upScale.ToString();
		downScaleText.text = selectedAbility.downScale.ToString();
		maxMoveText.text = selectedAbility.maxRange().ToString();
		minMoveText.text = selectedAbility.minRange().ToString();
		totalUpMoveText.text = selectedAbility.upRange().ToString();
		totalDownMoveText.text = selectedAbility.downRange().ToString();
		totalSideMoveText.text = selectedAbility.sideRange().ToString();
		cooldownText.text = selectedAbility.cooldown.ToString();
	}
}
