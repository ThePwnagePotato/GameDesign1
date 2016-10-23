using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
 * - The effectBlocks to be instantiated based upon the effectMap		NOTE: An effectblock must ALWAYS have a BlockEffect script.
 */

public class BoardManager : MonoBehaviour
{

	[Header ("Settings")]
	public bool generateTerrain;
	public bool instantiateUnits;
	public bool useRandomHeightmap;
	public bool useBoardProfile;

	[Header ("Parameters")]
	public BoardProfile boardProfile;
	public Vector3 dimensions;
	public int[,] heightMap;
	public int[,] textureMap;
	public int[,] deployMap;
	public int[,] effectMap;
	public int[,] spawnPositionMap;
	public Unit[] prespawnedUnits;

	[Header ("Prefabs")]
	public GameObject defaultBlock;
	public GameObject map;
	public BlockSet[] blockSets;
	public GameObject[] spawnUnits;
	public GameObject[] effectBlocks;
	public GameObject[] deployableUnits;

	[Header ("Dependencies")]
	public GameObject mapHolder;

	// Stuff this class manages and provides to others
	[HideInInspector] public Unit[,] unitMap;
	[HideInInspector] public List<Unit> friendlyUnits;
	[HideInInspector] public List<Unit> enemyUnits;

	[System.Serializable]
	public class BlockSet {
		public GameObject[] topBlocks;
		public GameObject[] sideBlocks;
	}

	void Awake () {
		if (map != null)
			Instantiate (map, Vector3.one, Quaternion.identity);
		if (useRandomHeightmap) {
			GenerateHeightmap ();
		}
		if (boardProfile != null && useBoardProfile) {
			loadBoardProfile ();
		}
		if (generateTerrain) {
			GenerateTerrain ();
		}
		unitMap = new Unit[(int)dimensions.x,(int)dimensions.z];
		foreach (Unit unit in prespawnedUnits) {
			unitMap[(int)unit.transform.position.x, (int)unit.transform.position.z] = unit;
			if (unit.isFriendly())
				friendlyUnits.Add (unit);
			else
				enemyUnits.Add (unit);
		}
		if (instantiateUnits) {
			InstantiateUnits ();
		}
	}

	// Use this for initialization
	void Start ()
	{
		
	}

	// load parameters from BoardProfile
	void loadBoardProfile() {
		dimensions = boardProfile.dimensions;
		heightMap = boardProfile.heightMap;
		deployMap = boardProfile.deployMap;
		textureMap = boardProfile.textureMap;
	}

	// generate terrain blocks (GameObject) based upon heightMap and effectMap
	void GenerateTerrain ()
	{
		for (int x = (int)dimensions.x - 1; x >= 0; x--) {
			for (int z = (int)dimensions.z - 1; z >= 0; z--) {
				// calculate height of lowest visible neighbor to calculate starting height for spawning blocks
				int lowest = Mathf.Min (x < dimensions.x - 1 ? heightMap [x + 1, z] : 0, z < dimensions.z - 1 ? heightMap [x, z + 1] : 0); // check lowest neighbor
				// instantiate and stack blocks
				for (int y = Mathf.Max(lowest,1); y <= heightMap [x, z]; y++) {
					// select blockSet based on textureMap, then random block prefab in the set
					GameObject blockPrefab = defaultBlock;
					// if this is location has an associated effect, load appropriate effectBlock
					if (effectMap != null && effectMap [x, z] != 0) {
						blockPrefab = effectBlocks[effectMap [x, z]];
					} else if (textureMap != null && blockSets != null && textureMap [x, z] < blockSets.Length) {
						// choose a random texture from appropriate BlockSet
						int blockSet = textureMap [x, z];
						if (y == heightMap [x, z]) // if the top block, choose a random top block
							blockPrefab = blockSets[blockSet].topBlocks[Random.Range(0,blockSets[blockSet].topBlocks.Length)];
						else // if not top block, choose a "side" block
							blockPrefab = blockSets[blockSet].sideBlocks[Random.Range(0,blockSets[blockSet].sideBlocks.Length)];
					}

					// instantiate the chosen prefab with a random rotation around the y axis
					GameObject newBlock = Instantiate (blockPrefab, mapHolder.transform) as GameObject; // instantiate block
					newBlock.transform.position = new Vector3 (x, y, z);
					newBlock.transform.rotation = Quaternion.identity;
					newBlock.transform.RotateAround (newBlock.transform.up, transform.position, 90 * Random.Range (0, 4));
				}
			}
		}
	}

