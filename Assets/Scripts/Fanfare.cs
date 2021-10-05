using UnityEngine;

public class Fanfare : MonoBehaviour
{
    public SoundManager soundManager;

    private void OnEnable()
    {
        soundManager.PlaySoundEffect("Fanfare");
    }
}
