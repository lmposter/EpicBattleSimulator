using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour {

	private bool paused;
	public string quitLevel = "Main Menu";
	public GUISkin skin;

	void Start () {
		Time.timeScale = 1f;
		paused = false;
	}

	void OnGUI(){
		GUI.skin = skin;
		if (paused){
			GUILayout.BeginArea (new Rect (Screen.width / 2 - (Screen.width / 2) / 2, Screen.height / 3, Screen.width / 2, Screen.height));

			if (GUILayout.Button ("Resume", GUILayout.Height (Screen.height / 15))) {
				Time.timeScale = 1f;
				paused = false;
			}

			GUILayout.Space (Screen.height / 30);

			if (GUILayout.Button ("Quit", GUILayout.Height (Screen.height / 15))) {
				Time.timeScale = 1f;
				paused = false;
				SceneManager.LoadScene (quitLevel);
			}

			GUILayout.EndArea ();
		}
	}

	void Update () {
		
		if(Input.GetKeyDown(KeyCode.Escape)){
			
			if(Time.timeScale == 1f){
				Time.timeScale = 0;
				paused = true;
			}else if(Time.timeScale ==0f){
				Time.timeScale = 1f;
				paused = false;
			}
		}
	}
}
