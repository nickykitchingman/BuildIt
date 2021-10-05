using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public UserDataSettings settingsLoader;
    public float WaitingTime;

    public GameObject Logo;
    public SoundManager soundManager;

    private Spin LogoSpin;
    private Expand LogoExpand;
    private Wiggle LogoWiggle;

    private bool ready = false;
    private bool started = false;

    private void Awake()
    {
        GameData.Mode = "menu";
    }

    private void Start()
    {
        LogoSpin = Logo.GetComponent<Spin>();
        LogoExpand = Logo.GetComponent<Expand>();
        LogoWiggle = Logo.GetComponent<Wiggle>();
    }

    private void Update()
    {
        //Load menu if not ready by start
        if (settingsLoader.loaded && !started)
            ready = true;

        if (ready && LogoSpin.CompletedCycle)
        {
            StartCoroutine(LoadMenu());
            if (LogoSpin)
                LogoSpin.Speed = 0;
            if (LogoExpand)
                LogoExpand.Speed = 0;
            if (LogoWiggle)
                LogoWiggle.Speed = 0;
            if (soundManager)
                soundManager.PlaySoundEffect("Bloop");
        }
    }

    private IEnumerator LoadMenu()
    {
        yield return new WaitForSecondsRealtime(WaitingTime);
        SceneManager.LoadScene(1);
    }
}
