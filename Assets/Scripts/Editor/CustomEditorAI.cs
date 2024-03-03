using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AI)), CanEditMultipleObjects]
public class CustomEditorAI : Editor {

	public SerializedProperty cM, targetTags, cT, iAG, troopType, m, kC, sRS, sVC, sKC, hasAnim, anim, anim1, anim2, anim3, anim4, wS, rS, vC, dD, 
								lATD, sD, c, mCD, mD, fP, b, bSpeed, bSpread, bD, sPM, aPM, wAPT, pT;

	void OnEnable(){
		#if UNITY_EDITOR
		cM = serializedObject.FindProperty ("combatMode");
		targetTags = serializedObject.FindProperty ("targetTags");
		cT = serializedObject.FindProperty ("currentTarget");
		troopType = serializedObject.FindProperty ("troopType");
		m = serializedObject.FindProperty ("mask");
		kC = serializedObject.FindProperty ("killCone");

		sRS = serializedObject.FindProperty ("showRangeSphere");
		sVC = serializedObject.FindProperty ("showVisionCone");
		sKC = serializedObject.FindProperty ("showKillCone");

		hasAnim = serializedObject.FindProperty ("hasAnimations");
		anim = serializedObject.FindProperty ("anim");
		anim1 = serializedObject.FindProperty ("movingAnim");
		anim2 = serializedObject.FindProperty ("meleeAttackAnim");
		anim3 = serializedObject.FindProperty ("shootingAnim");
		anim4 = serializedObject.FindProperty ("reloadingAnim");

		iAG = serializedObject.FindProperty ("isAGuard");
		wS = serializedObject.FindProperty ("walkSpeed");
		rS = serializedObject.FindProperty ("runSpeed");

		vC = serializedObject.FindProperty ("visionCone");
		dD = serializedObject.FindProperty ("detectDistance");
		lATD = serializedObject.FindProperty ("lookAtTargetDistance");
		sD = serializedObject.FindProperty ("stoppingDistance");
		c = serializedObject.FindProperty ("chasing");

		mCD = serializedObject.FindProperty ("meleeCoolDown");
		mD = serializedObject.FindProperty ("meleeDamage");

		fP = serializedObject.FindProperty ("firePoints");
		b = serializedObject.FindProperty ("bullet");
		bSpeed = serializedObject.FindProperty ("bulletSpeed");
		bSpread = serializedObject.FindProperty ("bulletSpread");
		bD = serializedObject.FindProperty ("bulletDamage");
		sPM = serializedObject.FindProperty ("shotsPerMinute");
		aPM = serializedObject.FindProperty ("ammoPerMag");

		wAPT = serializedObject.FindProperty ("waitAtPointTime");
		pT = serializedObject.FindProperty ("patrolTargets");
		#endif
	}

	public override void OnInspectorGUI(){
		#if UNITY_EDITOR
		try{
		AI.Type tType = (AI.Type)troopType.enumValueIndex;

		serializedObject.Update ();
		EditorGUILayout.PropertyField (cM);
		Show (targetTags, true);
		EditorGUILayout.PropertyField (cT);
		EditorGUILayout.PropertyField (troopType);
		EditorGUILayout.PropertyField (m);
		EditorGUILayout.PropertyField (kC);
		EditorGUILayout.PropertyField (sRS);
		EditorGUILayout.PropertyField (sVC);
		EditorGUILayout.PropertyField (sKC);

		EditorGUILayout.PropertyField (hasAnim);
		bool hasAnimations = hasAnim.boolValue;
		if (hasAnimations) {
			EditorGUILayout.PropertyField (anim);
			EditorGUILayout.PropertyField (anim1);
			if (tType == AI.Type.Melee)
				EditorGUILayout.PropertyField (anim2);
			else if (tType == AI.Type.Ranged) {
				EditorGUILayout.PropertyField (anim3);
				EditorGUILayout.PropertyField (anim4);
			}
		}
		bool iag = iAG.boolValue;

		EditorGUILayout.PropertyField (wS);
		EditorGUILayout.PropertyField (rS);
		EditorGUILayout.PropertyField (lATD);
		EditorGUILayout.PropertyField (sD);
		EditorGUILayout.PropertyField (iAG);

		if(iag) {
			EditorGUILayout.PropertyField (wAPT);
			Show (pT, true);
			serializedObject.ApplyModifiedProperties ();
		}

		EditorGUILayout.PropertyField (vC);
		EditorGUILayout.PropertyField (dD);
		EditorGUILayout.PropertyField (c);

		switch (tType) {
		case AI.Type.Melee:
			EditorGUILayout.PropertyField (mCD);
			EditorGUILayout.PropertyField (mD);
			break;
		case AI.Type.Ranged:
			Show (fP, true);
			EditorGUILayout.PropertyField (b);
			EditorGUILayout.PropertyField (bSpeed);
			EditorGUILayout.PropertyField (bSpread);
			EditorGUILayout.PropertyField (bD);
			EditorGUILayout.PropertyField (sPM);
			EditorGUILayout.PropertyField (aPM);
			break;
		}
		}catch(System.Exception){}
		serializedObject.ApplyModifiedProperties ();
		#endif
	}

	void Show(SerializedProperty list, bool showAmount){
		EditorGUILayout.PropertyField (list);
		EditorGUI.indentLevel += 1;
		if (list.isExpanded) {
			if(showAmount)
				EditorGUILayout.PropertyField (list.FindPropertyRelative ("Array.size"));
	
			for (int i = 0; i < list.arraySize; i++) {
				EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (i));
			}
		}
		EditorGUI.indentLevel -= 1;
	}
}
