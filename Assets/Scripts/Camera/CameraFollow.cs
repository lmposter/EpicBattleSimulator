using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	private Vector3 camTarget;
	public Transform player;
	public float followHeight = 20;
	public float followSpeed = 8;

	void Update () {
		if (player != null) {
			camTarget = new Vector3 (player.position.x, player.position.y + followHeight, player.position.z);
			transform.position = Vector3.Lerp (transform.position, camTarget, Time.deltaTime * followSpeed);
		} else {
			Debug.Log ("Player is either dead or not assigned");
		}
	}

	[ExecuteInEditMode]
	void OnValidate(){
		if(player == null)
			player = GameObject.FindObjectOfType<PlayerController> ().transform;
	}
}