	// returns 0 or 1 randomly
	int CoinFlip ()
	{
		return Random.Range (0, 2);
	}

	void GenerateHeightmap ()
	{
		if (dimensions.x == 0 || dimensions.z == 0) // guard
			return;
		heightMap = new int[(int)dimensions.x, (int)dimensions.z];

		// fill ring 0 (outer bounds)
		int baseHeight = 1;
		heightMap [(int)dimensions.x - 1, (int)dimensions.z - 1] = baseHeight; // outer corner
		for (int x = (int)dimensions.x - 2; x >= 0; x--) { // outer x edge
			heightMap [x, (int)dimensions.z - 1] = heightMap [x + 1, (int)dimensions.z - 1] + CoinFlip ();
		}
		for (int z = (int)dimensions.z - 2; z >= 0; z--) { // outer x edge
			heightMap [(int)dimensions.x - 1, z] = heightMap [(int)dimensions.x - 1, z + 1] + CoinFlip ();
		}


		// fill other rings
		int ring = 1, minDimen = (int)Mathf.Min (dimensions.x, dimensions.z);
		while (ring < minDimen) {
			int highest = (int)Mathf.Max (heightMap [(int)dimensions.x - ring, (int)dimensions.z - ring - 1], heightMap [(int)dimensions.x - ring - 1, (int)dimensions.z - ring]);
			heightMap [(int)dimensions.x - ring - 1, (int)dimensions.z - ring - 1] = highest + CoinFlip ();
			for (int zpos = (int)dimensions.z - ring - 2; zpos >= 0; zpos--) {
				int highestpos = Mathf.Max (heightMap [(int)dimensions.x - ring, zpos], heightMap [(int)dimensions.x - ring - 1, zpos + 1]);
				heightMap [(int)dimensions.x - ring - 1, zpos] = highestpos + CoinFlip ();
			}
			for (int xpos = (int)dimensions.x - ring - 2; xpos >= 0; xpos--) {
				int highestpos = Mathf.Max (heightMap [xpos, (int)dimensions.z - ring], heightMap [xpos + 1, (int)dimensions.z - ring - 1]);
				heightMap [xpos, (int)dimensions.z - ring - 1] = highestpos + CoinFlip ();
			}
			ring++;
		}
			
		for (int x = (int)dimensions.x - 1; x >= 0; x--) {
			for (int z = (int)dimensions.z - 1; z >= 0; z--) {
				heightMap [x, z] = Mathf.Min ((int)heightMap [x, z], (int)dimensions.y);
			}
		}
	}

	// run through the values of int[,] deployMap and instantiate the corresponding GameObjects from GameObject[] spawnUnits
	void InstantiateUnits ()
	{
		for (int x = (int)dimensions.x - 1; x >= 0; x--) {
			for (int z = (int)dimensions.z - 1; z >= 0; z--) {
				if (deployMap != null && deployMap [x, z] != 0) {
					// select prefab
					GameObject unitPrefab = spawnUnits[deployMap [x, z]-1]; // unitMap value 1 corresponds to unit 0;
					// instantiate the corresponding prefab
					GameObject newUnit = Instantiate (unitPrefab, mapHolder.transform) as GameObject; // instantiate unit GameObject
					newUnit.transform.position = new Vector3 (x, heightMap [x, z], z); // position at correct location
					// bookkeeping
					Unit newUnitScript = newUnit.GetComponent<Unit>();
					unitMap [x, z] = newUnitScript;
					if (newUnitScript.isFriendly ())
						friendlyUnits.Add (newUnitScript);
					else
						enemyUnits.Add (newUnitScript);
				}
			}
		}
	}

	// adds a unit to the unitMap, mostly for testing purposes
	public void AddToUnitMap (Unit unit) {
		Debug.Log ("TEST: Adding unit to map");
		if(unitMap[(int)unit.transform.position.x, (int)unit.transform.position.z] == null) 
			unitMap[(int)unit.transform.position.x, (int)unit.transform.position.z] = unit;
	}
}
