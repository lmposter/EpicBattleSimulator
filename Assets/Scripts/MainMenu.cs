using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

	public string[] levels;
	public GUISkin skin;

	private bool levelSelecting;

	private Vector2 scrollPosition;

	void Start(){
		Cursor.visible = true;
	}

	void OnGUI ()
	{
		GUI.skin = skin;

		if (!levelSelecting) {
			GUILayout.BeginArea (new Rect (Screen.width / 2 - (Screen.width / 2) / 2, Screen.height / 4, Screen.width / 2, Screen.height));

			if (GUILayout.Button ("Start", GUILayout.Height (Screen.height / 15)))
				SceneManager.LoadScene (levels [0]);

			GUILayout.Space (Screen.height / 30);

			if (GUILayout.Button ("Level Select", GUILayout.Height (Screen.height / 15)))
				levelSelecting = true;

			GUILayout.Space (Screen.height / 30);

			if (GUILayout.Button ("Quit", GUILayout.Height (Screen.height / 15)))
				Application.Quit ();
			
			GUILayout.EndArea ();

		} else if (levelSelecting) {

			scrollPosition = GUI.BeginScrollView (new Rect (Screen.width / 2 - (Screen.width / 2) / 2, Screen.height / 4, Screen.width / 2, Screen.height / 1.8f), 
				scrollPosition, new Rect (0, 0, 0, ((levels.Length+1) * (Screen.height / 15)) + ((levels.Length+1) * (Screen.height/60))));

			GUILayout.Space (2);

			for (int index = 0; index < levels.Length; index++) {
				if (GUILayout.Button (levels [index], GUILayout.Width (Screen.width / 2), GUILayout.Height (Screen.height / 15)))
					SceneManager.LoadScene (levels [index]);
				GUILayout.Space (Screen.height/100);

			}
			if (GUILayout.Button ("Back", GUILayout.Width (Screen.width / 2), GUILayout.Height (Screen.height / 15)))
				levelSelecting = false;

			GUI.EndScrollView ();

		}
	}
}
