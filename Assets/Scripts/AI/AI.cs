using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Entity))]
public class AI : MonoBehaviour {
	AudioSource shot;
	#region Setup variables
	public enum Type {Melee, Ranged}

	public enum BattleMode {ChaseOnSight, SeekAndDestroy}
	[Header("Setup variables")]
	[Tooltip("Chase on sight will only attack enemies seen (meaning in front of, within range and not being obscured). " +
		"Seek and destroy will chase enemies within chase distance.")]
	public BattleMode combatMode;

	[Tooltip("Add the tags of the enemy troops in here, you can have as many tags as you like, and this troop will attack anything of that tag.")]
	public string[] targetTags;

	public Transform currentTarget;


	[Tooltip("Use this to select whether this object is a melee soldier or a ranged soldier.")]
	public Type troopType;

	[Tooltip("Set this to true if you want this troop to patrol between way points.")]
	public bool isAGuard;

	[Tooltip("Any layers on this mask will be hit by the vision of the troop, the enemy layer MUST be selected, " +
		"it is also advisable to have any wall layers in this mask " +
		"(otherwise troops will fire at walls enemies are behind without trying to go around).")]
	public LayerMask mask;

	[Tooltip("The angle that a bullet must hit to create an instant kill, 10 is directly ahead, -10 is directly behind and 0 is either side.")]
	public float killCone = -5;
	[Header("Debugs")]
	[Tooltip("This will show a sphere representing the range of this soldier.")]
	public bool showRangeSphere;
	[Tooltip("This will show the cone of vision of this soldier.")]
	public bool showVisionCone;
	[Tooltip("This will show the two angles that a bullet must hit between in order to get an instant kill.")]
	public bool showKillCone;

	[HideInInspector]
	public List<GameObject> targets;
	#endregion

	#region Animation variables
	[Header("Animation variables")]
	public bool hasAnimations;
	[Tooltip("If adding animations, place the animator holding the required animations here.")]
	public Animator anim;
	public bool movingAnim;
	public bool meleeAttackAnim;
	public bool shootingAnim;
	public bool reloadingAnim;
	#endregion

	#region Movement variables
	private Quaternion targetRotation;


	private UnityEngine.AI.NavMeshAgent agent;
	[Header("Movement variables")]
	public float walkSpeed = 4;
	public float runSpeed = 8;
	private bool searching;
	#endregion

	#region Sight variables
	[Header("Sight variables")]
	[Tooltip("The angle at which this troop will see an enemy at, 10 is directly ahead, -10 is directly behind, and 0 is either side.")]
	public float visionCone = 5f;

	[Tooltip("The distance at which this troop will move towards their target tags.")]
	public float detectDistance = 8;

	[Tooltip("The distance at which this troop will aim at their target (for ranged troops they will also begin firing at this distance).")]
	public int lookAtTargetDistance = 5;

	[Tooltip("The distance at which this troop will stop chasing and simply attack (note that melee attacks will deal damage at this distance + 1).")]
	public float stoppingDistance = 2.5f;

	//[HideInInspector]
	public bool chasing;
	#endregion

	#region Melee variables
	[Header("Melee variables")]
	public float meleeCoolDown = 2;
	private float meleeAttackTimer;
	public float meleeDamage = 1;
	
	#endregion

	#region Ranged variables
	[Header("Ranged Variables")]

	[Tooltip("A bullet will fire from each of these transforms.")]
	public Transform[] firePoints;

	public Rigidbody bullet;
	public float bulletSpeed = 1000;

	[Tooltip("The higher this value, the greater the possible bullet spread.")]
	public float bulletSpread = 10;
	public float bulletDamage = 2;

	public float shotsPerMinute = 60;
	private float secondsBetweenShots;
	private float nextPossibleShootTime;

	public int ammoPerMag = 10;
	private int currentAmmoInMag;

	private bool reloading;
	#endregion

	#region Guard variables

	private int currentPatrolTarget = 0;

	[Header("Patrolling variables")]
	[Tooltip("This is the length of time the guard will wait at each point before moving on to the next.")]
	public float waitAtPointTime = 2;

	[Tooltip("This game object will travel to each of these in turn, cycling back to the first once the final point is reached.")]
	public List<Transform> patrolTargets;
	[HideInInspector]
	public bool changingPatrolTarget;
	#endregion

