using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Spawner)), CanEditMultipleObjects]
public class CustomEditorSpawner : Editor {

	public SerializedProperty fT, tTS, sP, mTAO, sRMin, sRMax, wB, tIEW, nOW, sBW;

	void OnEnable(){
		#if UNITY_EDITOR

		fT = serializedObject.FindProperty ("factionTag");
		tTS = serializedObject.FindProperty ("troopsToSpawn");
		sP = serializedObject.FindProperty ("spawnPoints");
		mTAO = serializedObject.FindProperty ("maxTroopsAtOnce");
		sRMin = serializedObject.FindProperty ("spawnRateMin");
		sRMax = serializedObject.FindProperty ("spawnRateMax");
		wB = serializedObject.FindProperty ("waveBased");
		tIEW = serializedObject.FindProperty ("troopsInEachWave");
		nOW = serializedObject.FindProperty ("numberOfWaves");
		sBW = serializedObject.FindProperty ("secondsBetweenWaves");
		#endif
	}

	public override void OnInspectorGUI(){
		#if UNITY_EDITOR

		serializedObject.Update ();
		EditorGUILayout.PropertyField (fT);
		Show (tTS);
		Show (sP);
		EditorGUILayout.PropertyField (mTAO);
		EditorGUILayout.PropertyField (sRMin);
		EditorGUILayout.PropertyField (sRMax);
		EditorGUILayout.PropertyField (wB);
		bool wBTF = wB.boolValue;

		if(wBTF) {
			Show (tIEW);
			EditorGUILayout.PropertyField (sBW);

		}

		serializedObject.ApplyModifiedProperties ();
		#endif
	}

	void Show(SerializedProperty list){
		EditorGUILayout.PropertyField (list);
		EditorGUI.indentLevel += 1;
		if (list.isExpanded) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("Array.size"));
			for (int i = 0; i < list.arraySize; i++) {
				EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
			}
		}
		EditorGUI.indentLevel -= 1;
	}
}
