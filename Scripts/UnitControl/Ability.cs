using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Ability : MonoBehaviour
{
	public abstract string getName ();

	public abstract string[] getDescription ();

	public abstract int maxCooldown ();

	public abstract int cooldown { get; set; }

	public abstract int minRange ();

	public abstract int maxRange ();

	public abstract int sideRange ();

	public abstract int upRange ();

	public abstract int downRange ();

	public abstract float upScale { get; set;}

	public abstract float downScale { get; set;}

	public abstract int getDamage (int power);

	public abstract int getRawDamage (int power);

	public abstract float projectileSpeed { get; set; }

	public abstract float projectileHeight { get; set; }

	public abstract GameObject model { get; set; }

	public abstract GameManager gameManager { get; set; }

	public abstract bool dealsDamage ();

	public virtual float critChance { get { return 0.1f; } set { critChance = value; }}

	public abstract void HitTarget (Unit caster, Vector3 target);

	public void Start () {
		// connect dependencies
		gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		if (gameManager == null)
			Debug.Log ("Ability: GameManager not found)");

		cooldown = 0;
	}

	public List<Vector3> getPossibleTargets ()
	{
		int[,] heightMap = gameManager.boardManager.heightMap;
		// the list to return
		List<Vector3> possibleTargets = new List<Vector3> ();

		// get the current position
		int casterX = (int)transform.position.x;
		int casterY = (int)transform.position.y;
		int casterZ = (int)transform.position.z;

		//start in  xz(-maxRange) coordinates, then check all coordinates from there
		int x, z;
		for (x = System.Math.Max (casterX - maxRange (), 0); x <= Mathf.Min(casterX + maxRange (), gameManager.boardManager.dimensions.x-1); x++) {
			//x starts at 0 or higher, if x is out of bounds, stop (since x always goes from low to high)

			for (z = System.Math.Max (casterZ - maxRange (), 0); z <= Mathf.Min(casterZ + maxRange (), gameManager.boardManager.dimensions.z-1); z++) {
				//z starts at 0 or higher, increase until not anymore, if z is out of bounds, go to next x value

				// distances
				int horDistance = System.Math.Abs (casterX - x) + System.Math.Abs (casterZ - z);
				int posHeight = heightMap [x, z] + 1;
				int verDistance = Mathf.Abs(posHeight - casterY);
				int distance = horDistance + verDistance;

				//check if x,z is in range horizontally
				if (distance < minRange () || distance > maxRange ()) {
					continue;
				}
				if (horDistance > sideRange()) continue;

				//check if the height on [x,z] is within range, if so add it to the list possibleTargets
				if (posHeight >= casterY - downRange () && posHeight <= casterY + upRange ()) {
					possibleTargets.Add (new Vector3 (x, posHeight, z));
				}
			}
		}
			
		return possibleTargets;
	}
		
	// Manage new ANIMATION GameState, animate projectile trajectory and impact, and apply ability effects to target area
	public void ActivateAbility (Vector3 target)
	{
		//after activating, set canMove and canAttack to false
		Unit caster = GetComponentInParent<Unit> ();
		caster.finishedAbility = false;
		caster.canMove = false;
		caster.canAttack = false;

		//also set the cooldown to -1
		cooldown = -1;

		// first create and push a ANIMATION gamestate to restrict input and whatever
		GameState gameState = new GameState (GameStateType.ANIMATION, this.gameObject);
		//gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		gameManager.Push (gameState);
		// now start coroutine to draw animation, the coroutine needs the gameState to declare it inactive once it's finished
		StartCoroutine (LaunchProjectile (target, gameState));
	}

	protected IEnumerator LaunchProjectile (Vector3 target, GameState gameState)
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

				newPos = Vector3.MoveTowards (newPos, target, (Mathf.Sqrt(switchPointDistance))*projectileSpeed*0.07f); // newPos is now the newPosition without the height of the parabola

				float progress = ((newPos-origin).magnitude)/switchPointDistance; // A value from 0-1 that represent what fraction of the parabola has been travelled

				if (projectileHeight != 0) {
					Plane plane = new Plane (origin, target, target + Vector3.up);
					Vector3 normal = Vector3.Cross (plane.normal, target - origin).normalized;

					//float sinAngle = Mathf.Abs(origin.y - target.y) / switchPointDistance;
					//Vector3 heightVector = Vector3.up*(-4*Mathf.Pow(progress, 2)+4*progress)*height;
					//newPos = newPos + (heightVector * (1-sinAngle)); // we now add the height of the parabola to get the real position

					float heightC = (-4*Mathf.Pow(progress, 2)+4*progress)*(switchPointDistance / (projectileHeight*3));
					normal = normal * heightC;
					newPos += normal;
				}
				projectile.transform.position = newPos;

				yield return new WaitForFixedUpdate ();
			}

			Destroy (projectile); // destroy projectile at impact
		}
		// 			code for hit-art/animation should be placed here

		// 			code for effect giving to units in or around target area should be placed here
		// get the caster from the origin of the ability, then check if it is null.
		Unit caster = gameManager.boardManager.unitMap[(int) origin.x, (int) origin.z];
		if (caster == null) {
			Debug.Log ("ERROR: No caster found at ability origin!");
		} else {
			HitTarget (caster, target);
		}

		//for enemy units
		caster.finishedAbility = true;

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
 * 			Heal
 * 			Empower
 * 			Blizzard
 * 
 * 
 * Rogue:	Stab (basic)
 * 			baskstepAttack
 * 			knifethrow
 * 			Bleed attack (inflicts bleeding)
 * 			
 * 
 * 
 * Ranger:	Shoot (bow) (basic)
 * 			Fast attack (couple arrows in a row)
 * 			Snipe (long range high damage)
 * 			RootShot
 * 			
 * 
 * Tank:	Stab (basic)
 * 			Taunt (draws aggro from enemy)
 * 			self buff, then debuff ()
 * 			utility (stun attack)
 * 			
 * 
 * 
 * */