	void Start(){
		shot = GetComponent<AudioSource>();
		#region Movement start
		agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		
		agent.stoppingDistance = stoppingDistance;
		#endregion

		#region Melee start
		meleeAttackTimer = meleeCoolDown;
		#endregion

		#region Ranged start
		secondsBetweenShots = 60 / shotsPerMinute;

		currentAmmoInMag = ammoPerMag;

		#endregion

		if(isAGuard)
			agent.SetDestination (patrolTargets [currentPatrolTarget].position);

	}
	
	#region Finding target and adding it to list
	GameObject FindClosestTarget(string tag) {
		GameObject[] gos1;
		gos1 = GameObject.FindGameObjectsWithTag("" + tag);
		GameObject closest = null;
		float distance = Mathf.Infinity;
		Vector3 position = transform.position;
		foreach (GameObject go in gos1) {
			Vector3 diff = go.transform.position - position;
			float curDistance = diff.sqrMagnitude;
			if (curDistance < distance) {
				closest = go;
				distance = curDistance;
			}
		}
		return closest;
	}

	void AddToList(GameObject obj){

		int targetLimit = targetTags.Length;
		targets.Add (obj);
		if (targets.Count > targetLimit)
			targets.Remove (targets[0]);
	}
	#endregion

	void Update(){

		#region Movement update (including animations)

																	//IDLE, WALKING AND RUNNING ANIMATION
		//If you want to add movement animations, add an animator float setter such as "anim.SetFloat ("Speed", agent.velocity.magnitude);"
		//and set the animator to play the appropriate animations at the appropriate values; if float is above zero, play walk, if it is above walk speed, play run, and if it is zero play idle.
		if(anim != null && movingAnim)
			anim.SetFloat ("Speed", agent.velocity.magnitude);
		
		#region Dealing with multiple target tags
		if(targetTags.Length == 1){
			if(GameObject.FindGameObjectsWithTag("" + targetTags[0]).Length > 0)
				currentTarget = FindClosestTarget(targetTags[0]).transform;
		}

		else if(targetTags.Length > 1){
			foreach(string tagToTarget in targetTags){
				if(GameObject.FindGameObjectsWithTag("" + tagToTarget).Length > 0){
					GameObject objectt = FindClosestTarget(tagToTarget);
					if(!targets.Contains(objectt))
						AddToList (FindClosestTarget(tagToTarget));
				}
			}

			for(int i = 0; i < targets.Count; i++){
				if(targets[i] == null){
					targets.Remove(targets[i]);
				}
			}
		}

		if(targets.Count == 1)
			currentTarget = targets[0].transform;
		else if(targets.Count > 1 && Vector3.Distance(transform.position, targets[0].transform.position) < Vector3.Distance (transform.position, targets[1].transform.position))
			currentTarget = targets[0].transform;
		else if(targets.Count > 1 && Vector3.Distance(transform.position, targets[0].transform.position) > Vector3.Distance (transform.position, targets[1].transform.position))
			currentTarget = targets[1].transform;
		#endregion

		#region No targets found and patrolling
		if(currentTarget == null || (isAGuard && Vector3.Distance (currentTarget.position, transform.position) > detectDistance && chasing)){

			if(!isAGuard)
				agent.SetDestination(transform.position);

			chasing = false;
		}

		if(isAGuard && !chasing){
			agent.stoppingDistance = 0;

			agent.speed = walkSpeed;
		}
		#endregion

		if (currentTarget != null) {
			
			#region sets chasing depending on battlemode, distance and vision
			if (combatMode == BattleMode.ChaseOnSight && (Vector3.Distance (currentTarget.position, transform.position) <= detectDistance && !chasing && TargetInSight())){
				chasing = true;
				agent.SetDestination (currentTarget.position);
			}
			else if (combatMode == BattleMode.SeekAndDestroy && (Vector3.Distance (currentTarget.position, transform.position) <= detectDistance && !chasing)){
				chasing = true;
				agent.SetDestination (currentTarget.position);
			}
			#endregion

			if(chasing){
				agent.speed = runSpeed;

				if (agent.enabled == false)
					agent.enabled = true;

				if(TargetInSight() || !TargetInCover())
					agent.stoppingDistance = stoppingDistance;
				else if(!TargetInSight())
					agent.stoppingDistance = 0;

				if(combatMode == BattleMode.SeekAndDestroy || !TargetInCover())
					agent.SetDestination (currentTarget.position);
				else if(combatMode == BattleMode.ChaseOnSight && TargetInCover() && agent.remainingDistance == 0 && !searching)
						StartCoroutine(ReturnToPatrol());

				if (Vector3.Distance (currentTarget.position, transform.position) <= lookAtTargetDistance && !TargetInCover()) {
					Vector3 lookLook = new Vector3 (currentTarget.position.x, 0, currentTarget.position.z);

					targetRotation = Quaternion.LookRotation (lookLook - new Vector3 (transform.position.x, 0, transform.position.z));

					transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle (transform.eulerAngles.y, targetRotation.eulerAngles.y, agent.angularSpeed * Time.deltaTime);

				}
			}
		}

		#endregion

		#region Melee update
		if (currentTarget != null && troopType == Type.Melee) {
			
			if (meleeAttackTimer > 0)
				meleeAttackTimer -= Time.deltaTime;
			
			if (meleeAttackTimer < 0)
				meleeAttackTimer = 0;
			
			if (meleeAttackTimer == 0) {
				MeleeAttack ();
				meleeAttackTimer = meleeCoolDown;
			}
		}
		#endregion

		#region Ranged update (including reload animation part 1)
		if(troopType == Type.Ranged){
				ControlAIRanged ();

			if(reloading && anim == null || !reloadingAnim){
				FinishReload();

			}
		}
		#endregion
	}

