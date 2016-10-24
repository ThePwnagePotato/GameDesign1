using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/* The boardmanager stores all relative spatial data.
 * 
 * The boardmanager depends on a BoardProfile script to set the following:
 * - int[,] heightMap - a map with the heightvalue for each tile, 0 or less if uncrossable
 * - int[,] textureMap - a map with the identifier for what blockSet to use for the instantiation of a particular block (see next)
 * - BlockSet[] blockSets - blockSets are the "texturePacks" for blocks. Using the textureMap they can be set for every block individually
 * - int [,] deployMap - a map with the type of units to be instantiated on [,]
 * 
 * 		IMPORTANT NOTE: A 0 in a map usually does NOT spawn the corresponding prefab 0 on that position.
 * 		This is because a 0 on a map usually denotes the ABSENCE of a block/unit/etc.
 * 		As a consequence, the unitMap should be "shifted" up by 1.
 * 
 * The prefabs corresponding to the above mentioned maps are to be supplied to the BoardManager directly, these include:
 * - A default block
 * - The blocksets to be used based upon the textureMap
 * - The units to be instantiated based upon the deployMap
 */

public class MapBoardManager : MonoBehaviour
{
	[Header ("Parameters")]
	public Unit[] mainUnits;

	[HideInInspector] public List<Unit> friendlyUnits;

	void Awake () {
		friendlyUnits = new List<Unit> ();
		for (int j = 0; j < mainUnits.Length; j++) {
			Unit unit = mainUnits [j];
			friendlyUnits.Add (unit);
			for (int i = 0; i < unit.abilities.Count; i++) {
				GameObject newAbility = Instantiate (unit.abilities[i], unit.transform) as GameObject;
			}
		}
	}
}
