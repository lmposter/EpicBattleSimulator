using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Entity))]
public class PlayerController : MonoBehaviour {
	AudioSource shot;
	[Header("Animation")]
	[Tooltip("If adding animations, place the animator holding the required animations here.")]
	public Animator anim;
	public bool hasAnimations;
	public bool movingAnim;
	public bool shootingAnim;
	public bool reloadingAnim;

	[Header("Movement Variables")]
	[Tooltip("The speed the player will rotate at to aim at the mouse.")]
	public float rotationSpeed = 450;
	public float walkSpeed = 4;
	public float runSpeed = 8;
	public float acceleration = 5;

	[Header("Stealth Variables")]
	[Tooltip("This object will scale depending on the speed of the player.")]
	public Transform noiseObject;
	[Tooltip("The radius of the Noise Object while the player is walking.")]
	public float walkingNoiseSize = 2.5f;
	[Tooltip("The radius of the Noise Object while the player is running.")]
	public float runningNoiseSize = 5f;
	[Tooltip("The speed at which the Noise Object scales.")]
	public float noiseChangeSpeed = 6f;

	public enum GunType {Auto, singleShot}

	[Header("Combat Variables")]

	public GunType gunType;
	[Tooltip("A bullet will fire from each of these transforms.")]
	public Transform[] firePoints;
	public Rigidbody bullet;
	public float bulletSpeed = 1000;
	public float bulletDamage = 2;
	public float shotsPerMinute = 60;

	public int maxAmmo = 300;
	[HideInInspector]
	public int ammoRemaining;
	public int ammoPerMag = 30;

	#region Private/Hidden variables

	[HideInInspector]
	public int currentAmmoInMag;

	private bool reloading;

	private float secondsBetweenShots;
	private float nextPossibleShootTime;

	private float speed;
	private bool sprinting;
	private float[] xY = { 0, 0 };
	private float scale = 0;

	private float fallSpeed = -Physics.gravity.y;
	private CharacterController controller;
	private Camera cam;

	private Quaternion targetRotation;
	private Vector3 currentVelocityMod;
	private Vector3 motion;
	#endregion

	void Start () {
		controller = GetComponent<CharacterController> ();
		cam = Camera.main;
		shot = GetComponent<AudioSource>();
		ammoRemaining = maxAmmo;
		secondsBetweenShots = 60 / shotsPerMinute;
		currentAmmoInMag = ammoPerMag;
	}
	
	void Update () {
		ControlMouse ();

		if ((Input.GetKeyDown (KeyCode.Mouse0))) {
			if (anim != null && shootingAnim && hasAnimations)
				anim.SetBool ("Firing", true);
			else if (anim == null || !shootingAnim)
				Shoot ();
				
		} else if ((Input.GetKey (KeyCode.Mouse0))) {
			if (anim != null && shootingAnim && hasAnimations)
				anim.SetBool ("Firing", true);
			else if (anim == null || !shootingAnim)
				ShootContinuos ();
				
		}

		if((Input.GetKeyDown (KeyCode.R))){
			if(CanReload()){
				if (anim != null && reloadingAnim && hasAnimations)
					anim.SetTrigger ("Reload");
				reloading = true;
			}
		}

		if (Input.GetKey (KeyCode.LeftShift)) {
			sprinting = true;
		} else
			sprinting = false;

		if (reloading && !reloadingAnim)
			FinishReload ();
	}

	#region Shooting section

	private void Shoot(){

		if (CanShoot ()) {
			foreach (Transform tran in firePoints) {
				Rigidbody newBullet = Instantiate (bullet, tran.position, Quaternion.Euler(tran.rotation.x, tran.eulerAngles.y, tran.rotation.z)) as Rigidbody;

				newBullet.GetComponent<Bullet> ().damage = bulletDamage;
				newBullet.AddForce (newBullet.transform.forward * bulletSpeed);
				shot.Play();

				nextPossibleShootTime = Time.time + secondsBetweenShots;
				currentAmmoInMag--;

			}
		}
	}

	public void ShootContinuos(){
		if(gunType == GunType.Auto){
			Shoot();
		}
	}

	private bool CanShoot(){
		bool canShoot = true;

		if(Time.time < nextPossibleShootTime || currentAmmoInMag == 0 || reloading){
			canShoot = false;
		}

		return canShoot;
	}
	#endregion

	#region Reloading section

	public bool CanReload(){
		if (ammoRemaining != 0 && currentAmmoInMag != ammoPerMag) {
			reloading = true;
			return true;
		} else
			return false;
	}

	public void FinishReload(){
		reloading = false;
		ammoRemaining -= (ammoPerMag - currentAmmoInMag);
		currentAmmoInMag = ammoPerMag;

		if(ammoRemaining < 0){
			currentAmmoInMag += ammoRemaining;
			ammoRemaining = 0;
		}
	}
	#endregion

	void ControlMouse(){

		Vector3 mousePos = Input.mousePosition;
		mousePos = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.transform.position.y - transform.position.y));
		targetRotation = Quaternion.LookRotation (mousePos - new Vector3(transform.position.x,0,transform.position.z));
		transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, rotationSpeed * Time.deltaTime);

		Vector3 input = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));

		currentVelocityMod = Vector3.MoveTowards (currentVelocityMod, input, acceleration * Time.deltaTime);

		motion = currentVelocityMod;
		motion *= (Mathf.Abs (input.x) == 1 && Mathf.Abs (input.z) == 1) ? 1 : 1;
		motion *= (Input.GetKey (KeyCode.LeftShift)) ? runSpeed : walkSpeed;
		motion += Vector3.up * -fallSpeed;

		controller.Move (motion * Time.deltaTime);

		if (anim != null && movingAnim && hasAnimations)
			anim.SetFloat ("Speed", controller.velocity.magnitude);

		#region Deals with scaling the sound object
		if (noiseObject != null) {
			if (motion.x < 0)
				xY [0] = -motion.x;
			else if (motion.x >= 0)
				xY [0] = motion.x;
			if (motion.z < 0)
				xY [1] = -motion.z;
			else if (motion.z >= 0)
				xY [1] = motion.z;

			speed = xY [0] + xY [1];

			if ((scale < walkingNoiseSize && speed > 0 && !sprinting) || (scale < runningNoiseSize && sprinting) && controller.velocity.magnitude != 0)
				scale += (noiseChangeSpeed) * Time.deltaTime;
			else if (speed < scale || !sprinting && scale > walkingNoiseSize + walkingNoiseSize / 10)
				scale -= (noiseChangeSpeed) * Time.deltaTime;
			else if (scale < walkingNoiseSize + walkingNoiseSize / 10 && scale > walkingNoiseSize / walkingNoiseSize / 10 && !sprinting)
				scale = walkingNoiseSize;
			else if (scale < runningNoiseSize + runningNoiseSize / 10 && scale > runningNoiseSize / runningNoiseSize / 10 && sprinting)
				scale = runningNoiseSize;

			noiseObject.localScale = new Vector3 (scale, noiseObject.localScale.y, scale);
		}else if(noiseObject == null)
			return;
		#endregion
	}

	[ExecuteInEditMode]
	void OnValidate(){
		walkingNoiseSize = Mathf.Clamp (walkingNoiseSize, 0, walkSpeed-0.1f);
		runningNoiseSize = Mathf.Clamp (runningNoiseSize, 0, runSpeed-0.1f);
	}

}