	IEnumerator ReturnToPatrol(){
		searching = true;
		agent.SetDestination (transform.position);
		yield return new WaitForSeconds (waitAtPointTime*1.5f);
		chasing = false;
		if (isAGuard)
			agent.SetDestination (patrolTargets[currentPatrolTarget].position);
		searching = false;
	}
	
	#region Movement functions
	public IEnumerator TargetNextPatrol(){
		changingPatrolTarget = true;
		yield return new WaitForSeconds (waitAtPointTime);

		if (currentPatrolTarget < patrolTargets.Count-1)
			currentPatrolTarget ++;
		else if (currentPatrolTarget >= patrolTargets.Count-1)
			currentPatrolTarget = 0;

		if (patrolTargets.Count > 0 && currentPatrolTarget < patrolTargets.Count) {
			agent.SetDestination (patrolTargets [currentPatrolTarget].position);
		}
		changingPatrolTarget = false;
	}
	#endregion

	#region Melee functions (comments cover melee animation)
	private void MeleeAttack(){

		if(Vector3.Distance (currentTarget.position, transform.position)  <= stoppingDistance) {

																	//MELEE ANIMATION
			//If you are adding a melee animation, remove MeleeHit from this if statement and instead use this if statement to activate the attacking variable in the animator 
			//for example using "anim.SetBool ("MeleeAttacking", true);" and making the animator play the animation when the bool is true, 
			//setting it to false after the animation is complete.
			//NOTE that if you use a collider on the weapon to deal damage, the MeleeHit() function becomes obsolete
			if (Direction (currentTarget) > 0.7) {
				if (anim != null && meleeAttackAnim)
					anim.SetTrigger ("MeleeAttacking");
				else if (anim == null || !meleeAttackAnim)
					MeleeHit ();
			}
		}
	}

	void MeleeHit(){

        if(currentTarget.GetComponent<Entity>())
    		currentTarget.GetComponent<Entity>().TakeDamage(meleeDamage);
		
	}
	#endregion

	#region Ranged functions (including animations)

	void ControlAIRanged(){

		if(currentAmmoInMag == 0 && !reloading){
			Reload ();
		}

		if (currentTarget != null) {

			if (Vector3.Distance (currentTarget.position, transform.position) <= lookAtTargetDistance) {

				if (TargetInSight ()) {
					if (Direction (currentTarget) > 0.994) {
						GetComponent<UnityEngine.AI.NavMeshAgent> ().stoppingDistance = stoppingDistance;

						//GUN SHOOTING ANIMATION
						//If you want to add a firing animation, uncomment,
						//make the animation play while the bool is true, and call Shoot() in an animation event at the appropriate time, removing it from here
						//NOTE that if you use animations to fire, it is advisable to set your shotsPerMinute high enough so as to not interfere with the animation, 
						//as this variable was made simply to limit firing the gun without animations

						if(anim != null && shootingAnim)
							anim.SetBool ("Firing", true);
						else if(anim == null || !shootingAnim)
							Shoot ();
					}
				} else {
					if(anim != null && shootingAnim)
						anim.SetBool ("Firing", false);
				}

			}
		} else if (currentTarget == null && anim != null && shootingAnim)
			anim.SetBool ("Firing", false);
	}

