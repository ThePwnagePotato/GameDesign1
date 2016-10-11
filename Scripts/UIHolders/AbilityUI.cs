using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour {

	// the ability this abilityUI represents
	public Ability ability;

	public Text powText;
	public Text upScaleText;
	public Text downScaleText;
	public Text maxMoveText;
	public Text minMoveText;
	public Text totalUpMoveText;
	public Text totalDownMoveText;

	public void updateValues(Ability selectedAbility, Unit selectedUnit) {
		powText.text = selectedAbility.getDamage(selectedUnit.power).ToString();
		upScaleText.text = selectedAbility.upScale.ToString();
		downScaleText.text = selectedAbility.downScale.ToString();
		maxMoveText.text = selectedAbility.maxRange().ToString();
		minMoveText.text = selectedAbility.minRange().ToString();
		totalUpMoveText.text = selectedAbility.maxHeight().ToString();
		totalDownMoveText.text = selectedAbility.minHeight().ToString();
	}
}
