using UnityEngine;
using System.Collections;

public class MeleeWeapon : MonoBehaviour {

	void OnTriggerEnter(Collider other){
		if (other.GetComponent<Entity> () && other.transform != transform.root && other.tag != transform.root.tag) {
			other.GetComponent<Entity> ().TakeDamage (transform.root.GetComponent<AI>().meleeDamage);
		}
	}
}
