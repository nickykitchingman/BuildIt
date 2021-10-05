using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[CustomEditor(typeof(SoundButtonManager))]
public class SoundButtonSettings : Editor
{
    SerializedProperty musicButton;
    SerializedProperty effectsButton;
    SerializedProperty musicSlider;
    SerializedProperty effectsSlider;

    SerializedProperty soundManager;

    [SerializeField] private Transform buttonParent;

    private void OnEnable()
    {
        findAUDIO();

        musicButton = serializedObject.FindProperty("MusicButton");
        effectsButton = serializedObject.FindProperty("EffectsButton");
        musicSlider = serializedObject.FindProperty("MusicSlider");
        effectsSlider = serializedObject.FindProperty("EffectsSlider");

        soundManager = serializedObject.FindProperty("soundManager");
    }

    private void findElements()
    {
        SoundButtonManager manager = (SoundButtonManager)target;
        if (buttonParent)
        {
            //Remove empty listeners
            removeEmptyListenersFromChildren(buttonParent.gameObject);

            //Music button
            if (manager.MusicButton == null)
            {
                Button[] buttons = buttonParent.GetComponentsInChildren<Button>(true);
                buttons = buttons.Where(f => f.name == "MusicButton").ToArray();
                if (buttons.Length > 0)
                {
                    serializedObject.FindProperty("MusicButton").objectReferenceValue = buttons.First();
                }
            }
            //Effects button
            if (manager.EffectsButton == null)
            {
                Button[] buttons = buttonParent.GetComponentsInChildren<Button>(true);
                buttons = buttons.Where(f => f.name == "EffectsButton").ToArray();
                if (buttons.Length > 0)
                {
                    serializedObject.FindProperty("EffectsButton").objectReferenceValue = buttons.First();
                }
            }
            //Music slider
            if (manager.MusicSlider == null)
            {
                Slider[] sliders = buttonParent.GetComponentsInChildren<Slider>(true);
                sliders = sliders.Where(f => f.name == "MusicSlider").ToArray();
                if (sliders.Length > 0)
                {
                    serializedObject.FindProperty("MusicSlider").objectReferenceValue = sliders.First();
                }
            }
            //Effects slider
            if (manager.EffectsSlider == null)
            {
                Slider[] sliders = buttonParent.GetComponentsInChildren<Slider>(true);
                sliders = sliders.Where(f => f.name == "EffectsSlider").ToArray();
                if (sliders.Length > 0)
                {
                    serializedObject.FindProperty("EffectsSlider").objectReferenceValue = sliders.First();
                }
            }
        }
    }

