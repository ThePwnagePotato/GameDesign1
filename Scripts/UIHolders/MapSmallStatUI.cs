using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MapSmallStatUI : MonoBehaviour {

	// the ability this abilityUI represents
	private Unit unit;
	private UnitManager gameManager;
	public Image backgroundImage;
	public Button unitSelectButton;

	public Text nameText;
	public Text maxHealthText;
	public Text currHealthText;
	public Slider healthSlider;
	public Text movText;
	public Text actText;

	void Start() {
		gameManager = GameObject.FindGameObjectWithTag ("UnitManager").GetComponent<UnitManager> ();
	}

	public void PushUnitSelect () {
		if (gameManager.gameStack.Peek ().type == MapStateType.PLAYERTURN) {
			MapState gameState = new MapState (MapStateType.SELECTEDUNIT, unit.gameObject);
			gameManager.Push (gameState);
		}
	}

	public void UpdateValues(Unit selectedUnit) {
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
