using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CameraFollow))]
public class HUD : MonoBehaviour {

	private CameraFollow cF;
	private PlayerController pC;
	private Entity ent;

	#region Setup variables
	public GUISkin skin;
	public bool aimingReticle = true;
	public bool showLevelInfo;
	public Texture victoryTexture;
	#endregion

	#region HP variables
	private float hpBarLength;
	private float hpBarPosX = 10;
	private float hpBarPosY = 10;
	private float hpFillBarLength;
	#endregion

	#region Ammo variables
	private float ammoBoxPosX;
	private float ammoBoxPosY;
	private float ammoBoxWidth;
	private float ammoBoxHeight;
	#endregion

	private float levelInfoWidth;
	private float levelInfoHeight;

	private LevelManager lM;
	#region Setting variables in start function
	// Use this for initialization
	void Start () {
		lM = GameObject.FindObjectOfType<LevelManager> ();
		cF = GetComponent<CameraFollow> ();
		if (cF.player != null) {
			pC = cF.player.GetComponent<PlayerController> ();
			ent = pC.GetComponent<Entity> ();
		}

		hpBarLength = Screen.width / 3;

		ammoBoxPosX = Screen.width / 60;
		ammoBoxPosY = Screen.height / 1.07f;

		ammoBoxWidth = Screen.width / 16;
		ammoBoxHeight = Screen.height / 20;

		levelInfoWidth = Screen.width / 8;
		levelInfoHeight = Screen.height / 21;
	}
	#endregion

	#region Update function
	void Update () {
		if (pC == null) {
			if (cF.player != null) {
				pC = cF.player.GetComponent<PlayerController> ();
				ent = pC.GetComponent<Entity> ();
			}
		}
		AdjustLengthHP ();

		Vector3 mousePos = Input.mousePosition;

		if (mousePos.x > Screen.width || mousePos.x < 0 || mousePos.y < 0 || mousePos.y > Screen.height)
			Cursor.visible = true;
		else
			Cursor.visible = false;
	}
	#endregion

	void AdjustLengthHP(){
		hpFillBarLength = (hpBarLength) * (ent.currentHealth / ent.maxHealth);
		if (hpFillBarLength < 35)
			hpFillBarLength = 35;
		if (hpFillBarLength >= hpBarLength - 10)
			hpFillBarLength = hpBarLength - 10;
	}

	void OnGUI(){
		if(skin != null)
			GUI.skin = skin;

		DisplayHP ();
		DisplayAmmo ();

		if (aimingReticle)
			DisplayReticle ();

		if (showLevelInfo) {
			if (lM.typeOfLevel == LevelManager.LevelType.Battleground) {
				DisplayBlueTroops ();
				DisplayRedTroops ();
				DisplayKills ();
			} else if (lM.typeOfLevel == LevelManager.LevelType.Waves) {
				DisplayWaveInfo ();
			} else if (lM.typeOfLevel == LevelManager.LevelType.Stealth) {
				DisplayRedTroops ();
			}
		}

		if(lM.won)
			GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), victoryTexture);
	}

	#region HUD functions
	void DisplayReticle(){
		GUI.Box (new Rect (Input.mousePosition.x-5, Screen.height - Input.mousePosition.y-5, 10, 10), "");
	}

	void DisplayBlueTroops(){
		GUI.Box (new Rect (Screen.width / 2 - 100, levelInfoHeight, 80, 25), "Allies: " + GameObject.FindGameObjectsWithTag("Blue").Length);
	}
	void DisplayRedTroops(){
		if(lM.typeOfLevel == LevelManager.LevelType.Battleground)
			GUI.Box (new Rect (Screen.width / 2 + 20, levelInfoHeight, 80, 25), "Axis: " + GameObject.FindGameObjectsWithTag("Red").Length);
		else if(lM.typeOfLevel == LevelManager.LevelType.Stealth)
			GUI.Box (new Rect (Screen.width / 2 - 40, levelInfoHeight, 80, 25), "Reds: " + GameObject.FindGameObjectsWithTag("Red").Length);
	}

	//void DisplayKills(){
		//GUI.Box (new Rect (Screen.width / 2 -40, levelInfoHeight*2+5, 80, 25), "Kills: " + lM.enemiesKilled + "/" + lM.targetKills);
	//}
	void DisplayKills(){
		GUI.Box (new Rect (Screen.width / 2 -75, levelInfoHeight*2+5, 150, 25), "Enemy Casualties: " + lM.enemiesKilled);
	}

	void DisplayHP(){

		GUI.Box (new Rect(hpBarPosX, hpBarPosY, hpBarLength, Screen.height/20),"");
		GUI.Box (new Rect((hpBarPosX + 5), (hpBarPosY + 5), hpFillBarLength, Screen.height/20 - 5), ent.currentHealth + "/" + ent.maxHealth);
	}

	void DisplayAmmo(){

		GUI.Box (new Rect (ammoBoxPosX - 10, ammoBoxPosY - ammoBoxHeight + 2, ammoBoxWidth + 30, ammoBoxHeight - 0), "Garbage Gun");

		GUI.Box (new Rect (ammoBoxPosX, ammoBoxPosY, ammoBoxWidth, ammoBoxHeight), 
			pC.currentAmmoInMag + "/" + pC.ammoRemaining);
	}

	void DisplayWaveInfo(){
		Spawner spawnerToShow = GameObject.FindObjectOfType<Spawner> ();
		if (!spawnerToShow.waveOver)
			GUI.Box (new Rect (Screen.width / 2 - levelInfoWidth/2, 10, levelInfoWidth, levelInfoHeight), "Troops left in wave: " + 
				(spawnerToShow.troopsInEachWave[spawnerToShow.currentWave] - spawnerToShow.troopsSpawned + GameObject.FindGameObjectsWithTag (spawnerToShow.factionTag).Length).ToString ());
		else if (spawnerToShow.waveOver) {
			GUI.Box (new Rect (Screen.width / 2 - levelInfoWidth/2, 10, levelInfoWidth, levelInfoHeight), "Time until next wave: " + spawnerToShow.currentSeconds.ToString ("F0"));
		}
	}
	#endregion
}
