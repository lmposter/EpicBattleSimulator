using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour {

	[HideInInspector]
	public float damage;

	public float destroyTime = 4;

	void Start(){
		Destroy (gameObject, destroyTime);
	}

	void OnCollisionEnter(Collision other){

		if (other.collider.GetComponent<AI> () && other.collider.GetComponent<AI> ().Direction (transform) <= other.collider.GetComponent<AI> ().killCone / 10) {
			other.collider.GetComponent<Entity> ().TakeDamage (other.collider.GetComponent<Entity> ().maxHealth);
		}
		else if (other.collider.GetComponent<AI> () && other.collider.GetComponent<AI> ().Direction (transform) > other.collider.GetComponent<AI> ().killCone/10) {
			other.collider.GetComponent<Entity> ().TakeDamage (damage);
			other.collider.GetComponent<AI> ().chasing = true;
		}
		else if (other.collider.GetComponent<Entity> ()) {
			other.collider.GetComponent<Entity> ().TakeDamage (damage);
		}

		Destroy (gameObject);
	}
}

