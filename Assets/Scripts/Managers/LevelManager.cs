using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

	public enum LevelType{Battleground, Stealth, Waves};

	public LevelType typeOfLevel;

	public Transform checkpoint;
	[Tooltip("The texture that will appear on screen when the level has been completed")]
	public Texture victoryTexture;

	public int targetKills;
	public int enemiesKilled;

	[Tooltip("The level to load upon completion of the level")]
	public string levelToLoad;

	private bool dead;
	[HideInInspector]
	public bool won;

	// Update is called once per frame
	void Update () {

		if (GameObject.FindGameObjectsWithTag ("Player").Length == 0 && !dead) {
			dead = true;
			StartCoroutine (Respawn ());
		}
		if (typeOfLevel == LevelType.Waves && GameObject.FindGameObjectsWithTag ("Spawner").Length == 0 && !won) {
			StartCoroutine (Winning ());
		}
		else
			if(typeOfLevel == LevelType.Stealth && GameObject.FindGameObjectsWithTag("Red").Length == 0 && !won){
			StartCoroutine (Winning ());
		}
		else
			if(typeOfLevel == LevelType.Battleground && enemiesKilled == targetKills && !won){
			StartCoroutine (Winning ());
		}
	}

	IEnumerator Winning(){
		won = true;
		yield return new WaitForSecondsRealtime (3);
		SceneManager.LoadScene (levelToLoad);
	}

	IEnumerator Respawn(){
		yield return new WaitForSecondsRealtime (3);
		PlayerController pC = Camera.main.GetComponent<CameraFollow> ().player.GetComponent<PlayerController>();
		pC.transform.position = checkpoint.position;
		pC.GetComponent<Entity> ().currentHealth = pC.GetComponent<Entity> ().maxHealth;
		pC.gameObject.SetActive (true);

		if (typeOfLevel != LevelType.Stealth)
			pC.transform.Find ("NoiseObject").gameObject.SetActive (false);
		dead = false;
	}
}
