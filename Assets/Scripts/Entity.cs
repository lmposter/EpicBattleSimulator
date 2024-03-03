using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour {
	
	public float maxHealth = 10;

	public float currentHealth;
	public GameObject deathPrefab; // attach the prefab in Unity Editor
	bool dead;

	void Start(){
		
		currentHealth = maxHealth;
	}

	public void TakeDamage(float dmg){
		currentHealth -= dmg;

		if (currentHealth <= 0) {
			currentHealth = 0;
			Die ();
		} else if (currentHealth > maxHealth)
			currentHealth = maxHealth;
	}

	public void Die(){
		if (!dead) {
			dead = true;
			if (GetComponent<AI> () && GameObject.FindObjectOfType<LevelManager>() && GameObject.FindObjectOfType<LevelManager>().typeOfLevel == LevelManager.LevelType.Battleground) {
				foreach (string ing in GetComponent<AI>().targetTags)
					if (ing == "Player")
						GameObject.FindObjectOfType<LevelManager> ().enemiesKilled++;
			}
			if(deathPrefab != null)
				Instantiate (deathPrefab, transform.position, transform.rotation);
			if (gameObject.tag != "Player")
				Destroy (gameObject);
			else
				
				GetComponent<PlayerController> ().ammoRemaining=90;
				GetComponent<PlayerController> ().currentAmmoInMag=30;
				gameObject.SetActive (false);
				dead = false;
				
				
		}
	}
}
