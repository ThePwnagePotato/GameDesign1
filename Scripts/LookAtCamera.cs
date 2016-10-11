using UnityEngine;
using System.Collections;

// makes the object this script is attached to face the (orthographic) camera
public class LookAtCamera : MonoBehaviour {

	private GameObject _camera;
	private Quaternion prevRotation;

	// Use this for initialization
	void Start () {
		_camera = GameObject.FindGameObjectWithTag ("MainCamera");
		transform.rotation = _camera.transform.rotation;
		transform.RotateAround (transform.position, transform.up, 180);
		prevRotation = _camera.transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
		if (_camera.transform.rotation != prevRotation) {
			transform.rotation = _camera.transform.rotation;
			transform.RotateAround (transform.position, transform.up, 180);
		}
		prevRotation = _camera.transform.rotation;
	}
}
