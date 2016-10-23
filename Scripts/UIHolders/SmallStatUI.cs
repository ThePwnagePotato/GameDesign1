using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SmallStatUI : MonoBehaviour {

	// the ability this abilityUI represents
	private Unit unit;
	private GameManager gameManager;
	public Image backgroundImage;
	public Button	unitSelectButton;

	public Text nameText;
	public Text maxHealthText;
	public Text currHealthText;
	public Slider healthSlider;
	public Text movText;
	public Text actText;

	void Start() {
		gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
	}

	public void PushUnitSelect () {
			GameState gameState = new GameState (GameStateType.SELECTEDUNIT, unit.gameObject);
			gameManager.Push (gameState);
	}

	public void UpdateValues(Unit selectedUnit) {
		unit = selectedUnit;
		if (!unit.isAlive || (!unit.canAttack && unit.canMove)) {
			unitSelectButton.interactable = false;
			backgroundImage.color = new Color (0.5f, 0.5f, 0.5f);
		} else {
			unitSelectButton.interactable = true;
			backgroundImage.color = new Color (1, 1, 1);
		}
		unit = selectedUnit;
		nameText.text = selectedUnit.getName ();
		maxHealthText.text = unit.maxHealth.ToString();
		currHealthText.text = unit.currentHealth.ToString();
		healthSlider.maxValue = unit.maxHealth;
		healthSlider.value = unit.currentHealth;
		movText.text = unit.canMove ? "M" : "";
		actText.text = unit.canAttack ? "A" : "";
	}
}
