using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class SelectedUI : MonoBehaviour {

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
	}
}
