using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Waypoint : MonoBehaviour {

	void OnTriggerEnter(Collider other){
		if(other.GetComponent<AI>()){
			AI ai = other.GetComponent<AI> ();
			if (ai.isAGuard && ai.patrolTargets.Contains (transform) && !ai.changingPatrolTarget)
				ai.StartCoroutine (ai.TargetNextPatrol());
		}
	}
}
