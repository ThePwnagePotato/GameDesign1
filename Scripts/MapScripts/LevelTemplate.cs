using UnityEngine;
using System.Collections;

public class LevelTemplate : BoardProfile {

	void Start() {
		// dimensions of board
		dimensions = new Vector3 (4, 4, 4);

		// The value in a cell represents the height of the map there
		heightMap = new int[4,4]
		{	{0,0,0,0},
			{0,0,0,0},
			{0,0,0,0},
			{0,0,0,0}	};

		// The value in a cell represents what BlockSet must be used in that position
		textureMap = new int[4,4]
		{	{0,0,0,0},
			{0,0,0,0},
			{0,0,0,0},
			{0,0,0,0}	};

		// The value in a cell represents the unit to be spawned there (0 is none)
		deployMap = new int[4,4]
		{	{0,0,0,0},
			{0,0,0,0},
			{0,0,0,0},
			{0,0,0,0}	};
	}
}
