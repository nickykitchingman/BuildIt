using System;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AddClicks : EditorWindow
{
    public SoundManager EffectManager;
    public GameObject parent;

    public string effect = "Button Click";

    [MenuItem("Window/Add Clicks")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(AddClicks));
    }

    public void AddClicksToChildren()
    {
        //Add a persistent listener of play sound effect to every child button
        if (EffectManager)
            foreach (Button button in parent.GetComponentsInChildren<Button>(true))
            {
                //Check for calls to sound manager
                bool skip = false;
                for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
                    if (button.onClick.GetPersistentTarget(i) == EffectManager)
                        skip = true;
                if (skip)
                    continue;

                //Add listener
                UnityAction<string> action = new UnityAction<string>(EffectManager.PlaySoundEffect);
                UnityEventTools.AddStringPersistentListener(button.onClick, action, effect);
                EditorUtility.SetDirty(button);
            }
    }
    
    public void RemoveClicksFromChildren()
    {
        //Removes a persistent listener of play sound effect from every child button
        if (EffectManager)
            foreach (Button button in parent.GetComponentsInChildren<Button>(true))
            {
                UnityAction<string> action = new UnityAction<string>(EffectManager.PlaySoundEffect);
                UnityEventTools.RemovePersistentListener(button.onClick, action);
                EditorUtility.SetDirty(button);
            }
    }

    public void RemoveEmptyClicksFromChildren()
    {
        //Removes empty persistent listeners from every child button
        foreach (Button button in parent.GetComponentsInChildren<Button>(true))
            for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
                if (button.onClick.GetPersistentTarget(i) == null)
                    UnityEventTools.RemovePersistentListener(button.onClick, i);
    }

    private void OnEnable()
    {
        findAUDIO();
    }

    private void findAUDIO()
    {
        if (EffectManager == null)
        {
            GameObject obj = GameObject.Find("AUDIO");
            if (obj)
                EffectManager = obj.GetComponent<SoundManager>();
        }
    }

    void OnGUI()
    {
        parent = (GameObject)EditorGUILayout.ObjectField("Object Parent", parent, typeof(GameObject), true);
        EffectManager = (SoundManager)EditorGUILayout.ObjectField("Sound Manager Script", EffectManager, typeof(SoundManager), true);

        //String parameter
        effect = EditorGUILayout.TextField("Effect", effect);

        //Add or remove clicks
        if (GUILayout.Button("Add Clicks To Children"))
            AddClicksToChildren();
        if (GUILayout.Button("Remove Clicks From Children"))
            RemoveClicksFromChildren();
        if (GUILayout.Button("Remove Empty Clicks From Children"))
            RemoveEmptyClicksFromChildren();
    }
}
