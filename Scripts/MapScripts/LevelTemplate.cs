using UnityEngine;
using System.Collections;

public class LevelTemplate : BoardProfile {

	public Vector3 _dimensions;
	public override Vector3 dimensions
	{
		get { return _dimensions; }
		set { _dimensions = value; }
	}

	public int[,]_heightMap;
	public override int[,] heightMap
	{
		get { return _heightMap; }
		set { _heightMap = value; }
	}

	public int[,] _textureMap;
	public override int[,] textureMap
	{
		get { return _textureMap; }
		set { _textureMap = value; }
	}

	public int[,] _deployMap;
	public override int[,] deployMap
	{
		get { return _deployMap; }
		set { _deployMap = value; }
	}

	void Awake() {
		// dimensions of board
		_dimensions = new Vector3 (4, 4, 4);

		// The value in a cell represents the height of the map there
		_heightMap = new int[4,4]
		{	{0,0,0,0},
			{0,0,0,0},
			{0,0,0,0},
			{0,0,0,0}	};

		// The value in a cell represents what BlockSet must be used in that position
		_textureMap = new int[4,4]
		{	{0,0,0,0},
			{0,0,0,0},
			{0,0,0,0},
			{0,0,0,0}	};

		// The value in a cell represents the unit to be spawned there (0 is none)
		_deployMap = new int[4,4]
		{	{0,0,0,0},
			{0,0,0,0},
			{0,0,0,0},
			{0,0,0,0}	};
	}
}
