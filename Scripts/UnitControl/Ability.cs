using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Ability : MonoBehaviour
{
	public abstract string getName ();

	public abstract int maxCooldown ();

	public abstract int cooldown { get; set; }

	public abstract int minRange ();

	public abstract int maxRange ();

	public abstract int minHeight ();

	public abstract int maxHeight ();

	public abstract int upScale { get; set;}

	public abstract int downScale { get; set;}

	public abstract int damage (int power);

	public abstract float projectileSpeed { get; set; }

	public abstract GameObject model { get; set; }

	public abstract GameManager gameManager { get; set; }

	public void Start () {
		// connect dependencies
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		if (gameManager == null)
			Debug.Log ("Ability: GameManager not found)");
	}

	public List<Vector3> getPossibleTargets (Unit caster, int[,] heightMap)
	{
		// the list to return
		List<Vector3> possibleTargets = new List<Vector3> ();

		// get the current position
		Vector3 casterPos = caster.transform.position;
		int casterX = (int)casterPos.x;
		int casterY = (int)casterPos.y;
		int casterZ = (int)casterPos.z;

		//start in  xz(-maxRange) coordinates, then check all coordinates from there
		int x, z;
		for (x = System.Math.Max (casterX - maxRange (), 0); x <= casterX + maxRange (); x++) {
			//x starts at 0 or higher, if x is out of bounds, stop (since x always goes from low to high)
			if (x > heightMap.Rank) {
				break;
			}

			for (z = System.Math.Max (casterZ - maxRange (), 0); z <= casterZ + maxRange (); z++) {
				//z starts at 0 or higher, increase until not anymore, if z is out of bounds, go to next x value
				if (z > heightMap.GetLength (0)) {
					break;
				}

				//check if x,z is in range horizontally
				int distance = System.Math.Abs (casterX - x) + System.Math.Abs (casterZ - z);
				if (distance < minRange () || distance > maxRange ()) {
					continue;
				}

				//check if the height on [x,z] is within range, if so add it to the list possibleTargets
				int posHeight = heightMap [x, z];
				if (posHeight >= casterY - minHeight () && posHeight <= casterY + maxHeight ()) {
					possibleTargets.Add (new Vector3 (x, posHeight, z));
					continue;
				}
			}
		}
			
		return possibleTargets;
	}
		
	// Manage new ANIMATION GameState, animate projectile trajectory and impact, and apply ability effects to target area
	public void ActivateAbility (Vector3 target)
	{
		// first create and push a ANIMATION gamestate to restrict input and whatever
		GameState gameState = new GameState (GameStateType.ANIMATION, this.gameObject);
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		gameManager.Push (gameState);
		// now start coroutine to draw animation, the coroutine needs the gameState to declare it inactive once it's finished
		StartCoroutine (LaunchProjectile (target, gameState));
	}

	public bool isValidTarget (Vector3 origin, Vector3 target) {
		int range = (int) Mathf.Abs (origin.x - target.x) + (int) Mathf.Abs (origin.z - target.z);
		if (range > maxRange() || range < minRange()) return false;
		int hDelta = (int)(target.y - origin.y);
		//if cannot reach because of vertical traversal return false
		return true;
	}

	/*
	// This function launches and animates the projectile trajectory and impact.
	// It then applies its effect to the target position and marks its associated ANIMATION GameState as inactive
	private IEnumerator LaunchProjectile (Vector3 target, GameState gameState)
	{
		Vector3 origin = transform.position;
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
					newPos = Vector3.MoveTowards (newPos, switchPoint, (switchPointDistance)*projectileSpeed*0.14f); // newPos is now the newPosition without the height of the parabola
					float progress = ((newPos-origin).magnitude)/switchPointDistance; // A value from 0-1 that represent what fraction of the parabola has been travelled
					float height = 2; // this is the max height of the parabola
					newPos = newPos + Vector3.up*(-4*Mathf.Pow(progress, 2)+4*progress)*height; // we now add the height of the parabola to get the real position
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
				}
				else { // after ascending, move in parabola to target
					// we use the fraction of (how much the projectile has moved in the x,z plane) / (how much needs to be moved in the x,z plane)
					// to calculate the height of the parabola at that point
					Vector3 newPos = projectile.transform.position; // we calculate the new position by first taking the current position
					newPos.y = target.y; // reset y to what y would be without the added parabola height
					newPos = Vector3.MoveTowards (newPos, target, (switchPointDistance) * projectileSpeed * 0.14f); // newPos is now the newPosition without the height of the parabola
					float progress = ((newPos-switchPoint).magnitude)/switchPointDistance; // A value from 0-1 that represent what fraction of the parabola has been travelled
					float height = 2; // this is the max height of the parabola
					newPos = newPos + Vector3.up*(-4*Mathf.Pow(progress, 2)+4*progress)*height; // we now add the height of the parabola to get the real position
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
	*/

	private IEnumerator LaunchProjectile (Vector3 target, GameState gameState)
	{
		Vector3 origin = transform.position;
		if (target != origin) { // only bother with trajectory and projectile creation if it's not a self-cast
			GameObject projectile = Instantiate (model, origin, Quaternion.identity) as GameObject; // instantiate projectile

			float switchPointDistance = (target - origin).magnitude; // distance beteen switchPoint and origin/target will be used later

			while (Mathf.Abs ((projectile.transform.position - target).magnitude) > 0.1f) { // while projectile is not at target
				// we use the fraction of (how much the projectile has moved in the x,z plane) / (how much needs to be moved in the x,z plane)
				// to calculate the height of the parabola at that point
				Vector3 newPos = projectile.transform.position; // we calculate the new position by first taking the current position
				// get the projection of the current position onto the direct line between the origin and target
				// first calculate the scalar projection of the currentPos to the noHeight position
				float scalarProjection = Vector3.Dot(newPos - origin, target - origin) / ((target - origin).magnitude);
				//Debug.Log (Vector3.Dot(newPos - origin, target - origin));
				//Debug.Log (((target - origin).magnitude));

				newPos = (target - origin).normalized * scalarProjection + origin;

				newPos = Vector3.MoveTowards (newPos, target, (switchPointDistance)*projectileSpeed*0.10f); // newPos is now the newPosition without the height of the parabola

				float progress = ((newPos-origin).magnitude)/switchPointDistance; // A value from 0-1 that represent what fraction of the parabola has been travelled
				float height = 3; // this is the max height of the parabola

				Plane plane = new Plane (origin, target, target + Vector3.up);
				Vector3 normal = Vector3.Cross (plane.normal, target - origin).normalized;

				//float sinAngle = Mathf.Abs(origin.y - target.y) / switchPointDistance;
				//Vector3 heightVector = Vector3.up*(-4*Mathf.Pow(progress, 2)+4*progress)*height;
				//newPos = newPos + (heightVector * (1-sinAngle)); // we now add the height of the parabola to get the real position

				float heightC = (-4*Mathf.Pow(progress, 2)+4*progress)*(switchPointDistance / height);
				normal = normal * heightC;
				newPos += normal;
				projectile.transform.position = newPos;

				yield return new WaitForFixedUpdate ();
			}

			Destroy (projectile); // destroy projectile at impact
		}
		// 			code for hit-art/animation should be placed here
		// 			code for effect giving to units in or around target area should be placed here
		// signal that animation is done to GameManager:
		gameState.active = false;
	}

	/*

//direct line from origin to target

		Vector3 origin = transform.position;
		if (target != origin) { // only bother with trajectory and projectile creation if it's not a self-cast
			GameObject projectile = Instantiate (model, origin, Quaternion.identity) as GameObject; // instantiate projectile

			float switchPointDistance = (target - origin).magnitude; // distance beteen switchPoint and origin/target will be used later

			while (Mathf.Abs ((projectile.transform.position - target).magnitude) > 0.1f) { 
				Vector3 newPos = projectile.transform.position; // we calculate the new position by first taking the current position
				float scalarProjection = Vector3.Dot(newPos - origin, target - origin) / ((target - origin).magnitude);
				newPos = (projectile.transform.position - origin).normalized * scalarProjection + origin;
				newPos = Vector3.MoveTowards (newPos, target, (switchPointDistance)*projectileSpeed*0.14f); // newPos is now the newPosition without the height of the parabola
				projectile.transform.position = newPos;
				yield return new WaitForFixedUpdate ();
			}

			Destroy (projectile); // destroy projectile at impact
		}
		gameState.active = false;

	 * */
}

/* Abilities
 * 
 * Mage:	Fireball (basic)
 * 			
 * 
 * 
 * Rogue:	Stab (basic)
 * 			Poison attack (inflicts poison)
 * 			
 * 
 * 
 * Ranger:	Shoot (bow) (basic)
 * 			Fast attack (couple arrows in a row)
 * 			Snipe (long range high damage)
 * 			
 * 
 * Tank:	Stab (basic)
 * 			Taunt (draws aggro from enemy)
 * 			self buff (decrease damage for x turns)
 * 			utility (stun attack)
 * 			
 * 
 * 
 * */





