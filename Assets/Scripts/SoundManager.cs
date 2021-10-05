using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource MusicSource;
    public AudioSource EffectSource;
    public SoundEffect[] SoundEffects;

    private float delay = 3;

    [Serializable]
    public struct SoundEffect
    {
        public string Name;
        public AudioClip Sound;
    }

    private void Start()
    {
        if (MusicSource)
        {
            if (GameData.FirstLoad && GameData.Mode != "menu")
            {
                MusicSource.Stop();
                MusicSource.timeSamples = 0;
                StartCoroutine(DelayMusicStart(delay));
            }

            if (GameData.Mode != "menu")
                if (GameData.SceneExitTime == 0)
                    MusicSource.timeSamples = GameData.MusicTime;
                else
                    MusicSource.timeSamples = (int)(GameData.MusicTime + (Time.time - GameData.SceneExitTime) / 44100f);

            MusicSource.mute = !GameData.MusicOn;
            if (!GameData.MusicOn)
                MusicSource.volume = 0;
            else
                MusicSource.volume = GameData.MusicVolume;
        }

        if (EffectSource)
        {
            EffectSource.volume = GameData.EffectsVolume;
        }
    }

    private bool fade = false;
    private bool fadeDirOut = false;
    private float fadeTime = 1f;
    private void Update()
    {
        if (MusicSource)
            if (GameData.Mode != "menu")
                GameData.MusicTime = MusicSource.timeSamples;

        if (fade)
        {
            if (fadeDirOut && MusicSource.volume > 0)
                MusicSource.volume -= (Time.deltaTime / (fadeTime + 1) * GameData.MusicVolume);
            else if (!fadeDirOut && MusicSource.volume < GameData.MusicVolume)
                MusicSource.volume += (Time.deltaTime / (fadeTime + 1) * GameData.MusicVolume);
            else
                fade = false;

            if (MusicSource.volume < 0)
                MusicSource.volume = 0;
            else if (MusicSource.volume > GameData.MusicVolume)
                MusicSource.volume = GameData.MusicVolume;
        }
    }

    private void OnDestroy()
    {
        GameData.SceneExitTime = Time.time;
    }

    private Coroutine muteCall;
    public void PlayBackgroundMusic(bool value)
    {
        if (MusicSource)
        {
            fade = true;
            fadeDirOut = !value;
            if (muteCall != null)
                StopCoroutine(muteCall);
            if (value)
                MusicSource.mute = false;
            else
                muteCall = StartCoroutine(MuteSource(MusicSource));
        }
        else
            Debug.Log("No audio source");
    }

    public void PlayBackgroundMusic(bool value, float fadeTime)
    {
        this.fadeTime = fadeTime;
        PlayBackgroundMusic(value);
    }

    public IEnumerator MuteSource(AudioSource source)
    {
        yield return new WaitForSecondsRealtime(fadeTime + 4);
        source.mute = true;
    }

    public IEnumerator DelayMusicStart(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        Debug.Log(MusicSource.timeSamples);
        MusicSource.Play();
        if (GameData.MusicOn) {
            MusicSource.volume = 0;
            PlayBackgroundMusic(true, 0);
        }
    }

    public void PlaySoundEffect(string name)
    {
        PlaySoundEffect(name, false);
    }

    public void PlaySoundEffect(string name, bool ignoreState)
    {
        if (GameData.EffectsOn || ignoreState)
        {
            var effects = SoundEffects.Where(f => f.Name == name).Select(g => g.Sound);
            if (effects.Count() > 0)
            {
                EffectSource.PlayOneShot(effects.First(), GameData.EffectsVolume);
            }
        }
    }

    public float MusicVolume
    {
        get
        {
            return GameData.MusicVolume;
        }
        set
        {
            value = Mathf.Clamp01(value);
            if (MusicSource)
            {
                MusicSource.volume = value;
                GameData.MusicVolume = value;
            }
            else
                Debug.Log("No audio source");
        }
    }
}
