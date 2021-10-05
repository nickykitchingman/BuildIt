using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundButtonManager : MonoBehaviour
{
    public Button MusicButton;
    public Button EffectsButton;
    public Slider MusicSlider;
    public Slider EffectsSlider;
    public SoundManager soundManager;

    public Color OnColour = new Color(78f, 204f, 69f);
    public Color OffColour = new Color(35f, 209f, 202f);

    private void Start()
    {
        //Initial appearance
        setMusic(GameData.MusicOn, false);
        setEffects(GameData.EffectsOn, false);
        if (MusicSlider)
            MusicSlider.value = oldMusicValue = GameData.MusicVolume;
        if (EffectsSlider)
            EffectsSlider.value = oldEffectsValue = GameData.EffectsVolume;
    }

    public void MusicButtonHandler()
    {
        setMusic(!GameData.MusicOn);
    }

    public void EffectsButtonHandler()
    {
        setEffects(!GameData.EffectsOn);
    }

    private void setMusic(bool value, bool playSound)
    {
        //Activate music
        GameData.MusicOn = value;
        soundManager.PlayBackgroundMusic(value);

        //Change appearance
        if (MusicButton)
        {
            var text = MusicButton.GetComponentInChildren<TextMeshProUGUI>();
            if (text)
            {
                text.text = value ? "Sound On" : "Sound Off";
                text.color = value ? OnColour : OffColour;
            }
        }
        else
            Debug.Log("No music button");
    }
    private void setMusic(bool value)
    {
        
        setMusic(value, true);
    }

    private void setEffects(bool value, bool playSound)
    {
        //Activate effects
        GameData.EffectsOn = value;

        //Play sound on change
        if (playSound)
            soundManager.PlaySoundEffect(value ? "Low Tick" : "Tick", true);

        //Change appearance
        if (EffectsButton)
        {
            var text = EffectsButton.GetComponentInChildren<TextMeshProUGUI>();
            if (text)
            {
                text.text = value ? "Effects On" : "Effects Off";
                text.color = value ? OnColour : OffColour;
            }
        }
    }


    private void setEffects(bool value)
    {
        setEffects(value, true);
    }

    private float oldMusicValue;
    public void SetMusicVolume(float value)
    {
        if (Mathf.Abs(value - oldMusicValue) > 0.05f)
        {
            soundManager.PlaySoundEffect("Tick");
            oldMusicValue = value;
        }
        soundManager.MusicVolume = value;
    }

    private float oldEffectsValue;
    public void SetEffectsVolume(float value)
    {
        if (Mathf.Abs(value - oldEffectsValue) > 0.05f)
        {
            soundManager.PlaySoundEffect("Tick");
            oldEffectsValue = value;
        }
        GameData.EffectsVolume = value;
    }
}
