using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerNoise : MonoBehaviour {

	void OnTriggerEnter(Collider other){
		if (other.GetComponent<AI>() && other.GetComponent<AI> ().combatMode == AI.BattleMode.ChaseOnSight) {
			other.GetComponent<AI> ().chasing = true;
		}
	}
}
