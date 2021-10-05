using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AddClicks))]
public class AddClickSettings : Editor
{
    SerializedProperty SoundManagerScript;

    private void OnEnable()
    {
        SoundManagerScript = serializedObject.FindProperty("EffectManager");
        findAUDIO();
    }

    private void findAUDIO()
    {
        AddClicks clickScript = (AddClicks)target;
        if (clickScript.EffectManager == null)
        {
            GameObject obj = GameObject.Find("AUDIO");
            if (obj)
                clickScript.EffectManager = obj.GetComponent<SoundManager>();
        }
    }

    public override void OnInspectorGUI()
    {        
        AddClicks clickScript = (AddClicks)target;

        EditorGUILayout.PropertyField(SoundManagerScript, new GUIContent("Sound Manager Script"));
        serializedObject.ApplyModifiedProperties();

        //String parameter
        clickScript.effect = EditorGUILayout.TextField("Effect", clickScript.effect);

        //Add or remove clicks
        if (GUILayout.Button("Add Clicks To Children"))
            clickScript.AddClicksToChildren();
        if (GUILayout.Button("Remove Clicks From Children"))
            clickScript.RemoveClicksFromChildren();
        if (GUILayout.Button("Remove Empty Clicks From Children"))
            clickScript.RemoveEmptyClicksFromChildren();
    }
}
