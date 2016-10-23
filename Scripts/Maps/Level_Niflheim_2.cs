﻿using UnityEngine;
using System.Collections;

public class Level_Niflheim_2: BoardProfile
{

	public Vector3 _dimensions;
	public override Vector3 dimensions
	{
		get { return new Vector3(16, 24, 16); }
		set { _dimensions = value; }
	}

	public int[,] _heightMap;
	public override int[,] heightMap
	{
		get { return new int[16, 16]
			{   {20,20,20,20,16,15,15,13,12,11,10,10,11,12,13,13},
				{20,20,20,20,16,15,14,13,12,11,10,09,08,07,11,12},
				{20,20,20,19,17,14,13,13,12,10,09,08,07,06,07,08},
				{19,18,18,17,15,14,13,12,11,10,09,08,07,06,06,07},
				{12,14,14,13,11,11,11,10,10,09,07,07,06,06,06,06},
				{11,13,12,12,11,10,10,05,04,00,00,06,06,05,05,05},
				{10,11,11,08,09,08,07,04,03,00,00,00,00,05,05,05},
				{09,09,08,03,06,06,05,02,00,00,00,00,00,05,05,04},
				{03,04,03,00,00,00,00,00,00,00,00,04,05,04,04,03},
				{00,00,00,00,00,00,00,00,00,00,00,05,04,03,03,03},
				{00,00,00,00,00,00,00,00,00,00,03,04,02,02,02,02},
				{00,00,00,00,00,00,00,00,00,03,03,02,02,02,02,02},
				{02,05,04,03,02,00,00,00,03,02,02,02,02,01,01,01},
				{02,04,03,02,01,01,01,02,02,02,02,02,01,01,01,01},
				{01,02,02,02,01,00,00,00,01,02,02,02,01,01,01,01},
				{00,01,01,01,00,00,00,00,00,01,02,02,01,01,01,01}   }; }
		set { _heightMap = value; }
	}

	public int[,] _textureMap;
	public override int[,] textureMap
	{
		get { return new int[16, 16]
			{   {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}   }; }
		set { _textureMap = value; }
	}

	public int[,] _deployMap;
	public override int[,] deployMap
	{
		get { return new int[16, 16]
			{   {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
				{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}   }; }
		set { _deployMap = value; }
	}
}