	private void Shoot(){

		if (CanShoot ()) {
			foreach (Transform tran in firePoints) {
				Rigidbody newBullet = Instantiate (bullet, tran.position, Quaternion.Euler(tran.rotation.x, Random.Range(tran.eulerAngles.y - bulletSpread, tran.eulerAngles.y + bulletSpread), tran.rotation.z)) as Rigidbody;
				shot.Play();
				newBullet.GetComponent<Bullet> ().damage = bulletDamage;
				newBullet.AddForce (newBullet.transform.forward * bulletSpeed);


				nextPossibleShootTime = Time.time + secondsBetweenShots;
				currentAmmoInMag--;

			}
		}
	}

	public void Reload(){
		if (anim != null && reloadingAnim)
			anim.SetTrigger ("Reload");
		reloading = true;
	}

	public void FinishReload(){
		currentAmmoInMag = ammoPerMag;
		reloading = false;
	}

	#endregion


	#region Variable functions
	public float Direction(Transform tran){
		Vector3 dir = (tran.position - transform.position).normalized;
		float direction = Vector3.Dot (dir, transform.forward);

		return direction;
	}

	float TargetInFront(Vector3 side, Vector3 targetDir, Vector3 up) {
		Vector3 perp = Vector3.Cross(side, targetDir);
		float dir = Vector3.Dot(perp, up);

		if (dir > 0f) {
			return 1f;
		} else if (dir < 0f) {
			return -1f;
		} else {
			return 0f;
		}
	}
		
	private bool CanShoot(){
		bool canShoot = true;

		if(Time.time < nextPossibleShootTime){
			canShoot = false;
		}

		if(currentAmmoInMag == 0){
			canShoot = false;
		}

		if(reloading){
			canShoot = false;
		}

		return canShoot;
	}

	bool TargetInSight(){
		bool targetInSight = true;

		if (Direction(currentTarget) <= visionCone/10 || TargetInCover()) {
			targetInSight = false;
		}

		return targetInSight;
	}

	bool TargetInCover(){
		bool targetInCover = false;

		RaycastHit hit;

		Physics.Raycast (transform.position, (currentTarget.position - transform.position), out hit, Mathf.Infinity, mask);

		if (hit.transform != currentTarget)
			targetInCover = true;

		return targetInCover;
	}
	#endregion

	void OnDrawGizmos(){
		if (showRangeSphere) {
			Gizmos.color = Color.white;
			Gizmos.DrawWireSphere (transform.position, detectDistance);
		}
		if (showVisionCone) {
			float angle = ((visionCone)*9) + 270;
			Gizmos.color = Color.white;

			Gizmos.DrawRay (transform.position, Quaternion.Euler (0, angle, 0) * transform.forward * detectDistance);
			Gizmos.DrawRay (transform.position, Quaternion.Euler (0, -angle, 0) * transform.forward * detectDistance);
		}

		if (showKillCone) {
			float angle = ((killCone)*9) + 270;
			Gizmos.color = Color.red;

			Gizmos.DrawRay (transform.position, Quaternion.Euler (0, angle, 0) * transform.forward * detectDistance/6);
			Gizmos.DrawRay (transform.position, Quaternion.Euler (0, -angle, 0) * transform.forward * detectDistance/6);
		}
	}

	[ExecuteInEditMode]
	void OnValidate(){
		//EditorScript.AddTag ("Red");
		//EditorScript.AddTag ("Blue");
		meleeCoolDown = Mathf.Clamp (meleeCoolDown, 0, Mathf.Infinity);
		meleeDamage = Mathf.Clamp (meleeDamage, 0, Mathf.Infinity);
		waitAtPointTime = Mathf.Clamp (waitAtPointTime, 0, Mathf.Infinity);
		bulletDamage = Mathf.Clamp (bulletDamage, 0, Mathf.Infinity);
		bulletSpeed = Mathf.Clamp (bulletSpeed, 0, Mathf.Infinity);
		bulletSpread = Mathf.Clamp (bulletSpread, 0, Mathf.Infinity);
		shotsPerMinute = Mathf.Clamp (shotsPerMinute, 0, Mathf.Infinity);

		visionCone = Mathf.Clamp (visionCone, -10, 10);
		killCone = Mathf.Clamp (killCone, -10, 0);
	}
}
