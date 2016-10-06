using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
	[Header ("TESTING VARIABLES:")]
	// START TESTING VARIABLES
	public float projectileSpeed;
	public GameObject model;
	public Vector3 testTarget;
	public Vector3 testOrigin;
	// END TESTING VARIABLES

	[Header ("Settings")]
	public float clickWindow;

	[Header ("Dependencies")]
	public GameManager gameManager;
	public BoardManager boardManager;

	private CameraController mainCamera;
	private Camera _camera;

	// internal variables
	private Vector3 prevMousePos;
	private Vector3 mouseDelta;
	private float mouse3LastClick;

	// Use this for initialization
	void Start ()
	{
		mainCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<CameraController> ();
		_camera = mainCamera.gameObject.GetComponent<Camera> ();
		prevMousePos = Input.mousePosition;
		mouse3LastClick = -1000;
	}

	void Update ()
	{
		GameState gameState = gameManager.gameStack.Peek ();
		if (gameState.type != GameStateType.DIALOGUE || gameState.type != GameStateType.ESCMENU) {
			if (Input.GetMouseButtonDown (2) == true)
				mouse3LastClick = Time.time;
			else if (Input.GetMouseButton (2) != true && Time.time - mouse3LastClick < clickWindow) {
				mouse3LastClick = -1;
				mainCamera.middleClick ();
			}
			if ((Input.GetMouseButton (2) == true || (Input.GetAxisRaw ("Left Alt") != 0 && Input.GetMouseButton (0)))) {
				if (mouseDelta.x != 0)
					mainCamera.RotateAroundFocus (-mouseDelta.x);
			}
			if (Input.GetAxisRaw ("Mouse ScrollWheel") != 0) {
				mainCamera.Zoom (-Input.GetAxis ("Mouse ScrollWheel"));
			}
			if (Input.GetAxis ("Horizontal") != 0 || Input.GetAxis ("Vertical") != 0) {
				mainCamera.Pan (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));
			}
			// the input stuff below is for testing purposes
			if (Input.GetKeyDown (KeyCode.Z)) {
				Debug.Log ("TEST: launching projectile");
				ActivateAbility (testTarget);
			}
		}

		// if player clicks left mouse button
		if (Input.GetMouseButtonDown (0)) {
			// if it's the player turn, check what the player selected
			if (gameState.type == GameStateType.PLAYERTURN) {
				RaycastHit hitInfo = MouseRaycast ();
				if (hitInfo.collider != null) {
					Unit selected = hitInfo.collider.gameObject.GetComponent<Unit> ();
					if (selected != null) { // if a unit is selected
						gameManager.Push (new GameState (GameStateType.SELECTEDUNIT, selected.gameObject));
						Debug.Log (selected.getName () + " selected");
					}
				}
			}
			else if (gameState.type == GameStateType.SELECTEDUNIT) {
				RaycastHit hitInfo = MouseRaycast ();
				if (hitInfo.collider != null) {
					// if terrain is selected
					if (hitInfo.collider.CompareTag ("Terrain")) { 
						Unit selected = gameState.evoker.GetComponent<Unit>();
						if (boardManager.unitMap[(int)hitInfo.collider.transform.position.x, (int)hitInfo.collider.transform.position.z] == null) { // if target not occupied
							Vector3 clickedTarget = hitInfo.collider.transform.position;
							Vector3 finalTarget = new Vector3((int) clickedTarget.x, boardManager.heightMap[(int) clickedTarget.x, (int) clickedTarget.z], (int) clickedTarget.z);
							if (clickedTarget == finalTarget) {
								gameManager.Pop ();
								selected.Move(finalTarget+Vector3.up); // move to one above the selected block
								Debug.Log (selected.getName () + " moved");
							}
						}
					}
					// if the selected player is selected
					else if (hitInfo.collider.gameObject == gameState.evoker) {
						gameManager.Pop ();
						Debug.Log ("Unit delesected");
					}
				}
			}

		}

		// if player clicks right mouse button, the below is for testing purposes
		else if (Input.GetMouseButtonDown (1)) {
			Debug.Log ("TEST: Checking fireball launch");
			// if it's the player turn, check what the player selected
			if (gameState.type == GameStateType.SELECTEDUNIT) {
				RaycastHit hitInfo = MouseRaycast ();
				if (hitInfo.collider != null) {
					if (hitInfo.collider.CompareTag ("Terrain")) { // if terrain is selected
						Ability ability = gameState.evoker.GetComponentInChildren<Ability> ();
						ability.transform.position = gameState.evoker.transform.position;

						Vector3 clickedTarget = hitInfo.collider.transform.position;
						Vector3 finalTarget = new Vector3((int) clickedTarget.x, boardManager.heightMap[(int) clickedTarget.x, (int) clickedTarget.z], (int) clickedTarget.z);
						if (clickedTarget == finalTarget) {
							gameManager.Pop ();
							ability.ActivateAbility (finalTarget + Vector3.up);
						}
					}
				}
			}
		}
	}

	// returns the hitInfo on a raycast on the mouse
	RaycastHit MouseRaycast ()
	{
		RaycastHit hitInfo = new RaycastHit ();
		Physics.Raycast (_camera.ScreenPointToRay (Input.mousePosition), out hitInfo);
		return hitInfo;
	}

	void FixedUpdate ()
	{
		mouseDelta = prevMousePos - Input.mousePosition;
		prevMousePos = Input.mousePosition;
	}

	//				BELOW IS A TESTING PORTION OF CODE, IT CAN BE REMOVED LATER

	// Manage new CASTABILITY GameState, animate projectile trajectory and impact, and apply ability effects to target area
	public void ActivateAbility (Vector3 target)
	{
		// first create and push a CASTABILITY gamestate to restrict input and whatever
		GameState gameState = new GameState (GameStateType.ANIMATION, this.gameObject);
		gameManager.Push (gameState);
		// now start coroutine to draw animation, the coroutine needs the gameState to declare it inactive once it's finished
		StartCoroutine (LaunchProjectile (target, gameState));
	}

	// This function launches and animates the projectile trajectory and impact.
	// It then applies its effect to the target position and marks its associated CASTABILITY GameState as inactive
	private IEnumerator LaunchProjectile (Vector3 target, GameState gameState)
	{
		Debug.Log ("TEST: launching projectile");
		Vector3 origin = testOrigin;
		if (target != origin) { // only bother with trajectory and projectile creation if it's not a self-cast
			GameObject projectile = Instantiate (model, origin, Quaternion.identity) as GameObject; // instantiate projectile
			// The trajectory is split up in two phases: In one phase the projectile moves straight up or down to get to the same height as the target
			// In the second phase the projectile moves horizontally in a parabola towards the target.
			// The order of the phases depends on how the height of the origin relates to the height of the target
			bool parabolaFirst = origin.y > target.y ? true : false; // calculate which phase is first
			Vector3 switchPoint; // calculate the switching point between the two phases
			float switchPointDistance; // distance beteen switchPoint and origin/target will be used later
			if (parabolaFirst) {
				switchPoint = target;
				switchPoint.y = origin.y;
				switchPointDistance = (switchPoint - origin).magnitude;
			} else {
				switchPoint = origin;
				switchPoint.y = target.y;
				switchPointDistance = (switchPoint - target).magnitude;
			}
			while (Mathf.Abs ((projectile.transform.position - switchPoint).magnitude) > 0.1f) { // while projectile is not at switchpoint
				if (parabolaFirst) { // if the parabola is first, then move in parabola to switchpoint
					// we use the fraction of (how much the projectile has moved in the x,z plane) / (how much needs to be moved in the x,z plane)
					// to calculate the height of the parabola at that point
					Vector3 newPos = projectile.transform.position; // we calculate the new position by first taking the current position
					newPos.y = switchPoint.y; // reset y to what y would be without the added parabola height
					newPos = Vector3.MoveTowards (newPos, switchPoint, projectileSpeed * 0.5f); // newPos is now the newPosition without the height of the parabola
					float progress = ((newPos - origin).magnitude) / switchPointDistance; // A value from 0-1 that represent what fraction of the parabola has been travelled
					float height = 2; // this is the max height of the parabola
					newPos = newPos + Vector3.up * (-4 * Mathf.Pow (progress, 2) + 4 * progress) * height; // we now add the height of the parabola to get the real position
					projectile.transform.position = newPos;
				} else { // if ascent is fist, then move to switchpointin straight line
					projectile.transform.position = Vector3.MoveTowards (projectile.transform.position, switchPoint, projectileSpeed);
				}
				yield return new WaitForFixedUpdate ();
			}
			while (Mathf.Abs ((projectile.transform.position - target).magnitude) > 0.1f) { // while projectile is not at target
				if (parabolaFirst) {
					// in the second phase descent towards target
					projectile.transform.position = Vector3.MoveTowards (projectile.transform.position, target, projectileSpeed);
				} else { // after ascending, move in parabola to target
					// we use the fraction of (how much the projectile has moved in the x,z plane) / (how much needs to be moved in the x,z plane)
					// to calculate the height of the parabola at that point
					Vector3 newPos = projectile.transform.position; // we calculate the new position by first taking the current position
					newPos.y = target.y; // reset y to what y would be without the added parabola height
					newPos = Vector3.MoveTowards (newPos, target, projectileSpeed * 0.5f); // newPos is now the newPosition without the height of the parabola
					float progress = ((newPos - switchPoint).magnitude) / switchPointDistance; // A value from 0-1 that represent what fraction of the parabola has been travelled
					float height = 2; // this is the max height of the parabola
					newPos = newPos + Vector3.up * (-4 * Mathf.Pow (progress, 2) + 4 * progress) * height; // we now add the height of the parabola to get the real position
					projectile.transform.position = newPos;
				}
				yield return new WaitForFixedUpdate ();
			}
			Destroy (projectile); // destroy projectile at impact
		}
		// 			code for hit-art/animation should be placed here
		// 			code for effect giving to units in or around target area should be placed here
		// signal that animation is done to GameManager:
		gameState.active = false;
	}
}