    private void addListenersToChildren()
    {
        SoundButtonManager manager = (SoundButtonManager)target;

        //Music Button
        if (manager.MusicButton)
        {
            //Check for calls to sound button manager
            bool skip = false;
            var onClick = manager.MusicButton.onClick;
            for (int i = 0; i < onClick.GetPersistentEventCount(); i++)
            {
                if (onClick.GetPersistentTarget(i) == manager.soundManager)
                    UnityEventTools.RemovePersistentListener(onClick, i);
                else if (onClick.GetPersistentTarget(i) == manager)
                    skip = true;
            }
            if (!skip)
            {
                //Add listener
                UnityAction action = new UnityAction(manager.MusicButtonHandler);
                UnityEventTools.AddPersistentListener(manager.MusicButton.onClick, action);
                EditorUtility.SetDirty(manager.MusicButton);
            }
        }
        //Effects button
        if (manager.EffectsButton)
        {
            //Check for calls to sound button manager
            bool skip = false;
            var onClick = manager.EffectsButton.onClick;
            for (int i = 0; i < onClick.GetPersistentEventCount(); i++)
            {
                if (onClick.GetPersistentTarget(i) == manager.soundManager)
                    UnityEventTools.RemovePersistentListener(onClick, i);
                else if (onClick.GetPersistentTarget(i) == manager)
                    skip = true;
            }
            if (!skip)
            {
                //Add listener
                UnityAction action = new UnityAction(manager.EffectsButtonHandler);
                UnityEventTools.AddPersistentListener(manager.EffectsButton.onClick, action);
                EditorUtility.SetDirty(manager.EffectsButton);
            }
        }
        //Music slider
        if (manager.MusicSlider)
        {
            //Check for calls to sound button manager
            bool skip = false;
            var onValueChange = manager.MusicSlider.onValueChanged;
            for (int i = 0; i < onValueChange.GetPersistentEventCount(); i++)
            {
                if (onValueChange.GetPersistentTarget(i) == manager.soundManager)
                    UnityEventTools.RemovePersistentListener(onValueChange, i);
                else if (onValueChange.GetPersistentTarget(i) == manager)
                    skip = true;
            }
            if (!skip)
            {
                //Add listener
                UnityAction<float> action = new UnityAction<float>(manager.SetMusicVolume);
                UnityEventTools.AddPersistentListener(manager.MusicSlider.onValueChanged, action);
                EditorUtility.SetDirty(manager.MusicSlider);
            }
        }
        //Effects slider
        if (manager.EffectsSlider)
        {
            //Check for calls to sound button manager
            bool skip = false;
            var onValueChange = manager.EffectsSlider.onValueChanged;
            for (int i = 0; i < onValueChange.GetPersistentEventCount(); i++)
            {
                if (onValueChange.GetPersistentTarget(i) == manager.soundManager)
                    UnityEventTools.RemovePersistentListener(onValueChange, i);
                else if (onValueChange.GetPersistentTarget(i) == manager)
                    skip = true;
            }
            if (!skip)
            {
                //Add listener
                UnityAction<float> action = new UnityAction<float>(manager.SetEffectsVolume);
                UnityEventTools.AddPersistentListener(manager.EffectsSlider.onValueChanged, action);
                EditorUtility.SetDirty(manager.EffectsSlider);
            }
        }
    }

    private void findAUDIO()
    {
        SoundButtonManager manager = (SoundButtonManager)target;

        if (manager.soundManager == null)
        {
            GameObject obj = GameObject.Find("AUDIO");
            if (obj)
            {
                var comp = obj.GetComponent<SoundManager>();
                serializedObject.FindProperty("soundManager").objectReferenceValue = comp;
                serializedObject.ApplyModifiedProperties();
            }
        }
    }

    private void removeEmptyListenersFromChildren(GameObject parent)
    {
        //Removes empty persistent listeners from every child button
        foreach (Button button in parent.GetComponentsInChildren<Button>(true))
            for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
                if (button.onClick.GetPersistentTarget(i) == null)
                    UnityEventTools.RemovePersistentListener(button.onClick, i);
        
        //Removes empty persistent listeners from every child slider
        foreach (Slider slider in parent.GetComponentsInChildren<Slider>(true))
            for (int i = 0; i < slider.onValueChanged.GetPersistentEventCount(); i++)
                if (slider.onValueChanged.GetPersistentTarget(i) == null)
                    UnityEventTools.RemovePersistentListener(slider.onValueChanged, i);
    }

    public override void OnInspectorGUI()
    {
        SoundButtonManager manager = (SoundButtonManager)target;

        //Editor buttons
        buttonParent = (Transform)EditorGUILayout.ObjectField("Button Parent", buttonParent, typeof(Transform), true);
        if (GUILayout.Button("Find Elements"))
            findElements();
        if (GUILayout.Button("Add Listeners"))
            addListenersToChildren();

        //Elements
        EditorGUILayout.PropertyField(musicButton, new GUIContent("Music Button"));
        EditorGUILayout.PropertyField(effectsButton, new GUIContent("Effects Button"));
        EditorGUILayout.PropertyField(musicSlider, new GUIContent("Music Slider"));
        EditorGUILayout.PropertyField(effectsSlider, new GUIContent("Effects Slider"));

        //Sound Manager
        EditorGUILayout.PropertyField(soundManager, new GUIContent("Sound Manager"));        
        
        //Colours
        manager.OnColour = EditorGUILayout.ColorField("On Colour", manager.OnColour);
        manager.OffColour = EditorGUILayout.ColorField("Off Colour", manager.OffColour);

        serializedObject.ApplyModifiedProperties();
    }
}
