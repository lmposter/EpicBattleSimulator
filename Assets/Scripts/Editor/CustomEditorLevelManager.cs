using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelManager))]
public class CustomEditorLevelManager : Editor {

	public SerializedProperty type, vT, cP, tK, eK, lTL;

	void OnEnable(){
		#if UNITY_EDITOR
		type = serializedObject.FindProperty ("typeOfLevel");
		vT = serializedObject.FindProperty ("victoryTexture");
		cP = serializedObject.FindProperty ("checkpoint");
		tK = serializedObject.FindProperty ("targetKills");
		eK = serializedObject.FindProperty ("enemiesKilled");
		lTL = serializedObject.FindProperty ("levelToLoad");
		#endif
	}

	public override void OnInspectorGUI(){
		#if UNITY_EDITOR
		serializedObject.Update ();
		EditorGUILayout.PropertyField (type);
		LevelManager.LevelType ty = (LevelManager.LevelType)type.enumValueIndex;
		EditorGUILayout.PropertyField (cP);

		switch( ty) {
		case LevelManager.LevelType.Battleground:

			EditorGUILayout.PropertyField (serializedObject.FindProperty ("targetKills"));
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("enemiesKilled"));
			break;
		}
		EditorGUILayout.PropertyField (vT);
		EditorGUILayout.PropertyField (lTL);

		serializedObject.ApplyModifiedProperties ();
		#endif
	}
}
