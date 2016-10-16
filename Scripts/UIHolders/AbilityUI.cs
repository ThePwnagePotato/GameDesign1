using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour {

	// the ability this abilityUI represents
	private Ability ability;
	private Unit unit;
	private GameManager gameManager;

	public Text powText;
	public Text upScaleText;
	public Text downScaleText;
	public Text maxMoveText;
	public Text minMoveText;
	public Text totalUpMoveText;
	public Text totalDownMoveText;

	void Start() {
		gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
	}

	public void PushAbilitySelect () {
		if (gameManager.gameStack.Peek().type == GameStateType.SELECTEDUNIT
			&& gameManager.gameStack.Peek().evoker.GetComponent<Unit>() == unit 
			&& unit.canAttack
			&& ability.cooldown <= 0) {
			GameState gameState = new GameState (GameStateType.SELECTEDABILITY, ability.gameObject);
			gameManager.Push (gameState);
		}
	}

	public void UpdateValues(Ability selectedAbility, Unit selectedUnit) {
		ability = selectedAbility;
		unit = selectedUnit;
		powText.text = selectedAbility.getDamage(selectedUnit.power).ToString();
		upScaleText.text = selectedAbility.upScale.ToString();
		downScaleText.text = selectedAbility.downScale.ToString();
		maxMoveText.text = selectedAbility.maxRange().ToString();
		minMoveText.text = selectedAbility.minRange().ToString();
		totalUpMoveText.text = selectedAbility.upRange().ToString();
		totalDownMoveText.text = selectedAbility.downRange().ToString();
	}
}
