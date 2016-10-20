using UnityEngine;
using System.Collections;

public abstract class BoardProfile : MonoBehaviour {

	public Vector3 dimensions;
	public int[,] heightMap;
	public int[,] textureMap;
	public int[,] deployMap;
}
