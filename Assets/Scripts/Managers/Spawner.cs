using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {

	[Header("Setup variables")]
	[Tooltip("The tag of the troops this spawner instantiates")]
	public string factionTag;

	[Tooltip("The different troops that can be spawned, they will be randomly selected to spawn")]
	public GameObject[] troopsToSpawn;
	[Tooltip("The number of possible spawn points for troops, they will be spawned at a random point within this array")]
	public Transform[] spawnPoints;

	[Tooltip("Maximum number of troops that can be in game at once")]
	public int maxTroopsAtOnce = 10;

	[Header("Spawn rate")]
	public float spawnRateMin = 1;
	public float spawnRateMax = 8;

	[Header("Wave setup")]

	[Tooltip("If false there will be no break between waves, effectively causing a non stop battle")]
	public bool waveBased = true;

	[Tooltip("Number of troops are in each wave")]
	public int[] troopsInEachWave = {5,10,15};

	private int numberOfWaves;

	public float secondsBetweenWaves = 10;

	private float troopDetectDistance = Mathf.Infinity;

	[HideInInspector]
	public int troopsSpawned;
	private bool waveSpawned;
	[HideInInspector]
	public bool waveOver;
	[HideInInspector]
	public int currentWave = 0;
	[HideInInspector]
	public float currentSeconds;

	// Use this for initialization
	void Start () {
		numberOfWaves = troopsInEachWave.Length;
		StartCoroutine (SpawnEnemy ());
		currentSeconds = secondsBetweenWaves;
	}

	void Update(){
		if (waveBased) {
			if (troopsSpawned >= troopsInEachWave [currentWave] && waveBased && !waveSpawned)
				waveSpawned = true;

			if (waveSpawned && GameObject.FindGameObjectsWithTag (factionTag).Length == 0 && !waveOver) {
				waveOver = true;
			}

			else if (currentWave == numberOfWaves - 1 && waveOver)
				Destroy (gameObject);
			else if (waveOver && currentSeconds > 0)
				currentSeconds -= Time.deltaTime;
			else if (currentSeconds <= 0 && waveOver) {
				waveOver = false;
				waveSpawned = false;
				troopsSpawned = 0;
				currentWave++;
				currentSeconds = secondsBetweenWaves;
			}

		}
	}

	IEnumerator SpawnEnemy(){
		yield return new WaitForSeconds(Random.Range(spawnRateMin,spawnRateMax));
		if (GameObject.FindGameObjectsWithTag(factionTag).Length < maxTroopsAtOnce && (!waveBased || (troopsSpawned < troopsInEachWave[currentWave] && !waveSpawned))) {
			Transform sP = spawnPoints [Random.Range (0, spawnPoints.Length)];
			GameObject newEnemy = Instantiate (troopsToSpawn [Random.Range (0, troopsToSpawn.Length)], sP.position, sP.rotation) as GameObject;
			newEnemy.GetComponent<AI> ().detectDistance = troopDetectDistance;
			newEnemy.GetComponent<AI> ().combatMode = AI.BattleMode.SeekAndDestroy;
			troopsSpawned++;
		}
		StartCoroutine (SpawnEnemy ());
	}

	IEnumerator WaitBetweenWaves(){
		waveOver = true;
		yield return new WaitForSeconds (secondsBetweenWaves);
		troopsSpawned = 0;
		waveSpawned = false;
		waveOver = false;
		currentWave++;
		if (currentWave > numberOfWaves)
			Destroy (gameObject);
		else
			StartCoroutine (SpawnEnemy ());
	}
}
