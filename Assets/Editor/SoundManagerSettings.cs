using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SoundManager))]
public class SoundManagerSettings : Editor
{
    private void OnEnable()
    {
        addNewClips();
        findSources();
    }

    private void addNewClips()
    {
        SoundManager manager = (SoundManager)target;

        //Find new clips
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Audio/Effects");
        clips = clips.Except(manager.SoundEffects.Select(f => f.Sound)).ToArray();
        if (clips.Count() > 0)
        {
            addClips(clips);
        }
    }

    private void addClips(AudioClip[] clips)
    {
        SoundManager manager = (SoundManager)target;

        List<SoundManager.SoundEffect> newEffects = new List<SoundManager.SoundEffect>();
        newEffects.AddRange(manager.SoundEffects);

        //Add every clip
        foreach (AudioClip clip in clips)
        {
            //Set default name
            var effect = new SoundManager.SoundEffect();
            effect.Sound = clip;
            effect.Name = clip.name;
            newEffects.Add(effect);
        }

        manager.SoundEffects = newEffects.ToArray();
        EditorUtility.SetDirty(manager);
    }

    private void renameClips()
    {
        SoundManager manager = (SoundManager)target;

        var effects = manager.SoundEffects;
        for (int i = 0; i < effects.Length; i++)
            effects[i].Name = effects[i].Sound.name;
    }

    private void findSources()
    {
        SoundManager manager = (SoundManager)target;

        if (manager.MusicSource == null)
        {
            var sources = manager.GetComponentsInChildren<AudioSource>(true);
            sources = sources.Where(f => f.gameObject.name == "Music").ToArray();
            if (sources.Length > 0)
                manager.MusicSource = sources.First();
        }
        if (manager.EffectSource == null)
        {
            var sources = manager.GetComponentsInChildren<AudioSource>(true);
            sources = sources.Where(f => f.gameObject.name == "Effects").ToArray();
            if (sources.Length > 0)
                manager.EffectSource = sources.First();
        }
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Rename Sound Effects"))
            renameClips();

        if (GUILayout.Button("Refresh Sound Effects"))
            addNewClips();

        base.OnInspectorGUI();
    }
}
