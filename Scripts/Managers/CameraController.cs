using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

	[Header ("Settings")]
	public float verticalPadding;
	public float maxRotationAngle;
	public float mapRotationSpeed;
	public float dampingAngle;
	public float minCameraSize;
	public int zoomLevels;
	public float cameraZoomTime;
	[Range (0, 1)] public float zoomMouseFocus;
	public float panSpeed;
	[Range (0, 1)] public float xPanFraction;

	[Header ("Dependencies")]
	public BoardManager boardManager;
	public Transform cameraAnchor;

	private Vector3 focusPoint;
	private Camera _camera;
	private CameraInfo defaultState;
	private float currentAngle;
	private int zoomLevel;
	private float cameraTargetSize;
	private float cameraSizeDelta;
	private float cameraZoomDeadline;
	private float cameraResetDeadline;
	private Vector3 cameraPositionTarget;

	public class CameraInfo
	{
		public Vector3 position;
		public Quaternion rotation;
		public Vector3 scale;
		public float size;

		public void Save (Transform inputTransform, Camera inputCamera)
		{
			position = inputTransform.position;
			rotation = inputTransform.rotation;
			scale = inputTransform.localScale;
			size = inputCamera.orthographicSize;
		}
	}

	// Use this for initialization
	void Start ()
	{
		_camera = GetComponent<Camera> ();
		focusPoint.x = 0.5f * boardManager.dimensions.x;
		focusPoint.z = 0.5f * boardManager.dimensions.z;
		focusPoint.y = 0.5f * boardManager.dimensions.y;
		cameraAnchor.transform.position += focusPoint;
		// the projection of the vector (from the top of the map to the outer edge) onto the plane vertically dissecting the camera 
		Vector3 proj = Vector3.ProjectOnPlane (new Vector3 (boardManager.dimensions.x, -boardManager.dimensions.y, boardManager.dimensions.z), new Vector3 (-1, 0, 1));
		_camera.orthographicSize = 0.5f * (Vector3.Project (proj, cameraAnchor.transform.up).magnitude) + verticalPadding; // stage always fits in vision
		defaultState = new CameraInfo ();
		defaultState.Save (cameraAnchor.transform, _camera);
		currentAngle = 0;
		zoomLevel = 0;
		cameraTargetSize = defaultState.size;
		cameraZoomDeadline = -1;
		cameraResetDeadline = -1;
		cameraPositionTarget = Vector3.zero;
	}

	void Update ()
	{
		if (cameraResetDeadline > Time.time) {
			float t = 1 - ((cameraResetDeadline - Time.time) / cameraZoomTime);
			_camera.orthographicSize = Mathf.Lerp (_camera.orthographicSize, defaultState.size, t);
			transform.localPosition = Vector3.Lerp (transform.localPosition, Vector3.zero, t);
			cameraAnchor.transform.position = Vector3.Lerp (cameraAnchor.transform.position, defaultState.position, t);
			cameraAnchor.transform.rotation = Quaternion.Lerp (cameraAnchor.transform.rotation, defaultState.rotation, t);
			zoomLevel = 0;
		} else if (cameraZoomDeadline > Time.time) {
			float t = 1 - ((cameraZoomDeadline - Time.time) / cameraZoomTime);
			_camera.orthographicSize = Mathf.Lerp (_camera.orthographicSize, cameraTargetSize, t);
			transform.localPosition = Vector3.Lerp (transform.localPosition, cameraPositionTarget, t);
		}
	}

	public void RotateAroundFocus (float angle)
	{ // gets mouse movement x delta
		angle *= mapRotationSpeed;
		if ((Mathf.Abs (currentAngle) > maxRotationAngle - dampingAngle) && angle * currentAngle > 0) { // damp if close to max rotation
			float dampingDepth = Mathf.Max (Mathf.Abs (currentAngle) - (maxRotationAngle - dampingAngle), 0) / dampingAngle;
			angle = Mathf.Lerp (angle, 0, dampingDepth);
		}
		if (angle > 0)
			angle = Mathf.Min (angle, maxRotationAngle - currentAngle);
		else
			angle = Mathf.Max (angle, -maxRotationAngle - currentAngle);
		cameraAnchor.transform.RotateAround (focusPoint, Vector3.up, angle);
		currentAngle += angle;
		Mathf.Clamp (currentAngle, -maxRotationAngle, +maxRotationAngle);
	}

	public void Zoom (float input)
	{ // camera jumps between zoomlevels, with 0 beings min
		// Calculating raw camera size
		input *= -1;
		zoomLevel += input > 0 ? 1 : -1;
		zoomLevel = (int)Mathf.Clamp (zoomLevel, 0, zoomLevels);
		float zoomScale = Mathf.Pow ((1 - zoomLevel / ((float)zoomLevels)), 2);
		cameraTargetSize = minCameraSize + zoomScale * (defaultState.size - minCameraSize);
		// Focus on mouse when zooming (shift with cameraFocusDelta)
		Vector3 cameraFocusDelta = Vector3.zero;
		if (input > 0 && zoomLevel != zoomLevels) {
			Vector3 viewPortFocus = _camera.ScreenToViewportPoint (Input.mousePosition) * _camera.orthographicSize * 2;
			viewPortFocus.x *= _camera.aspect;
			cameraFocusDelta = viewPortFocus - new Vector3 (_camera.orthographicSize * _camera.aspect, _camera.orthographicSize, 0);
			cameraFocusDelta *= zoomMouseFocus * ((1 - zoomScale) + 0.4f / zoomLevels);
		}
		Vector3 newPos = transform.localPosition + cameraFocusDelta;
		// clamp Camera in viewing rectangle
		newPos.y = Mathf.Clamp (newPos.y, -defaultState.size + _camera.orthographicSize, defaultState.size - _camera.orthographicSize);
		newPos.x = Mathf.Clamp (newPos.x, (-defaultState.size + _camera.orthographicSize) * _camera.aspect * xPanFraction, (+defaultState.size - _camera.orthographicSize) * _camera.aspect * xPanFraction);
		// issue order to be executed in Update();
		cameraPositionTarget = newPos;
		cameraZoomDeadline = cameraZoomTime + Time.time;
	}

	public void Pan (float horizontal, float vertical)
	{
		Vector3 cameraPanPositionTarget = transform.localPosition + (Vector3.right * horizontal + Vector3.up * +vertical); // target position of camera
		Vector3 newPos = Vector3.Lerp (transform.localPosition, cameraPanPositionTarget, Time.deltaTime * panSpeed); // damping of camera movement
		// restrict new position to view rectangle
		newPos.y = Mathf.Clamp (newPos.y, -defaultState.size + _camera.orthographicSize, defaultState.size - _camera.orthographicSize);
		newPos.x = Mathf.Clamp (newPos.x, (-defaultState.size + _camera.orthographicSize) * _camera.aspect * xPanFraction, (+defaultState.size - _camera.orthographicSize) * _camera.aspect * xPanFraction);
		cameraPositionTarget += newPos - transform.localPosition;
		transform.localPosition = newPos; // assign new position
	}

	public void middleClick ()
	{
		if (zoomLevel != 0) {
			cameraResetDeadline = Time.time + cameraZoomTime;
			currentAngle = 0;
		}
		else {
			Vector3 viewPortFocus = _camera.ScreenToViewportPoint (Input.mousePosition) * _camera.orthographicSize * 2;
			viewPortFocus.x *= _camera.aspect;
			Vector3 cameraFocusDelta = viewPortFocus - new Vector3 (_camera.orthographicSize * _camera.aspect, _camera.orthographicSize, 0);
			Vector3 newPos = transform.localPosition + cameraFocusDelta;
			// clamp Camera in viewing rectangle
			newPos.y = Mathf.Clamp (newPos.y, -defaultState.size + minCameraSize, defaultState.size - minCameraSize);
			newPos.x = Mathf.Clamp (newPos.x, (-defaultState.size + minCameraSize) * _camera.aspect * xPanFraction, (+defaultState.size - minCameraSize) * _camera.aspect * xPanFraction);
			// issue order to be executed in Update();
			cameraPositionTarget = newPos;
			cameraZoomDeadline = cameraZoomTime + Time.time;

			cameraTargetSize = minCameraSize;
			zoomLevel = zoomLevels;
		}
	}

	public void Reset ()
	{	// resets camera parameters to start values
		cameraAnchor.transform.position = defaultState.position;
		cameraAnchor.transform.rotation = defaultState.rotation;
		cameraAnchor.transform.localScale = defaultState.scale;
		_camera.orthographicSize = defaultState.size;
		_camera.transform.localPosition = Vector3.zero;
		zoomLevel = 0;
		currentAngle = 0;
	}
}
