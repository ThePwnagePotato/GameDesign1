using UnityEngine;
using System.Collections;

public abstract class BoardProfile : MonoBehaviour {

	public abstract Vector3 dimensions { get; set; }
	public abstract int[,] heightMap { get; set; }
	public abstract int[,] textureMap { get; set; }
	public abstract int[,] deployMap { get; set; }

}
